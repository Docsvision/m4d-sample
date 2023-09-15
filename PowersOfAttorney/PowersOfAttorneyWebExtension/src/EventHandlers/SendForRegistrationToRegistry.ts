import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";

export const sendForRegistrationToRegistry = async (sender: CustomButton) => {
  const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
  const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
  const reprINN = sender.layout.controls.get<TextBox>("reprINN")?.value;
  await sender.layout
    .getService($PowersOfAttorneyButtonController)
    .sendForRegistrationToRegistry(powerOfAttorneyUserCardId, reprINN);
};
