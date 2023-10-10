import { Powers } from "@docsvision/webclient/BackOffice/Powers";
import { EMPLOYEE_SECTION_ID, STAFF_DIRECTORY_ID } from "@docsvision/webclient/BackOffice/StaffDirectoryConstants";
import { StaffDirectoryItems } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";
import { Block } from "@docsvision/webclient/Platform/Block";
import { CheckBox } from "@docsvision/webclient/Platform/CheckBox";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { IRowEventArgs } from "@docsvision/webclient/Platform/IRowEventArgs";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { Table } from "@docsvision/webclient/Platform/Table";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { IDataChangedEventArgs } from "@docsvision/webclient/System/IDataChangedEventArgs";
import { IEventArgs } from "@docsvision/webclient/System/IEventArgs";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { Layout } from "@docsvision/webclient/System/Layout";
import { resources } from "@docsvision/webclient/System/Resources";
import IMask from "imask";
import { checkValueLength } from "../../Utils/CheckValueLength";

export const customizeSingleFormatSPOACardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const substPOABasis = controls.get<CheckBox>("substPOABasis");
    const ceo = controls.get<StaffDirectoryItems>("ceo");
    const representative = controls.get<StaffDirectoryItems>("representative");
    const powersType = controls.get<Dropdown>("powersType");
    const refPowersTable = controls.get<Table>("refPowersTable");
    const ceoCitizenshipSign = controls.get<Dropdown>("ceoCitizenshipSign");
    const reprCitizenshipSign = controls.get<Dropdown>("reprCitizenshipSign");
    const poaScope = controls.get<RadioGroup>("poaScope");

    onSubstPOABasisDataChanged(sender);
    onPowersTypeDataChanged(sender);
    customizeInputFields(sender);
    onPowersTypeDataChanged(sender);

    sender.params.beforeCardSaving.subscribe(checkPowersBeforeSaving);
    substPOABasis && substPOABasis.params.dataChanged.subscribe(onSubstPOABasisDataChanged);
    ceo && ceo.params.dataChanged.subscribe(onCeoDataChanged);
    representative && representative.params.dataChanged.subscribe(onRepresentativeDataChanged);
    powersType && powersType.params.dataChanged.subscribe(onPowersTypeDataChanged);
    refPowersTable && refPowersTable.params.rowAdded.subscribe(onRefPowersTableRowAdded);
    ceoCitizenshipSign && ceoCitizenshipSign.params.dataChanged.subscribe(onDataChangedCeoCitizenshipSign);
    reprCitizenshipSign && reprCitizenshipSign.params.dataChanged.subscribe(onDataChangedReprCitizenshipSign);
    poaScope && poaScope.params.dataChanged.subscribe(onDataChangedPoaScope);
}

const onDataChangedPoaScope = (sender: Layout) => {
    const controls = sender.layout.controls;
    const poaScope = controls.get<RadioGroup>("poaScope");
    const codeTaxAuthSubmitBlock = controls.get<Block>("codeTaxAuthSubmitBlock");
    const codeTaxAuthValidBlock = controls.get<Block>("codeTaxAuthValidBlock");
    const delegatorCitizenshipSignBlock = controls.get<Block>("delegatorCitizenshipSignBlock");
    const ceoAddressBlock = controls.get<Block>("ceoAddressBlock");
    const reprCitizenshipSignBlock = controls.get<Block>("reprCitizenshipSignBlock");
    const reprAddressBlock = controls.get<Block>("reprAddressBlock");
    const codeTaxAuthSubmit = controls.get<TextBox>("codeTaxAuthSubmit");
    const codeTaxAuthValid = controls.get<TextBox>("codeTaxAuthValid");
    const ceoCitizenshipSign = controls.get<Dropdown>("ceoCitizenshipSign");
    const reprCitizenshipSign = controls.get<Dropdown>("reprCitizenshipSign");
    const ceoAddrSubRus = controls.get<TextBox>("ceoAddrSubRus");
    const ceoAddrRus = controls.get<TextArea>("ceoAddrRus");
    const reprAddrSubRus = controls.get<TextBox>("reprAddrSubRus");
    const reprAddrRus = controls.get<TextArea>("reprAddrRus");

    if (poaScope.params.value === "B2B") {
        codeTaxAuthSubmitBlock.params.visibility = false;
        codeTaxAuthValidBlock.params.visibility = false;
        ceoAddressBlock.params.visibility = false;
        delegatorCitizenshipSignBlock.params.visibility = false;
        reprCitizenshipSignBlock.params.visibility == false;
        reprAddressBlock.params.visibility = false;
        codeTaxAuthSubmit.params.required = false;
        codeTaxAuthValid.params.required = false;
        ceoCitizenshipSign.params.required = false;
        reprCitizenshipSign.params.required = false;
        ceoAddrSubRus.params.required = false;
        ceoAddrRus.params.required = false;
        reprAddrSubRus.params.required = false;
        reprAddrRus.params.required = false;
    } else {
        codeTaxAuthSubmitBlock.params.visibility = true;
        codeTaxAuthValidBlock.params.visibility = true;
        delegatorCitizenshipSignBlock.params.visibility = true;
        ceoAddressBlock.params.visibility = true;
        reprCitizenshipSignBlock.params.visibility = true;
        reprAddressBlock.params.visibility = true;
        codeTaxAuthSubmit.params.required = true;
        codeTaxAuthValid.params.required = true;
        ceoCitizenshipSign.params.required = true;
        reprCitizenshipSign.params.required = true;
        ceoAddrSubRus.params.required = true;
        ceoAddrRus.params.required = true;
        reprAddrSubRus.params.required = true;
        reprAddrRus.params.required = true;
    }
}

const checkPowersBeforeSaving = (sender: Layout, args: ICancelableEventArgs<ILayoutBeforeSavingEventArgs>) => {
    const refPowersTable = sender.controls.get<Table>("refPowersTable");
    const powersType = sender.controls.get<Dropdown>("powersType");
    if (powersType.params.value === "machReadPower" && refPowersTable.params.rows.length === 0) {
        sender.params.services.messageWindow.showError(resources.Error_PowersEmpty);
        args.cancel();
    }
}

const onDataChangedCeoCitizenshipSign = (sender: Layout) => {
    const controls = sender.layout.controls;
    const ceoCitizenshipSign = controls.get<Dropdown>("ceoCitizenshipSign");
    const ceoCitizenship = controls.get<TextBox>("ceoCitizenship");
    if (ceoCitizenshipSign.params.value === 'statelessPerson') {
        ceoCitizenship.params.value = "";
        ceoCitizenship.params.visibility = false;
        ceoCitizenship.params.required = false;
    } else  if (ceoCitizenshipSign.params.value === 'rusCitizen') {
        ceoCitizenship.params.value = "643";
        ceoCitizenship.params.visibility = true;
        ceoCitizenship.params.required = true;
    } else {
        ceoCitizenship.params.value = "";
        ceoCitizenship.params.visibility = true;
        ceoCitizenship.params.required = true;
    }
}

const onDataChangedReprCitizenshipSign = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprCitizenshipSign = controls.get<Dropdown>("reprCitizenshipSign");
    const reprCitizenship = controls.get<TextBox>("reprCitizenship");
    if (reprCitizenshipSign.params.value === 'statelessPerson') {
        reprCitizenship.params.value = "";
        reprCitizenship.params.visibility = false;
        reprCitizenship.params.required = false;
    } else if (reprCitizenshipSign.params.value === 'rusCitizen') {
        reprCitizenship.params.value = "643";
        reprCitizenship.params.visibility = true;
        reprCitizenship.params.required = true;
    } else {
        reprCitizenship.params.value = "";
        reprCitizenship.params.visibility = true;
        reprCitizenship.params.required = true;
    }
}

const onRefPowersTableRowAdded = (sender: Table, args: IRowEventArgs) => {
    const refPowersCode = sender.layout.controls.refPowersCode;
    const index = sender.getRowIndex(args.rowId);
    refPowersCode[index].params.dataChanged.subscribe(onRefPowersCodeDataChanged);
}

const onRefPowersCodeDataChanged = (sender: Powers, args: IDataChangedEventArgs) => {
    if (args.newValue.code.length < 6) {
        sender.layout.params.services.messageWindow.showError(resources.Error_PowerCodeLength)
        sender.params.value = args.oldValue;
    }
    
}

const onSubstPOABasisDataChanged = (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const substPOABasis = controls.get<CheckBox>("substPOABasis");
    const parentalPOABlock = controls.get<Block>("parentalPOABlock");
    if (substPOABasis.params.value) {
        parentalPOABlock.params.visibility = true;
    } else {
        parentalPOABlock.params.visibility = false;
    }
}

const onCeoDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const ceoBirthDate = controls.get<DateTimePicker>("ceoBirthDate");
    const ceoGender = controls.get<Dropdown>("ceoGender");
    const ceoPhone = controls.get<TextBox>("ceoPhone");
    const ceoEmail = controls.get<TextBox>("ceoEmail");
    const numCEOID = controls.get<TextBox>("numCEOID");
    const authIssCEOID = controls.get<TextArea>("authIssCEOID");


    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${EMPLOYEE_SECTION_ID}/${args.newValue.id}`) as any;
        ceoBirthDate.params.value = data.fields.find(field => field.alias === "BirthDate").value;
        ceoGender.params.value = data.fields.find(field => field.alias === "Gender").value.toString();
        ceoPhone.params.value = data.fields.find(field => field.alias === "Phone").value;
        ceoEmail.params.value = data.fields.find(field => field.alias === "Email").value;
        numCEOID.params.value = data.fields.find(field => field.alias === "IDNumber").value;
        authIssCEOID.params.value = data.fields.find(field => field.alias === "IDIssuedBy").value;
    } else {
        ceoBirthDate.params.value = null;
        ceoGender.params.value = "";
        ceoPhone.params.value = "";
        ceoEmail.params.value = "";
        numCEOID.params.value = "";
        authIssCEOID.params.value = "";
    }  
}

const onRepresentativeDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const reprBirthDate = controls.get<DateTimePicker>("reprBirthDate");
    const reprGender = controls.get<Dropdown>("reprGender");
    const reprPhone = controls.get<TextBox>("reprPhone");
    const reprEmail = controls.get<TextBox>("reprEmail");
    const numReprID = controls.get<TextBox>("numReprID");
    const authIssReprID = controls.get<TextArea>("authIssReprID");

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${EMPLOYEE_SECTION_ID}/${args.newValue.id}`) as any;
        reprBirthDate.params.value = data.fields.find(field => field.alias === "BirthDate").value;
        reprGender.params.value = data.fields.find(field => field.alias === "Gender").value.toString();
        reprPhone.params.value = data.fields.find(field => field.alias === "Phone").value;
        reprEmail.params.value = data.fields.find(field => field.alias === "Email").value;
        numReprID.params.value = data.fields.find(field => field.alias === "IDNumber").value;
        authIssReprID.params.value = data.fields.find(field => field.alias === "IDIssuedBy").value;
    } else {
        reprBirthDate.params.value = null;
        reprGender.params.value = "";
        reprPhone.params.value = "";
        reprEmail.params.value = "";
        numReprID.params.value = "";
        authIssReprID.params.value = "";
    }
}

const onPowersTypeDataChanged = (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const powersType = controls.get<Dropdown>("powersType");
    const refPowersTable = controls.get<Table>("refPowersTable");
    const textPowersDescr = controls.get<TextArea>("textPowersDescr");
    if (powersType.params.value === "humReadPower") {
        textPowersDescr.params.visibility = true;
        textPowersDescr.params.required = true;
        refPowersTable.params.visibility = false;
    } else {
        textPowersDescr.params.visibility = false;
        textPowersDescr.params.required = false;
        refPowersTable.params.visibility = true;
    }
}

const limitations = [
    { name: "codeTaxAuthSubmit", length: 4 },
    { name: "codeTaxAuthValid", length: 4 },
    { name: "ceoINN", length: 12 },
    { name: "ceoCitizenship", length: 3 },
    { name: "ceoAddrSubRus", length: 2 },
    { name: "reprINN", length: 12 },
    { name: "reprCitizenship", length: 3 },
    { name: "reprAddrSubRus", length: 2}
   
]

const customizeInputFields = (sender: Layout) => {
    limitations.forEach(limitation => {
        const element = document.querySelector(`[data-control-name="${limitation.name}"] input`);
        element.setAttribute("maxLength", `${limitation.length}`);
        sender.controls.get<TextBox>(limitation.name).params.blur.subscribe((sender: TextBox) => {   
            checkValueLength(element, sender.params.value.length, sender.layout.params.services, limitation.length);
        })
    })

    const numReprID = document.querySelector('[data-control-name="numReprID"]');
    numReprID?.getElementsByTagName('input')[0].setAttribute("maxLength", "25");
    const numCEOID = document.querySelector('[data-control-name="numCEOID"]');
    numCEOID?.getElementsByTagName('input')[0].setAttribute("maxLength", "25");

    const maskOptions = {
        SNILS: {
            mask: '000-000-000 00'
        },
        code: {
            mask: '000-000'
        }
    }

    const ceoSNILS = document.querySelector('[data-control-name="ceoSNILS"] input') as HTMLElement;
    IMask(ceoSNILS, maskOptions.SNILS);
    ceoSNILS.addEventListener("change", (event) => sender.controls.ceoSNILS.params.value = (event.target as HTMLInputElement).value);
    sender.controls.ceoSNILS.params.blur.subscribe((sender: TextBox) => {
        checkValueLength(ceoSNILS, sender.params.value?.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })

    const reprSNILS = document.querySelector('[data-control-name="reprSNILS"] input') as HTMLElement;
    IMask(reprSNILS, maskOptions.SNILS);
    reprSNILS.addEventListener("change", (event) => sender.controls.reprSNILS.params.value = (event.target as HTMLInputElement).value);
    sender.controls.reprSNILS.params.blur.subscribe((sender: TextBox) => {
        checkValueLength(reprSNILS, sender.params.value?.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })

    const codeAuthIssCEOID = document.querySelector('[data-control-name="codeAuthIssCEOID"] input') as HTMLElement;
    IMask(codeAuthIssCEOID, maskOptions.code);
    sender.controls.codeAuthIssCEOID.params.blur.subscribe((sender: TextBox, args: IDataChangedEventArgs) => {
        checkValueLength(codeAuthIssCEOID, sender.params.value?.replaceAll("-", "").replaceAll(" ", "").length, sender.layout.params.services, 6);
    })

    const codeAuthIssReprID = document.querySelector('[data-control-name="codeAuthIssReprID"] input') as HTMLElement;
    IMask(codeAuthIssReprID, maskOptions.code);
    sender.controls.codeAuthIssReprID.params.blur.subscribe((sender: TextBox, args: IDataChangedEventArgs) => {
        checkValueLength(codeAuthIssReprID, sender.params.value?.replaceAll("-", "").replaceAll(" ", "").length, sender.layout.params.services, 6);
    })
}
