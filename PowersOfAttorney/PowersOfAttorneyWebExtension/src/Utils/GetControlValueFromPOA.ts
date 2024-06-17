import { PlatformModeConditionTypes } from "@docsvision/webclient/Platform/PlatformModeConditionTypes";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";

export const getControlValueFromPOA = async (sender: LayoutControl, cardId: string, controlName: string, controlTypeName: string) => {
    const model = await sender.layout.params.services.layoutController.getPartWithParams({
        cardId: cardId, 
        layoutMode: PlatformModeConditionTypes.VIEW,
        locationName: undefined,
        controlName: controlName,
        layoutParams: null,
        loadOnlyChildren: false,
    })
    const element = model.layoutModel.children.find((x) => x.controlTypeName === controlTypeName);
    return element.properties.binding.value;
}