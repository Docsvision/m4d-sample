import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Router } from "@docsvision/webclient/System/$Router";


export const revokePowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    await sender.layout.getService($PowerOfAttorneyApiController).requestRevocationPowerOfAttorney(powerOfAttorneyId);
    await sender.layout.getService($PowerOfAttorneyApiController).revokePowerOfAttorney({powerOfAttorneyId: powerOfAttorneyId, withChildrenPowerOfAttorney: true});
    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "To revoke").id;
    await sender.layout.changeState(operationId);
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo("Доверенность отозвана");
}