﻿# PowersOfAttorney

Данный пример содержит методы работы с машиночитаемой доверенностью для Web-клиента и для Windows клиента.

## Сокращения

СКД - системная карточка доверенности.

ПКД - пользовательская карточки доверенности.

МЧД - машиночитаемая доверенность.

## Описание примера

Пример включает компоненты:

- PowersOfAttorney > PowersOfAttorneyServerExtension – папка с серверным расширением Web-клиента, в котором реализованы функции создания СКД из демонстрационной карточки доверенностей.
- PowersOfAttorney > PowersOfAttorneyWebExtension – папка с клиентским расширением, в котором реализованы обработчики смены состояния ПКД и управления МЧД, видимостью и обязательностью полей.
- PowersOfAttorney > PowersOfAttorney.UserCard.Common - папка с проектом PowersOfAttorney.UserCard.Common, который используется и для Web-клиента и для Windows клиента. 
- PowersOfAttorney > PowersOfAttorney.Scripts - содержит файл скрипта для Windows клиента. 
- PowersOfAttorney > Data > PowersOfAttorneySolution - папка с решением, включающим разметки тестовой карточки доверенности.
- PowersOfAttorney > Data > SolutionOfPOA.sol - решение, добавляющее в систему Docsvision новые виды карточки Документ (Доверенность и Передоверие) и необходимые расширенные метаданные.

**Примечание:** Обращаем внимание, что текущие виды передоверия предназначены только для ознакомления. Для их использования необходима доработка под ситуации, не требующие нотариального удостоверения. По умолчанию создание карточек данных видов запрещено.
## Ограничения примера
   1. Виды передоверия в примере предназначены только для ознакомления. Для их использования необходима доработка под ситуации, не требующие нотариального удостоверения. По умолчанию создание карточек данных видов запрещено.
   2. Формат 5.01 не реализован.
   3. Для формата 002 не реализованы следующие сценарии:

      3.1. Основная доверенность
      | Доверитель | Лицо, действующее без доверенности от имени доверителя | Представитель |
      | --- | --- | --- |
      | Российское юридическое лицо | Юридическое лицо | Организация / Индивидуальный предприниматель |
      | Иностранное юридическое лицо | Все типы | Все типы |
      | Индивидуальный предприниматель | Все типы | Все типы |
      | Физическое лицо | Все типы | Все типы |

      3.2. Передоверие 
      | Лицо, передавшее полномочия | Лицо, получившее полномочия |
      | --- | --- |
      | Российское юридическое лицо | Все типы |
      | Индивидуальный предприниматель | Все типы |
      | Физическое лицо | Организация/ Индивидуальный предприниматель |

      3.3. Нотариальное удостоверение доверенностей.
   4. Для формата EMCHD_1 (003) не реализованы следующие сценарии:

      4.1. Основная доверенность
      | Доверитель | Единоличный исполнительный орган | Представитель |
      | --- | --- | --- |
      | Российское юридическое лицо | Управляющая компания/ Индивидуальный предприниматель | Юридическое лицо/ Индивидуальный предприниматель/ Филиал (обособленное подразделение) российского юридического лица/ Филиал (аккредитованное представительство) иностранного юридического лица |
      | Иностранное юридическое лицо | Все типы | Все типы |
      | Индивидуальный предприниматель | Все типы | Все типы |
      | Физическое лицо | Все типы | Все типы |

      4.2. Передоверие

      | Лицо, передавшее полномочия | Лицо, получившее полномочия |
      | --- | --- |
      | Российское юридическое лицо | Все типы |
      | Индивидуальный предприниматель | Все типы |
      | Физическое лицо | Юридическое лицо/ Индивидуальный предприниматель/ Филиал (обособленное подразделение) российского юридического лица/ Филиал (аккредитованное представительство) иностранного юридического лица |
      | Филиал (обособленное подразделение) российского юридического лица | Все типы |
      | Филиал (аккредитованное представительство) иностранного юридического лица | Все типы |

      4.3. Нотариальное удостоверение доверенностей.

## Настройка среды разработки

**Перечень необходимых инструментов:** 
* [Visual Studio 2017/2019](https://www.visualstudio.com)
* [NodeJS v14.17.0+](https://nodejs.org/en/)

## Сборка проекта

1. Сборка серверной части.
   1. Откройте решение PowersOfAttorney > PowersOfAttorneyServerExtension.sln
   2. Соберите проект PowersOfAttorneyServerExtension

2. Сборка клиентской части.
   1. Откройте в командной строке папку PowersOfAttorney > PowersOfAttorneyWebExtension.

   2. Выполните команды:

      ```
      npm install
      npm update
      npm run build:prod
      ```

3. Публикация компонентов на сервере Web-клиент.

   1. Скопируйте папку `PowersOfAttorney\SamplesOutput\Site\Content\Modules\PowersOfAttorneyWebExtension\` в  `<Каталог установки Web-клиента>\Site\Content\Modules`.
   2. Скопируйте папку `PowersOfAttorney\SamplesOutput\Site\Extensions\PowersOfAttorneyServerExtension` в  `<Каталог установки Web-клиента>\Site\Extensions`.
   3. Перезапустите IIS.

## Проверка примера

**Для проверки примера требуется установить версию Backoffice и Web-клиент с поддержкой СКД.**

1. В Менеджере Решений импортируйте решение "Машиночитаемая доверенность" - `PowersOfAttorney\Data\SolutionOfPOA.sol`. 
   
   **Для сокращения времени импорта решения рекомендуем использовать версию Менеджера решений - 5.5.3494.35 и выше.**

   Компоненты данного решения:
    - Справочник видов: 
      1. Документ - МЧД Доверенность(версия 002), Передоверие(версия 002), Доверенность(версия EMHCD_1), Передоверие(версия EMHCD_1).
      2. Задание - Задание КС - На подписание МЧД, На согласование МЧД
    - Метаданные: Полномочия, МЧД формат единой формы, Текстовые полномочия (а также все подчиненные)
    - Конструктор правил нумерации: МЧД
    - Поиск: МЧД (МЧД все, МЧД действующие, МЧД – я автор, МЧД  – я подписант, МЧД отозванные, МЧД мои, Поиск МЧД)
    - Представление: МЧД -> МЧД единого формата - представление
    - Папки: 
	   - Папка "Доверенность" для хранения маршрутов согласования и подчиненные папки для работы с доверенностями.
    - Согласование: маршрут "согласование и подписание МЧД единого формата" - согласование настроено на группу юридического отдела (справочник сотрудников), подписание настроено на группу генеральные директора (справочник сотрудников). Согласование параллельное, без ограничений по времени. Можно убрать участников при отправке на согласование.
	
2. В справочнике видов (тип - Согласование, вид - Усовершенствованное согласование) настройте согласование для видов карточек МЧД. В окне "Настройка способа создания карточки" выберите шаблон "Согласование и подписание МЧД единый формат".

3. В конструкторе web-разметок импортируйте решение "Машиночитаемая доверенность". Для этого выберите файл Solution.xml в папке решения `PowersOfAttorney\Data\PowersOfAttorneySolution\`.

4. Проверьте, что в Справочнике сотрудников заполнены поля для участников МЧД как описано в Инструкции пользователя.

5. В Web-клиенте создайте карточку документ вида Доверенность (версия 002). Заполните обязательные поля. Если заполнены не все обязательные поля, при сохранении будет выдано предупреждение. 
   Для создания своей разметки нужно:
      - добавить скрипт для отображения в модальной окне незаполненных обязательных полей showRequiredFields в качестве обработчика события "Before card saving" root для разметок создания и/или редактирования в конструкторе разметок;
      - добавить скрипт customizePowerOfAttorneyCardForEditLayout в качестве обработчика события "On all controls loaded" root для разметок создания и редактирования карточки доверенности в конструкторе разметок;
      - добавить скрипт customizePowerOfAttorneyCardForViewCard в качестве обработчика события "On all controls loaded" root для разметок просмотра карточки доверенности в конструкторе разметок;
      - добавить скрипт customizeSubstitutionPowerOfAttorneyCardForEditLayout в качестве обработчика события "On all controls loaded" root для разметок создания и редактирования карточки передоверия в конструкторе разметок;
      - добавить скрипт customizeSubstitutionPowerOfAttorneyCardForViewLayout в качестве обработчика события "On all controls loaded" root для разметок просмотра карточки передоверия в конструкторе разметок.

6. В Web-клиенте создайте карточку документ вида Доверенность (версия EMCHD_1). Заполните обязательные поля. Если заполнены не все обязательные поля, при сохранении будет выдано предупреждение. 
   Для создания своей разметки нужно:
      - добавить скрипт для отображения в модальной окне незаполненных обязательных полей showRequiredFields в качестве обработчика события "Before card saving" root для разметок создания и/или редактирования в конструкторе разметок;
      - добавить скрипт customizeSingleFormatPowerOfAttorneyForEditLayout в качестве обработчика события "On all controls loaded" root для разметок создания и редактирования карточки доверенности в конструкторе разметок; 
      - добавить скрипт customizeSingleFormatPowerOfAttorneyForViewLayout в качестве обработчика события "On all controls loaded" root для разметок просмотра, описания и справки карточки доверенности в конструкторе разметок;
      - добавить скрипт customizeSingleFormatSPOACardForEditLayout в качестве обработчика события "On all controls loaded" root для разметок создания и редактирования карточки передоверия в конструкторе разметок;
      - добавить скрипт customizeSingleFormatSPOACardForViewLayout в качестве обработчика события "On all controls loaded" root для разметок просмотра, описания и справки карточки передоверия в конструкторе разметок;
      - добавить скрипт customizeSingleFormatPowerOfAttorneyForLocationLayout в качестве обработчика события "On all controls loaded" root для разметки локация карточки доверенности в конструкторе разметок;
      - добавить скрипт customizeSingleFormatPowerOfAttorneyForLocationLayout в качестве обработчика события "On all controls loaded" root для разметки локация карточки передоверия в конструкторе разметок;

7. Нажмите кнопку создания МЧД. В результате будет создана МЧД, связанная с текущей карточкой документа.
   Для создания своей разметки нужно:
      - добавить скрипт createPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности (версия 002) в конструкторе разметок;
      - добавить скрипт createRetrustPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки передоверия (версия 002) в конструкторе разметок.
      - добавить скрипт createEMCHDPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки доверенности (версия EMCHD_1) в конструкторе разметок.
      - добавить скрипт createEMCHDRetrustPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки передоверия (версия EMCHD_1) в конструкторе разметок.

8. Нажмите кнопку экспорта МЧД. На компьютер будет сохранён архив, содержащий файл МЧД в формате XML.
   Для создания своей разметки нужно добавить скрипт exportPowerOfAttorneyWithoutSignature в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

9. Если планируется отправлять доверенность на регистрацию в распределённый реестр ФНС через провайдеров внешнего ЭДО необходимо выполнить соответствующие настройки.
	Для работы через Контур.Диадок выполните настройки описанные в [документации модуля ЭДО](https://help.docsvision.com/edi/5.5.5/admin/attorney-settings/).
	Для этого случая, в примере решения есть преднастроенные разметки: «Доверенность версии 002 (просмотр)_Контур.Диадок» и «Доверенность версии EMCHD_1 (просмотр)_Контур.Диадок». 
	Для работы через Контур.Доверенность выполните настройки описанные в [документации модуля Интеграции с реестром МЧД](https://help.docsvision.com/m4d-registry/dev/admin/connection-settings/).
	Для этого случая, в примере решения есть преднастроенные разметки: «Доверенность версии 002 (просмотр)_Контур.Доверенность» и «Доверенность версии EMCHD_1 (просмотр)_Контур.Доверенность». 

10. Нажмите кнопку подписания МЧД. Будет предложено выбрать сертификат подписи - выполнено подписание МЧД.
   - Для создания своей разметки нужно добавить скрипт signPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки доверенности и передоверия в конструкторе разметок.
   - При работе через Контур.Диадок, для подписания и последующей регистрации доверенности по файлу нужно добавить скрипт signAndSendPowerOfAttorneyToRegistrationAsFile в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности.
   Если требуется разделить события подписания и отправки, можно воспользоваться скриптом sendPowerOfAttorneyToRegistrationAsFile (он выполняет только отправку уже подписанной доверенности в реестр через Контур.Диадок).
   - При работе через Контур.Доверенность, для подписания и последующей регистрации доверенности по файлу нужно добавить скрипт signAndSendForRegistrationToRegistry в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности.
   - Если требуется разделить события подписания и отправки, можно воспользоваться скриптом sendForRegistrationToRegistry (он выполняет только отправку уже подписанной доверенности в реестр через Контур.Доверенность).

11. Нажмите кнопку экспорта МЧД. На компьютер будет сохранён архив, содержащий файл МЧД в формате XML и его подпись.
   Для создания своей разметки нужно добавить скрипт exportPowerOfAttorneyWithSignature в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

12. Нажмите кнопку отзыва МЧД. 
   Для доверенности и передоверия появится окно заявления на отзыв, после заполнения которого появится окно с возможностью подписать сформированное заявление на отзыв, затем демокарточка будет переходить в состояние "Отозвана".
   - Для создания своей разметки нужно добавить скрипт revokePowerOfAttorney в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.
   - Для создания своей разметки с отзывом без заявления нужно добавить скрипт revokePowerOfAttorneyWithoutApplication в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.
   - При работе через Контур.Доверенность, для формирования, подписания и последующей отправки заявления на отзыв в ЦПРР ФНС нужно добавить скрипт revokeAndRecallPowerOfAttorney в качестве обработчика события "On click" на кнопку отзыва для разметки просмотра карточки доверенности.
   - Если требуется разделить события формирования заявления и его отправки, можно воспользоваться скриптом recallPowerOfAttorney (он выполняет только отправку уже сформированного и подписанного заявления на отзыв через Контур.Доверенность).
	
13. Нажмите кнопку экспорта заявления на отзыв. На компьютер будет сохранён архив, содержащий файл заявления на отзыв в формате XML и его подпись.
   Для создания своей разметки нужно добавить скрипт exportApplicationForRevocation в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

14. Нажмите кнопку удаления пользовательской карточки доверенности. С пользовательской карточки доверенности также удаляется системная карточка доверенности.
   Для создания совей разметки нужно добавить скрипт  deletePowerOfAttorney в качестве обработчика события "On card deleting" на root в конструкторе разметок.

15. Доверенность можно подписать в ходе согласования. Отправьте Доверенность на согласование как описано в Инструкции пользователя.
    Нажмите кнопку "Подписать" в Задании. В результате сформируется СКД, затем подпишется доверенность, ПКД перейдет в статус "Подписана", а задание перейдет в статус "Завершено".
    - При работе через Контур.Диадок, для создания своей разметки нужно добавить скрипт signAndSendPowerOfAttorneyToRegistrationAsFileFromTask в качестве обработчика события "Before executing operation" на ЭУ StateButtons для подписания и последующей регистрации доверенности по файлу.
	 - При работе через Контур.Доверенность, для создания своей разметки нужно добавить скрипт signAndSendPowerOfAttorneyToKonturForRegistrationAsFileFromTask в качестве обработчика события "Before executing operation" на ЭУ StateButtons для подписания и последующей регистрации доверенности по файлу.
	Для подписания без регистрации используйте скрипт signPowerOfAttorneyFromTask.

Пример скриптов для Windows клиента

1. Скрипт находится в файле CardDocumentДоверенность__версия_EMHCD_1_Script.cs в проекте PowersOfAttorney.Scripts
2. Проект нужен только для проверки компилируемости скрипта. Ссылки на сборку PowersOfAttorney.Scripts.dll добавлять не надо.
3. Скрипт необходимо скопировать в справочник скриптов для двух видов (для доверенности EMHCD и передоверия EMHCD). Если у родительского вида для этих видов нет скрипта, надо открыть его и сгенерировать для него скрипт по-умолчанию. 
4. Необходимо закомментировать в файле скрипта для обоих видов строчку "using CardDocumentМЧДScript = DocsVision.BackOffice.WinForms.ScriptClassBase;" 
Она нужна только для компиляции файла скрипта в составе проекта PowersOfAttorney.Scripts.
5. Скрипты для этих видов отличаются только названиями классов. Необходимо для каждого вида оставить только одно соответствующее ему название класса (см. комментарий в скрипте) 
6. В скриптах необходимо добавить ссылку на сборку PowersOfAttorney.UserCard.Common.dll, которую также необходимо распрастранить на все клиентские рабочие места.
7. В конструкторе разметок необходимо добавить кнопки (например в риббон). Названия кнопок должны соответствовать обработчикам в скрипте.
В скрипте обработчики выглядят как имяКнопки__ItemClick(); Если названия кнопки будут соответствовать обработчикам, то обработчики к кнопкам
привяжутся автоматически, вручную их создавать не надо.
8. Можно привязать к кнопкам соответствующие операции, чтобы кнопки были доступны только в тех состояниях, когда их нажатие имеет смысл.
