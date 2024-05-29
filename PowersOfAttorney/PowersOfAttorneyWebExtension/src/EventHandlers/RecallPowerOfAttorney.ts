import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { $ApplicationSettings } from "@docsvision/webclient/StandardServices";
import { $PowersOfAttorneyButtonController } from "../ServerRequests/PowersOfAttorneyButtonController";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { revokePowerOfAttorney } from "./RevokePowerOfAttorney";
import { resources } from "@docsvision/webclient/System/Resources";
import { IEventArgs } from "@docsvision/webclient/System/IEventArgs";
import { $MessageWindow } from "@docsvision/web/components/modals/message-box";

export const recallPowerOfAttorney = async (sender: CustomButton) => {    
    try {
        sender.params.isLoading = true;        
        const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId;
        const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);    
        const employeeId = sender.layout.getService($ApplicationSettings).employee.id
        
        const msg = await powersOfAttorneyButtonController?.recallPowerOfAttorney(powerOfAttorneyId, employeeId);
        sender.layout.getService($MessageWindow).showInfo(msg); 
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
      
        await powersOfAttorneyButtonController?.checkCardTransferLogStatus(powerOfAttorneyId, employeeId);
        await revokePowerOfAttorney(sender, e, onAttachSignatureToCard, false);
    } finally {
        sender.params.isLoading = false;
    }
};


const onAttachSignatureToCard = async (sender: CustomButton) => {
    const operationId = sender.layout.layoutInfo.operations.find(operation => operation.alias === "To revoke").id; 
    const powersOfAttorneyButtonController = sender.layout.getService($PowersOfAttorneyButtonController);
    const employeeId = sender.layout.getService($ApplicationSettings).employee.id;
    const powerOfAttorneyId = sender.layout.controls.powerOfAttorneySysCard.params.value?.cardId; 

    await powersOfAttorneyButtonController?.recallPowerOfAttorney(powerOfAttorneyId, employeeId)
    sender.layout.getService($MessageWindow).showInfo(resources.M4DRegistry_Recall_Success);
    await sender.layout.changeState(operationId);
};