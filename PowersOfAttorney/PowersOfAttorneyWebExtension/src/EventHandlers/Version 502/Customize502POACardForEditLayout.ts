import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { Layout } from "@docsvision/webclient/System/Layout";
import { Block } from "@docsvision/webclient/Platform/Block";
import { StaffDirectoryItems } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";
import { IDataChangedEventArgs } from "@docsvision/webclient/System/IDataChangedEventArgs";
import { EMPLOYEE_SECTION_ID, STAFF_DIRECTORY_ID, UNIT_STAFF_SECTION_ID } from "@docsvision/webclient/BackOffice/StaffDirectoryConstants";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { ILimitation, setLimitInControls } from "../../Utils/SetLimitInControls";
import { setSnilsMask } from "../../Utils/SetSnilsMask";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { Table } from "@docsvision/webclient/Platform/Table";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { resources } from "@docsvision/webclient/System/Resources";

const limitations: ILimitation[] = [
    { name: "codeTaxAuthSubmit", length: 4 },
    { name: "codeTaxAuthValid", length: 4 },
    { name: "princINN", length: 10 },
    { name: "princKPP", length: 9 },
    { name: "princOGRN", length: 13 },
    { name: "entityExecutiveBodyINN", length: 10 },
    { name: "entityExecutiveBodyKPP", length: 9 },
    { name: "ceoINN", length: 12 },
    { name: "reprEntityINN", length: 10 },
    { name: "reprEntityKPP", length: 9 }
]

export const customize502POACardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const entityPrincipal = controls.get<StaffDirectoryItems>("entityPrincipal");
    const executiveBodyType = controls.get<Dropdown>("executiveBodyType");
    const entityExecutiveBody = controls.get<StaffDirectoryItems>("entityExecutiveBody");
    const ceo = controls.get<StaffDirectoryItems>("ceo");
    const reprType = controls.get<Dropdown>("reprType");
    const reprID = controls.get<Dropdown>("reprID");

    customizeInputFields(sender);
    onExecutiveBodyTypeDataChanged(sender);
    onReprTypeDataChanged(sender);
    onReprIDDataChanged(sender);
    
    sender.params.beforeCardSaving.subscribe(checkPowersBeforeSaving);
    entityPrincipal && entityPrincipal.params.dataChanged.subscribe(onPrincipalDataChanged);
    entityExecutiveBody && entityExecutiveBody.params.dataChanged.subscribe(onEntityExecutiveBodyDataChanged);
    executiveBodyType && executiveBodyType.params.dataChanged.subscribe(onExecutiveBodyTypeDataChanged);
    ceo && ceo.params.dataChanged.subscribe(onCeoDataChanged);
    reprType && reprType.params.dataChanged.subscribe(onReprTypeDataChanged);
    reprID && reprID.params.dataChanged.subscribe(onReprIDDataChanged);
}

const customizeInputFields = (sender: Layout) => {
    setLimitInControls(sender, limitations);
    setSnilsMask(sender, "ceoSNILS");
    setSnilsMask(sender, "reprSNILS");
    document.querySelector('[data-control-name="princAddrRus"] textarea').setAttribute("maxLength", "255");
}

const checkPowersBeforeSaving = (sender: Layout, args: ICancelableEventArgs<ILayoutBeforeSavingEventArgs>) => {
    const refPowersTable = sender.controls.get<Table>("refPowersTable");    
    if (refPowersTable.params.rows.length === 0) {
        sender.params.services.messageWindow.showError(resources.Error_PowersEmpty);
        args.cancel();
    }
}

const onPrincipalDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const princINN = controls.get<TextArea>("princINN");
    const princKPP = controls.get<TextArea>("princKPP");
    const princOGRN = controls.get<TextArea>("princOGRN");
    const princAddrRus = controls.get<TextArea>("princAddrRus");

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${UNIT_STAFF_SECTION_ID}/${args.newValue.id}`) as any;
        princINN.params.value = data.fields.find(field => field.alias === "INN").value;
        princKPP.params.value = data.fields.find(field => field.alias === "KPP").value;
        princOGRN.params.value = data.fields.find(field => field.alias === "OGRN").value;
    
        const addressSection = data.sections.find(section => section.id === "dc55dca5-5d69-4fc4-90b1-c62e93a91b73");
        const addressData = addressSection.rows.length ? addressSection.rows[0] : null;
        const zipCode = addressData?.fields.find(field => field.alias === "ZipCode").value || "";
        const country = addressData?.fields.find(field => field.alias === "Country").value || "";
        const city = addressData?.fields.find(field => field.alias === "City").value || "";
        const address = addressData?.fields.find(field => field.alias === "Address").value || "";
        princAddrRus.params.value = `${zipCode} ${country} ${city} ${address}`;
    } else {
        princINN.params.value = "";
        princKPP.params.value = "";
        princOGRN.params.value = "";
        princAddrRus.params.value = "";
    }
}

const onExecutiveBodyTypeDataChanged = (sender: Layout) => {
    const controls = sender.layout.controls;
    const executiveBodyType = controls.get<RadioGroup>("executiveBodyType");
    const entityExecutiveBodyInfoBlock = controls.get<Block>("entityExecutiveBodyInfoBlock");
    const entityExecutiveBody = controls.get<StaffDirectoryItems>("entityExecutiveBody");
    const entityExecutiveBodyINN = controls.get<TextBox>("entityExecutiveBodyINN");
    const entityExecutiveBodyKPP = controls.get<TextBox>("entityExecutiveBodyKPP");
    const entityValue = executiveBodyType.params.value === "Entity";

    entityExecutiveBodyInfoBlock.params.visibility = entityValue;
    entityExecutiveBody.params.required = entityValue;
    entityExecutiveBodyINN.params.required = entityValue;
    entityExecutiveBodyKPP.params.required = entityValue; 
    if (!entityValue) {
        entityExecutiveBody.params.value = null;
        entityExecutiveBodyINN.params.value = "";
        entityExecutiveBodyKPP.params.value = "";
    }
}

const onEntityExecutiveBodyDataChanged = async (sender: Layout, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const entityExecutiveBodyINN = controls.get<TextBox>("entityExecutiveBodyINN");
    const entityExecutiveBodyKPP = controls.get<TextBox>("entityExecutiveBodyKPP");

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${UNIT_STAFF_SECTION_ID}/${args.newValue.id}`) as any;
        entityExecutiveBodyINN.params.value = data.fields.find(field => field.alias === "INN").value;
        entityExecutiveBodyKPP.params.value = data.fields.find(field => field.alias === "KPP").value;
    } else {
        entityExecutiveBodyINN.params.value = "";
        entityExecutiveBodyKPP.params.value = "";
    }
}

const onCeoDataChanged = async (sender: StaffDirectoryItems, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const ceoPosition = controls.get<TextBox>("ceoPosition");

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${EMPLOYEE_SECTION_ID}/${args.newValue.id}`) as any;
        ceoPosition.params.value = data.fields.find(field => field.alias === "PositionName").value;
    } else {
        ceoPosition.params.value = "";
    }  
}

const onReprTypeDataChanged = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprType = controls.get<RadioGroup>("reprType");
    const reprEntityInfoBlock = controls.get<Block>("reprEntityInfoBlock");
    const reprEntity = controls.get<StaffDirectoryItems>("reprEntity");
    const reprEntityINN = controls.get<TextBox>("reprEntityINN");
    const reprEntityKPP = controls.get<TextBox>("reprEntityKPP");
    const entityValue = reprType.params.value === "Entity";

    reprEntityInfoBlock.params.visibility = entityValue;
    reprEntity.params.required = entityValue;
    reprEntityINN.params.required = entityValue;
    reprEntityKPP.params.required = entityValue; 
    if (!entityValue) {
        reprEntity.params.value = null;
        reprEntityINN.params.value = "";
        reprEntityKPP.params.value = "";
    }
}

const onReprIDDataChanged = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprID = controls.get<Dropdown>("reprID");
    const authIssReprID = controls.get<TextArea>("authIssReprID");

    if (reprID.params.value === "passRusCitizen") {
        authIssReprID.params.required = true;
    } else {
        authIssReprID.params.required = false;
    }
}