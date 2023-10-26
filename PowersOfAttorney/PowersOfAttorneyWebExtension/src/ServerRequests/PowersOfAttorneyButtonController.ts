import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";

export class PowersOfAttorneyButtonController {
    constructor(private services: $RequestManager) {}

    sendForRegistrationToRegistry(powerOfAttorneyId: string, employeeId: string): Promise<any> {
        const url = `M4dRegistryIntegration/Register?PowerOfAttorneyId=${powerOfAttorneyId}&EmployeeId=${employeeId}`;
        return this.services.requestManager.post(url, "");
    }

    recallPowerOfAttorney(powerOfAttorneyId: string, employeeId: string): Promise<any> {
        const url = `M4dRegistryIntegration/Recall?PowerOfAttorneyId=${powerOfAttorneyId}&EmployeeId=${employeeId}`;
        return this.services.requestManager.post(url, "");
    }

    checkPowerOfAttorney(powerOfAttorneyId: string, employeeId: string): Promise<any> {
        const url = `M4dRegistryIntegration/Check?PowerOfAttorneyId=${powerOfAttorneyId}&EmployeeId=${employeeId}`;
        return this.services.requestManager.post(url, "");
    }
}

export type $PowersOfAttorneyButtonController = { powersOfAttorneyButtonController: PowersOfAttorneyButtonController };
export const $PowersOfAttorneyButtonController = serviceName((x: $PowersOfAttorneyButtonController) => x.powersOfAttorneyButtonController);
