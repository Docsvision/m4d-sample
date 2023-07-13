import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Router } from "@docsvision/webclient/System/$Router";



export const signPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
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
                        await sender.layout.getService($PowerOfAttorneyApiController).attachSignatureToPowerOfAttorney({powerOfAttorneyId, signature});
                        const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
                        await sender.layout.changeState(operationId);
                        sender.layout.getService($Router).refresh();
                        sender.layout.getService($MessageWindow).showInfo("Доверенность подписана");
                    } catch(err) {

                    }   
                } 
                return {} as IEncryptedInfo;
            },
            onAttachSignatureToCard: async () => {}
        });
}



