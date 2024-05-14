import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $ApplicationSettings } from "@docsvision/webclient/StandardServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";
import { signPowerOfAttorney } from "./SignPowerOfAttorney";
import { resources } from "@docsvision/webclient/System/Resources";
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";

export const sendForRegistrationToRegistry = async (sender: CustomButton) => {
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;

    await powersOfAttorneyButtonController?.sendForRegistrationToRegistry(powerOfAttorneyId, employeeId);
    sender.layout.getService($MessageWindow).showInfo(resources.M4DRegistry_Register_Success);
};

export const signAndSendForRegistrationToRegistry = async (sender: CustomButton) => {
    await signPowerOfAttorney(sender, false, false);

    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;

    await powersOfAttorneyButtonController?.sendForRegistrationToRegistry(powerOfAttorneyId, employeeId);
    sender.layout.getService($MessageWindow).showInfo(resources.M4DRegistry_Register_Success);

    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
    await sender.layout.changeState(operationId);
}
