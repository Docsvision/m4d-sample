import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";

export const sendForRegistrationToRegistry = async (sender: CustomButton) => {
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);

    powersOfAttorneyButtonController?.sendForRegistrationToRegistry(powerOfAttorneyId);
};
