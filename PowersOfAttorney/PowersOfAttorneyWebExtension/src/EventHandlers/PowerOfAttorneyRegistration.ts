import { CardLink } from "@docsvision/webclient/Platform/CardLink";
import { Label } from "@docsvision/webclient/Platform/Label";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";

// Functions from WebClient EDI extension
declare function edi_sendPowerOfAttorneyToRegistrationAsFile(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_sendPowerOfAttorneyToRegistrationAsNumber(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_revokePowerOfAttorney(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_getPowerOfAttorneyRegistrationStatus(sender: LayoutControl, powerOfAttroneyCardId: string): Promise<string>;

export async function sendPowerOfAttorneyToRegistrationAsFile(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    try {
        await edi_sendPowerOfAttorneyToRegistrationAsFile(sender, powerOfAttorneyIdControl.value?.cardId)
    }
    catch (err) {
        console.error(err);
    }
}
export async function sendPowerOfAttorneyToRegistrationAsNumber(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    edi_sendPowerOfAttorneyToRegistrationAsNumber(sender, powerOfAttorneyIdControl.value?.cardId)
}
export async function sendPowerOfAttorneyRevokation(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    edi_revokePowerOfAttorney(sender, powerOfAttorneyIdControl.value?.cardId)
}
export async function getPowerOfAttorneyRegistrationStatus(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    const powerOfAttorneyRegistrationStatus = sender.layout.getService($ControlStore).get<Label>("powerOfAttorneyRegistrationStatus");
    var status = await edi_getPowerOfAttorneyRegistrationStatus(sender, powerOfAttorneyIdControl.value?.cardId);
    powerOfAttorneyRegistrationStatus.params.text = status;
}
