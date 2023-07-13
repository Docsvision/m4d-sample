import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";
import { PowerOfAttorneyRevocationType } from "../Interfaces";

export class PowersOfAttorneyDemoController {
    
    constructor(private services: $RequestManager) {
    }

    createEMCHDPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/CreateEMCHDPowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`, "")
    }

    createEMCHDRetrustPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/CreateEMCHDRetrustPowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`, "");
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

    getPowerOfAttorneyNumber(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.get(`api/PowersOfAttorneyDemo/GetPowerOfAttorneyNumber?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`);
    }

    requestRevocationPowerOfAttorney(powerOfAttorneyUserCardId: string, revocationType: PowerOfAttorneyRevocationType ,revocationReason: string ): Promise<any> {
        const data = {
            "powerOfAttorneyUserCardId": powerOfAttorneyUserCardId,
            "revocationType": revocationType,
            "revocationReason": revocationReason
        }
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/RequestRevocationPowerOfAttorney`, JSON.stringify(data));
    }
}

export type $PowersOfAttorneyDemoController = {  powersOfAttorneyDemoController: PowersOfAttorneyDemoController }
export const $PowersOfAttorneyDemoController = serviceName((x: $PowersOfAttorneyDemoController) => x.powersOfAttorneyDemoController);