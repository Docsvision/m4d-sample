import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { Layout } from "@docsvision/webclient/System/Layout";
import { resources } from "@docsvision/webclient/System/Resources";
import IMask from "imask";
import { checkValueLength } from "../../Utils/CheckValueLength";

export const customizeSubstitutionPowerOfAttorneyCardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprSignCitshipSPOA = controls.reprSignCitshipSPOA;
    const indsignCitizenship = controls.indsignCitizenship;
    const possibilityOfSubstSPOA = controls.possibilityOfSubstSPOA;

    customizeInputFields(sender);
    onDataChangedPossibilityOfSubstSPOA(sender);
    onDataChangedReprSignCitshipSPOA(sender);
    onDataChangedIndsignCitizenship(sender);

    sender.params.beforeCardSaving.subscribe(checkPowersBeforeSaving);
    possibilityOfSubstSPOA && possibilityOfSubstSPOA.params.dataChanged.subscribe(onDataChangedPossibilityOfSubstSPOA);
    reprSignCitshipSPOA && reprSignCitshipSPOA.params.dataChanged.subscribe(onDataChangedReprSignCitshipSPOA);
    indsignCitizenship && indsignCitizenship.params.dataChanged.subscribe(onDataChangedIndsignCitizenship);
}

const checkPowersBeforeSaving = (sender: Layout, args: ICancelableEventArgs<ILayoutBeforeSavingEventArgs>) => {
    const refPowersTable = sender.controls.refPowersTable;
    const powersSPOA = sender.controls. powersSPOA;
    if (refPowersTable.params.rows.length === 0 && powersSPOA.params.rows.length === 0) {
        sender.params.services.messageWindow.showError(resources.Error_PowersEmpty);
        args.cancel();
    }
}

const limitations = [
    { name: "INNIndividual", length: 12 },
    { name: "indCodeCitizenship", length: 3 },
    { name: "reprINNSPOA", length: 12 },
    { name: "foreignReprCitshipSPOA", length: 3 }
]

const customizeInputFields = (sender: Layout) => {
    const maskOptions = {
        mask: '000-000-000 00'
    }

    limitations.forEach(limitation => {
        const element = document.querySelector(`[data-control-name="${limitation.name}"] input`);
        element.setAttribute("maxLength", `${limitation.length}`);
        sender.controls.get<TextBox>(limitation.name).params.blur.subscribe((sender: TextBox) => {   
            checkValueLength(element, sender.params.value.length, sender.layout.params.services, limitation.length);
        })
    })
    
    const SNILSIndividual = document.querySelector('[data-control-name="SNILSIndividual"] input') as HTMLElement;
    IMask(SNILSIndividual, maskOptions);
    sender.controls.SNILSIndividual.params.blur.subscribe((sender: TextBox) => {
        checkValueLength(SNILSIndividual, sender.params.value?.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })

    const reprSNILSSPOA = document.querySelector('[data-control-name="reprSNILSSPOA"]')?.getElementsByTagName('input')[0];
    IMask(reprSNILSSPOA, maskOptions);    
    sender.controls.reprSNILSSPOA.params.blur.subscribe((sender: TextBox) => {
        checkValueLength(reprSNILSSPOA, sender.params.value?.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })

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
    if (reprSignCitshipSPOA.params.value === 'foreignCitizen') {
        foreignReprCitshipSPOA.params.visibility = true;
        foreignReprCitshipSPOA.params.required = true;
    } else {
        foreignReprCitshipSPOA.params.value = "";
        foreignReprCitshipSPOA.params.visibility = false;
        foreignReprCitshipSPOA.params.required = false;
    }
}

const onDataChangedIndsignCitizenship = (sender: Layout) => {
    const controls = sender.layout.controls;
    const indsignCitizenship = controls.indsignCitizenship;
    const indCodeCitizenship = controls.indCodeCitizenship;
    if (indsignCitizenship.params.value === 'foreignCitizen') {
        indCodeCitizenship.params.visibility = true;
        indCodeCitizenship.params.required = true;
    } else {
        indCodeCitizenship.params.value = "";
        indCodeCitizenship.params.visibility = false;
        indCodeCitizenship.params.required = false;
    }
}
