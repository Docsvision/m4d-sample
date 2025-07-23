import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Router } from "@docsvision/webclient/System/$Router";
import { resources } from "@docsvision/webclient/System/Resources";
import { GenModels } from '@docsvision/webclient/Generated/DocsVision.WebClient.Models';


export const signPowerOfAttorney = async (sender: CustomButton, refreshLayout = true, showMessage = true) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    await sender.layout.params.services.digitalSignature.showDocumentSignDialog(powerOfAttorneyUserCardId,
        {
            signWithoutLabel: true,
            dialogProps: {
                hideSimpleSign: true
            },
            sourceCardInfo: sender.layout.cardInfo,
            onCreateSignature: async (options) => {
                if (options.method.certificateInfo.source === GenModels.SignatureMethodSources.Cloud) {
                    await sender.layout.getService($MessageWindow).showWarning(resources.PowerOfAttorneyCloudSignatureWarning);
                } else {
                    const signatureData = await sender.layout.getService($PowerOfAttorneyApiController).getMachineReadablePowerOfAttorneyData(powerOfAttorneyId);
                    const info = new EncryptedInfo(options.method.certificateInfo.thumberprint);
                    info.Attributes.push(new EncryptedAttribute(Crypto.DocumentNameOIDAttribute, getBstrBase64(signatureData.fileName)));
                    const certificate = await Crypto.GetCertificateByThumbprint(options.method.certificateInfo.thumberprint);
                    if (certificate) {
                        const signType = await Crypto.GetSignatureType(sender.getControlServices(), certificate);                        
                        const signature = await Crypto.SignData(info, signatureData.content, signType.cadesSignType, signType.tspAddress);                    
                        if (signature) {
                            try {
                                await sender.layout.getService($PowerOfAttorneyApiController).attachSignatureToPowerOfAttorney({ powerOfAttorneyId, signature });
                                const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
                                if (refreshLayout) {
                                    await sender.layout.changeState(operationId);
                                    sender.layout.getService($Router).refresh();
                                }
                            } catch (err) {
                                console.error(err);
                                return Promise.reject();
                            }
                        }
                    }
                    if (showMessage) {
                        await sender.layout.getService($MessageWindow).showInfo(resources.PowerOfAttorneySigned);
                    }
                }
                return {} as IEncryptedInfo;
            },
            onAttachSignatureToCard: async () => { }
        });
}



