import { Layout } from "@docsvision/webclient/System/Layout";

export const customizePowerOfAttorneyCardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const irrevocablePOA = controls.irrevocablePOA;
    const revocationCondition = controls.revocationCondition;
    const principalType = controls.principalType;
    const indicateEntity = controls.indicateEntity;
    const signCitizenshipfIAWPOA = controls.signCitizenshipfIAWPOA;
    const kindCodeOfDocProvIdenIAWPOA = controls.kindCodeOfDocProvIdenIAWPOA;
    const authReprType = controls.authReprType;   
    const possibilityOfSubst = controls.possibilityOfSubst;
    const kindDocProvIdenRepr = controls.kindDocProvIdenRepr;
    const kindDocProvIdenReprSoleProp = controls.kindDocProvIdenReprSoleProp;
    const kindDocProvIdenIndReprEntity = controls.kindDocProvIdenIndReprEntity;
    const kindDocProvingIdenPI = controls.kindDocProvingIdenPI;
    const kindDocProvIdenPrinSP = controls.kindDocProvIdenPrinSP;
    const kindDocProvIdenHead = controls.kindDocProvIdenHead;
    
    onDataChangedPossibilityOfSubst(sender);
    onDataChangedIrrevocablePOA(sender);
    onDataChangedRevocationCondition(sender);
    onDataChangedPrincipalType(sender);
    onDataChangedIndicateEntity(sender);
    onDataChangedSignCitizenshipfIAWPOA(sender);
    onDataChangedKindCodeOfDocProvIdenIAWPOA(sender);
    onDataChangedAuthReprType(sender);
    onDataChangedKindDocProvIdenPrinSP(sender);
    onDataChangedKindDocProvingIdenPI(sender);
    onDataChangedKindDocProvIdenIndReprEntity(sender);
    onDataChangedKindDocProvIdenReprSoleProp(sender);
    onDataChangedKindDocProvIdenRepr(sender);
    onDataChangedKindDocProvIdenHead(sender);

    
    possibilityOfSubst.params.dataChanged.subscribe(onDataChangedPossibilityOfSubst);
    irrevocablePOA.params.dataChanged.subscribe(onDataChangedIrrevocablePOA);
    revocationCondition.params.dataChanged.subscribe(onDataChangedRevocationCondition);
    principalType && principalType.params.dataChanged.subscribe(onDataChangedPrincipalType);
    indicateEntity.params.dataChanged.subscribe(onDataChangedIndicateEntity);
    signCitizenshipfIAWPOA.params.dataChanged.subscribe(onDataChangedSignCitizenshipfIAWPOA);
    kindCodeOfDocProvIdenIAWPOA.params.dataChanged.subscribe(onDataChangedKindCodeOfDocProvIdenIAWPOA);
    authReprType && authReprType.params.dataChanged.subscribe(onDataChangedAuthReprType);
    kindDocProvIdenPrinSP && kindDocProvIdenPrinSP.params.dataChanged.subscribe(onDataChangedKindDocProvIdenPrinSP);
    kindDocProvingIdenPI && kindDocProvingIdenPI.params.dataChanged.subscribe(onDataChangedKindDocProvingIdenPI);
    kindDocProvIdenIndReprEntity && kindDocProvIdenIndReprEntity.params.dataChanged.subscribe(onDataChangedKindDocProvIdenIndReprEntity);
    kindDocProvIdenReprSoleProp && kindDocProvIdenReprSoleProp.params.dataChanged.subscribe(onDataChangedKindDocProvIdenReprSoleProp);
    kindDocProvIdenRepr.params.dataChanged.subscribe(onDataChangedKindDocProvIdenRepr);
    kindDocProvIdenHead && kindDocProvIdenHead.params.dataChanged.subscribe(onDataChangedKindDocProvIdenHead);
}



const onDataChangedKindDocProvIdenPrinSP = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenPrinSP = controls.kindDocProvIdenPrinSP;
    const authIssDocProvIdenPrinSP = controls.authIssDocProvIdenPrinSP;
    const divCodeAuthIssDocProvIdenPrinSP = controls.divCodeAuthIssDocProvIdenPrinSP;

    if (!kindDocProvIdenPrinSP) return;

    if (kindDocProvIdenPrinSP.value === '21') {
        authIssDocProvIdenPrinSP.params.required = true;
        divCodeAuthIssDocProvIdenPrinSP.params.required = true;
    } else {
        authIssDocProvIdenPrinSP.params.required = false;
        divCodeAuthIssDocProvIdenPrinSP.params.required = false;
    }
    authIssDocProvIdenPrinSP.forceUpdate();
    divCodeAuthIssDocProvIdenPrinSP.forceUpdate();
}

const onDataChangedKindDocProvingIdenPI = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvingIdenPI = controls.kindDocProvingIdenPI;
    const authIssDocProvIdenPI = controls.authIssDocProvIdenPI;
    const divCodeAuthIssDocPI = controls.divCodeAuthIssDocPI;

    if (!kindDocProvingIdenPI) return; 

    if (kindDocProvingIdenPI.value === '21') {
        authIssDocProvIdenPI.params.required = true;
        divCodeAuthIssDocPI.params.required = true;
    } else {
        authIssDocProvIdenPI.params.required = false;
        divCodeAuthIssDocPI.params.required = false;
    }
    authIssDocProvIdenPI.forceUpdate();
    divCodeAuthIssDocPI.forceUpdate();
}


const onDataChangedKindDocProvIdenIndReprEntity = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenIndReprEntity = controls.kindDocProvIdenIndReprEntity;
    const authIssDocProvIdenIndReprEntity = controls.authIssDocProvIdenIndReprEntity;
    const divAuthIssDocProvIdenIndReprEntity = controls.divAuthIssDocProvIdenIndReprEntity;

    if (!kindDocProvIdenIndReprEntity) return;

    if (kindDocProvIdenIndReprEntity.value === '21') {
        authIssDocProvIdenIndReprEntity.params.required = true;
        divAuthIssDocProvIdenIndReprEntity.params.required = true;
    } else {
        authIssDocProvIdenIndReprEntity.params.required = false;
        divAuthIssDocProvIdenIndReprEntity.params.required = false;
    }
    authIssDocProvIdenIndReprEntity.forceUpdate();
    divAuthIssDocProvIdenIndReprEntity.forceUpdate();
}

const onDataChangedKindDocProvIdenReprSoleProp = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenReprSoleProp = controls.kindDocProvIdenReprSoleProp;
    const authIssDocConfidenReprSoleProp = controls.authIssDocConfidenReprSoleProp;
    const divAuthIssDocConfIDReprSP = controls.divAuthIssDocConfIDReprSP;

    if (!kindDocProvIdenReprSoleProp) return;

    if (kindDocProvIdenReprSoleProp.value === '21') {
        authIssDocConfidenReprSoleProp.params.required = true;
        divAuthIssDocConfIDReprSP.params.required = true;
    } else {
        authIssDocConfidenReprSoleProp.params.required = false;
        divAuthIssDocConfIDReprSP.params.required = false;
    }
    authIssDocConfidenReprSoleProp.forceUpdate();
    divAuthIssDocConfIDReprSP.forceUpdate();
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

const onDataChangedIndicateEntity = (sender: Layout) => {
    const controls = sender.layout.controls;
    const indicateEntity = controls.indicateEntity;
    const entityActingWithoutPOABlock = controls.entityActingWithoutPOABlock;
    if (indicateEntity.value) {
        entityActingWithoutPOABlock.params.visibility = true;
    } else {
        entityActingWithoutPOABlock.params.visibility = false;
    }

}

const onDataChangedRevocationCondition = (sender: Layout) => {
    const controls = sender.layout.controls;
    const revocationCondition = controls.revocationCondition;
    const descrOfRevocCondition = controls.descrOfRevocCondition;
    const irrevocablePOA = controls.irrevocablePOA;
    if (revocationCondition.value === 'Other terms of irrevocable power of attorney' && irrevocablePOA.value === 'Without possibility of revocation') {
        descrOfRevocCondition.params.visibility = true;
        descrOfRevocCondition.params.required = true;
    }

    if (revocationCondition.value === 'Upon expiration') {
        descrOfRevocCondition.params.visibility = false;
        descrOfRevocCondition.params.required = false;
    }

}

const onDataChangedIrrevocablePOA = (sender: Layout) => {
    const controls = sender.layout.controls;
    const irrevocablePOA = controls.irrevocablePOA;
    const transferOfIrrevocablePOA = controls.transferOfIrrevocablePOA;
    const revocationCondition = controls.revocationCondition;
    const descrOfRevocCondition = controls.descrOfRevocCondition;
    if (irrevocablePOA.value === 'Without possibility of revocation') {
        transferOfIrrevocablePOA.params.visibility = true;
        revocationCondition.params.visibility = true;
        transferOfIrrevocablePOA.params.required = true;
        revocationCondition.params.required = true;
    }

    if (irrevocablePOA.value === 'Revocation is possible') {
        transferOfIrrevocablePOA.params.visibility = false;
        revocationCondition.params.visibility = false;
        transferOfIrrevocablePOA.params.required = false;
        revocationCondition.params.required = false;
        descrOfRevocCondition.params.visibility = false;
        descrOfRevocCondition.params.required = false;
    }
}

const onDataChangedPrincipalType = (sender: Layout) => {
    const controls = sender.layout.controls;
    const principalType = controls.principalType;
    const principalRusEntityBlock = controls.principalRusEntityBlock;
    const principalForeignEntityBlock = controls.principalForeignEntityBlock;
    const prinSPBlock = controls.prinSPBlock;
    const prinIndsBlock = controls.prinIndsBlock;

    if (!principalType || !principalRusEntityBlock || !principalForeignEntityBlock || prinSPBlock || prinIndsBlock) return;

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

const onDataChangedKindDocProvIdenHead = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenHead = controls.kindDocProvIdenHead;
    const authorityIssDocProvIdenHead = controls.authorityIssDocProvIdenHead;
    const divCodeAuthIssDocProvIdenHead = controls.divCodeAuthIssDocProvIdenHead;

    if(!kindDocProvIdenHead) return;

    if (kindDocProvIdenHead.value === '21') {
        authorityIssDocProvIdenHead.params.required = true;
        divCodeAuthIssDocProvIdenHead.params.required = true;
    } else {
        authorityIssDocProvIdenHead.params.required = false;
        divCodeAuthIssDocProvIdenHead.params.required = false;
    }
    authorityIssDocProvIdenHead.forceUpdate();
    divCodeAuthIssDocProvIdenHead.forceUpdate();
}

const onDataChangedAuthReprType = (sender: Layout) => {
    const controls = sender.layout.controls;
    const authReprType = controls.authReprType;
    const entityReprBlock = controls.entityReprBlock;
    const solePropReprBlock = controls.solePropReprBlock;
    const indReprBlock = controls.indReprBlock;

    if (!authReprType) return;

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