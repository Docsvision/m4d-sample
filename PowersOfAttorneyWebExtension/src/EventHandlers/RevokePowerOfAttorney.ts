import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests.ts/PowersOfAttorneyDemoController";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Router } from "@docsvision/webclient/System/$Router";

const REVOKE_OPERATION = "8cd727de-6258-411a-b45f-8f9ecad5960a";

export const revokePowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    await sender.layout.getService($PowerOfAttorneyApiController).requestRevocationPowerOfAttorney(powerOfAttorneyId);
    await sender.layout.getService($PowerOfAttorneyApiController).revokePowerOfAttorney({powerOfAttorneyId: powerOfAttorneyId, withChildrenPowerOfAttorney: true});
    await sender.layout.changeState(REVOKE_OPERATION);
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo("Доверенность отозвана");
}