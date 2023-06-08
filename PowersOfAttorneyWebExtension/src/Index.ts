﻿import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import * as CreatePowerOfAttorney from "./EventHandlers/CreatePowerOfAttorney";
import * as CreateRetrustPowerOfAttorney from "./EventHandlers/CreateRetrustPowerOfAttorney";
import * as ExportPowerOfAttorney from "./EventHandlers/ExportPowerOfAttorney";
import * as SignPowerOfAttorney from "./EventHandlers/SignPowerOfAttorney";
import * as RevokePowerOfAttorney from "./EventHandlers/RevokePowerOfAttorney";
import * as DeletePowerOfAttorney from "./EventHandlers/DeletePowerOfAttorney";
import * as CustomizePowerOfAttorneyCardForEditLayout  from "./EventHandlers/CustomizePowerOfAttorneyCardForEditLayout";
import * as CustomizePowerOfAttorneyCardForViewCard from "./EventHandlers/CustomizePowerOfAttorneyCardForViewLayout";
import * as CustomizeSubstitutionPowerOfAttorneyCardForEditLayout from "./EventHandlers/CustomizeSubstitutionPowerOfAttorneyCardForEditLayout";
import * as CustomizeSubstitutionPowerOfAttorneyCardForViewLayout from "./EventHandlers/CustomizeSubstitutionPowerOfAttorneyCardForViewLayout";
import { $PowersOfAttorneyDemoController, PowersOfAttorneyDemoController } from "./ServerRequests/PowersOfAttorneyDemoController";
import * as FillInTheFieldsAutomatically from "./EventHandlers/FillInTheFieldsAutomatically";
import * as CustomizeInputFields from "./EventHandlers/CustomizeInputFields";
import * as CreateEMCHDRetrustPowerOfAttorney from "./EventHandlers/CreateEMCHDRetrustPowerOfAttorney";
import * as CreateEMCHDPowerOfAttorney from "./EventHandlers/CreateEMCHDPowerOfAttorney";

// Главная входная точка всего расширения
// Данный файл должен импортировать прямо или косвенно все остальные файлы, 
// чтобы rollup смог собрать их все в один бандл.

// Регистрация расширения позволяет корректно установить все
// обработчики событий, сервисы и прочие сущности web-приложения.
extensionManager.registerExtension({
    name: "Powers Of Attorney web extension",
    version: "5.5.17",
    globalEventHandlers: [ CreatePowerOfAttorney, CreateRetrustPowerOfAttorney, ExportPowerOfAttorney, SignPowerOfAttorney, RevokePowerOfAttorney, DeletePowerOfAttorney,
        CustomizePowerOfAttorneyCardForEditLayout, CustomizePowerOfAttorneyCardForViewCard, CustomizeSubstitutionPowerOfAttorneyCardForEditLayout, CustomizeSubstitutionPowerOfAttorneyCardForViewLayout,
        FillInTheFieldsAutomatically, CustomizeInputFields, CreateEMCHDPowerOfAttorney, CreateEMCHDRetrustPowerOfAttorney ],
    layoutServices: [ 
        Service.fromFactory($PowersOfAttorneyDemoController, (services: $RequestManager) => new PowersOfAttorneyDemoController(services))
    ]
})