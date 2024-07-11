import { IDigitalSignOptions, SignatureMethod } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { DOCUMENT_CARD_TYPE_ID } from "@docsvision/webclient/BackOffice/Constants";
import { GenModels } from "@docsvision/webclient/Generated/DocsVision.WebClient.Models";
import { Button } from "@docsvision/webclient/Helpers/Button";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IBatchOperationInfo } from "@docsvision/webclient/Platform/$BatchOperations";
import { BatchOperationErrorInfo } from "@docsvision/webclient/Platform/BatchOperationErrorInfo";
import { ITableRowModel } from "@docsvision/webclient/Platform/ITableRowModel";
import { arrayDifference } from "@docsvision/webclient/System/ArrayUtils";
import { BaseControlState } from "@docsvision/webclient/System/BaseControl";
import { BaseControlImpl } from "@docsvision/webclient/System/BaseControlImpl";
import { showIf } from "@docsvision/webclient/System/CssUtils";
import { resources } from "@docsvision/webclient/System/Resources";
import { urlStore } from "@docsvision/webclient/System/UrlStore";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";

import React from "react";
import { POWER_OF_ATTORNEY_KIND_ID, EMCHD_POWER_OF_ATTORNEY_KIND_ID } from "../../../../../PowerOfAttorneyConstants";
import { SignPOABatchOperationParams } from "./DocumentSignBatchOperation";
import { IRow } from "@docsvision/web/components/table/interfaces/IRow";

export interface ISignPOABatchOperationState extends SignPOABatchOperationParams, BaseControlState {
    selectedRowsInfo: GenModels.LayoutCardViewModel[]
}

export class SignPOABatchOperationImpl extends BaseControlImpl<SignPOABatchOperationParams, ISignPOABatchOperationState> {
    columnNameWithState: string;
    columnNameWithKindId: string;
    constructor(state, props) {
        super(state, props);
        this.onSignClick = this.onSignClick.bind(this);
        this.state.selectedRowsInfo = [];
        
    }

    componentDidMount() {
        super.componentDidMount();

        this.state.services.batchOperations?.register(this.getOperationInfo());
        this.state.services.tableRowSelection?.selectionChanged?.subscribe(this.onSelectionChanged);
        this.columnNameWithState = this.state.columnNameWithState;
        this.columnNameWithKindId = this.state.columnNameWithKindId;
    }

    componentWillUnmount() {
        super.componentWillUnmount();
        this.state.services.batchOperations?.unregister(this.getOperationInfo().id);
        this.state.services.tableRowSelection?.selectionChanged?.unsubscribe(this.onSelectionChanged);
    }

    onSelectionChanged = () => {
        this.state.services.batchOperations?.update(this.state.name, this.getOperationInfo());
        this.forceUpdate();
    }

    getOperationInfo(): IBatchOperationInfo {
        return {
            id: this.state.name,
            controlName: this.state.name,
            isAvailable: this.isVisible(),
            isVisible: this.isVisible(),
            displayName: resources.POABatchSign_SignOperationName
        };
    }

    protected onSignClick = async () => {
        this.previewDocumentAndConfirmSign(this.state.services.tableRowSelection.selection.selectedRows as any);
    }

    private async signDocuments() {
        let selectedMethod: SignatureMethod;
        const doNothing = () => {};
        const saveSignDialogData = async (signOptions: IDigitalSignOptions) => {
            selectedMethod = signOptions.method;
            return { cardInfo: signOptions.cardInfo }
        }

        const selectedRows = this.state.services.tableRowSelection.selection.selectedRows as any;
        try {
            await this.state.services.digitalSignature.showDocumentSignDialog(
            selectedRows[0].row?.entityId,
            {
                signWithoutLabel: true,
                dialogProps: {
                    hideSimpleSign: true
                },
                onCreateSignature: saveSignDialogData as any,
                onAttachSignatureToCard: doNothing as any,
            });
        } catch(err) {
            console.log(err);
        } 

        if (!selectedMethod.isSimple && !selectedMethod.certificateInfo) {
            MessageBox.ShowError(resources.POABatchSign_СertificateError);
            return;
        }

        const signDocument = async (row: ITableRowModel): Promise<BatchOperationErrorInfo[]> => {
            return new Promise(async (resolve, reject) => {
                const errors = [] as BatchOperationErrorInfo[];
                const pushError = (cardName?: string, message?: string) => {
                    errors.push({
                        gridRow: row,
                        cardDigest: cardName || resources.POABatchSign_Card,
                        errorMessage: message
                    });
                }
                const cardState = row.get(this.columnNameWithState);
                const cardRow = (this.state.services.tableRowSelection.selection.selectedRows
                    .find(selection => selection.instanceId === row.instanceId) as ITableRowModel & { row })?.row;
                const cardName = this.getCardLinkPresentation(cardRow);
                if (cardState !== 'Is created') {
                    pushError(cardName, resources.POABatchSign_AccessError);
                    resolve(errors);
                    return
                }
    
                try {
                    const url = urlStore.urlResolver.resolveApiUrl(`GetSignatureData?powerOfAttorneyUserCardId=${row.instanceId}`, "PowersOfAttorneyDemo");
                    const POASignatureData = await this.state.services.requestManager.get<IPOASignatureData>(url, { disableDialogsOnErrors: true});
                    const signOperationPOA = POASignatureData.operations.find(operation => operation.alias === "Sign");
                    if (!signOperationPOA.available) {
                        pushError(cardName, resources.POABatchSign_AccessError);
                        resolve(errors);
                        return
                    }
                    const info = new EncryptedInfo(selectedMethod.certificateInfo.thumberprint);
                    info.Attributes.push(new EncryptedAttribute(Crypto.DocumentNameOIDAttribute, getBstrBase64(POASignatureData.powerOfAttorneyContent)));
                    let signature;
                    try {
                        signature = await Crypto.SignData(info, POASignatureData.powerOfAttorneyContent);
                    } catch (error) {
                        pushError(cardName, resources.POABatchSign_ErrorCreateSign);
                        resolve(errors);
                        return
                    }
                    if (signature) {
                        await this.state.services.powerOfAttorneyApiController.attachSignatureToPowerOfAttorney({ powerOfAttorneyId: POASignatureData.powerOfAttorneyId, signature }, { disableDialogsOnErrors: true})                            
                        const signOperationPOA = POASignatureData.operations.find(operation => operation.alias === "Sign");
                        await this.state.services.layoutCardController.changeState({ cardId: POASignatureData.cardId, operationId: signOperationPOA.id, timestamp: POASignatureData.timestamp, comment: "", layoutParams: null }, { disableDialogsOnErrors: true});
                    }
                    
                } catch (responseObject) {
                    console.log(responseObject);
                    if (typeof responseObject === "string" && responseObject.includes(resources.Error_AccessDenied)) {
                        responseObject = resources.SignatureNotAllowed
                    }
                    pushError(responseObject?.data || cardName, responseObject?.message || responseObject);
                }  
                resolve(errors);
            });
        }
            
        const errors = await this.state.services.batchOperationsPerformer.perform(
            resources.POABatchSign_SignOperationName, 
            this.state.services.tableRowSelection.selection, 
            signDocument,
            resources.POABatchSign_SignOperationDescription
        );

        await this.state.services.folderDataLoading.loadData({ refresh: true, refreshSource: true, refreshGrouping: true });

        if (errors.length == 0) {
            this.state.services.tableMode.rowsSelectionMode = false;
            this.state.services.tableRowSelection.clearSelection();
            MessageBox.ShowInfo(resources.POABatchSign_Signed);
        } else {
            this.state.services.tableRowSelection.clearSelection(
                arrayDifference(
                    this.state.services.tableRowSelection.selection.selectedRows.map(x => x.instanceId),
                    errors.map(x => x.gridRow.instanceId)
                )
            );
        }
    }

    private async previewDocumentAndConfirmSign(selectedRows: (ITableRowModel & { row: IRow })[]) {
        const filelist = selectedRows.map(selected => 
            <a key={selected.instanceId} className="document-sign-batch-operation-filelist" href={`#/CardView/${selected.instanceId}`} 
                target="_blank" style={{ display: "block", fontWeight: "bold"}} onClick={ev => (ev.target as HTMLElement).style.fontWeight = "normal"}>
                {this.getCardLinkPresentation(selected.row)}
            </a>
        );
        const header = <div style={{ marginBottom: "15px"}}>{resources.POABatchSign_SignConfirmationHeader}</div>;
        const content = <div style={{ height: "100%", maxHeight: "50vh", overflowY: "auto"}}>{filelist}</div>
        await MessageBox.ShowConfirmation([header, content]);
        this.signDocuments();
    }

    private getCardLinkPresentation = (row: IRow) => {
        const presentations = (this.state.columnNameForPresentation?.split(',') || [])
            .map(p => p.trim());
        return (row?.cells || []).filter(cell => presentations.includes(cell.columnId))
            .map(cell => cell.value || "")
            .join(" ") 
            || resources.POABatchSign_Card;
    }

    isVisible() {
        return this.state.services.tableRowSelection?.selection?.selectedRows?.length != 0
            && this.state.services.tableRowSelection?.selection?.selectedRows.every(selection => {
                const kindId = selection.getGuid(this.columnNameWithKindId);          
                return selection.cardTypeId === DOCUMENT_CARD_TYPE_ID 
                && (kindId === POWER_OF_ATTORNEY_KIND_ID || kindId === EMCHD_POWER_OF_ATTORNEY_KIND_ID)
            });
    }

    getCssClass() {
        const isVisible = this.isVisible();
        return showIf(isVisible) + super.getCssClass();
    }

    private getButtonText() : string {
        if (this.state.buttonText == null || this.state.buttonText.trim() === "")
            return resources.POABatchSign_SignOperationName;
        else
            return this.state.buttonText;
    }

    protected renderControl() {
        return (
            <Button onClick={this.onSignClick} stretchWidth={false}>
               {this.getButtonText()}
            </Button>
        );
    }
}

interface IPOASignatureData {
    cardId: string,
    kindId: string,
    state: GenModels.StateModel,
    operations: Array<GenModels.OperationModel>,
    powerOfAttorneyId: string,
    powerOfAttorneyContent: any,
    powerOfAttorneyFileName: string,
    timestamp: number
}