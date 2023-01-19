import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import * as CreatePowerOfAttorney from "./EventHandlers/CreatePowerOfAttorney";
import * as CreateRetrustPowerOfAttorney from "./EventHandlers/CreateRetrustPowerOfAttorney";
import * as ExportPowerOfAttorney from "./EventHandlers/ExportPowerOfAttorney";
import * as SignPowerOfAttorney from "./EventHandlers/SignPowerOfAttorney";
import * as RevokePowerOfAttorney from "./EventHandlers/RevokePowerOfAttorney";
import { $PowersOfAttorneyDemoController, PowersOfAttorneyDemoController } from "./ServerRequests.ts/PowersOfAttorneyDemoController";

// Главная входная точка всего расширения
// Данный файл должен импортировать прямо или косвенно все остальные файлы, 
// чтобы rollup смог собрать их все в один бандл.

// Регистрация расширения позволяет корректно установить все
// обработчики событий, сервисы и прочие сущности web-приложения.
extensionManager.registerExtension({
    name: "Powers Of Attorney web extension",
    version: "5.5.16",
    globalEventHandlers: [ CreatePowerOfAttorney, CreateRetrustPowerOfAttorney, ExportPowerOfAttorney, SignPowerOfAttorney, RevokePowerOfAttorney ],
    layoutServices: [ 
        Service.fromFactory($PowersOfAttorneyDemoController, (services: $RequestManager) => new PowersOfAttorneyDemoController(services))
    ]
})