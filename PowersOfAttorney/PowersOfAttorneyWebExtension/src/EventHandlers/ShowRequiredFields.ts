import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { Layout } from "@docsvision/webclient/System/Layout";
import { resources } from "@docsvision/webclient/System/Resources";

export function showRequiredFields(sender: Layout, args: ICancelableEventArgs<ILayoutBeforeSavingEventArgs>) {
    const controls = sender.layout.controls;
    const validationResults = args.data?.control?.validate({ ShowErrorMessage: true }) || [];
    const invalidResults = validationResults.filter((value) => !value.Passed);
    if (invalidResults.length !== 0) {
        const labelTexts = invalidResults.map((control) => {
            if (control.ControlName.includes("[]")) {
                return controls.get<any>(`${control.ControlName.replace("[]", "")}`)[0].parent.props.header;
            } else {
                return controls.get<any>(`${control.ControlName}`).params.labelText;
            }
        });
        sender.layout.params.services.messageWindow.showInfo(`${resources.TheFollowingFieldsAreRequired}: ${labelTexts.join('; ')}`);
    }
}