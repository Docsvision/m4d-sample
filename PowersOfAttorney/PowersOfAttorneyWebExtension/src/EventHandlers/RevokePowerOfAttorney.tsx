import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Router } from "@docsvision/webclient/System/$Router";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import React from "react";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { PowerOfAttorneyRevocationType } from "../Interfaces";
import { resources } from '@docsvision/webclient/System/Resources';
import { ModalDialogButtonPanel } from "@docsvision/webclient/Helpers/ModalDialog/ModalDialogButtonPanel";
import { ModalHost } from "@docsvision/webclient/Helpers/ModalHost";
import { Button } from "@docsvision/webclient/Helpers/Button";
import { ModalDialogBox } from "@docsvision/webclient/Helpers/ModalDialog/ModalDialogBox";
import { ModalDialog } from "@docsvision/webclient/Helpers/ModalDialog/ModalDialog";
import { ModalDialogHeader } from "@docsvision/webclient/Helpers/ModalDialog/ModalDialogHeader";
import { ModalDialogContent } from "@docsvision/webclient/Helpers/ModalDialog/ModalDialogContent";
import { ModalBackdrop } from "@docsvision/webclient/Helpers/ModalBackdrop";
import { ModalDialogCloseButton } from "@docsvision/webclient/Helpers/ModalDialog/ModalDialogCloseButton";


export const revokePowerOfAttorney = async (sender: CustomButton) => {
    const items = [{ key: PowerOfAttorneyRevocationType.Principal.toString(), value: resources.CancellationOfThePowerOfAttorneyByThePrincipal }, { key: PowerOfAttorneyRevocationType.Representative.toString(), value: resources.RefusalOfTheRepresentativeFromThePowers }]
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyNumber = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyNumber(powerOfAttorneyUserCardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    let typeElement = null;
    let reasonElement = null;
    const onSave = () => {
        createAndSignApplication();
        modalHost.unmount();
    };

    const createAndSignApplication = async () => {

        const signatureData = await sender.layout.getService($PowersOfAttorneyDemoController).requestRevocationPowerOfAttorney(powerOfAttorneyUserCardId, +typeElement.params.value, reasonElement.value);
        await sender.layout.params.services.digitalSignature.showDocumentSignDialog(powerOfAttorneyUserCardId,
            {
                signWithoutLabel: true,
                dialogProps: {
                    hideSimpleSign: true
                },
                onCreateSignature: async (options) => {
                    const info = new EncryptedInfo(options.method.certificateInfo.thumberprint);
                    info.Attributes.push(new EncryptedAttribute(Crypto.DocumentNameOIDAttribute, getBstrBase64(signatureData.fileName)));
                    const signature = await Crypto.SignData(info, signatureData.content);
                    if (signature) {
                        try {
                            await sender.layout.getService($PowerOfAttorneyApiController).attachSignatureToRevocationPowerOfAttorney({ powerOfAttorneyId, signature });
                            await sender.layout.getService($PowerOfAttorneyApiController).revokePowerOfAttorney({ powerOfAttorneyId: powerOfAttorneyId, withChildrenPowerOfAttorney: true });
                            const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "To revoke").id;
                            await sender.layout.changeState(operationId);
                            sender.layout.getService($Router).refresh();
                            sender.layout.getService($MessageWindow).showInfo(resources.PowerOfAttorneyRevoked);
                        } catch (err) {
                            console.error(err);
                        }
                    }
                    return {} as IEncryptedInfo;
                },
                onAttachSignatureToCard: async () => { }
            });
    }

    const modalHost = new ModalHost("application-for-revocation-dialog", () => (
        <ModalBackdrop visible={true} onClick={() => modalHost.unmount()}>
            <ModalDialog isOpen={true}>
                <ModalDialogBox>
                    <ModalDialogHeader>
                        <span>{resources.ApplicationForRevocationOfThePowerOfAttorney}</span>
                        <ModalDialogCloseButton onClick={() => modalHost.unmount()} />
                    </ModalDialogHeader>
                    <ModalDialogContent>
                        <div style={{padding: "0 30px"}}>
                            <div>{`${resources.PowerOfAttorney} № ${powerOfAttorneyNumber}`}</div>
                            <RadioGroup ref={el => typeElement = el} value={PowerOfAttorneyRevocationType.Principal.toString()} items={items} labelText={resources.SelectTheTypeOfApplicationForRevocation}></RadioGroup>
                            <label>{resources.Reason}:</label>
                            <textarea ref={el => reasonElement = el} maxLength={150} rows={4} style={{ height: "auto" }} placeholder={resources.SpecifyTheReasonForCancellationOrRefusal}></textarea>
                        </div>
                        
                    </ModalDialogContent>
                    <ModalDialogButtonPanel>
                        <Button onClick={onSave} primaryButton={true}>{resources.SignTheApplication}</Button>
                        <Button onClick={() => modalHost.unmount()}>{resources.Dialog_Cancel}</Button>
                    </ModalDialogButtonPanel>
                </ModalDialogBox>
            </ModalDialog>
        </ModalBackdrop>
    ));

    modalHost.mount();
}

export const revokePowerOfAttorneyWithoutApplication = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    await sender.layout.getService($PowerOfAttorneyApiController).markAsRevokedPowerOfAttorney({ powerOfAttorneyId: powerOfAttorneyId, withChildrenPowerOfAttorney: true });
    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "To revoke").id;
    await sender.layout.changeState(operationId);
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo(resources.PowerOfAttorneyRevoked);
}