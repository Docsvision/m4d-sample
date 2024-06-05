import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { Layout } from "@docsvision/webclient/System/Layout";
import IMask from "imask";
import { checkValueLength } from "./CheckValueLength";

const maskOptions = {
    mask: '000-000-000 00'
}

export const setSnilsMask = (sender: Layout, controlName: string) => {
    const control = document.querySelector(`[data-control-name="${controlName}"] input`) as HTMLElement;
    IMask(control, maskOptions);
    control.addEventListener("change", (event) => (sender.controls.get<TextBox>(`${controlName}`).params.value = (event.target as HTMLInputElement).value));
    sender.controls.get<TextBox>(`${controlName}`).params.blur.subscribe((sender: TextBox) => {
        checkValueLength(control, sender.params.value?.replaceAll("-", "").replace(" ", "").length, sender.layout.params.services, 11);
    })
}