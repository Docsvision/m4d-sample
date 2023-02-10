import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $LayoutCardController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $Router } from "@docsvision/webclient/System/$Router";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests.ts/PowersOfAttorneyDemoController";

const CREATE_OPERATION = "79b7f5fe-a6eb-4e91-89d8-1aaa4bf2e2f4";

export const createPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    await sender.layout.getService($PowersOfAttorneyDemoController).createPowerOfAttorney(powerOfAttorneyUserCardId);
    await sender.layout.getService($LayoutCardController).changeState({cardId:powerOfAttorneyUserCardId, operationId: CREATE_OPERATION, timestamp: sender.layout.cardInfo.timestamp, comment: "", layoutParams: sender.layout.layoutInfo.layoutParams});
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo("Доверенность сформирована");
}