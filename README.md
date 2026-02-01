# RegistrationAndAuthorisationViaCookieMinimalAPI

Минималистичное API на .NET 9 для регистрации и авторизации пользователей с использованием Cookie-аутентификации, безопасного хеширования паролей через Argon2 и поддержки `ClaimsPrincipal`.

## Содержание

* [Функциональность](#функциональность)
* [Технологии](#технологии)
* [Установка и настройка](#установка-и-настройка)
* [Миграции и база данных](#миграции-и-база-данных)
* [Запуск приложения](#запуск-приложения)
* [Краткое описание эндпоинтов](#краткое-описание-эндпоинтов)
* [Тестирование](#тестирование)
* [Скриншоты](#скриншоты)

## Функциональность

* Регистрация пользователей с безопасным хешированием пароля (Argon2)
* Авторизация с помощью Cookie и Claims
* Доступ к защищённым ресурсам по policy
* Выход пользователя
* Получение текущего авторизованного пользователя

## Технологии

* .NET 9 (Minimal API)
* Entity Framework Core
  * `Microsoft.EntityFrameworkCore`
  * `Microsoft.EntityFrameworkCore.Design` — для миграций
  * `Npgsql.EntityFrameworkCore.PostgreSQL` — для PostgreSQL
  * `Microsoft.EntityFrameworkCore.InMemory` — для тестов
* Аутентификация: `AddAuthentication("Cookies")`
* Хеширование паролей: `Konscious.Security.Cryptography.Argon2`
* Юнит-тесты: `Moq`, `xUnit`

## Установка и настройка

1. Клонировать репозиторий:



```bash
git clone https://github.com/Archikey/RegistrationAndAuthorisationViaCookieMinimalAPI.git
cd RegistrationAndAuthorisationViaCookieMinimalAPI
git checkout development
```

2. Установить зависимости:
    

```bash
dotnet restore
```

3. Настроить строку подключения в `appsettings.json`:
    

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=YourDbName;Username=postgres;Password=your_password"
}
```

## Миграции и база данных

Используемые пакеты:

- `Npgsql.EntityFrameworkCore.PostgreSQL`
    
- `Microsoft.EntityFrameworkCore`
    
- `Microsoft.EntityFrameworkCore.Design`
    

### Создание первой миграции

```bash
dotnet ef migrations add InitMigration
```

### Применение миграции

```bash
dotnet ef database update
```

## Запуск приложения

```bash
dotnet run
```

По умолчанию API будет доступен на `https://localhost:5001` (или `http://localhost:5000`).

## Краткое описание эндпоинтов

|Метод|URL|Описание|Авторизация|
|---|---|---|---|
|POST|`/register`|Регистрация нового пользователя|❌|
|POST|`/login`|Вход пользователя|❌|
|POST|`/logout`|Выход пользователя|✅ (cookie)|
|GET|`/profile`|Получение текущего пользователя|✅|
|GET|`/secret`|Защищённый доступ по policy "User"|✅ (policy)|

## Примеры запросов

### 🔸 Регистрация

```http
POST /register
Content-Type: application/json

{
  "userName": "TestUser",
  "email": "test@test.test",
  "password": "password"
}
```

### 🔸 Вход

```http
POST /login
Content-Type: application/json

{
  "userName": "TestUser",
  "email": "test@test.test",
  "password": "password"
}
```

После успешного входа Cookie сохраняется — можно обращаться к `/profile` и `/secret`.

## Тестирование

Модульные тесты реализованы с использованием:

- `Moq`
    
- `xUnit`
    
- `Microsoft.EntityFrameworkCore.InMemory`
    

Пример запуска тестов:

```bash
cd tests
dotnet test
```

## Скриншоты

![Снимок экрана 2025-07-09 121718](https://github.com/user-attachments/assets/1d478733-6a02-4525-a5e9-d40d030ad68e)
![Снимок экрана 2025-07-09 121745](https://github.com/user-attachments/assets/2e14401e-740f-4caf-984c-96fa7c269232)
![Снимок экрана 2025-07-09 121752](https://github.com/user-attachments/assets/543771ff-de34-4573-8c2b-d313e9913622)
![Снимок экрана 2025-07-09 122543](https://github.com/user-attachments/assets/023cd912-afca-4425-b566-0d1333cd2ac2)
![Снимок экрана 2025-07-09 122612](https://github.com/user-attachments/assets/fcba18e1-d3a3-40ed-9f07-eb727169fc44)
![Снимок экрана 2025-07-09 122627](https://github.com/user-attachments/assets/87d1d37e-6a15-4e5b-9e64-b923976bce49)



