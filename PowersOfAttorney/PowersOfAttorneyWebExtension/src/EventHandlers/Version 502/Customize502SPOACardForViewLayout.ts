import { Block } from "@docsvision/webclient/Platform/Block";
import { CardLink } from "@docsvision/webclient/Platform/CardLink";
import { CheckBox } from "@docsvision/webclient/Platform/CheckBox";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { Layout } from "@docsvision/webclient/System/Layout";

export const customize502SPOACardForViewLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const substPOABasis = controls.get<CheckBox>("substPOABasis");
    const parentalPOACardLink = controls.get<CardLink>("parentalPOACardLink");
    const reprType = controls.get<RadioGroup>("reprType");
    const reprEntityInfoBlock = controls.get<Block>("reprEntityInfoBlock");
    const regNumber = controls.get<TextBox>("regNumber");

    parentalPOACardLink.params.visibility = substPOABasis.params.value;
    reprEntityInfoBlock.params.visibility = reprType.params.value === "Entity";

    if (!regNumber.params.value) {
        regNumber.params.value = sender.layout.cardInfo.id;
        regNumber.save();
    }
}