import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { IEventArgs } from "@docsvision/webclient/System/IEventArgs";
import { revokePowerOfAttorney } from "../RevokePowerOfAttorney";

export const revoke502PowerOfAttorney = async (sender: CustomButton, e: IEventArgs) => {
    revokePowerOfAttorney(sender, e, null, true, false);
}