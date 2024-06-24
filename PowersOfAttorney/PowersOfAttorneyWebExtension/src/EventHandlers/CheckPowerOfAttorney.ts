import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $ApplicationSettings } from "@docsvision/webclient/StandardServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";

export const checkPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;

    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);

    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;

    powersOfAttorneyButtonController?.checkPowerOfAttorney(powerOfAttorneyId, employeeId).then(msg => {
        if (msg.Success) {
            msg.Data && MessageBox.ShowInfo(msg.Data);
        } else {
            MessageBox.ShowError(msg.Message);
        }
    });
};
