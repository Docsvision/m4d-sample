import { Powers } from "@docsvision/webclient/BackOffice/Powers";
import { EMPLOYEE_SECTION_ID, STAFF_DIRECTORY_ID, UNIT_STAFF_SECTION_ID } from "@docsvision/webclient/BackOffice/StaffDirectoryConstants";
import { StaffDirectoryItems } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";
import { IRowEventArgs } from "@docsvision/webclient/Platform/IRowEventArgs";
import { Table } from "@docsvision/webclient/Platform/Table";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { IDataChangedEventArgs } from "@docsvision/webclient/System/IDataChangedEventArgs";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { Layout } from "@docsvision/webclient/System/Layout";
import { resources } from "@docsvision/webclient/System/Resources";
import IMask from 'imask';
import { checkValueLength } from "../../Utils/CheckValueLength";

export const customizeSingleFormatPowerOfAttorneyForEditLayout = async (sender: Layout) => {
    const controls = sender.layout.controls;
    const entityPrincipal = controls.entityPrincipal;
    const ceo = controls.ceo;
    const representative = controls.representative;
    const powersType = controls.powersType;
    const refPowersTable = controls.refPowersTable;

    customizeInputFields(sender);
    onPowersTypeDataChanged(sender);

    sender.params.beforeCardSaving.subscribe(checkPowersBeforeSaving);
    entityPrincipal && entityPrincipal.params.dataChanged.subscribe(onPrincipalDataChanged);
    ceo && ceo.params.dataChanged.subscribe(onCeoDataChanged);
    representative && representative.params.dataChanged.subscribe(onRepresentativeDataChanged);
    powersType && powersType.params.dataChanged.subscribe(onPowersTypeDataChanged);
    refPowersTable && refPowersTable.params.rowAdded.subscribe(onRefPowersTableRowAdded);

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

const checkPowersBeforeSaving = (sender: Layout, args: ICancelableEventArgs<ILayoutBeforeSavingEventArgs>) => {
    const refPowersTable = sender.controls.refPowersTable;
    const powersType = sender.controls.powersType;
    if (powersType.params.value === "machReadPower" && refPowersTable.params.rows.length === 0) {
        sender.params.services.messageWindow.showError(resources.Error_PowersEmpty);
        args.cancel();
    }
}

const onPrincipalDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const princINN = controls.princINN;
    const princKPP = controls.princKPP;
    const princOGRN = controls.princOGRN;
    const princPhone = controls.princPhone;
    const princEmail = controls.princEmail;
    const princAddrRus = controls.princAddrRus;

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${UNIT_STAFF_SECTION_ID}/${args.newValue.id}`) as any;
        princINN.params.value = data.fields.find(field => field.alias === "INN").value;
        princKPP.params.value = data.fields.find(field => field.alias === "KPP").value;
        princOGRN.params.value = data.fields.find(field => field.alias === "OGRN").value;
        princPhone.params.value = data.fields.find(field => field.alias === "Phone").value;
        princEmail.params.value = data.fields.find(field => field.alias === "Email").value;

        const addressData = data.sections.find(section => section.id === "dc55dca5-5d69-4fc4-90b1-c62e93a91b73").rows[0];
        const zipCode = addressData.fields.find(field => field.alias === "ZipCode").value;
        const country = addressData.fields.find(field => field.alias === "Country").value;
        const city = addressData.fields.find(field => field.alias === "City").value;
        const address = addressData.fields.find(field => field.alias === "Address").value;
        princAddrRus.params.value = `${zipCode} ${country} ${city} ${address}`;
    } else {
        princINN.params.value = "";
        princKPP.params.value = "";
        princOGRN.params.value = "";
        princPhone.params.value = "";
        princEmail.params.value = "";
        princAddrRus.params.value = "";
    }
}

const onCeoDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const ceoPosition = controls.ceoPosition;
    const ceoBirthDate = controls.ceoBirthDate;
    const ceoGender = controls.ceoGender;
    const ceoPhone = controls.ceoPhone;
    const ceoEmail = controls.ceoEmail;
    const numCEOID = controls.numCEOID;
    const authIssCEOID = controls.authIssCEOID;

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${EMPLOYEE_SECTION_ID}/${args.newValue.id}`) as any;
        ceoPosition.params.value = data.fields.find(field => field.alias === "PositionName").value;
        ceoBirthDate.params.value = data.fields.find(field => field.alias === "BirthDate").value;
        ceoGender.params.value = data.fields.find(field => field.alias === "Gender").value.toString();
        ceoPhone.params.value = data.fields.find(field => field.alias === "Phone").value;
        ceoEmail.params.value = data.fields.find(field => field.alias === "Email").value;
        numCEOID.params.value = data.fields.find(field => field.alias === "IDNumber").value;
        authIssCEOID.params.value = data.fields.find(field => field.alias === "IDIssuedBy").value;
    } else {
        ceoPosition.params.value = "";
        ceoBirthDate.params.value = "";
        ceoGender.params.value = "";
        ceoPhone.params.value = "";
        ceoEmail.params.value = "";
        numCEOID.params.value = "";
        authIssCEOID.params.value = "";
    }  
}

const onRepresentativeDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const reprBirthDate = controls.reprBirthDate;
    const reprGender = controls.reprGender;
    const reprPhone = controls.reprPhone;
    const reprEmail = controls.reprEmail;
    const numReprID = controls.numReprID;
    const authIssReprID = controls.authIssReprID;

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${EMPLOYEE_SECTION_ID}/${args.newValue.id}`) as any;
        reprBirthDate.params.value = data.fields.find(field => field.alias === "BirthDate").value;
        reprGender.params.value = data.fields.find(field => field.alias === "Gender").value.toString();
        reprPhone.params.value = data.fields.find(field => field.alias === "Phone").value;
        reprEmail.params.value = data.fields.find(field => field.alias === "Email").value;
        numReprID.params.value = data.fields.find(field => field.alias === "IDNumber").value;
        authIssReprID.params.value = data.fields.find(field => field.alias === "IDIssuedBy").value;
    } else {
        reprBirthDate.params.value = "";
        reprGender.params.value = "";
        reprPhone.params.value = "";
        reprEmail.params.value = "";
        numReprID.params.value = "";
        authIssReprID.params.value = "";
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
    { name: "princINN", length: 10 },
    { name: "princKPP", length: 9 },
    { name: "princOGRN", length: 13 },
    { name: "princAddrSubRus", length: 2 }
   
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
    sender.controls.ceoSNILS.params.blur.subscribe((sender: TextBox) => {
        checkValueLength(ceoSNILS, sender.params.value.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })

    const reprSNILS = document.querySelector('[data-control-name="reprSNILS"] input') as HTMLElement;
    IMask(reprSNILS, maskOptions.SNILS);
    sender.controls.reprSNILS.params.blur.subscribe((sender: TextBox) => {
        checkValueLength(reprSNILS, sender.params.value.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })

    const codeAuthIssCEOID = document.querySelector('[data-control-name="codeAuthIssCEOID"] input') as HTMLElement;
    IMask(codeAuthIssCEOID, maskOptions.code);
    sender.controls.codeAuthIssCEOID.params.blur.subscribe((sender: TextBox, args: IDataChangedEventArgs) => {
        checkValueLength(codeAuthIssCEOID, args.newValue.replaceAll("-", "").replaceAll(" ", "").length, sender.layout.params.services, 6);
    })

    const codeAuthIssReprID = document.querySelector('[data-control-name="codeAuthIssReprID"] input') as HTMLElement;
    IMask(codeAuthIssReprID, maskOptions.code);
    sender.controls.codeAuthIssReprID.params.blur.subscribe((sender: TextBox, args: IDataChangedEventArgs) => {
        checkValueLength(codeAuthIssReprID, args.newValue.replaceAll("-", "").replaceAll(" ", "").length, sender.layout.params.services, 6);
    })
}

const onPowersTypeDataChanged = (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const powersType = controls.powersType;
    const refPowersTable = controls.refPowersTable;
    const textPowersDescr = controls.textPowersDescr;
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


