import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $ApplicationSettings } from "@docsvision/webclient/StandardServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { signPowerOfAttorney } from "./SignPowerOfAttorney";
import { resources } from "@docsvision/webclient/System/Resources";

export const sendForRegistrationToRegistry = async (sender: CustomButton) => {
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;

    powersOfAttorneyButtonController?.sendForRegistrationToRegistry(powerOfAttorneyId, employeeId).then(msg => {
        if (msg.Success) {
            msg.Data && MessageBox.ShowInfo(msg.Data);
        } else {
            MessageBox.ShowError(msg.Message);
        }
    });
};

export const signAndSendForRegistrationToRegistry = async (sender: CustomButton) => {
    await signPowerOfAttorney(sender, false, false);

    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;

    const msg = await powersOfAttorneyButtonController?.sendForRegistrationToRegistry(powerOfAttorneyId, employeeId);
    if (msg.Success) {
        MessageBox.ShowInfo(resources.M4DRegistry_Register_Success);
    } else {
        MessageBox.ShowError(msg.Message);
    }

    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
    await sender.layout.changeState(operationId);
}
