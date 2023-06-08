import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowerOfAttorneyApiController } from '@docsvision/webclient/Generated/DocsVision.WebClient.Controllers';;
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";

export const exportPowerOfAttorneyWithSignature = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    try {
        const url  = sender.layout.params.services.urlResolver.resolveApiUrl("exportPowerOfAttorney", "powerOfAttorneyApi");
        const request = `${url}?powerOfAttorneyId=${powerOfAttorneyId}&withSignature=true`;
        sender.layout.params.services.fileDownload.download(request);
    } catch (err) {
        throw err;
    } 
}

export const exportPowerOfAttorneyWithoutSignature = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    try {
        const url  = sender.layout.params.services.urlResolver.resolveApiUrl("exportPowerOfAttorney", "powerOfAttorneyApi");
        const request = `${url}?powerOfAttorneyId=${powerOfAttorneyId}&withSignature=false`;
        sender.layout.params.services.fileDownload.download(request);
    } catch (err) {
        throw err;
    } 
}