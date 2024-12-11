import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { IEventArgs } from "@docsvision/webclient/System/IEventArgs";
import { revokeAndRecallPowerOfAttorney } from "../RecallPowerOfAttorney";

export const revokeAndRecall502PowerOfAttorney = async (sender: CustomButton, e: IEventArgs) => {
    revokeAndRecallPowerOfAttorney(sender, e, false);
}
