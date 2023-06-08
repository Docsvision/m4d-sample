import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";

export const deletePowerOfAttorney = async (sender: LayoutControl) => {
    const powerOfAttorneyUserCardId = sender.layout.getService($CardId);
    const powerOfAttorneyId = await sender.layout.getService($PowersOfAttorneyDemoController).getPowerOfAttorneyCardId(powerOfAttorneyUserCardId);
    sender.layout.params.services.layoutCardController.delete({cardId: powerOfAttorneyId, isNew: false})
}