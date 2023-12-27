import { $DigitalSignature } from "@docsvision/webclient/BackOffice/$DigitalSignature";
import { $LayoutCardController, $PowerOfAttorneyApiController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { $BatchOperations } from "@docsvision/webclient/Platform/$BatchOperations";
import { $BatchOperationsPerformer } from "@docsvision/webclient/Platform/$BatchOperationsPerformer";
import { $FolderDataLoading } from "@docsvision/webclient/Platform/$FolderDataLoading";
import { $FolderGrid } from "@docsvision/webclient/Platform/$FolderGrid";
import { $TableManagement } from "@docsvision/webclient/Platform/$TableManagement";
import { $TableMode } from "@docsvision/webclient/Platform/$TableMode";
import { $TableRowSelection } from "@docsvision/webclient/Platform/$TableRowSelection";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { BaseControl, BaseControlParams } from "@docsvision/webclient/System/BaseControl";
import { r } from "@docsvision/webclient/System/Readonly";
import { rw } from "@docsvision/webclient/System/Readwrite";
import { resources } from "@docsvision/webclient/System/Resources";
import { signPOABatchOperationState, signPOABatchOperationImpl } from "./DocumentSignBatchOperationImpl";


export class signPOABatchOperationParams extends BaseControlParams {       
    /** Стандартный CSS класс со стилями элемента управления */
    @r standardCssClass?: string = "system-documents-sign-batch-operation";
    /** Строка формата "<Имя1>,<Имя2>..." с перечнем имен столбцов представления для отображения
     * карточки в списке групповой операции.
     */
    @r columnNameForPresentation?: string;
    /** Текст кнопки операции. */
    @rw buttonText?: string = resources.POABatchSign_signOperationName;

    @rw services?: $LayoutCardController & $FolderDataLoading & $RequestManager & $PowerOfAttorneyApiController &
        $TableRowSelection & $BatchOperationsPerformer & $TableManagement & $TableMode & $BatchOperations &
        $DigitalSignature & $FolderGrid;
}

export class signPOABatchOperation extends BaseControl<signPOABatchOperationParams, signPOABatchOperationState> {

    /** @notest @internal */
    protected createParams() {
        return new signPOABatchOperationParams();
    }

    protected createImpl() {
        return new signPOABatchOperationImpl(this.props, this.state);
    }
}