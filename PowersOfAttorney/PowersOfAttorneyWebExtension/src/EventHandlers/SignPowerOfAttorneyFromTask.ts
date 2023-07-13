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
    if (e.data.operationData.additionalInfo.decisionName === "Подписать") {
        const powerOfAttorneyUserCardId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.id;
        const powerOfAttorneyUserKindId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.kindId;
        if (powerOfAttorneyUserKindId === 'e1925a07-6f57-406d-9073-294381ea5aed') {
            await sender.layout.getService($PowersOfAttorneyDemoController).createPowerOfAttorney(powerOfAttorneyUserCardId);
        } else if (powerOfAttorneyUserKindId === '9df7c9ab-a7b2-4061-ab3a-0c35814cdad8') {
            await sender.layout.getService($PowersOfAttorneyDemoController).createRetrustPowerOfAttorney(powerOfAttorneyUserCardId);
        } else if (powerOfAttorneyUserKindId === '6ac009bc-fd9c-4b7a-ba69-eaed27675264') {
            await sender.layout.getService($PowersOfAttorneyDemoController).createEMCHDPowerOfAttorney(powerOfAttorneyUserCardId);
        }
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
                    if (signature) {
                        try {
                            await sender.layout.getService($PowerOfAttorneyApiController).attachSignatureToPowerOfAttorney({ powerOfAttorneyId, signature })
                            const signOperationIdPOA = sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
                            await sender.layout.getService($LayoutCardController).changeState({ cardId: powerOfAttorneyUserCardId, operationId: signOperationIdPOA, timestamp: sender.layout.controls.locationContainer.params.layoutModel.cardInfo.timestamp, comment: "", layoutParams: sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.layoutParams });
                            e.accept();
                        } catch (err) {
                            console.error(err);
                            e.cancel()
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