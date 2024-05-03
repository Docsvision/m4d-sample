import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $ApplicationSettings } from "@docsvision/webclient/StandardServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { revokePowerOfAttorney } from "./RevokePowerOfAttorney";
import { resources } from "@docsvision/webclient/System/Resources";
import { IEventArgs } from "@docsvision/webclient/System/IEventArgs";

export const recallPowerOfAttorney = async (sender: CustomButton) => {    
    try {
        sender.params.isLoading = true;        
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
    } finally {
        sender.params.isLoading = false;
    }
};

export const revokeAndRecallPowerOfAttorney = async (sender: CustomButton, e: IEventArgs) => {
    try {
        sender.params.isLoading = true; 
        const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
        const employeeId = sender.layout.getService($ApplicationSettings).employee.id;
        const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;                            
        powersOfAttorneyButtonController?.checkCardTransferLogStatus(powerOfAttorneyId, employeeId).then(async msg => {
            if (msg.Success) {
                await revokePowerOfAttorney(sender, e, onAttachSignatureToCard, false);
            } else {
                MessageBox.ShowError(msg.Message);
            }
        });
    } finally {
        sender.params.isLoading = false;
    }
};


const onAttachSignatureToCard = async (sender: CustomButton) => {
    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "To revoke").id; 
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId; 
    const msg = await powersOfAttorneyButtonController?.recallPowerOfAttorney(powerOfAttorneyId, employeeId)
    if (msg.Success) {
        MessageBox.ShowInfo(resources.M4DRegistry_Recall_Success);
        await sender.layout.changeState(operationId);
    } else {
        MessageBox.ShowError(msg.Message);
    }
};