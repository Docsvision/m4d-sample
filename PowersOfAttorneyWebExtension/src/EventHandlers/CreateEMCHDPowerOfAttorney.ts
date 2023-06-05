import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $LayoutCardController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $Router } from "@docsvision/webclient/System/$Router";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests.ts/PowersOfAttorneyDemoController";
import { CREATE_OPERATION_POA } from "./Constants";



export const сreateEMCHDPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    await sender.layout.getService($PowersOfAttorneyDemoController).createEMCHDPowerOfAttorney(powerOfAttorneyUserCardId);
    await sender.layout.getService($LayoutCardController).changeState({cardId:powerOfAttorneyUserCardId, operationId: CREATE_OPERATION_POA, timestamp: sender.layout.cardInfo.timestamp, comment: "", layoutParams: sender.layout.layoutInfo.layoutParams});
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo("Доверенность сформирована");
}