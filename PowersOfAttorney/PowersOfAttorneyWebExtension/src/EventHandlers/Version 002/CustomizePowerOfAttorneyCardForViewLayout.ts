import { Layout } from "@docsvision/webclient/System/Layout";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { Block } from "@docsvision/webclient/Platform/Block";

export const customizePowerOfAttorneyCardForViewCard = (sender: Layout) => {
    const controls = sender.layout.controls;
    const signCitizenshipfIAWPOA = controls.get<Dropdown>("signCitizenshipfIAWPOA");
    const codeForeignCitizenshipIAWPOA = controls.get<TextBox>("codeForeignCitizenshipIAWPOA");
    const possibilityOfSubst = controls.get<RadioGroup>("possibilityOfSubst");
    const lossOfPowersUponSubstBlock = controls.get<Block>("lossOfPowersUponSubstBlock");
    const reprSignCitizenship = controls.get<Dropdown>("reprSignCitizenship");
    const foreignReprCitizenship = controls.get<TextBox>("foreignReprCitizenship");

    if (signCitizenshipfIAWPOA.value === 'foreignCitizen') {
        codeForeignCitizenshipIAWPOA.params.visibility = true;
    } else {
        codeForeignCitizenshipIAWPOA.params.visibility = false;
    }

    if (reprSignCitizenship.params.value === 'foreignCitizen') {
        foreignReprCitizenship.params.visibility = true;
    } else {
        foreignReprCitizenship.params.visibility = false;
    }

    if (possibilityOfSubst.value === 'One-time substitution' || possibilityOfSubst.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstBlock.params.visibility = true;
    } else if (possibilityOfSubst.value === 'Without right of substitution') {
        lossOfPowersUponSubstBlock.params.visibility = false;
    }
}
