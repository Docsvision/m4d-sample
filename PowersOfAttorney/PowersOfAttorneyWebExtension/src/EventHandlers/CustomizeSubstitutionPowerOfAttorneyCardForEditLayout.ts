import { Layout } from "@docsvision/webclient/System/Layout";
import IMask from "imask";

export const customizeSubstitutionPowerOfAttorneyCardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const irrevocableSPOA = controls.irrevocableSPOA;
    const subjSubstPowType = controls.subjSubstPowType;
    const authReprTypeSPOA = controls.authReprTypeSPOA;
    const kindDocProvIdenSP = controls.kindDocProvIdenSP;
    const kindDocProvingIdenInd = controls.kindDocProvingIdenInd;
    const kindDocProvIdenIndReprEntitySPOA = controls.kindDocProvIdenIndReprEntitySPOA;
    const kindDocProvIdenReprSolePrSPOA = controls.kindDocProvIdenReprSolePrSPOA;
    const kindDocProvIdenReprSPOA = controls.kindDocProvIdenReprSPOA;
    const revocationConditionSPOA = controls.revocationConditionSPOA;

    customizeInputFields()
    onDataChangedPossibilityOfSubstSPOA(sender);
    onDataChangedIrrevocableSPOA(sender);
    onDataChangedSubjSubstPowType(sender);
    onDataChangedAuthReprTypeSPOA(sender);
    onDataChangedKindDocProvIdenSP(sender);
    onDataChangedKindDocProvingIdenInd(sender);
    onDataChangedKindDocProvIdenIndReprEntitySPOA(sender);
    onDataChangedKindDocProvIdenReprSolePrSPOA(sender);
    onDataChangedKindDocProvIdenReprSPOA(sender);
    onDataChangedConditionSPOA(sender);

    controls.possibilityOfSubstSPOA.params.dataChanged.subscribe(onDataChangedPossibilityOfSubstSPOA);
    irrevocableSPOA.params.dataChanged.subscribe(onDataChangedIrrevocableSPOA);
    subjSubstPowType && subjSubstPowType.params.dataChanged.subscribe(onDataChangedSubjSubstPowType);
    authReprTypeSPOA && authReprTypeSPOA.params.dataChanged.subscribe(onDataChangedAuthReprTypeSPOA);
    
    kindDocProvIdenSP && kindDocProvIdenSP.params.dataChanged.subscribe(onDataChangedKindDocProvIdenSP);
    kindDocProvingIdenInd.params.dataChanged.subscribe(onDataChangedKindDocProvingIdenInd);
    kindDocProvIdenIndReprEntitySPOA && kindDocProvIdenIndReprEntitySPOA.params.dataChanged.subscribe(onDataChangedKindDocProvIdenIndReprEntitySPOA);
    kindDocProvIdenReprSolePrSPOA && kindDocProvIdenReprSolePrSPOA.params.dataChanged.subscribe(onDataChangedKindDocProvIdenReprSolePrSPOA);
    kindDocProvIdenReprSPOA.params.dataChanged.subscribe(onDataChangedKindDocProvIdenReprSPOA);
    revocationConditionSPOA.params.dataChanged.subscribe(onDataChangedConditionSPOA);
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

    if (kindDocProvIdenReprSPOA.value === '21') {
        authIssDocConfIdenReprSPOA.params.required = true;
        divAuthIssDocConfIDOfReprSPOA.params.required = true;
    } else {
        authIssDocConfIdenReprSPOA.params.required = false;
        divAuthIssDocConfIDOfReprSPOA.params.required = false;
    }
    authIssDocConfIdenReprSPOA.forceUpdate();
    divAuthIssDocConfIDOfReprSPOA.forceUpdate();
}

const onDataChangedKindDocProvIdenReprSolePrSPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenReprSolePrSPOA = controls.kindDocProvIdenReprSolePrSPOA;
    const authIssDocConfidenReprSolePrSPOA = controls.divAuthIssDocConfIDReprSPSPOA;
    const divAuthIssDocConfIDReprSPSPOA = controls.divAuthIssDocConfIDOfReprSPOA;

    if (!kindDocProvIdenReprSolePrSPOA) return;

    if (kindDocProvIdenReprSolePrSPOA.value === '21') {
        authIssDocConfidenReprSolePrSPOA.params.required = true;
        divAuthIssDocConfIDReprSPSPOA.params.required = true;
    } else {
        authIssDocConfidenReprSolePrSPOA.params.required = false;
        divAuthIssDocConfIDReprSPSPOA.params.required = false;
    }
    authIssDocConfidenReprSolePrSPOA.forceUpdate();
    divAuthIssDocConfIDReprSPSPOA.forceUpdate();
}


const onDataChangedKindDocProvIdenIndReprEntitySPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenIndReprEntitySPOA = controls.kindDocProvIdenIndReprEntitySPOA;
    const authIssDocProvIdenIndReprEntitySPOA = controls.authIssDocProvIdenIndReprEntitySPOA;
    const divAuthIssDocProvIdenIndReprEntitySPOA = controls.divAuthIssDocProvIdenIndReprEntitySPOA;

    if (!kindDocProvIdenIndReprEntitySPOA) return;

    if (kindDocProvIdenIndReprEntitySPOA.value === '21') {
        authIssDocProvIdenIndReprEntitySPOA.params.required = true;
        divAuthIssDocProvIdenIndReprEntitySPOA.params.required = true;
    } else {
        authIssDocProvIdenIndReprEntitySPOA.params.required = false;
        divAuthIssDocProvIdenIndReprEntitySPOA.params.required = false;
    }
    authIssDocProvIdenIndReprEntitySPOA.forceUpdate();
    divAuthIssDocProvIdenIndReprEntitySPOA.forceUpdate();
}

const onDataChangedKindDocProvingIdenInd = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvingIdenInd = controls.kindDocProvingIdenInd;
    const authIssDocProvIdenInd = controls.authIssDocProvIdenInd;
    const divCodeAuthIssDocInd = controls.divCodeAuthIssDocInd;

    if (kindDocProvingIdenInd.value === '21') {
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

    if (possibilityOfSubstSPOA.value === 'One-time substitution' || possibilityOfSubstSPOA.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = true;
        lossOfPowersUponSubstSPOA.params.required = true;
    } else if (possibilityOfSubstSPOA.value === 'Without right of substitution') {
        lossOfPowersUponSubstSPOA.params.required = false;
        lossOfPowersUponSubstSPOABlock.params.visibility = false;
    }

}

const onDataChangedConditionSPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const revocationConditionSPOA = controls.revocationConditionSPOA;
    const descrOfRevocConditionSPOA = controls.descrOfRevocConditionSPOA;
    const irrevocableSPOA = controls.irrevocableSPOA;
    if (revocationConditionSPOA.value === 'Other terms of irrevocable power of attorney' &&  irrevocableSPOA.value === 'Without possibility of revocation') {
        descrOfRevocConditionSPOA.params.visibility = true;
        descrOfRevocConditionSPOA.params.required = true;
    }

    if (revocationConditionSPOA.value === 'Upon expiration') {
        descrOfRevocConditionSPOA.params.visibility = false;
        descrOfRevocConditionSPOA.params.required = false;
    }

}

const onDataChangedIrrevocableSPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const irrevocableSPOA = controls.irrevocableSPOA;
    const transferOfIrrevocableSPOA = controls.transferOfIrrevocableSPOA;
    const revocationConditionSPOA = controls.revocationConditionSPOA;
    const descrOfRevocConditionSPOA = controls.descrOfRevocConditionSPOA;
    if (irrevocableSPOA.value === 'Without possibility of revocation') {
        transferOfIrrevocableSPOA.params.visibility = true;
        revocationConditionSPOA.params.visibility = true;
        transferOfIrrevocableSPOA.params.required = true;
        revocationConditionSPOA.params.required = true;
    }

    if (irrevocableSPOA.value === 'Revocation is possible') {
        transferOfIrrevocableSPOA.params.visibility = false;
        revocationConditionSPOA.params.visibility = false;
        transferOfIrrevocableSPOA.params.required = false;
        revocationConditionSPOA.params.required = false;
        descrOfRevocConditionSPOA.params.visibility = false;
        descrOfRevocConditionSPOA.params.required = false;
    }
}

const onDataChangedSubjSubstPowType = (sender: Layout) => {
    const controls = sender.layout.controls;
    const subjSubstPowType = controls.subjSubstPowType;
    const rusEntityPOABlock = controls.rusEntityPOABlock;
    const forEntityPOABlock = controls.forEntityPOABlock;
    const sPBlock = controls.sPBlock;
    const indBlock = controls.indBlock;

    if (!subjSubstPowType) return;

    if (subjSubstPowType.value === 'Russian entity') {
        rusEntityPOABlock.params.visibility = true;
        forEntityPOABlock.params.visibility = false;
        sPBlock.params.visibility = false;
        indBlock.params.visibility = false;
    }

    if (subjSubstPowType.value === 'Foreign entity') {
        rusEntityPOABlock.params.visibility = false;
        forEntityPOABlock.params.visibility = true;
        sPBlock.params.visibility = false;
        indBlock.params.visibility = false;
    }

    if (subjSubstPowType.value === 'Sole proprietor') {
        rusEntityPOABlock.params.visibility = false;
        forEntityPOABlock.params.visibility = false;
        sPBlock.params.visibility = true;
        indBlock.params.visibility = false;
    }

    if (subjSubstPowType.value === 'Individual') {
        rusEntityPOABlock.params.visibility = false;
        forEntityPOABlock.params.visibility = false;
        sPBlock.params.visibility = false;
        indBlock.params.visibility = true;
    }
}

const onDataChangedAuthReprTypeSPOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const authReprTypeSPOA = controls.authReprTypeSPOA;
    const entityReprSPOABlock = controls.entityReprSPOABlock;
    const solePrReprSPOABlock = controls.solePrReprSPOABlock;
    const indReprSPOABlock = controls.indReprSPOABlock;

    if (!authReprTypeSPOA) return;

    if (authReprTypeSPOA.value === 'Organization') {
        entityReprSPOABlock.params.visibility = true;
        solePrReprSPOABlock.params.visibility = false;
        indReprSPOABlock.params.visibility = false;
    }

    if (authReprTypeSPOA.value === 'Sole proprietor') {
        entityReprSPOABlock.params.visibility = false;
        solePrReprSPOABlock.params.visibility = true;
        indReprSPOABlock.params.visibility = false;
    }

    if (authReprTypeSPOA.value === 'Individual') {
        entityReprSPOABlock.params.visibility = false;
        solePrReprSPOABlock.params.visibility = false;
        indReprSPOABlock.params.visibility = true;
    }
}


const onDataChangedKindDocProvIdenSP = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenSP = controls.kindDocProvIdenSP;
    const authIssDocProvIdenSP = controls.authIssDocProvIdenSP;
    const divCodeAuthIssDocProvIdenSP = controls.divCodeAuthIssDocProvIdenSP;

    if (!kindDocProvIdenSP) return;

    if (kindDocProvIdenSP.value === '21') {
        authIssDocProvIdenSP.params.required = true;
        divCodeAuthIssDocProvIdenSP.params.required = true;
    } else {
        authIssDocProvIdenSP.params.required = false;
        divCodeAuthIssDocProvIdenSP.params.required = false;
    }
    authIssDocProvIdenSP.forceUpdate();
    divCodeAuthIssDocProvIdenSP.forceUpdate();
}
