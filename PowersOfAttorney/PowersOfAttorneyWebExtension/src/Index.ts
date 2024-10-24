﻿import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import * as CreatePowerOfAttorney from "./EventHandlers/Version 002/CreatePowerOfAttorney";
import * as CreateRetrustPowerOfAttorney from "./EventHandlers/Version 002/CreateRetrustPowerOfAttorney";
import * as ExportPowerOfAttorney from "./EventHandlers/ExportPowerOfAttorney";
import * as SignPowerOfAttorney from "./EventHandlers/SignPowerOfAttorney";
import * as RevokePowerOfAttorney from "./EventHandlers/RevokePowerOfAttorney";
import * as DeletePowerOfAttorney from "./EventHandlers/DeletePowerOfAttorney";
import * as CustomizePowerOfAttorneyCardForEditLayout  from "./EventHandlers/Version 002/CustomizePowerOfAttorneyCardForEditLayout";
import * as CustomizePowerOfAttorneyCardForViewCard from "./EventHandlers/Version 002/CustomizePowerOfAttorneyCardForViewLayout";
import * as CustomizeSubstitutionPowerOfAttorneyCardForEditLayout from "./EventHandlers/Version 002/CustomizeSubstitutionPowerOfAttorneyCardForEditLayout";
import * as CustomizeSubstitutionPowerOfAttorneyCardForViewLayout from "./EventHandlers/Version 002/CustomizeSubstitutionPowerOfAttorneyCardForViewLayout";
import { $PowersOfAttorneyDemoController, PowersOfAttorneyDemoController } from "./ServerRequests/PowersOfAttorneyDemoController";
import * as CustomizeSingleFormatPowerOfAttorneyForEditLayout from "./EventHandlers/Version EMCHD_1/CustomizeSingleFormatPowerOfAttorneyForEditLayout";
import * as CustomizeSingleFormatPowerOfAttorneyForViewLayout from "./EventHandlers/Version EMCHD_1/CustomizeSingleFormatPowerOfAttorneyForViewLayout";
import * as CreateEMCHDPowerOfAttorney from "./EventHandlers/Version EMCHD_1/CreateEMCHDPowerOfAttorney";
import * as SignPowerOfAttorneyFromTask from "./EventHandlers/SignPowerOfAttorneyFromTask";
import * as ShowRequiredFields from "./EventHandlers/ShowRequiredFields";
import * as CustomizeSingleFormatSPOACardForEditLayout from "./EventHandlers/Version EMCHD_1/CustomizeSingleFormatSPOACardForEditLayout";
import * as CustomizeSingleFormatSPOACardForViewLayout from "./EventHandlers/Version EMCHD_1/CustomizeSingleFormatSPOACardForViewLayout";
import * as CreateEMCHDRetrustPowerOfAttorney from "./EventHandlers/Version EMCHD_1/CreateEMCHDRetrustPowerOfAttorney";
import * as ExportApplicationForRevocation from "./EventHandlers/ExportApplicationForRevocation";
import * as PowerOfAttorneyRegistration from "./EventHandlers/PowerOfAttorneyRegistration";
import * as SignAndSendPowerOfAttorneyToRegistrationAsFile from "./EventHandlers/SignAndSendPowerOfAttorneyToRegistrationAsFile";
import * as SignAndSendPowerOfAttorneyToRegistrationAsFileFromTask from "./EventHandlers/SignAndSendPowerOfAttorneyToRegistrationAsFileFromTask";
import * as SendForRegistrationToRegistry  from "./EventHandlers/SendForRegistrationToRegistry";
import * as RecallPOwerOfAttorney from "./EventHandlers/RecallPowerOfAttorney";
import * as CheckPowerOfAttorney from './EventHandlers/CheckPowerOfAttorney'
import { $PowersOfAttorneyButtonController, PowersOfAttorneyButtonController } from "./ServerRequests/PowersOfAttorneyButtonController";
import * as CustomizeSingleFormatPowerOfAttorneyForLocationLayout from "./EventHandlers/Version EMCHD_1/CustomizeSingleFormatPowerOfAttorneyForLocationLayout";
import * as CustomizeSingleFormatSPOAForLocationLayout from "./EventHandlers/Version EMCHD_1/CustomizeSingleFormatSPOAForLocationLayout";
import * as SignAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask from "./EventHandlers/SignAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask";

// Главная входная точка всего расширения
// Данный файл должен импортировать прямо или косвенно все остальные файлы, 
// чтобы rollup смог собрать их все в один бандл.

// Регистрация расширения позволяет корректно установить все
// обработчики событий, сервисы и прочие сущности web-приложения.
extensionManager.registerExtension({
    name: "Powers Of Attorney web extension",
    version: "6.1",
    globalEventHandlers: [ CreatePowerOfAttorney, CreateRetrustPowerOfAttorney, ExportPowerOfAttorney, SignPowerOfAttorney, RevokePowerOfAttorney, DeletePowerOfAttorney,
        CustomizePowerOfAttorneyCardForEditLayout, CustomizePowerOfAttorneyCardForViewCard, CustomizeSubstitutionPowerOfAttorneyCardForEditLayout, CustomizeSubstitutionPowerOfAttorneyCardForViewLayout,
        CustomizeSingleFormatPowerOfAttorneyForEditLayout, CustomizeSingleFormatPowerOfAttorneyForViewLayout, CustomizeSingleFormatPowerOfAttorneyForLocationLayout, CustomizeSingleFormatSPOAForLocationLayout, CreateEMCHDPowerOfAttorney, SignPowerOfAttorneyFromTask, ShowRequiredFields, CustomizeSingleFormatSPOACardForEditLayout, 
        CustomizeSingleFormatSPOACardForViewLayout, CreateEMCHDRetrustPowerOfAttorney, ExportApplicationForRevocation,
        PowerOfAttorneyRegistration, SignAndSendPowerOfAttorneyToRegistrationAsFile, SignAndSendPowerOfAttorneyToRegistrationAsFileFromTask, SendForRegistrationToRegistry, RecallPOwerOfAttorney, CheckPowerOfAttorney, SignAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask ],
    layoutServices: [ 
        Service.fromFactory($PowersOfAttorneyDemoController, (services: $RequestManager) => new PowersOfAttorneyDemoController(services)),
        Service.fromFactory($PowersOfAttorneyButtonController, (services: $RequestManager) => new PowersOfAttorneyButtonController(services)),
    ]
})