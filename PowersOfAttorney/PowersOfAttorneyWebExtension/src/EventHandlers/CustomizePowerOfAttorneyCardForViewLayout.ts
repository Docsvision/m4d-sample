import { Layout } from "@docsvision/webclient/System/Layout";

export const customizePowerOfAttorneyCardForViewCard = (sender: Layout) => {
    const controls = sender.layout.controls;
    const irrevocablePOA = controls.irrevocablePOA;
    const transferOfIrrevocablePOA = controls.transferOfIrrevocablePOA;
    const revocationCondition = controls.revocationCondition;
    const descrOfRevocCondition = controls.descrOfRevocCondition;
    const principalType = controls.principalType;
    const principalRusEntityBlock = controls.principalRusEntityBlock;
    const principalForeignEntityBlock = controls.principalForeignEntityBlock;
    const prinSPBlock = controls.prinSPBlock;
    const prinIndsBlock = controls.prinIndsBlock;
    const indicateEntity = controls.indicateEntity;
    const entityActingWithoutPOABlock = controls.entityActingWithoutPOABlock;
    const signCitizenshipfIAWPOA = controls.signCitizenshipfIAWPOA;
    const codeForeignCitizenshipIAWPOA = controls.codeForeignCitizenshipIAWPOA;;
    const authReprType = controls.authReprType;
    const entityReprBlock = controls.entityReprBlock;
    const solePropReprBlock = controls.solePropReprBlock;
    const indReprBlock = controls.indReprBlock;  
    const possibilityOfSubst = controls.possibilityOfSubst;
    const lossOfPowersUponSubstBlock = controls.lossOfPowersUponSubstBlock;

    if (signCitizenshipfIAWPOA.value === 'foreign citizen') {
        codeForeignCitizenshipIAWPOA.params.visibility = true;
    } else {
        codeForeignCitizenshipIAWPOA.params.visibility = false;
    }

    if (indicateEntity.value) {
        entityActingWithoutPOABlock.params.visibility = true;
    } else {
        entityActingWithoutPOABlock.params.visibility = false;
    }

    if (revocationCondition.value === 'Other terms of irrevocable power of attorney') {
        descrOfRevocCondition.params.visibility = true;
    }

    if (revocationCondition.value === 'Upon expiration') {
        descrOfRevocCondition.params.visibility = false;
    }

    if (irrevocablePOA.value === 'Without possibility of revocation') {
        transferOfIrrevocablePOA.params.visibility = true;
        revocationCondition.params.visibility = true;
    }

    if (irrevocablePOA.value === 'Revocation is possible') {
        transferOfIrrevocablePOA.params.visibility = false;
        revocationCondition.params.visibility = false;
    }

    if (principalType && principalRusEntityBlock && principalForeignEntityBlock && prinSPBlock && prinIndsBlock) {
        if (principalType.value == 'Russian entity') {
            principalRusEntityBlock.params.visibility = true;
            principalForeignEntityBlock.params.visibility = false;
            prinSPBlock.params.visibility = false;
            prinIndsBlock.params.visibility = false;
        }
    
        if (principalType.value == 'Foreign entity') {
            principalRusEntityBlock.params.visibility = false;
            principalForeignEntityBlock.params.visibility = true;
            prinSPBlock.params.visibility = false;
            prinIndsBlock.params.visibility = false;
        }
    
        if (principalType.value == 'Sole proprietor') {
            principalRusEntityBlock.params.visibility = false;
            principalForeignEntityBlock.params.visibility = false;
            prinSPBlock.params.visibility = true;
            prinIndsBlock.params.visibility = false;
        }
    
        if (principalType.value == 'Individual') {
            principalRusEntityBlock.params.visibility = false;
            principalForeignEntityBlock.params.visibility = false;
            prinSPBlock.params.visibility = false;
            prinIndsBlock.params.visibility = true;
        }
    }

    if (authReprType) {
        if (authReprType.value === 'Organization') {
            entityReprBlock.params.visibility = true;
            solePropReprBlock.params.visibility = false;
            indReprBlock.params.visibility = false;
        }
    
        if (authReprType.value === 'Sole proprietor') {
            entityReprBlock.params.visibility = false;
            solePropReprBlock.params.visibility = true;
            indReprBlock.params.visibility = false;
        }
    
        if (authReprType.value === 'Individual') {
            entityReprBlock.params.visibility = false;
            solePropReprBlock.params.visibility = false;
            indReprBlock.params.visibility = true;
        }
    }

    if (possibilityOfSubst.value === 'One-time substitution' || possibilityOfSubst.value === 'Substitution is possible with subsequent substitution') {
        lossOfPowersUponSubstBlock.params.visibility = true;
    } else if (possibilityOfSubst.value === 'Without right of substitution') {
        lossOfPowersUponSubstBlock.params.visibility = false;
    }
}
