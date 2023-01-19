import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $PowersOfAttorneyDemoController } from "../ServerRequests.ts/PowersOfAttorneyDemoController";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { doNothing } from "@docsvision/web/utils/common";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";


export const signPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    sender.layout.params.services.digitalSignature.showDocumentSignDialog(powerOfAttorneyUserCardId,
        {
            signWithoutLabel: true,
            dialogProps: {
                hideSimpleSign: true
            },
            onCreateSignature: async (options) => {
                const signatureData = await sender.layout.getService($PowerOfAttorneyApiController).getMachineReadablePowerOfAttorney(powerOfAttorneyId) as any;
                const info = new EncryptedInfo(options.method.certificateInfo.thumberprint);
                info.Attributes.push(new EncryptedAttribute(Crypto.DocumentNameOIDAttribute, getBstrBase64(signatureData.fileName)));
                const signature = Crypto.SignData(info, signatureData.data);
                await sender.layout.getService($PowerOfAttorneyApiController).attachSignatureToPowerOfAttorney(powerOfAttorneyId, signature)
                return {} as IEncryptedInfo
            },
            onAttachSignatureToCard: doNothing
        });
}


