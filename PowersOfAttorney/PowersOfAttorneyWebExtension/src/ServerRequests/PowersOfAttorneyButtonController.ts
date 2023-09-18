import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";

export class PowersOfAttorneyButtonController {
    constructor(private services: $RequestManager) {
    }

    sendForRegistrationToRegistry(powerOfAttorneyId: string, INN: string): Promise<string | null> {
        const url = `api/M4dRegistryIntegration/Register?PowerOfAttorneyId=${powerOfAttorneyId}&INN=${INN}`;
        return this.services.requestManager.post(url, "");
    }
}

export type $PowersOfAttorneyButtonController = { powersOfAttorneyButtonController: PowersOfAttorneyButtonController };
export const $PowersOfAttorneyButtonController = serviceName((x: $PowersOfAttorneyButtonController) => x.powersOfAttorneyButtonController);
