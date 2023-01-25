import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests.ts/PowersOfAttorneyDemoController";

export const createPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    await sender.layout.getService($PowersOfAttorneyDemoController).createPowerOfAttorney(powerOfAttorneyUserCardId);
    sender.layout.getService($MessageWindow).showInfo("Доверенность сформирована");
}