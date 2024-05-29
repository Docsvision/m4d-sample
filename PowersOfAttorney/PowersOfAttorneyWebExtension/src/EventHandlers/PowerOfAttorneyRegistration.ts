import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { CardLink } from "@docsvision/webclient/Platform/CardLink";
import { Label } from "@docsvision/webclient/Platform/Label";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";
import { resources } from "@docsvision/webclient/System/Resources";

// Functions from WebClient EDI extension
declare function edi_sendPowerOfAttorneyToRegistrationAsFile(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_sendPowerOfAttorneyToRegistrationAsNumber(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_deleteEmployeePowerOfAttorney(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_getPowerOfAttorneyRegistrationStatus(sender: LayoutControl, powerOfAttroneyCardId: string): Promise<string>;

export async function sendPowerOfAttorneyToRegistrationAsFile(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    try {
        if (typeof edi_sendPowerOfAttorneyToRegistrationAsFile !== 'undefined') {
            await edi_sendPowerOfAttorneyToRegistrationAsFile(sender, powerOfAttorneyIdControl.value?.cardId)
        }
    }
    catch (err) {
        sender.layout.getService($MessageWindow).showWarning(err?.message || err);
        console.error(err);
    }
}
export async function sendPowerOfAttorneyToRegistrationAsNumber(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    try {
        if (typeof edi_sendPowerOfAttorneyToRegistrationAsNumber !== 'undefined') {
            edi_sendPowerOfAttorneyToRegistrationAsNumber(sender, powerOfAttorneyIdControl.value?.cardId)
        }
    } catch (err) {
        sender.layout.getService($MessageWindow).showWarning(err?.message || err);
        console.error(err);
    }
}
export async function deleteEmployeePowerOfAttorney(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    edi_deleteEmployeePowerOfAttorney(sender, powerOfAttorneyIdControl.value?.cardId)
}
export async function getPowerOfAttorneyRegistrationStatus(sender: LayoutControl) {
    let status = '';
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    const powerOfAttorneyRegistrationStatus = sender.layout.getService($ControlStore).get<Label>("powerOfAttorneyRegistrationStatus");
    if (typeof edi_getPowerOfAttorneyRegistrationStatus !== 'undefined') {
        status = await edi_getPowerOfAttorneyRegistrationStatus(sender, powerOfAttorneyIdControl.value?.cardId) || resources.PoaDidNotSendToRegistrationStatus;
    }
    powerOfAttorneyRegistrationStatus.params.text = status;
}
