import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { checkValueLength } from "./CheckValueLength";
import { Layout } from "@docsvision/webclient/System/Layout";


export interface ILimitation {
    name: string,
    length: number
}

export const setLimitInControls = (sender: Layout, limitations: ILimitation[]) => {
    limitations.forEach(limitation => {
        const element = document.querySelector(`[data-control-name="${limitation.name}"] input`);
        element.setAttribute("maxLength", `${limitation.length}`);
        sender.controls.get<TextBox>(limitation.name).params.blur.subscribe((sender: TextBox) => {   
            checkValueLength(element, sender.params.value?.length && 0, sender.layout.params.services, limitation.length);
        })
    })
}