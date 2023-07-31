import { EMPLOYEE_SECTION_ID, STAFF_DIRECTORY_ID, UNIT_STAFF_SECTION_ID } from "@docsvision/webclient/BackOffice/StaffDirectoryConstants";
import { StaffDirectoryItems } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { IDataChangedEventArgs } from "@docsvision/webclient/System/IDataChangedEventArgs";
import IMask from 'imask';

export const customizeSingleFormatPowerOfAttorneyForEditLayout = async (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const entityPrincipal = controls.entityPrincipal;
    const ceo = controls.ceo;
    const representative = controls.representative;
    const powersType = controls.powersType;

    customizeInputFields();
    onPowersTypeDataChanged(sender);

    entityPrincipal && entityPrincipal.params.dataChanged.subscribe(onPrincipalDataChanged);
    ceo && ceo.params.dataChanged.subscribe(onCeoDataChanged);
    representative && representative.params.dataChanged.subscribe(onRepresentativeDataChanged);
    powersType && powersType.params.dataChanged.subscribe(onPowersTypeDataChanged);

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


const customizeInputFields = () => {
    const codeTaxAuthSubmit = document.querySelector('[data-control-name="codeTaxAuthSubmit"]');
    codeTaxAuthSubmit?.getElementsByTagName('input')[0].setAttribute("maxLength", "4");
    const codeTaxAuthValid = document.querySelector('[data-control-name="codeTaxAuthValid"]');
    codeTaxAuthValid?.getElementsByTagName('input')[0].setAttribute("maxLength", "4");
    const ceoINN = document.querySelector('[data-control-name="ceoINN"]');
    ceoINN?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const ceoCitizenship = document.querySelector('[data-control-name="ceoCitizenship"]');
    ceoCitizenship?.getElementsByTagName('input')[0].setAttribute("maxLength", "3");
    const ceoAddrSubRus = document.querySelector('[data-control-name="ceoAddrSubRus"]');
    ceoAddrSubRus?.getElementsByTagName('input')[0].setAttribute("maxLength", "3");
    const reprINN = document.querySelector('[data-control-name="reprINN"]');
    reprINN?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const reprCitizenship = document.querySelector('[data-control-name="reprCitizenship"]');
    reprCitizenship?.getElementsByTagName('input')[0].setAttribute("maxLength", "3");
    const reprAddrSubRus = document.querySelector('[data-control-name="reprAddrSubRus"]');
    reprAddrSubRus?.getElementsByTagName('input')[0].setAttribute("maxLength", "2");
    const maskOptions = {
        mask: '000-000-000 00'
    }
    const ceoSNILSInputElement = document.querySelector('[data-control-name="ceoSNILS"]')?.getElementsByTagName('input')[0];
    IMask(ceoSNILSInputElement, maskOptions)
    const reprSNILSInputElement = document.querySelector('[data-control-name="reprSNILS"]')?.getElementsByTagName('input')[0];
    IMask(reprSNILSInputElement, maskOptions)
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


