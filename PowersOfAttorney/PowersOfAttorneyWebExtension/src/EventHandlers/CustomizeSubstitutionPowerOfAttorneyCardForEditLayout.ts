import { Layout } from "@docsvision/webclient/System/Layout";
import IMask from "imask";

export const customizeSubstitutionPowerOfAttorneyCardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvingIdenInd = controls.kindDocProvingIdenInd;
    const kindDocProvIdenReprSPOA = controls.kindDocProvIdenReprSPOA;
    const reprSignCitshipSPOA = controls.reprSignCitshipSPOA;
    const indsignCitizenship = controls.indsignCitizenship;

    customizeInputFields()
    onDataChangedPossibilityOfSubstSPOA(sender);
    onDataChangedKindDocProvingIdenInd(sender);
    onDataChangedKindDocProvIdenReprSPOA(sender);
    onDataChangedReprSignCitshipSPOA(sender);
    onDataChangedIndsignCitizenship(sender);

    controls.possibilityOfSubstSPOA.params.dataChanged.subscribe(onDataChangedPossibilityOfSubstSPOA);
    kindDocProvingIdenInd.params.dataChanged.subscribe(onDataChangedKindDocProvingIdenInd);
    kindDocProvIdenReprSPOA && kindDocProvIdenReprSPOA.params.dataChanged.subscribe(onDataChangedKindDocProvIdenReprSPOA);
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

const onDataChangedKindDocProvIdenReprSPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenReprSPOA = controls.kindDocProvIdenReprSPOA;
    const authIssDocConfIdenReprSPOA = controls.authIssDocConfIdenReprSPOA;
    const divAuthIssDocConfIDOfReprSPOA = controls.divAuthIssDocConfIDOfReprSPOA;

    if (kindDocProvIdenReprSPOA.params.value === '21') {
        authIssDocConfIdenReprSPOA.params.required = true;
        divAuthIssDocConfIDOfReprSPOA.params.required = true;
    } else {
        authIssDocConfIdenReprSPOA.params.required = false;
        divAuthIssDocConfIDOfReprSPOA.params.required = false;
    }
    authIssDocConfIdenReprSPOA.forceUpdate();
    divAuthIssDocConfIDOfReprSPOA.forceUpdate();
}


const onDataChangedKindDocProvingIdenInd = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvingIdenInd = controls.kindDocProvingIdenInd;
    const authIssDocProvIdenInd = controls.authIssDocProvIdenInd;
    const divCodeAuthIssDocInd = controls.divCodeAuthIssDocInd;

    if (kindDocProvingIdenInd.params.value === '21') {
        authIssDocProvIdenInd.params.required = true;
        divCodeAuthIssDocInd.params.required = true;
    } else {
        authIssDocProvIdenInd.params.required = false;
        divCodeAuthIssDocInd.params.required = false;
    }
    authIssDocProvIdenInd.forceUpdate();
    divCodeAuthIssDocInd.forceUpdate();
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
