import { Layout } from "@docsvision/webclient/System/Layout";
import IMask from "imask";

export const customizePowerOfAttorneyCardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const signCitizenshipfIAWPOA = controls.signCitizenshipfIAWPOA;
    const kindCodeOfDocProvIdenIAWPOA = controls.kindCodeOfDocProvIdenIAWPOA; 
    const possibilityOfSubst = controls.possibilityOfSubst;
    const kindDocProvIdenRepr = controls.kindDocProvIdenRepr;
    
    customizeInputFields();
    onDataChangedPossibilityOfSubst(sender);
    onDataChangedSignCitizenshipfIAWPOA(sender);
    onDataChangedKindCodeOfDocProvIdenIAWPOA(sender);
    onDataChangedKindDocProvIdenRepr(sender);

    possibilityOfSubst.params.dataChanged.subscribe(onDataChangedPossibilityOfSubst);
    signCitizenshipfIAWPOA.params.dataChanged.subscribe(onDataChangedSignCitizenshipfIAWPOA);
    kindCodeOfDocProvIdenIAWPOA.params.dataChanged.subscribe(onDataChangedKindCodeOfDocProvIdenIAWPOA);
    kindDocProvIdenRepr.params.dataChanged.subscribe(onDataChangedKindDocProvIdenRepr);
}

const customizeInputFields = () => {
    const IINIAWPOA = document.querySelector('[data-control-name="IINIAWPOA"]');
    IINIAWPOA?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const reprINN = document.querySelector('[data-control-name="reprINN"]');
    reprINN?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const maskOptions = {
        mask: '000-000-000 00'
    }
    const SNILSIAWPOAInputElement = document.querySelector('[data-control-name="SNILSIAWPOA"]')?.getElementsByTagName('input')[0];
    IMask(SNILSIAWPOAInputElement, maskOptions);
    const reprSNILSInputElement = document.querySelector('[data-control-name="reprSNILS"]')?.getElementsByTagName('input')[0];
    IMask(reprSNILSInputElement, maskOptions);
}

const onDataChangedKindDocProvIdenRepr = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenRepr = controls.kindDocProvIdenRepr;
    const authIssDocConfIdenRepr = controls.authIssDocConfIdenRepr;
    const divAuthIssDocConfIDOfRepr = controls.divAuthIssDocConfIDOfRepr;

    if (kindDocProvIdenRepr.value === '21') {
        authIssDocConfIdenRepr.params.required = true;
        divAuthIssDocConfIDOfRepr.params.required = true;
    } else {
        authIssDocConfIdenRepr.params.required = false;
        divAuthIssDocConfIDOfRepr.params.required = false;
    }
    authIssDocConfIdenRepr.forceUpdate();
    divAuthIssDocConfIDOfRepr.forceUpdate();
}

const onDataChangedKindCodeOfDocProvIdenIAWPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindCodeOfDocProvIdenIAWPOA = controls.kindCodeOfDocProvIdenIAWPOA;
    const authIssDocProvIdenIAWPOA = controls.authIssDocProvIdenIAWPOA;
    const divCodeAuthIssDocProvIdenIAWPOA = controls.divCodeAuthIssDocProvIdenIAWPOA;

    if (kindCodeOfDocProvIdenIAWPOA.value === '21') {
        authIssDocProvIdenIAWPOA.params.required = true;
        divCodeAuthIssDocProvIdenIAWPOA.params.required = true;
    } else {
        authIssDocProvIdenIAWPOA.params.required = false;
        divCodeAuthIssDocProvIdenIAWPOA.params.required = false;
    }
    authIssDocProvIdenIAWPOA.forceUpdate();
    divCodeAuthIssDocProvIdenIAWPOA.forceUpdate();
}

const onDataChangedPossibilityOfSubst = (sender: Layout) => {
    const controls = sender.layout.controls;
    const possibilityOfSubst = controls.possibilityOfSubst;
    const lossOfPowersUponSubstBlock = controls.lossOfPowersUponSubstBlock;
    const lossOfPowersUponSubst = controls.lossOfPowersUponSubst;

    if (possibilityOfSubst.value === 'One-time substitution' || possibilityOfSubst.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstBlock.params.visibility = true;
        lossOfPowersUponSubst.params.required = true;
    } else if (possibilityOfSubst.value === 'Without right of substitution') {
        lossOfPowersUponSubst.params.value = null;
        lossOfPowersUponSubst.params.required = false;
        lossOfPowersUponSubstBlock.params.visibility = false;
    }
}

const onDataChangedSignCitizenshipfIAWPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const signCitizenshipfIAWPOA = controls.signCitizenshipfIAWPOA;
    const codeForeignCitizenshipIAWPOA = controls.codeForeignCitizenshipIAWPOA;
    if (signCitizenshipfIAWPOA.value === 'foreign citizen') {
        codeForeignCitizenshipIAWPOA.params.visibility = true;
    } else {
        codeForeignCitizenshipIAWPOA.params.visibility = false;
    }
}