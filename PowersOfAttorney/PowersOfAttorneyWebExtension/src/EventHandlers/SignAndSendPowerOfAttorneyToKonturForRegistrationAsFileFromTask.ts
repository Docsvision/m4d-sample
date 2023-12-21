import { OperationExecutingEventArgs } from "@docsvision/webclient/BackOffice/OperationExecutingEventArgs";
import { $LayoutCardController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { CardLink } from "@docsvision/webclient/Platform/CardLink";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";
import { signPowerOfAttorneyFromTask } from "./SignPowerOfAttorneyFromTask";
import { SIGN_OPERATION_ID, SING_OPERATION_COMPLETION_OPTION_ID } from "../PowerOfAttorneyConstants";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { sendForRegistrationToRegistry } from "./SendForRegistrationToRegistry";
import { GenModels } from "@docsvision/webclient/Generated/DocsVision.WebClient.Models";

export const signAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask = async (sender: CustomButton, e: ICancelableEventArgs<OperationExecutingEventArgs>) => {
    if (e.data.operationData.builtInOperationId === SIGN_OPERATION_ID 
        && e.data.operationData.additionalInfo.decisionSemantics === GenModels.DecisionSemantics.Positive) {
            await signPowerOfAttorneyFromTask(sender, e, false);
            const powerOfAttorneyIdControl = sender.layout.getService($ControlStore).get<CardLink>("powerOfAttorneySysCard");
            await powerOfAttorneyIdControl.reloadFromServer();
            await sendForRegistrationToRegistry(sender);
            const signOperationIdPOA = sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
            const powerOfAttorneyUserCardId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.id;
            await sender.layout.getService($LayoutCardController).changeState({ cardId: powerOfAttorneyUserCardId, operationId: signOperationIdPOA, timestamp: sender.layout.controls.locationContainer.params.layoutModel.cardInfo.timestamp, comment: "", layoutParams: sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.layoutParams });
            e.accept();
    }
}