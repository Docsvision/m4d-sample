import { OperationExecutingEventArgs } from "@docsvision/webclient/BackOffice/OperationExecutingEventArgs";
import { $LayoutCardController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { ICancelableEventArgs } from "@docsvision/webclient/System/ICancelableEventArgs";
import { sendPowerOfAttorneyToRegistrationAsFile } from "./PowerOfAttorneyRegistration";
import { signPowerOfAttorneyFromTask } from "./SignPowerOfAttorneyFromTask";


export const signAndSendPowerOfAttorneyToRegistrationAsFileFromTask = async (sender: LayoutControl, e: ICancelableEventArgs<OperationExecutingEventArgs>) => {
    if (e.data.operationData.additionalInfo.decisionName === "Подписать") {
        await signPowerOfAttorneyFromTask(sender, e, false);
        await sendPowerOfAttorneyToRegistrationAsFile(sender);
        const signOperationIdPOA = sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.operations.find(operation => operation.alias === "Sign").id;
        const powerOfAttorneyUserCardId = sender.layout.controls.locationContainer.params.layoutModel.cardInfo.id;
        await sender.layout.getService($LayoutCardController).changeState({ cardId: powerOfAttorneyUserCardId, operationId: signOperationIdPOA, timestamp: sender.layout.controls.locationContainer.params.layoutModel.cardInfo.timestamp, comment: "", layoutParams: sender.layout.controls.locationContainer.params.layoutModel.layoutModel.layoutInfo.layoutParams });
        e.accept();
    }
}