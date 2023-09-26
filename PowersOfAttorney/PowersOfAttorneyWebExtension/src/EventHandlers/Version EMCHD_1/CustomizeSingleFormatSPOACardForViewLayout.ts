import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";

export const customizeSingleFormatSPOACardForViewLayout = (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const powersType = controls.powersType;
    const refPowersTable = controls.refPowersTable;
    const textPowersDescr = controls.textPowersDescr;
    const substPOABasis = controls.substPOABasis;
    const parentalPOABlock = controls.parentalPOABlock;
    const powersType1: Dropdown = controls.powersType1;
    const textPowersDescr1: TextArea = controls.textPowersDescr1;

    if (powersType.params.value === "humReadPower") {
        refPowersTable.params.visibility = false;
        textPowersDescr.params.visibility = true;
    } else {
        refPowersTable.params.visibility = true;
        textPowersDescr.params.visibility = false;
    }

    if (powersType1.params.value === "humReadPower") {
        refPowersTable.params.visibility = false;
        textPowersDescr1.params.visibility = true;
    } else {
        refPowersTable.params.visibility = true;
        textPowersDescr1.params.visibility = false;
    }

    if (substPOABasis.params.value) {
        parentalPOABlock.params.visibility = true;
    } else {
        parentalPOABlock.params.visibility = false;
    }

    const reprCitizenshipSign = controls.reprCitizenshipSign;
    const reprCitizenship = controls.reprCitizenship;
    const ceoCitizenshipSign = controls.ceoCitizenshipSign;
    const ceoCitizenship = controls.ceoCitizenship;

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
}