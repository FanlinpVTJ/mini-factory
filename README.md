# Mini Factory

Небольшой мобильный idle-проект на Unity. На одном экране игрок открывает и улучшает производственные машины, получает валюту в реальном времени и после отсутствия, активирует временный Boost и может купить набор валюты через Unity IAP.

## Требования

- Unity `6000.3.22f1`;
- установленный модуль Android Build Support для Android-сборки;
- активная сцена `Assets/GameAssets/Scenes/LeaderboardScene.unity`;
- Unity Test Framework для Edit Mode тестов.

## Запуск

1. Открыть корень репозитория через Unity Hub с Unity `6000.3.22f1`.
2. Дождаться импорта пакетов и компиляции скриптов.
3. Открыть `Assets/GameAssets/Scenes/MainScene.unity`.
4. Убедиться, что в `ProjectContext` установлен существующий `ValuesInstaller`, а в контексте игровой сцены установлен `FactoryInstaller`.
5. Запустить Play Mode.

`FactoryInstaller` получает `FactoryConfigurationAsset` из сериализованного поля. Код не создаёт конфигурационные ассеты во время выполнения.

## Конфигурация

Локальная конфигурация хранится в `FactoryConfigurationAsset`. Ассет создаётся через `Create > Mini Factory > Factory Configuration` и назначается в `FactoryInstaller`.

Настраиваемые параметры:

- строковый ID валюты;
- список машин, их начальное состояние, производство, стоимость открытия, формула улучшения и максимальный уровень;
- доступность, длительность и множитель Boost;
- максимальная продолжительность offline production;
- Product ID и размер награды за покупку.

В `ValueSystem` должен существовать `ValueData` с тем же ID, который указан в конфигурации фабрики. Сам `ValueData` назначается существующему `ValuesInstaller`. Фабрика обращается к валюте только по строковому ID через `IValueSystem`.

Конфигурация доступна игровой логике через `IFactoryConfigurationSource`. Благодаря этому локальный ScriptableObject можно заменить или дополнить Remote Config без изменения экономики и производства.

## UI

- `ValueView` показывает баланс валюты;
- `FactoryProductionView` показывает общую производительность;
- отдельный `MachineView` отображает состояние, уровень, производство и цены каждой машины;
- `BoostView` отображает готовность, множитель и оставшееся время Boost;
- `OfflineIncomeView` показывает начисленный offline-доход;
- `PurchaseView` отображает цену, награду, состояние магазина и запускает покупку.

Идентификатор в каждом `MachineView` должен совпадать с идентификатором соответствующей машины в `FactoryConfigurationAsset`.

## Сохранение и lifecycle

Прогресс фабрики хранится в активном профиле `GameSaver` вместе с данными `ValueSystem`. Сохраняются уровни машин, производственный timestamp, остаток дробного дохода, окончание Boost и обработанные IAP-транзакции.

При сворачивании производство фиксирует текущее состояние. После возврата или нового запуска доход рассчитывается по реально прошедшему времени с учётом Boost и ограничения максимального offline-интервала. Повторная выдача награды для уже обработанной транзакции блокируется.

## Unity IAP

Используется Unity In-App Purchasing `5.0.4` и consumable-продукт `coins_pack_small` по умолчанию. Gameplay зависит от `IPurchaseService`, а работа с Unity Purchasing изолирована в `UnityPurchaseService`.

Проверка через Fake Store:

1. Запустить проект в Editor.
2. Дождаться состояния готовности на кнопке покупки.
3. Нажать кнопку покупки.
4. В окне Fake Store подтвердить или отменить операцию.
5. При подтверждении проверить начисление валюты и событие `purchase_succeeded` в Console.
6. При отмене проверить отсутствие начисления и событие `purchase_failed`.

Для реального магазина необходимо зарегистрировать тот же Product ID в Google Play Console.

## Аналитика

`IAnalyticsService` отправляет каждое событие всем зарегистрированным `IAnalyticsProvider`. В обязательной реализации подключён `ConsoleAnalyticsProvider`; gameplay не зависит от конкретного SDK.

Поддерживаются события:

- `game_started`;
- `machine_unlocked`;
- `machine_upgraded`;
- `boost_started`;
- `boost_finished`;
- `offline_income_applied`;
- `purchase_succeeded`;
- `purchase_failed`.

## Тесты

Открыть `Window > General > Test Runner`, выбрать `Edit Mode` и нажать `Run All`.

В `FactoryLogicTests` проверяются:

- открытие машины и списание валюты;
- улучшение машины и рост цены;
- ограничение продолжительности offline production;
- расчёт производства и завершение Boost по прошедшему времени.

Все четыре теста успешно пройдены в Unity Editor.

## Android build

1. Открыть `File > Build Profiles` и выбрать Android.
2. Выполнить `Switch Platform`.
3. Убедиться, что игровая сцена включена в список сцен.
4. Задать итоговые Product Name, Package Name, версию и параметры подписи.
5. Выбрать `Build` или `Build And Run`.

Android build на устройстве или эмуляторе в рамках текущей работы не запускался. Перед сдачей нужно указать здесь использованное устройство или эмулятор и результат проверки.

## Основные решения

- Zenject отвечает за создание и связывание сервисов;
- `FactoryEconomy` управляет состоянием машин и использует существующий `IValueSystem`;
- `FactoryProduction` рассчитывает активный, boosted и offline-доход по часам `IFactoryClock`;
- `FactorySession` связывает lifecycle, периодическое сохранение и восстановление;
- `GameSaverFactoryProgressStorage` изолирует фабрику от конкретного API сохранения;
- `IPurchaseService` и интерфейсы аналитики изолируют сторонние SDK;
- UI подписывается на события доменных сервисов.

## Использованные пакеты и системы

- Unity IAP `5.0.4`;
- Unity Test Framework `1.6.0`;
- Zenject;
- ValueSystem;
- GameSaver;
- TextMesh Pro / Unity UI;
- Newtonsoft Json, используемый инфраструктурой проекта.

## Известные ограничения

- подключён только Console analytics provider;
- Firebase Remote Config не подключён, используется локальный ScriptableObject;
- реальный продукт Google Play не создавался, покупки проверяются через Fake Store;
- Android build и profiling не выполнялись;
- в `ProjectSettings` пока сохранен шаблонный Android application ID;


