import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $LayoutCardController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $Router } from "@docsvision/webclient/System/$Router";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";


export const createEMCHDRetrustPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    await sender.layout.getService($PowersOfAttorneyDemoController).createEMCHDRetrustPowerOfAttorney(powerOfAttorneyUserCardId);
    sender.layout.getService($MessageWindow).showInfo("Доверенность сформирована");
    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "Create").id;
    await sender.layout.getService($LayoutCardController).changeState({cardId: powerOfAttorneyUserCardId, operationId: operationId, timestamp: sender.layout.cardInfo.timestamp, comment: "", layoutParams: sender.layout.layoutInfo.layoutParams});
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo("Доверенность сформирована");
}