import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $ApplicationSettings } from "@docsvision/webclient/StandardServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";

export const checkPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;

    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);

    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;

    try {
        const msg = await powersOfAttorneyButtonController?.checkPowerOfAttorney(powerOfAttorneyId, employeeId)
        sender.layout.getService($MessageWindow).showInfo(msg);
    } catch(err) {
        sender.layout.getService($MessageWindow).showError(err?.message || err);
    }
};
