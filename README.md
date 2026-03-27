[![Deploy](https://github.com/2Xpro-pop/TestAssignment/actions/workflows/deploy.yml/badge.svg?branch=master)](https://github.com/2Xpro-pop/TestAssignment/actions/workflows/deploy.yml)

# Тестовое задание

## Навигация
- [Запуск через Aspire](#если-у-вас-есть-aspire)
- [Запуск без Aspire](#если-у-вас-нет-aspire)
- [Все роуты](#все-роуты)
- [Тестовые пользователи](#тестовые-пользователи)
- [ТЗ](#тз)
- [Чеклист соответствия требованиям](#чеклист-соответствия-требованиям)

## Если у вас есть aspire 
Если у вас есть aspire можете просто запустить проект, все необходимые зависимости загрузит сам aspire при условии что у вас есть Docker Desktop, и он запущен.

## Если у вас нет aspire
Если у вас нет aspire то вам нужно зайти в ветку [aspire-ouput](https://github.com/2Xpro-pop/TestAssignment/tree/aspire-output) и загрузить `.env.Production` и `docker-compose.yaml`

### Затем нужно загрузить пакейджы 

```bash
docker compose --env-file .env.Production -f docker-compose.yaml pull
```

Ну и поднять пайкеджы

```bash
docker compose --env-file .env.Production -f docker-compose.yaml up -d --remove-orphans
```

## Все роуты
- [V1/IdentityApi](https://github.com/2Xpro-pop/TestAssignment/blob/cd45991c992160d029a234f755919bf7a8929829/src/TestAssignment.IdentityApi/V1/IdentityApiV1.cs#L13) - логин, logout, выдача/инвалидация токенов
- [V1/PaymentApi](https://github.com/2Xpro-pop/TestAssignment/blob/cd45991c992160d029a234f755919bf7a8929829/src/TestAssignment.PaymentApi/V1/PaymentApi.cs#L16) - списание `1.1 USD`, хранение платежей, баланс
- [Yarp](https://github.com/2Xpro-pop/TestAssignment/blob/cd45991c992160d029a234f755919bf7a8929829/src/TestAssignment.AppHost/YarpExtensions.cs#L22) - единая точка входа

## Тестовые пользователи

Все тестовые пользователи находятся в [IdentityDbContextSeeder](https://github.com/2Xpro-pop/TestAssignment/blob/cd45991c992160d029a234f755919bf7a8929829/src/TestAssignment.IdentityApi/Infrastructure/Persistence/IdentityDbContextSeeder.cs#L9)

| Login | Password |
|-------|----------|
| test  | test123  |
| admin | admin123 |

## ТЗ
```
Сделать API с возможностью авторизации пользователя и совершения платежа только после успешной авторизации. 
Должны быть 3 endpoint'а: login (вводим логин и пароль, при успехе выдает токен), logout (делает токен недействительным) 
и payment (при добавлении пользователя в БД ставим баланс 8 USD, сама операция позволяет снимать с баланса пользователя 1.1 USD при каждом вызове, 
все совершенные платежи хранятся в БД). Сделанный проект надо выгрузить в репозиторий на Github. 

Требования к функционалу (авторизация):
- если логин/пароль неправильные - выводим ошибку
- одновременная поддержка нескольких сессий пользователя
- не хранить пароли в базе в открытом виде
- защита от брутфорса (подбора пароля)

Требования к функционалу (платеж):
- защита от ошибочных списаний (изоляция транзакций)
- отсутствие ошибок округления
- корректное хранение и операции с финансовыми данными

Требования к коду:
- конкретный стек (фреймворки и библиотеки) не принципиален
- простая реализация логики и БД

Требования к выполнению задания:
Проект должен быть упакован в Docker (предоставить Dockerfile).
Подготовить docker-compose.yml, который запускает:
сам проект;
все необходимые зависимости (например: база данных)
```

## Чеклист соответствия требованиям

| Требование | Как реализовано |
|------------|------------------|
| Если логин или пароль неверные — должна возвращаться ошибка | В endpoint [`login`](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.IdentityApi/V1/IdentityApiV1.cs#L43) возвращается ошибка авторизации при неверных учетных данных |
| Одновременная поддержка нескольких сессий пользователя | Каждый успешный вход [создаёт отдельную независимую сессию / токен](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.IdentityApi/Application/Users/Commands/Login/LoginCommandHandler.cs#L69) |
| Не хранить пароли в базе в открытом виде | Пароли хранятся в базе данных в виде [хэшей](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.IdentityApi/Infrastructure/Persistence/IdentityDbContextSeeder.cs#L29) |
| Защита от брутфорса | Реализовано ограничение / контроль неудачных [попыток входа](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.IdentityApi/Application/Users/Commands/Login/LoginCommandHandler.cs#L53) |
| Платёж возможен только после успешной авторизации | Endpoint `payment` требует [валидный токен доступа](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.PaymentApi/V1/PaymentApi.cs#L21) |
| При добавлении пользователя в БД баланс равен 8 USD | Тестовые пользователи [создаются](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.PaymentApi/Domain/Accounts/Account.cs#L27) с начальным балансом `8.00 USD` |
| Каждый вызов `payment` списывает 1.1 USD | При каждом успешном вызове `payment` с баланса [списывается](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.PaymentApi/Application/Payments/CreatePayment/CreatePaymentCommandHandler.cs#L16) `1.10 USD` |
| Все совершённые платежи хранятся в БД | Каждая успешная операция платежа сохраняется в базе данных |
| Защита от ошибочных списаний | Защита реализована через optimistic concurrency: у счёта используется `Guid` [concurrency token](https://github.com/2Xpro-pop/TestAssignment/blob/c327c2ba02acb12c23eb308736e176388d645ec6/src/TestAssignment.PaymentApi/Infrastructure/Configurations/AccountConfiguration.cs#L48), поэтому конкурентные изменения не могут быть незаметно перезаписаны; сохранение выполняется через [UnitOfWork](https://github.com/2Xpro-pop/TestAssignment/blob/cd45991c992160d029a234f755919bf7a8929829/src/TestAssignment.ServiceDefaults/IUnitOfWork.cs) |
| Отсутствие ошибок округления | Для [денежных](https://github.com/2Xpro-pop/TestAssignment/blob/master/src/TestAssignment.PaymentApi/Domain/Shared/Money.cs) который хранит сумму в минимальных единицах (`long`), что исключает ошибки округления при вычислениях |
| Корректное хранение и операции с финансовыми данными | Денежные значения хранятся и обрабатываются в формате, подходящем для финансовых операций |
| Проект упакован в Docker | Используетьяс Aspire который сам упаковывает все в докер |
| Подготовлен `docker-compose.yml` | В репозитории есть `docker-compose.yaml` для запуска приложения и зависимостей, но в ветке [aspire-ouput](https://github.com/2Xpro-pop/TestAssignment/tree/aspire-output)|
| В docker-compose включены необходимые зависимости | Вместе с приложением поднимается PostgreSQL и остальные нужные сервисы |

