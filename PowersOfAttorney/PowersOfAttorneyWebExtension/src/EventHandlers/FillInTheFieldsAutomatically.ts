import { EMPLOYEE_SECTION_ID, STAFF_DIRECTORY_ID, UNIT_STAFF_SECTION_ID } from "@docsvision/webclient/BackOffice/StaffDirectoryConstants";
import { StaffDirectoryItems } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { IDataChangedEventArgs } from "@docsvision/webclient/System/IDataChangedEventArgs";

export const onPrincipalDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
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

export const onCeoDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
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

export const onRepresentativeDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
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

export const onSignPossIssSubstDataChanged = (sender: RadioGroup, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    if (args.newValue === "withoutSubstitution") {
        controls.textPowersTable.params.columns[3].visibility = false;
        controls.refPowersTable.params.columns[3].visibility = false;  
    } else {
        controls.refPowersTable.params.columns[3].visibility = true;
        controls.textPowersTable.params.columns[3].visibility = true;
    }  
}