import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Resources } from "@docsvision/web/core/localization/$Resources";
import { $LayoutCardController, $PowerOfAttorneyApiController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $Router } from "@docsvision/webclient/System/$Router";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { resources } from "@docsvision/webclient/System/Resources";
import { $PowersOfAttorneyDemoController } from "../../ServerRequests/PowersOfAttorneyDemoController";


export const createRetrustPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;    
    await sender.layout.getService($PowersOfAttorneyDemoController).createRetrustPowerOfAttorney(powerOfAttorneyUserCardId);
    if (powerOfAttorneyId) {
        const cardInfo = await sender.layout.getService($PowerOfAttorneyApiController).getPowerOfAttorneyInfo(powerOfAttorneyId);
        const resources = sender.layout.getService($Resources);
        if (cardInfo.status === resources.PowerOfAttorney_StatusPreparation) {
            sender.layout.getService($LayoutCardController).delete({cardId: powerOfAttorneyId, isNew: false})
        }  
    }
    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "Create").id;
    await sender.layout.getService($LayoutCardController).changeState({cardId:powerOfAttorneyUserCardId, operationId: operationId, timestamp: sender.layout.cardInfo.timestamp, comment: "", layoutParams: sender.layout.layoutInfo.layoutParams});
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo(resources.PowerOfAttorneyGenerated);
}