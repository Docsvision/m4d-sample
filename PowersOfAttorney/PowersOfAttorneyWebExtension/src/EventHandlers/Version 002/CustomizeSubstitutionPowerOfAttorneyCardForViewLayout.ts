import { Layout } from "@docsvision/webclient/System/Layout";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { Block } from "@docsvision/webclient/Platform/Block";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";

export const customizeSubstitutionPowerOfAttorneyCardForViewLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const possibilityOfSubstSPOA = controls.get<RadioGroup>("possibilityOfSubstSPOA");
    const lossOfPowersUponSubstSPOABlock = controls.get<Block>("lossOfPowersUponSubstSPOABlock");
    const reprSignCitshipSPOA = controls.get<Dropdown>("reprSignCitshipSPOA");
    const foreignReprCitshipSPOA = controls.get<TextBox>("foreignReprCitshipSPOA");
    const indsignCitizenship = controls.get<Dropdown>("indsignCitizenship");
    const indCodeCitizenship = controls.get<TextBox>("indCodeCitizenship");

    if (possibilityOfSubstSPOA.value === 'One-time substitution' || possibilityOfSubstSPOA.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = true;
    } else if (possibilityOfSubstSPOA.value === 'Without right of substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = false;
    }

    if (reprSignCitshipSPOA.params.value === 'foreignCitizen') {
        foreignReprCitshipSPOA.params.visibility = true;
    } else {
        foreignReprCitshipSPOA.params.visibility = false;
    }

    if (indsignCitizenship.params.value === 'foreignCitizen') {
        indCodeCitizenship.params.visibility = true;
    } else {
        indCodeCitizenship.params.visibility = false;
    }

}