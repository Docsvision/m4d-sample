import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";

export const sendForRegistrationToRegistry = async (sender: CustomButton) => {
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
    const reprINN = sender.layout.controls.get<TextBox>("reprINN")?.value;
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);

    if (reprINN) {
        powersOfAttorneyButtonController?.sendForRegistrationToRegistry(powerOfAttorneyId, reprINN);
    }
};
