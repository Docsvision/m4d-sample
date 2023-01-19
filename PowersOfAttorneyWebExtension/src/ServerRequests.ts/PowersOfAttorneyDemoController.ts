import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";

export class PowersOfAttorneyDemoController {
    
    constructor(private services: $RequestManager) {
    }

    createPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/CreatePowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`, "");
    }

    retrustPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/RetrustPowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`, "");
    }

    getPowerOfAttorneyCardId(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.get(`api/PowersOfAttorneyDemo/GetPowerOfAttorneyCardId?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`);
    }
}

export type $PowersOfAttorneyDemoController = {  powersOfAttorneyDemoController: PowersOfAttorneyDemoController }
export const $PowersOfAttorneyDemoController = serviceName((x: $PowersOfAttorneyDemoController) => x.powersOfAttorneyDemoController);