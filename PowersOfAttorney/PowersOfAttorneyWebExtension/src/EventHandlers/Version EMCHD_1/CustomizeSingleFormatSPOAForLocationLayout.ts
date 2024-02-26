import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { Table } from "@docsvision/webclient/Platform/Table";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { customizeSingleFormatSPOACardForViewLayout } from "./CustomizeSingleFormatSPOACardForViewLayout";

export const customizeSingleFormatSPOAForLocationLayout = (sender: LayoutControl) => {
    const controls = sender.layout.controls;
    const powersType1 = controls.get<Dropdown>("powersType1");
    const textPowersDescr1 = controls.get<TextArea>("textPowersDescr1");
    const refPowersTable2 = controls.get<Table>("refPowersTable2");

    customizeSingleFormatSPOACardForViewLayout(sender);
    
    if (powersType1.params.value === "humReadPower") {
        refPowersTable2.params.visibility = false;
        textPowersDescr1.params.visibility = true;
    } else {
        refPowersTable2.params.visibility = true;
        textPowersDescr1.params.visibility = false;
    }
}

