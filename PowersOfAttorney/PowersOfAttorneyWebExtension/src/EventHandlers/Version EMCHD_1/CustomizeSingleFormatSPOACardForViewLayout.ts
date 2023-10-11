import { Block } from "@docsvision/webclient/Platform/Block";
import { CheckBox } from "@docsvision/webclient/Platform/CheckBox";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { RadioGroup } from "@docsvision/webclient/Platform/RadioGroup";
import { Table } from "@docsvision/webclient/Platform/Table";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";

export const customizeSingleFormatSPOACardForViewLayout = (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const powersType = controls.get<Dropdown>("powersType");
    const refPowersTable = controls.get<Table>("refPowersTable");
    const textPowersDescr = controls.get<TextArea>("textPowersDescr");
    const reprCitizenshipSign = controls.get<Dropdown>("reprCitizenshipSign");
    const reprCitizenship = controls.get<TextBox>("reprCitizenship");
    const ceoCitizenshipSign = controls.get<Dropdown>("ceoCitizenshipSign");
    const ceoCitizenship = controls.get<TextBox>("ceoCitizenship");
    const substPOABasis = controls.get<CheckBox>("substPOABasis");
    const parentalPOABlock = controls.get<Block>("parentalPOABlock");
    const poaScope = controls.get<RadioGroup>("poaScope");
    const codeTaxAuthSubmitBlock = controls.get<Block>("codeTaxAuthSubmitBlock");
    const codeTaxAuthValidBlock = controls.get<Block>("codeTaxAuthValidBlock");
    const delegatorCitizenshipSignBlock = controls.get<Block>("delegatorCitizenshipSignBlock");
    const ceoAddressBlock = controls.get<Block>("ceoAddressBlock");
    const reprCitizenshipSignBlock = controls.get<Block>("reprCitizenshipSignBlock");
    const reprAddressBlock = controls.get<Block>("reprAddressBlock");

    if (powersType.params.value === "humReadPower") {
        refPowersTable.params.visibility = false;
        textPowersDescr.params.visibility = true;
    } else {
        refPowersTable.params.visibility = true;
        textPowersDescr.params.visibility = false;
    }

    if (substPOABasis.params.value) {
        parentalPOABlock.params.visibility = true;
    } else {
        parentalPOABlock.params.visibility = false;
    }

    if (ceoCitizenshipSign.value === 'foreignCitizen') {
        ceoCitizenship.params.visibility = true;
    } else {
        ceoCitizenship.params.visibility = false;
    }

    if (reprCitizenshipSign.params.value === 'foreignCitizen') {
        reprCitizenship.params.visibility = true;
    } else {
        reprCitizenship.params.visibility = false;
    }
    
    const isNotB2BValue = poaScope.params.value !== "B2B";
    codeTaxAuthSubmitBlock.params.visibility = isNotB2BValue;
    codeTaxAuthValidBlock.params.visibility = isNotB2BValue;
    ceoAddressBlock.params.visibility = isNotB2BValue;
    delegatorCitizenshipSignBlock.params.visibility = isNotB2BValue;
    reprCitizenshipSignBlock.params.visibility = isNotB2BValue;
    reprAddressBlock.params.visibility = isNotB2BValue;
  
}