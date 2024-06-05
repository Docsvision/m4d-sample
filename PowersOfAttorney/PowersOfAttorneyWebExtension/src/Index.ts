import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { $PowersOfAttorneyDemoController, PowersOfAttorneyDemoController } from "./ServerRequests/PowersOfAttorneyDemoController";
import { $PowersOfAttorneyButtonController, PowersOfAttorneyButtonController } from "./ServerRequests/PowersOfAttorneyButtonController";
import * as ExportPowerOfAttorney from "./EventHandlers/ExportPowerOfAttorney";
import * as SignPowerOfAttorney from "./EventHandlers/SignPowerOfAttorney";
import * as RevokePowerOfAttorney from "./EventHandlers/RevokePowerOfAttorney";
import * as DeletePowerOfAttorney from "./EventHandlers/DeletePowerOfAttorney";
import * as SignPowerOfAttorneyFromTask from "./EventHandlers/SignPowerOfAttorneyFromTask";
import * as ShowRequiredFields from "./EventHandlers/ShowRequiredFields";
import * as ExportApplicationForRevocation from "./EventHandlers/ExportApplicationForRevocation";
import * as PowerOfAttorneyRegistration from "./EventHandlers/PowerOfAttorneyRegistration";
import * as SendForRegistrationToRegistry  from "./EventHandlers/SendForRegistrationToRegistry";
import * as RecallPOwerOfAttorney from "./EventHandlers/RecallPowerOfAttorney";
import * as CheckPowerOfAttorney from './EventHandlers/CheckPowerOfAttorney'
import * as SignAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask from "./EventHandlers/SignAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask";
import * as SignAndSendPowerOfAttorneyToRegistrationAsFile from "./EventHandlers/SignAndSendPowerOfAttorneyToRegistrationAsFile";
import * as SignAndSendPowerOfAttorneyToRegistrationAsFileFromTask from "./EventHandlers/SignAndSendPowerOfAttorneyToRegistrationAsFileFromTask"
import { Version002EventHandlers } from "./EventHandlers/Version 002/Index";
import { Version502EventHandlers } from "./EventHandlers/Version 502/Index";
import { VersionEMCHDEventHandlers } from "./EventHandlers/Version EMCHD_1/Index";



// Главная входная точка всего расширения
// Данный файл должен импортировать прямо или косвенно все остальные файлы, 
// чтобы rollup смог собрать их все в один бандл.

// Регистрация расширения позволяет корректно установить все
// обработчики событий, сервисы и прочие сущности web-приложения.
extensionManager.registerExtension({
    name: "Powers Of Attorney web extension",
    version: "5.5.17",
    globalEventHandlers: [ ExportPowerOfAttorney, SignPowerOfAttorney, RevokePowerOfAttorney, DeletePowerOfAttorney,
        SignPowerOfAttorneyFromTask, ShowRequiredFields, ExportApplicationForRevocation, PowerOfAttorneyRegistration, 
        SignAndSendPowerOfAttorneyToRegistrationAsFile, SendForRegistrationToRegistry, SignAndSendPowerOfAttorneyToRegistrationAsFileFromTask,
        RecallPOwerOfAttorney, CheckPowerOfAttorney, SignAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask, 
        ...Version002EventHandlers, ...VersionEMCHDEventHandlers, ...Version502EventHandlers ],
    layoutServices: [ 
        Service.fromFactory($PowersOfAttorneyDemoController, (services: $RequestManager) => new PowersOfAttorneyDemoController(services)),
        Service.fromFactory($PowersOfAttorneyButtonController, (services: $RequestManager) => new PowersOfAttorneyButtonController(services)),
    ]
})