import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";

export class PowersOfAttorneyButtonController {
    constructor(private services: $RequestManager) {
    }

    sendForRegistrationToRegistry(powerOfAttorneyId: string, INN: string): void {
        const url = `M4dRegistryIntegration/Register?PowerOfAttorneyId=${powerOfAttorneyId}&INN=${INN}`;
        this.services.requestManager.post(url, "");
    }
}

export type $PowersOfAttorneyButtonController = { powersOfAttorneyButtonController: PowersOfAttorneyButtonController };
export const $PowersOfAttorneyButtonController = serviceName((x: $PowersOfAttorneyButtonController) => x.powersOfAttorneyButtonController);
