import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { Table } from "@docsvision/webclient/Platform/Table";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { IDataChangedEventArgs } from "@docsvision/webclient/System/IDataChangedEventArgs";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { Layout } from "@docsvision/webclient/System/Layout";
import { resources } from "@docsvision/webclient/System/Resources";
import IMask from "imask";
import { checkValueLength } from "../../Utils/CheckValueLength";


export const customizePowerOfAttorneyCardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const signCitizenshipfIAWPOA = controls.get<Dropdown>("signCitizenshipfIAWPOA");
    const kindCodeOfDocProvIdenIAWPOA = controls.get<Dropdown>("kindCodeOfDocProvIdenIAWPOA"); 
    const possibilityOfSubst = controls.get<RadioGroup>("possibilityOfSubst");
    const kindDocProvIdenRepr = controls.get<Dropdown>("kindDocProvIdenRepr");
    const reprSignCitizenship = controls.get<Dropdown>("reprSignCitizenship");
    
    customizeInputFields(sender);
    onDataChangedPossibilityOfSubst(sender);
    onDataChangedSignCitizenshipfIAWPOA(sender);
    onDataChangedReprSignCitizenship(sender);
    onDataChangedKindCodeOfDocProvIdenIAWPOA(sender);
    onDataChangedKindDocProvIdenRepr(sender);

    sender.params.beforeCardSaving.subscribe(checkPowersBeforeSaving);
    possibilityOfSubst && possibilityOfSubst.params.dataChanged.subscribe(onDataChangedPossibilityOfSubst);
    signCitizenshipfIAWPOA && signCitizenshipfIAWPOA.params.dataChanged.subscribe(onDataChangedSignCitizenshipfIAWPOA);
    kindCodeOfDocProvIdenIAWPOA && kindCodeOfDocProvIdenIAWPOA.params.dataChanged.subscribe(onDataChangedKindCodeOfDocProvIdenIAWPOA);
    kindDocProvIdenRepr && kindDocProvIdenRepr.params.dataChanged.subscribe(onDataChangedKindDocProvIdenRepr);
    reprSignCitizenship && reprSignCitizenship.params.dataChanged.subscribe(onDataChangedReprSignCitizenship);
}

const checkPowersBeforeSaving = (sender: Layout, args: ICancelableEventArgs<ILayoutBeforeSavingEventArgs>) => {
    const refPowersTable = sender.controls.get<Table>("refPowersTable");
    const textPowersTable = sender.controls.get<Table>("textPowersTable");
    if (refPowersTable.params.rows.length === 0 && textPowersTable.params.rows.length === 0) {
        sender.params.services.messageWindow.showError(resources.Error_PowersEmpty);
        args.cancel();
    }
}

const limitations = [
    { name: "IINIAWPOA", length: 12 },
    { name: "codeForeignCitizenshipIAWPOA", length: 3 },
    { name: "reprINN", length: 12 },
    { name: "foreignReprCitizenship", length: 3 }
]

const customizeInputFields = (sender: Layout) => {
    limitations.forEach(limitation => {
        const element = document.querySelector(`[data-control-name="${limitation.name}"] input`);
        element.setAttribute("maxLength", `${limitation.length}`);
        sender.controls.get<TextBox>(limitation.name).params.blur.subscribe((sender: TextBox) => {   
            checkValueLength(element, sender.params.value.length, sender.layout.params.services, limitation.length);
        })
    })

    document.querySelector('[data-control-name="numberDocProvIdenIAWPOA"]  input').setAttribute("maxLength", "25");
    document.querySelector('[data-control-name="numberDocProvIdenRepr"] input').setAttribute("maxLength", "25");
    document.querySelector('[data-control-name="authIssDocConfIdenRepr"] textarea').setAttribute("maxLength", "255");
    
    const maskOptions = {
        SNILS: {
            mask: '000-000-000 00'
        },
        code: {
            mask: '000-000'
        }
    }
    const SNILSIAWPOA = document.querySelector('[data-control-name="SNILSIAWPOA"] input') as HTMLElement;
    IMask(SNILSIAWPOA, maskOptions.SNILS);
    SNILSIAWPOA.addEventListener("change", (event) => sender.controls.SNILSIAWPOA.params.value = (event.target as HTMLInputElement).value);
    sender.controls.get<TextBox>("SNILSIAWPOA").params.blur.subscribe((sender: TextBox) => {
        checkValueLength(SNILSIAWPOA, sender.params.value?.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })

    const reprSNILS = document.querySelector('[data-control-name="reprSNILS"] input') as HTMLElement;
    IMask(reprSNILS, maskOptions.SNILS);
    reprSNILS.addEventListener("change", (event) => sender.controls.reprSNILS.params.value = (event.target as HTMLInputElement).value);
    sender.controls.get<TextBox>("reprSNILS").params.blur.subscribe((sender: TextBox) => {
        checkValueLength(reprSNILS, sender.params.value?.replaceAll("-", "").replaceAll(" ", "").length, sender.layout.params.services, 11);
    })

    const divCodeAuthIssDocProvIdenIAWPOA = document.querySelector('[data-control-name="divCodeAuthIssDocProvIdenIAWPOA"] input') as HTMLElement;
    IMask(divCodeAuthIssDocProvIdenIAWPOA, maskOptions.code);
    sender.controls.get<TextBox>("divCodeAuthIssDocProvIdenIAWPOA").params.blur.subscribe((sender: TextBox) => {
        checkValueLength(divCodeAuthIssDocProvIdenIAWPOA, sender.params.value?.replaceAll("-", "").replaceAll(" ", "").length, sender.layout.params.services, 6);
    })

    const divAuthIssDocConfIDOfRepr = document.querySelector('[data-control-name="divAuthIssDocConfIDOfRepr"] input') as HTMLElement;
    IMask(divAuthIssDocConfIDOfRepr, maskOptions.code);
    sender.controls.get<TextBox>("divAuthIssDocConfIDOfRepr").params.blur.subscribe((sender: TextBox) => {
        checkValueLength(divAuthIssDocConfIDOfRepr, sender.params.value?.replaceAll("-", "").replaceAll(" ", "").length, sender.layout.params.services, 6);
    })
}

const onDataChangedKindDocProvIdenRepr = (sender: Layout) => {
    const controls = sender.layout.controls;
    const kindDocProvIdenRepr = controls.get<Dropdown>("kindDocProvIdenRepr");
    const authIssDocConfIdenRepr = controls.get<TextArea>("authIssDocConfIdenRepr");
    const divAuthIssDocConfIDOfRepr = controls.get<TextBox>("divAuthIssDocConfIDOfRepr");

    if (kindDocProvIdenRepr.params.value === '21') {
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
    const kindCodeOfDocProvIdenIAWPOA = controls.get<Dropdown>("kindCodeOfDocProvIdenIAWPOA");
    const authIssDocProvIdenIAWPOA = controls.get<TextArea>("authIssDocProvIdenIAWPOA");
    const divCodeAuthIssDocProvIdenIAWPOA = controls.divCodeAuthIssDocProvIdenIAWPOA;

    if (kindCodeOfDocProvIdenIAWPOA.params.value === '21') {
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

    if (possibilityOfSubst.params.value === 'One-time substitution' || possibilityOfSubst.params.value === 'Substitution is possible with subsequent substitution') {
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
    if (signCitizenshipfIAWPOA.params.value === 'foreignCitizen') {
        codeForeignCitizenshipIAWPOA.params.visibility = true;
        codeForeignCitizenshipIAWPOA.params.required = true;
    } else {
        codeForeignCitizenshipIAWPOA.params.value = "";
        codeForeignCitizenshipIAWPOA.params.visibility = false;
        codeForeignCitizenshipIAWPOA.params.required = false;
    }
}

const onDataChangedReprSignCitizenship = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprSignCitizenship = controls.reprSignCitizenship;
    const foreignReprCitizenship = controls.foreignReprCitizenship;
    if (reprSignCitizenship.params.value === 'foreignCitizen') {
        foreignReprCitizenship.params.visibility = true;
        foreignReprCitizenship.params.required = true;
    } else {
        foreignReprCitizenship.params.value = "";
        foreignReprCitizenship.params.visibility = false;
        foreignReprCitizenship.params.required = false;
    }
}

