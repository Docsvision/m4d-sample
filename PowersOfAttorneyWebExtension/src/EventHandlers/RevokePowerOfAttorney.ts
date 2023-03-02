import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests.ts/PowersOfAttorneyDemoController";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { $Router } from "@docsvision/webclient/System/$Router";
import { POWER_OF_ATTORNEY_KIND_ID, REVOKE_OPERATION_POA, REVOKE_OPERATION_SPOA } from "./Constants";


export const revokePowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    await sender.layout.getService($PowerOfAttorneyApiController).requestRevocationPowerOfAttorney(powerOfAttorneyId);
    await sender.layout.getService($PowerOfAttorneyApiController).revokePowerOfAttorney({powerOfAttorneyId: powerOfAttorneyId, withChildrenPowerOfAttorney: true});

    if (sender.layout.cardInfo.kindId === POWER_OF_ATTORNEY_KIND_ID) {
        await sender.layout.changeState(REVOKE_OPERATION_POA);
    } else {
        await sender.layout.changeState(REVOKE_OPERATION_SPOA);
    }
    
    sender.layout.getService($Router).refresh();
    sender.layout.getService($MessageWindow).showInfo("Доверенность отозвана");
}