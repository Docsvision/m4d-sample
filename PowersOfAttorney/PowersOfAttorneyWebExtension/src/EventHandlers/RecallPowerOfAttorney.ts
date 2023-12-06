import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $ApplicationSettings } from "@docsvision/webclient/StandardServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { revokePowerOfAttorney } from "./RevokePowerOfAttorney";
import { resources } from "@docsvision/webclient/System/Resources";

export const recallPowerOfAttorney = async (sender: CustomButton) => {
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;

    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
    
    const employeeId = sender.layout.getService($ApplicationSettings).employee.id
    
    powersOfAttorneyButtonController?.recallPowerOfAttorney(powerOfAttorneyId, employeeId).then(msg => {
        if (msg.Success) {
            msg.Data && MessageBox.ShowInfo(msg.Data);
        } else {
            MessageBox.ShowError(msg.Message);
        }
    });
};

export const revokeAndRecallPowerOfAttorney = async (sender: CustomButton) => {
    const onAttachSignatureToCard = async (sender) => {
        const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;

        const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
        const employeeId = sender.layout.getService($ApplicationSettings).employee.id
        const msg = await powersOfAttorneyButtonController?.recallPowerOfAttorney(powerOfAttorneyId, employeeId)
        if (msg.Success) {
            MessageBox.ShowInfo(resources.M4DRegistry_Recall_Success);
        } else {
            MessageBox.ShowError(msg.Message);
        }
    };
    await revokePowerOfAttorney(sender, onAttachSignatureToCard, false);
    
};
