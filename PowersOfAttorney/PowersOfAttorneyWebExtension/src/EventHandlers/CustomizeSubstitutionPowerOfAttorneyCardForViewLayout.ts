import { Layout } from "@docsvision/webclient/System/Layout";

export const customizeSubstitutionPowerOfAttorneyCardForViewLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const possibilityOfSubstSPOA = controls.possibilityOfSubstSPOA;
    const lossOfPowersUponSubstSPOABlock = controls.lossOfPowersUponSubstSPOABlock;
    const reprSignCitshipSPOA = controls.reprSignCitshipSPOA;
    const foreignReprCitshipSPOA = controls.foreignReprCitshipSPOA;
    const indsignCitizenship = controls.indsignCitizenship;
    const indCodeCitizenship = controls.indCodeCitizenship;

    if (possibilityOfSubstSPOA.value === 'One-time substitution' || possibilityOfSubstSPOA.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = true;
    } else if (possibilityOfSubstSPOA.value === 'Without right of substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = false;
    }

    if (reprSignCitshipSPOA.params.value == "Foreign citizen") {
        foreignReprCitshipSPOA.params.visibility = true;
    } else {
        foreignReprCitshipSPOA.params.visibility = false;
    }

    if (indsignCitizenship.params.value == "Foreign citizen") {
        indCodeCitizenship.params.visibility = true;
    } else {
        indCodeCitizenship.params.visibility = false;
    }

}