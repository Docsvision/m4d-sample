import { LayoutControl } from "@docsvision/webclient/System/BaseControl";

export const customizeSingleFormatSPOACardForViewLayout = (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const powersType = controls.powersType;
    const refPowersTable = controls.refPowersTable;
    const textPowersDescrBlock = controls.textPowersDescrBlock;
    const substPOABasis = controls.substPOABasis;
    const parentalPOABlock = controls.parentalPOABlock;

    if (powersType.params.value === "humReadPower") {
        refPowersTable.params.visibility = false;
        textPowersDescrBlock.params.visibility = true;
    } else {
        refPowersTable.params.visibility = true;
        textPowersDescrBlock.params.visibility = false;
    }

    if (substPOABasis.params.value) {
        parentalPOABlock.params.visibility = true;
    } else {
        parentalPOABlock.params.visibility = false;
    }
}