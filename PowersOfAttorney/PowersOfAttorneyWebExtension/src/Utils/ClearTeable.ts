import { Table } from "@docsvision/webclient/Platform/Table";

export const clearTable = async (control: Table) => {
    for (let rowId of control.params.rows) { 
        await control.removeRow(rowId); 
    }
}