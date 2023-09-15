import { CardLink } from "@docsvision/webclient/Platform/CardLink";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";
import { serviceName } from "@docsvision/webclient/System/ServiceUtils";
import { PowerOfAttorneyRevocationType } from "../Interfaces";

export class PowersOfAttorneyButtonController {
    
    constructor(private services: $RequestManager) {
    }

    createEMCHDPowerOfAttorney(powerOfAttorneyUserCardId: string): Promise<string | null> {
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/CreateEMCHDPowerOfAttorney?powerOfAttorneyUserCardId=${powerOfAttorneyUserCardId}`, "")
    }


    requestRevocationPowerOfAttorney(powerOfAttorneyUserCardId: string, revocationType: PowerOfAttorneyRevocationType ,revocationReason: string ): Promise<any> {
        const data = {
            "powerOfAttorneyUserCardId": powerOfAttorneyUserCardId,
            "revocationType": revocationType,
            "revocationReason": revocationReason
        }
        return this.services.requestManager.post(`api/PowersOfAttorneyDemo/RequestRevocationPowerOfAttorney`, JSON.stringify(data));
    }

    sendForRegistrationToRegistry(powerOfAttorneyUserCardId:string, reprINN:string){
        const data={
            "powerOfAttorneyUserCardId":powerOfAttorneyUserCardId,
            "reprINN":reprINN
        }
        return this.services.requestManager.post(`api/M4dRegistryIntegration/Register`,JSON.stringify(data))
        
    }
}

export type $PowersOfAttorneyButtonController = {  powersOfAttorneyButtonController: PowersOfAttorneyButtonController }
export const $PowersOfAttorneyButtonController = serviceName((x: $PowersOfAttorneyButtonController) => x.powersOfAttorneyButtonController);