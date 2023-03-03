﻿# PowersOfAttorney

Данный пример содержит методы работы с машиночитаемой доверенностью.

## Сокращения

СКД - системная карточка доверенности
ПКД - пользовательская карточки доверенности
МЧД - машиночитаемая доверенность

## Описание примера

Пример включает два компонента:

- PowersOfAttorneyServerExtension – папка с серверным расширением Web-клиента, в котором реализованы функции создания СКД из демонстрационной карточки доверенностей.
- PowersOfAttorneyWebExtension – папка с клиентским расширением, в котором реализованы обработчики жизненного цикла ПКД.
- PowersOfAttorneySolution.zip - архив с решением, включающим разметки тестовой карточки доверенности.
- PowersOfAttorneyCardsMetadata.zip - архив с данными карточек, добавляющие в систему Docsvision новые виды карточки Документ (Доверенность и Передоверие) и необходимые расширенные метаданные.


## Настройка среды разработки

**Перечень необходимых инструментов:** 
* [Visual Studio 2017/2019](https://www.visualstudio.com)
* [NodeJS v14.17.0+](https://nodejs.org/en/)

## Сборка проекта

1. Сборка серверной части.
   1. Откройте решение Samples.sln
   2. Соберите проект Other > PowersOfAttorney > PowersOfAttorneyServerExtension

2. Сборка клиентской части.
   1. Откройте в командной строке папку Others > PowersOfAttorney > PowersOfAttorneyWebExtension.

   2. Выполните команды:

      ```
      npm install
      npm update
      npm run build:prod
      ```

3. Публикация компонентов на сервере Web-клиент.

   1. Скопируйте папку `SamplesOutput\Site\Content\Modules\PowersOfAttorneyWebExtension\` в  `<Каталог установки Web-клиента>\Site\Content\Modules`.
   2. Скопируйте папку `SamplesOutput\Site\Extensions\PowersOfAttorneyServerExtension` в  `<Каталог установки Web-клиента>\Site\Extensions`.
   3. Перезапустите IIS.

## Проверка примера

1. Распакуйте PowersOfAttorneyCardsMetadata.zip и импортируйте данные из архива в систему Docsvision.

2. Распакуйте PowersOfAttorneySolution.zip и в Конструкторе web-разметок импортируйте решение Solution.xml.

3. В Web-клиенте создайте карточку документ вида Доверенность. Заполните обязательные поля. Если заполнены не все обязательные поля, при сохранении будет выдано предупреждение. 

4. Нажмите кнопку создания МЧД. В результате будет создана МЧД, связанная с текущей карточкой документа.
   Для создания своей разметки нужно:
      -добавить скрипт createPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности в конструкторе разметок.
      -добавить скрипт createRetrustPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки передоверия в конструкторе разметок.

5. Нажмите кнопку экспорта МЧД. На компьютер будет сохранён архив, содержащий файл МЧД в формате XML.
   Для создания своей разметки нужно добавить скрипт exportPowerOfAttorneyWithoutSignature в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

6. Нажмите кнопку подписания МЧД. Будет предложено выбрать сертификат подписи - выполнено подписание МЧД.
   Для создания своей разметки нужно добавить скрипт signPowerOfAttorney в качестве обработчика события "On click" на кнопку для разметок просмотра карточки доверенности и передоверия в конструкторе разметок.

7. Нажмите кнопку экспорта МЧД. На компьютер будет сохранён архив, содержащий файл МЧД в формате XML и его подпись.
   Для создания своей разметки нужно добавить скрипт exportPowerOfAttorneyWithSignature в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

8. Нажмите кнопку отзыва МЧД. Демокарточка будет переходить в состояние "Отозвана".
   Для создания своей разметки нужно добавить скрипт revokePowerOfAttorney в качестве обработчика события "On click" на кнопку для разметки просмотра карточки доверенности и передоверия в конструкторе разметок.

9. Нажмите кнопку удаления пользовательской карточки доверенности. С пользовательской карточки доверенности также удаляется системная карточка доверенности.
   Для создания совей разметки нужно добавить скрипт  deletePowerOfAttorney в качестве обработка события "On card deleting" на root в конструкторе разметок.