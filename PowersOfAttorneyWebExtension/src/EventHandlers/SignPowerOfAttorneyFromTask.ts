import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $LayoutCardController, $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { CREATE_OPERATION_POA, SIGN_OPERATION_POA } from "./Constants";


export const signPowerOfAttorneyFromTask = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.id
    await sender.layout.getService($PowersOfAttorneyDemoController).createPowerOfAttorney(powerOfAttorneyUserCardId);
    await sender.layout.getService($LayoutCardController).changeState({cardId:powerOfAttorneyUserCardId, operationId: CREATE_OPERATION_POA, timestamp: sender.layout.controls.locationContainer.params.layoutModel.cardInfo.timestamp, comment: "", layoutParams: sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.layoutParams});

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
                    } catch(err) {

                    }   
                } 
                return {} as IEncryptedInfo;
            },
            onAttachSignatureToCard: async () => {}
        });
    await sender.layout.getService($LayoutCardController).changeState({cardId:powerOfAttorneyUserCardId, operationId: SIGN_OPERATION_POA, timestamp: sender.layout.controls.locationContainer.params.layoutModel.cardInfo.timestamp, comment: "", layoutParams: sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.layoutParams});
    await sender.layout.changeState("c6dc4647-86b8-450a-a8f5-17f7259ba7a1");
}