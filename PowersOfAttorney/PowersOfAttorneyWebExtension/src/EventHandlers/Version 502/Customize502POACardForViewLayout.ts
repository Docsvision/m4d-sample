import { Block } from "@docsvision/webclient/Platform/Block";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { Layout } from "@docsvision/webclient/System/Layout";

export const customize502POACardForViewLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const executiveBodyType = controls.get<RadioGroup>("executiveBodyType");
    const entityExecutiveBodyInfoBlock = controls.get<Block>("entityExecutiveBodyInfoBlock");
    const reprType = controls.get<RadioGroup>("reprType");
    const reprEntityInfoBlock = controls.get<Block>("reprEntityInfoBlock");
    const regNumber = controls.get<TextBox>("regNumber");
    
    entityExecutiveBodyInfoBlock.params.visibility = executiveBodyType.params.value === "Entity";
    reprEntityInfoBlock.params.visibility = reprType.params.value === "Entity";

    if (!regNumber.params.value) {
        regNumber.params.value = sender.layout.cardInfo.id;
        regNumber.save();
    }
}