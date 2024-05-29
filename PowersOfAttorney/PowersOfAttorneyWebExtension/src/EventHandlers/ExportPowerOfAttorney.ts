import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";

export const exportPowerOfAttorneyWithSignature = async (sender: CustomButton) => {
    try {
        sender.params.isLoading = true;
        const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
        const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);    
        const url  = sender.layout.params.services.urlResolver.resolveApiUrl("exportPowerOfAttorney", "powerOfAttorneyApi");
        const request = `${url}?powerOfAttorneyId=${powerOfAttorneyId}&withSignature=true`;
        sender.layout.params.services.fileDownload.download(request);
    } catch (err) {
        throw err;
    } finally {
        sender.params.isLoading = false;
    }
}

export const exportPowerOfAttorneyWithoutSignature = async (sender: CustomButton) => {
    try {
        sender.params.isLoading = true;
        const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
        const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);    
        const url  = sender.layout.params.services.urlResolver.resolveApiUrl("exportPowerOfAttorney", "powerOfAttorneyApi");
        const request = `${url}?powerOfAttorneyId=${powerOfAttorneyId}&withSignature=false`;
        sender.layout.params.services.fileDownload.download(request);
    } catch (err) {
        throw err;
    } finally {
        sender.params.isLoading = false;
    }
}