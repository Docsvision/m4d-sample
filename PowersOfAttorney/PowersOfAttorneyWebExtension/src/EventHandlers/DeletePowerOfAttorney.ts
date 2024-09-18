import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { $CardId } from "@docsvision/webclient/System/LayoutServices";
import { $PowersOfAttorneyDemoController } from "../ServerRequests/PowersOfAttorneyDemoController";

export const deletePowerOfAttorney = async (sender: LayoutControl) => {
    const powerOfAttorneyId = await sender.layout.controls.powerOfAttorneySysCard.params.value.cardId;
    if(powerOfAttorneyId) {
        await sender.layout.params.services.layoutCardController.delete({cardId: powerOfAttorneyId, isNew: false});
    } 
}