import { Layout } from "@docsvision/webclient/System/Layout";

export const customizePowerOfAttorneyCardForViewCard = (sender: Layout) => {
    const controls = sender.layout.controls;
    const signCitizenshipfIAWPOA = controls.signCitizenshipfIAWPOA;
    const codeForeignCitizenshipIAWPOA = controls.codeForeignCitizenshipIAWPOA;;
    const possibilityOfSubst = controls.possibilityOfSubst;
    const lossOfPowersUponSubstBlock = controls.lossOfPowersUponSubstBlock;

    if (signCitizenshipfIAWPOA.value === 'foreign citizen') {
        codeForeignCitizenshipIAWPOA.params.visibility = true;
    } else {
        codeForeignCitizenshipIAWPOA.params.visibility = false;
    }

    if (possibilityOfSubst.value === 'One-time substitution' || possibilityOfSubst.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstBlock.params.visibility = true;
    } else if (possibilityOfSubst.value === 'Without right of substitution') {
        lossOfPowersUponSubstBlock.params.visibility = false;
    }
}
