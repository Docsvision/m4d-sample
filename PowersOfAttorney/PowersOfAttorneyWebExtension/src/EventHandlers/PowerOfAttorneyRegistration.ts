import { CardLink } from "@docsvision/webclient/Platform/CardLink";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";

// Functions from WebClient EDI extension
declare function edi_sendPowerOfAttorneyToRegistrationAsFile(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_sendPowerOfAttorneyToRegistrationAsNumber(sender: LayoutControl, powerOfAttroneyCardId: string);
declare function edi_revokePowerOfAttorney(sender: LayoutControl, powerOfAttroneyCardId: string);

export async function sendPowerOfAttorneyToRegistrationAsFile(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    edi_sendPowerOfAttorneyToRegistrationAsFile(sender, powerOfAttorneyIdControl.value?.cardId)
}
export async function sendPowerOfAttorneyToRegistrationAsNumber(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    edi_sendPowerOfAttorneyToRegistrationAsNumber(sender, powerOfAttorneyIdControl.value?.cardId)
}
export async function sendPowerOfAttorneyRevokation(sender: LayoutControl) {
    const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
    edi_revokePowerOfAttorney(sender, powerOfAttorneyIdControl.value?.cardId)
}
