import { LayoutControl } from "@docsvision/webclient/System/BaseControl";

export const customizeSingleFormatPowerOfAttorneyForViewLayout = async (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const powersType = controls.powersType;
    const refPowersTable = controls.refPowersTable;
    const textPowersDescrBlock = controls.textPowersDescrBlock;
    if (powersType.params.value === "humReadPower") {
        refPowersTable.params.visibility = false;
        textPowersDescrBlock.params.visibility = true;
    } else {
        refPowersTable.params.visibility = true;
        textPowersDescrBlock.params.visibility = false;
    }
}