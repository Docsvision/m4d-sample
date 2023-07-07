import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Router } from "@docsvision/webclient/System/$Router";
import { EncryptedAttribute, EncryptedInfo } from "@docsvision/webclient/Legacy/EncryptedInfo";
import { IEncryptedInfo } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { Crypto, getBstrBase64 } from "@docsvision/webclient/Libs/CryptoPro/Crypto";
import React, { useState } from "react";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { $MessageBox } from "@docsvision/webclient/System/$MessageBox";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { EditMode } from "@docsvision/webclient/System/EditMode";
import { PowerOfAttorneyRevocationType } from "../Interfaces";
import { resources } from '@docsvision/webclient/System/Resources';


export const revokePowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyNumber = await sender.layout.getService($PowersOfAttorneyDemoController).getPowersOfAttorneyNumber(powerOfAttorneyUserCardId);
    const items = [{key: PowerOfAttorneyRevocationType.Principal.toString() , value: resources.CancellationOfThePowerOfAttorneyByThePrincipal }, {key: PowerOfAttorneyRevocationType.Representative.toString(), value: resources.RefusalOfTheRepresentativeFromThePowers}]
    
    const [type, setType] = useState<PowerOfAttorneyRevocationType>(PowerOfAttorneyRevocationType.Principal);
    const [reason, setReason] = useState<string>();

    const createAndSignApplication = async () =>{
        const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
        const signatureData =  await sender.layout.getService($PowersOfAttorneyDemoController).requestRevocationPowerOfAttorney(powerOfAttorneyUserCardId, type , reason);
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
                    if(signature) {
                        try {
                            await sender.layout.getService($PowersOfAttorneyDemoController).attachSignatureToRevocation(powerOfAttorneyUserCardId, signature);
                        } catch(err) {
    
                        }   
                    } 
                    return {} as IEncryptedInfo;
                },
                onAttachSignatureToCard: async () => {}
            }); 
    
        const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
        await sender.layout.getService($PowerOfAttorneyApiController).revokePowerOfAttorney({powerOfAttorneyId: powerOfAttorneyId, withChildrenPowerOfAttorney: true});
        const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "To revoke").id;
        await sender.layout.changeState(operationId);
        sender.layout.getService($Router).refresh();
        sender.layout.getService($MessageWindow).showInfo(resources.PowerOfAttorneyIsRevoked);
    }
    
    const  modalContent = (
        <div>
           <div>`{resources.PowerOfAttorney} № ${powerOfAttorneyNumber}`</div>
           <RadioGroup items={items} value={type.toString()} dataChanged={(e) => setType(e.target.value)} labelText={resources.SelectTheTypeOfApplicationForRevocation}></RadioGroup> 
           <TextArea useHtml={false} value={reason} dataChanged={(e) => setReason(e.target.value)} labelText={resources.Reason} placeHolder={resources.SpecifyTheReasonForCancellationOrRefusal}></TextArea>
        </div>
    );

    const buttons = [
        { name: resources.SignTheApplication, id: "open", onClick: createAndSignApplication}, 
        { name: resources.Cancel, isCancel: true }]

    sender.layout.getService($MessageBox).showCustom(modalContent, resources.ApplicationForRevocationOfThePowerOfAttorney, buttons)
}

