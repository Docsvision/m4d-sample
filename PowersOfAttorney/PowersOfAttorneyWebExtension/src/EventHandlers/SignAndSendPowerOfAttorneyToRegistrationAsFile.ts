import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $Router } from "@docsvision/webclient/System/$Router";
import { sendPowerOfAttorneyToRegistrationAsFile } from "./PowerOfAttorneyRegistration";
import { signPowerOfAttorney } from "./SignPowerOfAttorney";


export const signAndSendPowerOfAttorneyToRegistrationAsFile = async (sender: CustomButton) => {
    await signPowerOfAttorney(sender, false);
    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
    await sender.layout.changeState(operationId);
    await sendPowerOfAttorneyToRegistrationAsFile(sender);
}