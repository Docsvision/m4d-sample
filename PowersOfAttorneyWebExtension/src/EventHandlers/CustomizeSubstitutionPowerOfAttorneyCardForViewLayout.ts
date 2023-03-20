import { Layout } from "@docsvision/webclient/System/Layout";

export const customizeSubstitutionPowerOfAttorneyCardForViewLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const possibilityOfSubstSPOA = controls.possibilityOfSubstSPOA;
    const lossOfPowersUponSubstSPOABlock = controls.lossOfPowersUponSubstSPOABlock;
    const transferOfIrrevocableSPOA = controls.transferOfIrrevocableSPOA;
    const irrevocableSPOA = controls.irrevocableSPOA;
    const revocationConditionSPOA = controls.revocationConditionSPOA;
    const descrOfRevocConditionSPOA = controls.descrOfRevocConditionSPOA;
    const subjSubstPowType = controls.subjSubstPowType;
    const rusEntityPOABlock = controls.rusEntityPOABlock;
    const forEntityPOABlock = controls.forEntityPOABlock;
    const sPBlock = controls.sPBlock;
    const indBlock = controls.indBlock;
    const authReprTypeSPOA = controls.authReprTypeSPOA;
    const entityReprSPOABlock = controls.entityReprSPOABlock;
    const solePrReprSPOABlock = controls.solePrReprSPOABlock;
    const indReprSPOABlock = controls.indReprSPOABlock;


    if (possibilityOfSubstSPOA.value === 'One-time substitution' || possibilityOfSubstSPOA.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = true;
    } else if (possibilityOfSubstSPOA.value === 'Without right of substitution') {
        lossOfPowersUponSubstSPOABlock.params.visibility = false;
    }

    if (irrevocableSPOA.value === 'Without possibility of revocation') {
        transferOfIrrevocableSPOA.params.visibility = true;
        revocationConditionSPOA.params.visibility = true;
    }

    if (irrevocableSPOA.value === 'Revocation is possible') {
        transferOfIrrevocableSPOA.params.visibility = false;
        revocationConditionSPOA.params.visibility = false;
    }

    if (revocationConditionSPOA.value === 'Other terms of irrevocable power of attorney') {
        descrOfRevocConditionSPOA.params.visibility = true;
    }

    if (revocationConditionSPOA.value === 'Upon expiration') {
        descrOfRevocConditionSPOA.params.visibility = false;
    }

    if (subjSubstPowType) {
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

    if (authReprTypeSPOA) {
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

}