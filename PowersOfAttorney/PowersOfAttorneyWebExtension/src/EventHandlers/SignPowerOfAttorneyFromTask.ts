import { $LayoutCardController, $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { OperationExecutingEventArgs } from "@docsvision/webclient/BackOffice/OperationExecutingEventArgs";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { EMCHD_POWER_OF_ATTORNEY_KIND_ID, EMCHD_RETRUST_POWER_OF_ATTORNEY_KIND_ID, POWER_OF_ATTORNEY_KIND_ID, RETRUST_POWER_OF_ATTORNEY_KIND_ID, SIGN_OPERATION_ID } from '../PowerOfAttorneyConstants';
import { MessageBox } from '@docsvision/webclient/Helpers/MessageBox/MessageBox';
import { resources } from '@docsvision/webclient/System/Resources';

export const signPowerOfAttorneyFromTask = async (sender: LayoutControl, e: ICancelableEventArgs<OperationExecutingEventArgs>, refreshLayout = true) => {
    e.wait();
    if (e.data.operationData.builtInOperationId === SIGN_OPERATION_ID) {
        const powerOfAttorneyUserCardId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.id;
        const powerOfAttorneyUserKindId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.kindId;
        if (powerOfAttorneyUserKindId === POWER_OF_ATTORNEY_KIND_ID) {
            await sender.layout.getService($PowersOfAttorneyDemoController).createPowerOfAttorney(powerOfAttorneyUserCardId);
        } else if (powerOfAttorneyUserKindId === RETRUST_POWER_OF_ATTORNEY_KIND_ID) {
            await sender.layout.getService($PowersOfAttorneyDemoController).createRetrustPowerOfAttorney(powerOfAttorneyUserCardId);
        } else if (powerOfAttorneyUserKindId === EMCHD_POWER_OF_ATTORNEY_KIND_ID) {
            await sender.layout.getService($PowersOfAttorneyDemoController).createEMCHDPowerOfAttorney(powerOfAttorneyUserCardId);
        } else if (powerOfAttorneyUserKindId === EMCHD_RETRUST_POWER_OF_ATTORNEY_KIND_ID) {
            await sender.layout.getService($PowersOfAttorneyDemoController).createEMCHDRetrustPowerOfAttorney(powerOfAttorneyUserCardId);
        }
        const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
        await sender.layout.params.services.digitalSignature.showDocumentSignDialog(powerOfAttorneyUserCardId,
            {
                signWithoutLabel: true,
                dialogProps: {
                    hideSimpleSign: true
                },
                sourceCardInfo: sender.layout.cardInfo,
                onCreateSignature: async (options) => {
                    const signatureData = await sender.layout.getService($PowerOfAttorneyApiController).getMachineReadablePowerOfAttorneyData(powerOfAttorneyId);
                    const info = new EncryptedInfo(options.method.certificateInfo.thumberprint);
                    info.Attributes.push(new EncryptedAttribute(Crypto.DocumentNameOIDAttribute, getBstrBase64(signatureData.fileName)));
                    const signature = await Crypto.SignData(info, signatureData.content);
                    if (signature) {
                        try {
                            await sender.layout.getService($PowerOfAttorneyApiController).attachSignatureToPowerOfAttorney({ powerOfAttorneyId, signature })
                            if (refreshLayout) {                               
                                const signOperationIdPOA = sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
                                await sender.layout.getService($LayoutCardController).changeState({ cardId: powerOfAttorneyUserCardId, operationId: signOperationIdPOA, timestamp: sender.layout.controls.locationContainer.params.layoutModel.cardInfo.timestamp, comment: "", layoutParams: sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.layoutParams });
                                e.accept();
                            }
                            
                        } catch (err) {
                            console.error(err);
                            MessageBox.ShowError(resources.PowerOfAttorneyErrorSigning)
                            e.cancel()
                            return Promise.reject();
                        }
                    } else {
                        e.cancel()
                    }
                    return {} as IEncryptedInfo;
                },
                onAttachSignatureToCard: async () => { }
            });


    } else {
        e.accept();
    }
}