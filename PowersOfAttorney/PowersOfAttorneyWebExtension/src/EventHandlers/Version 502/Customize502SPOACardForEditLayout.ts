import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { Layout } from "@docsvision/webclient/System/Layout";
import { Block } from "@docsvision/webclient/Platform/Block";
import { StaffDirectoryItems } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";
import { IDataChangedEventArgs, IDataChangedEventArgsEx } from "@docsvision/webclient/System/IDataChangedEventArgs";
import { EMPLOYEE_SECTION_ID, STAFF_DIRECTORY_ID, UNIT_STAFF_SECTION_ID } from "@docsvision/webclient/BackOffice/StaffDirectoryConstants";
import { CheckBox } from "@docsvision/webclient/Platform/CheckBox";
import { CardLink } from "@docsvision/webclient/Platform/CardLink";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { GenModels } from "@docsvision/webclient/Generated/DocsVision.WebClient.Models";
import { PlatformModeConditionTypes } from "@docsvision/webclient/Platform/PlatformModeConditionTypes";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { Table } from "@docsvision/webclient/Platform/Table";
import { Powers } from "@docsvision/webclient/BackOffice/Powers";
import { getControlValueFromPOA } from "../../Utils/GetControlValueFromPOA";
import { ILimitation, setLimitInControls } from "../../Utils/SetLimitInControls";
import { setSnilsMask } from "../../Utils/SetSnilsMask";

let entityExecutiveBodyChangedFromCardLink: boolean;

const limitations: ILimitation[] = [
    { name: "codeTaxAuthSubmit", length: 4 },
    { name: "codeTaxAuthValid", length: 4 },
    { name: "entityExecutiveBodyINN", length: 10 },
    { name: "entityExecutiveBodyKPP", length: 9 },
    { name: "ceoINN", length: 12 },
    { name: "reprEntityINN", length: 10 },
    { name: "reprEntityKPP", length: 9 }
]


export const customize502SPOACardForEditLayout = (sender: Layout) => {
    const controls = sender.layout.controls;
    const substPOABasis = controls.get<CheckBox>("substPOABasis");
    const entityExecutiveBody = controls.get<StaffDirectoryItems>("entityExecutiveBody");
    const ceo = controls.get<StaffDirectoryItems>("ceo");
    const reprType = controls.get<Dropdown>("reprType");
    const reprID = controls.get<Dropdown>("reprID");
    const parentalPOACardLink = controls.get<CardLink>("parentalPOACardLink");
    const originalPOACardLink = controls.get<CardLink>("originalPOACardLink");
    const ceoID = controls.get<Dropdown>("ceoID");

    customizeInputFields(sender);
    onSubstPOABasisDataChanged(sender);
    onReprTypeDataChanged(sender);
    onReprIDDataChanged(sender);
    onCeoIDDataChanged(sender);

    substPOABasis && substPOABasis.params.dataChanged.subscribe(onSubstPOABasisDataChanged);
    parentalPOACardLink && parentalPOACardLink.params.dataChanged.subscribe(onCardLinkDataChanged);
    originalPOACardLink && originalPOACardLink.params.dataChanged.subscribe(onOriginalPOACardLinkDataChanged);
    entityExecutiveBody && entityExecutiveBody.params.dataChanged.subscribe(onEntityExecutiveBodyDataChanged);
    ceo && ceo.params.dataChanged.subscribe(onCeoDataChanged);
    reprType && reprType.params.dataChanged.subscribe(onReprTypeDataChanged);
    reprID && reprID.params.dataChanged.subscribe(onReprIDDataChanged);
    ceoID && ceoID.params.dataChanged.subscribe(onCeoIDDataChanged);
}

const customizeInputFields = (sender: Layout) => {
    setLimitInControls(sender, limitations);
    setSnilsMask(sender, "ceoSNILS");
    setSnilsMask(sender, "reprSNILS");
}


const onSubstPOABasisDataChanged = (sender: Layout) => {
    const controls = sender.layout.controls;
    const substPOABasis = controls.get<CheckBox>("substPOABasis");
    const parentalPOACardLink = controls.get<CardLink>("parentalPOACardLink");
    if (substPOABasis.params.value) {
        parentalPOACardLink.params.visibility = true;
        parentalPOACardLink.params.required = true;
    } else {
        parentalPOACardLink.params.visibility = false;
        parentalPOACardLink.params.required = false;
        if (parentalPOACardLink.params.value) {
            parentalPOACardLink.params.value = null;
        }
    }
}

const onCardLinkDataChanged = async (sender: CardLink, args: IDataChangedEventArgsEx<GenModels.CardLinkDataModel>) => {
    const controls = sender.layout.controls;
    const entityExecutiveBody = controls.get<TextBox>("entityExecutiveBody");
    const entityExecutiveBodyINN = controls.get<TextBox>("entityExecutiveBodyINN");
    const entityExecutiveBodyKPP = controls.get<TextBox>("entityExecutiveBodyKPP");

    const ceoSNILS = controls.get<TextBox>("ceoSNILS");
    const ceoID = controls.get<Dropdown>("ceoID");
    const numCEOID = controls.get<TextBox>("numCEOID");
    const dateIssCEOID = controls.get<DateTimePicker>("dateIssCEOID");
    const authIssReprID = controls.get<TextArea>("authIssReprID");
    const refPowersTable = controls.get<Table>("refPowersTable");
    
    if (args.newValue.cardId) {
        entityExecutiveBodyChangedFromCardLink = true;     
    
        entityExecutiveBody.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "reprEntity", "StaffDirectoryItems");
        entityExecutiveBodyINN.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "reprEntityINN", "TextBox");
        entityExecutiveBodyKPP.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "reprEntityKPP", "TextBox");

        ceoSNILS.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "reprSNILS", "TextBox");
        ceoID.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "reprID", "Dropdown");
        numCEOID.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "numReprID", "TextBox");
        dateIssCEOID.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "dateIssReprID", "DateTimePicker");
        authIssReprID.params.value = await getControlValueFromPOA(sender, args.newValue.cardId, "authIssReprID", "TextArea");


        const tableModel = await sender.layout.params.services.layoutController.getPartWithParams({
            cardId: args.newValue.cardId, 
            layoutMode: PlatformModeConditionTypes.VIEW,
            locationName: undefined,
            controlName: "refPowersTable",
            layoutParams: null,
            loadOnlyChildren: false,
        })

        const tableRows = tableModel.layoutModel.children.find((x) => x.controlTypeName === "Table").children.filter((x) => x.controlTypeName === "TableRow");
        tableRows.pop();

        for (const tableRow of tableRows) {
            const rowId = await refPowersTable.addRow() as string;
            const rowIndex = refPowersTable.getRowIndex(rowId);
            const refPowersCode = controls.get<Powers>("refPowersCode[]")[rowIndex];
            const powersElement = tableRow.children.find((x) => x.controlTypeName === "TableColumn").children.find((x) => x.controlTypeName === "Powers");
            refPowersCode.params.value = powersElement.properties.binding.value;
        }

    } else {
        entityExecutiveBody.params.value = "";
        ceoSNILS.params.value = "";
        ceoID.params.value = "";
        numCEOID.params.value = "";
        dateIssCEOID.params.value = null;
        authIssReprID.params.value = "";
        refPowersTable.clear();
    }
}


const onOriginalPOACardLinkDataChanged = (sender: CardLink, args: IDataChangedEventArgsEx<GenModels.CardLinkDataModel>) => {
    const controls = sender.layout.controls;
    const substPOABasis = controls.get<CheckBox>("substPOABasis");
    if (!substPOABasis.params.value) {
        onCardLinkDataChanged(sender, args);
    };
}

const onEntityExecutiveBodyDataChanged = async (sender: Layout, args: IDataChangedEventArgs) => {
    const controls = sender.layout.controls;
    const entityExecutiveBodyINN = controls.get<TextBox>("entityExecutiveBodyINN");
    const entityExecutiveBodyKPP = controls.get<TextBox>("entityExecutiveBodyKPP");

    if (entityExecutiveBodyChangedFromCardLink) {
        entityExecutiveBodyChangedFromCardLink = false;
        return;
    }

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
    const ceoDateOfBirth = controls.get<DateTimePicker>("ceoDateOfBirth");

    if (args.newValue) {
        const data = await sender.layout.params.services.requestManager.get(`api/v1/cards/${STAFF_DIRECTORY_ID}/${EMPLOYEE_SECTION_ID}/${args.newValue.id}`) as any;
        ceoDateOfBirth.params.value = data.fields.find(field => field.alias === "BirthDate").value;
    } else {
        ceoDateOfBirth.params.value = null;
    }  
}

const onReprTypeDataChanged = (sender: Layout) => {
    const controls = sender.layout.controls;
    const reprType = controls.get<RadioGroup>("reprType");
    const reprEntityInfoBlock = controls.get<Block>("reprEntityInfoBlock");
    const reprEntity = controls.get<TextBox>("reprEntity");
    const reprEntityINN = controls.get<TextBox>("reprEntityINN");
    const reprEntityKPP = controls.get<TextBox>("reprEntityKPP");
    const entityValue = reprType.params.value === "Entity";

    reprEntityInfoBlock.params.visibility = entityValue;
    reprEntity.params.required = entityValue;
    reprEntityINN.params.required = entityValue;
    reprEntityKPP.params.required = entityValue;
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

const onCeoIDDataChanged = (sender: Layout) => {
    const controls = sender.layout.controls;
    const ceoID = controls.get<Dropdown>("ceoID");
    const authIssCEOID = controls.get<TextArea>("authIssCEOID");

    if (ceoID.params.value === "passRusCitizen") {
        authIssCEOID.params.required = true;
    } else {
        authIssCEOID.params.required = false;
    }
}