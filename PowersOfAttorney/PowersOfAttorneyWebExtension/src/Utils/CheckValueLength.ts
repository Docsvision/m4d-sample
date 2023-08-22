import { $MessageWindow } from "@docsvision/web/components/modals/message-box";
import { resources } from "@docsvision/webclient/System/Resources";
import { formatString } from "@docsvision/webclient/System/StringUtils";

export const checkValueLength = async (element: Element, valueLength: number, services: $MessageWindow, length: number) => {
    if (valueLength > 0 && valueLength < length) {
        await services.messageWindow.showError(formatString(resources.Error_ValueLength, length));
        setTimeout(() => (element as HTMLElement).focus(), 0);
    }
}