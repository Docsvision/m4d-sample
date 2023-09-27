import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { Table } from "@docsvision/webclient/Platform/Table";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";

export const toggleElementVisibilityForLocationLayout = (sender: LayoutControl) =>  {
    const controls = sender.layout.controls;
    const powersType1: Dropdown = controls.powersType1;
    const textPowersDescr1: TextArea = controls.textPowersDescr1;
    const refPowersTable: Table = controls.refPowersTable;
    const refPowersTable2: Table = controls.refPowersTable2;
    
    if (powersType1.params.value === "humReadPower") {
        refPowersTable.params.visibility = false;
        refPowersTable2.params.visibility = false;
        textPowersDescr1.params.visibility = true;
    } else {
        refPowersTable.params.visibility = true;
        textPowersDescr1.params.visibility = false;
    }
}