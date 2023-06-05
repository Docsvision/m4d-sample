import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";

export class PowersOfAttorneyDemoController {
    
    constructor(private services: $RequestManager) {
    }

    createPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/CreatePowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`, "");
    }

    createRetrustPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/CreateRetrustPowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`, "");
    }

    getPowerOfAttorneyCardId(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.get(`api/PowersOfAttorneyDemo/GetPowerOfAttorneyCardId?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`);
    }

    createEMCHDPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.get(`api/PowersOfAttorneyDemo/CreateEMCHDPowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`)
    }
    
    сreateEMCHDRetrustPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.get(`api/PowersOfAttorneyDemo/CreateEMCHDRetrustPowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`);
    }
}

export type $PowersOfAttorneyDemoController = {  powersOfAttorneyDemoController: PowersOfAttorneyDemoController }
export const $PowersOfAttorneyDemoController = serviceName((x: $PowersOfAttorneyDemoController) => x.powersOfAttorneyDemoController);