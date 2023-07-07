import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";

export const exportApplicationForRevocation = async (sender: CustomButton) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    try {
        const url  = sender.layout.params.services.urlResolver.resolveApiUrl("exportPowerOfAttorneyRevocation", "powerOfAttorneyApi");
        const request = `${url}?powerOfAttorneyId=${powerOfAttorneyId}`;
        sender.layout.params.services.fileDownload.download(request);
    } catch (err) {
        throw err;
    } 
}