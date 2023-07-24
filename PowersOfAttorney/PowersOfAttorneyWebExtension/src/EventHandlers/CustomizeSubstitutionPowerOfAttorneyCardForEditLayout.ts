import { Layout } from "@docsvision/webclient/System/Layout";
import IMask from "imask";

export const customizeSubstitutionPowerOfAttorneyCardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprSignCitshipSPOA = controls.reprSignCitshipSPOA;
    const indsignCitizenship = controls.indsignCitizenship;

    customizeInputFields()
    onDataChangedPossibilityOfSubstSPOA(sender);
    onDataChangedReprSignCitshipSPOA(sender);
    onDataChangedIndsignCitizenship(sender);

    controls.possibilityOfSubstSPOA.params.dataChanged.subscribe(onDataChangedPossibilityOfSubstSPOA);
    reprSignCitshipSPOA && reprSignCitshipSPOA.params.dataChanged.subscribe(onDataChangedReprSignCitshipSPOA);
    indsignCitizenship && indsignCitizenship.params.dataChanged.subscribe(onDataChangedIndsignCitizenship);
}

const customizeInputFields = () => {
    const INNIndividual = document.querySelector('[data-control-name="INNIndividual"]');
    INNIndividual?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const reprINNSPOA = document.querySelector('[data-control-name="reprINNSPOA"]');
    reprINNSPOA?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const maskOptions = {
        mask: '000-000-000 00'
    }
    const SNILSIndividualInputElement = document.querySelector('[data-control-name="SNILSIndividual"]')?.getElementsByTagName('input')[0];
    IMask(SNILSIndividualInputElement, maskOptions);
    const reprSNILSSPOAInputElement = document.querySelector('[data-control-name="reprSNILSSPOA"]')?.getElementsByTagName('input')[0];
    IMask(reprSNILSSPOAInputElement, maskOptions);    
}

const onDataChangedPossibilityOfSubstSPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const possibilityOfSubstSPOA = controls.possibilityOfSubstSPOA;
    const lossOfPowersUponSubstSPOABlock = controls.lossOfPowersUponSubstSPOABlock;
    const lossOfPowersUponSubstSPOA = controls.lossOfPowersUponSubstSPOA;

    if (possibilityOfSubstSPOA.params.value === 'One-time substitution' || possibilityOfSubstSPOA.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = true;
        lossOfPowersUponSubstSPOA.params.required = true;
    } else if (possibilityOfSubstSPOA.value === 'Without right of substitution') {
        lossOfPowersUponSubstSPOA.params.value = null;
        lossOfPowersUponSubstSPOA.params.required = false;
        lossOfPowersUponSubstSPOABlock.params.visibility = false;
    }

}

const onDataChangedReprSignCitshipSPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprSignCitshipSPOA = controls.reprSignCitshipSPOA;
    const foreignReprCitshipSPOA = controls.foreignReprCitshipSPOA;
    if (reprSignCitshipSPOA.params.value == "Foreign citizen") {
        foreignReprCitshipSPOA.params.visibility = true;
        foreignReprCitshipSPOA.params.required = true;
    } else {
        foreignReprCitshipSPOA.params.visibility = false;
        foreignReprCitshipSPOA.params.required = false;
    }
}

const onDataChangedIndsignCitizenship = (sender: Layout) => {
    const controls = sender.layout.controls;
    const indsignCitizenship = controls.indsignCitizenship;
    const indCodeCitizenship = controls.indCodeCitizenship;
    if (indsignCitizenship.params.value == "Foreign citizen") {
        indCodeCitizenship.params.visibility = true;
        indCodeCitizenship.params.required = true;
    } else {
        indCodeCitizenship.params.visibility = false;
        indCodeCitizenship.params.required = false;
    }
}
