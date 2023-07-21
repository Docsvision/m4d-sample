﻿# PowersOfAttorney

Данный пример содержит методы работы с машиночитаемой доверенностью.

## Сокращения

СКД - системная карточка доверенности
ПКД - пользовательская карточки доверенности
МЧД - машиночитаемая доверенность

## Описание примера

Пример включает два компонента:

- PowersOfAttorney > PowersOfAttorneyServerExtension – папка с серверным расширением Web-клиента, в котором реализованы функции создания СКД из демонстрационной карточки доверенностей.
- PowersOfAttorney > PowersOfAttorneyWebExtension – папка с клиентским расширением, в котором реализованы обработчики смены состояния ПКД и управления МЧД, видимостью и обязательностью полей.
- PowersOfAttorney > Data > PowersOfAttorneySolution - папка с решением, включающим разметки тестовой карточки доверенности.
- PowersOfAttorney > Data > SolutionOfPOA.sol - решение, добавляющее в систему Docsvision новые виды карточки Документ (Доверенность и Передоверие) и необходимые расширенные метаданные.


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
   
   Компоненты данного решения:
    - Справочник видов: 
      1. Документ - МЧД Доверенность(версия 002), Передоверие(версия 002), Доверенность(версия EMHCD_1), Передоверие(версия EMHCD_1) (*вид находится в разработке*).
      2. Задание - Задание КС - Подписание МЧД, Согласование МЧД
    - Метаданные: Полномочия, МЧД формат единой формы, Текстовые полномочия (а также все подчиненные)
    - Конструктор правил нумерации: МЧД
    - Поиск: МЧД (МЧД все, МЧД действующие, МЧД – я автор, МЧД  – я подписант, МЧД отозванные, МЧД мои, Поиск МЧД)
    - Представление: МЧД -> МЧД единого формата - представление
    - Папки: 
	   - Папка "Доверенность" для хранения маршрутов согласования и подчиненные папки для работы с доверенностями.
    - Согласование: маршрут "согласование и подписание МЧД единого формата" - согласование настроено на группу юридического отдела (справочник сотрудников), подписание настроено на группу генеральные директора (справочник сотрудников). Согласование параллельное, без ограничений по времени. Можно убрать участников при отправке на согласование.
	
	Обратите внимание, что импорт решения может занять длительное время, примерно 1 час.

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

7. Нажмите кнопку создания МЧД. В результате будет создана МЧД, связанная с текущей карточкой документа.
   Для создания своей разметки нужно:
      - добавить скрипт createPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности (версия 002) в конструкторе разметок;
      - добавить скрипт createRetrustPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки передоверия (версия 002) в конструкторе разметок.
      - добавить скрипт createEMCHDPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки доверенности (версия EMCHD_1) в конструкторе разметок. 

8. Нажмите кнопку экспорта МЧД. На компьютер будет сохранён архив, содержащий файл МЧД в формате XML.
   Для создания своей разметки нужно добавить скрипт exportPowerOfAttorneyWithoutSignature в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

9. Нажмите кнопку подписания МЧД. Будет предложено выбрать сертификат подписи - выполнено подписание МЧД.
   Для создания своей разметки нужно добавить скрипт signPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки доверенности и передоверия в конструкторе разметок.

10. Нажмите кнопку экспорта МЧД. На компьютер будет сохранён архив, содержащий файл МЧД в формате XML и его подпись.
   Для создания своей разметки нужно добавить скрипт exportPowerOfAttorneyWithSignature в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

11. Нажмите кнопку отзыва МЧД. 
      - Для доверенности и передоверия (версия 002) появится окно заявления на отзыв, после заполнения которого появится окно с возможностью подписать сформированное заявление на отзыв, затем демокарточка будет переходить в состояние "Отозвана".
      Для создания своей разметки нужно добавить скрипт revokePowerOfAttorney в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности (версия 002) и передоверия (версия 002) в конструкторе разметок.
      - Для доверенности (версия EMCHD_1) демокарточка будет переходить в состояние "Отозвана".
      Для создания своей разметки нужно добавить скрипт revokePowerOfAttorneyWithoutApplication в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности (версия EMCHD_1) в конструкторе разметок.
   
12. Нажмите кнопку экспорта заявления на отзыв (для доверенности и передоверия (версия 002)). На компьютер будет сохранён архив, содержащий файл заявления на отзыв в формате XML и его подпись.
   Для создания своей разметки нужно добавить скрипт exportApplicationForRevocation в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности (версия 002) и передоверия (версия 002) в конструкторе разметок.

13. Нажмите кнопку удаления пользовательской карточки доверенности. С пользовательской карточки доверенности также удаляется системная карточка доверенности.
   Для создания совей разметки нужно добавить скрипт  deletePowerOfAttorney в качестве обработчика события "On card deleting" на root в конструкторе разметок.

14. Доверенность можно подписать в ходе согласования. Отправьте Доверенность на согласование как описано в Инструкции пользователя.
    Нажмите кнопку "Подписать" в Задании. В результате сформируется СКД, затем подпишется доверенность, ПКД перейдет в статус "Подписана", а задание перейдет в статус "Завершено".
    Для создания своей разметки нужно добавить скрипт signPowerOfAttorneyFromTask в качестве обработчика события "Before executing operation" на ЭУ StateButtons.
