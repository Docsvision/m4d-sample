import { $LayoutCardController, $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { OperationExecutingEventArgs } from "@docsvision/webclient/BackOffice/OperationExecutingEventArgs";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";


export const signPowerOfAttorneyFromTask = async (sender: LayoutControl, e: ICancelableEventArgs<OperationExecutingEventArgs>) => {
    e.wait();
    const signCompletionOperationId = "ce8418b9-6699-43e1-b429-e0a7a115e452";
    if(e.data.operationData.completionOptionId === signCompletionOperationId) {
        const powerOfAttorneyUserCardId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.id
        await sender.layout.getService($PowersOfAttorneyDemoController).createPowerOfAttorney(powerOfAttorneyUserCardId);
        
        const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
        await sender.layout.params.services.digitalSignature.showDocumentSignDialog(powerOfAttorneyUserCardId,
            {
                signWithoutLabel: true,
                dialogProps: {
                    hideSimpleSign: true
                },
                onCreateSignature: async (options) => {
                    const signatureData = await sender.layout.getService($PowerOfAttorneyApiController).getMachineReadablePowerOfAttorneyData(powerOfAttorneyId);
                    const info = new EncryptedInfo(options.method.certificateInfo.thumberprint);
                    info.Attributes.push(new EncryptedAttribute(Crypto.DocumentNameOIDAttribute, getBstrBase64(signatureData.fileName)));
                    const signature = await Crypto.SignData(info, signatureData.content);
                    if(signature) {
                        try {
                            await sender.layout.getService($PowerOfAttorneyApiController).attachSignatureToPowerOfAttorney({powerOfAttorneyId, signature})
                            const signOperationIdPOA = sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
                            await sender.layout.getService($LayoutCardController).changeState({cardId:powerOfAttorneyUserCardId, operationId: signOperationIdPOA, timestamp: sender.layout.controls.locationContainer.params.layoutModel.cardInfo.timestamp, comment: "", layoutParams: sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.layoutParams});
                            e.accept();
                        } catch(err) {
                            e.cancel()
                        }   
                    } else {
                        e.cancel()
                    }
                    return {} as IEncryptedInfo;
                },
                onAttachSignatureToCard: async () => {}
            });
            
        
    } else {
        e.accept();
    } 
}