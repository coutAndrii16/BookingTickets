# Лабораторна робота №5 — Booking Tickets

## Структура рішення

```
BookingSln/
├── BookingData/          ← Бібліотека: моделі, контексти, репозиторії
├── BookingMvc/           ← MVC-проект: UI + Identity аутентифікація
└── BookingApi/           ← Web API: REST endpoints + JWT
```

---

## 1. Встановлення пакетів та міграції

### BookingData
```bash
cd BookingData
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
```

### BookingMvc
```bash
cd BookingMvc
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
```

### BookingApi
```bash
cd BookingApi
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
```

---

## 2. Міграції баз даних

Виконувати з папки **BookingMvc** (або **BookingApi**):

### EventDbContext (основна БД)
```bash
cd BookingMvc
dotnet ef migrations add Initial --context EventDbContext --output-dir Migrations/EventDb
dotnet ef database update --context EventDbContext
```

### AppIdentityDbContext (Identity)
```bash
dotnet ef migrations add InitialIdentity --context AppIdentityDbContext --output-dir Migrations/IdentityDb
dotnet ef database update --context AppIdentityDbContext
```

---

## 3. Запуск

### MVC (http://localhost:5100)
```bash
cd BookingMvc
dotnet run
```

### API + Swagger (http://localhost:5200)
```bash
cd BookingApi
dotnet run
```

---

## 4. Ролі та тестові акаунти

При першому запуску автоматично створюються:
- Ролі: **Admin**, **User**
- Адмін за замовчуванням:
  - Email: `admin@booking.com`
  - Password: `Admin123!`

---

## 5. Тестування API через Swagger / Postman

### Крок 1 — Реєстрація
```
POST /api/auth/register
{
  "email": "user@test.com",
  "password": "Test123!",
  "role": "User"
}
```

### Крок 2 — Логін (отримання JWT)
```
POST /api/auth/login
{
  "email": "user@test.com",
  "password": "Test123!"
}
```
→ Повертає `token`. Скопіюйте його.

### Крок 3 — Авторизовані запити
У Swagger: натисніть **Authorize** → введіть `Bearer <ваш_токен>`

У Postman: Headers → `Authorization: Bearer <ваш_токен>`

---

## 6. API Endpoints

### Auth
| Метод | URL | Доступ | Опис |
|-------|-----|--------|------|
| POST | /api/auth/register | Публічний | Реєстрація |
| POST | /api/auth/login | Публічний | Логін → JWT |

### Events
| Метод | URL | Доступ | Опис |
|-------|-----|--------|------|
| GET | /api/events | Публічний | Всі події |
| GET | /api/events/{id} | Публічний | Одна подія |
| POST | /api/events | Admin | Створити |
| PUT | /api/events/{id} | Admin | Оновити |
| DELETE | /api/events/{id} | Admin | Видалити |

### Tickets
| Метод | URL | Доступ | Опис |
|-------|-----|--------|------|
| GET | /api/tickets | Admin | Всі квитки |
| GET | /api/tickets/{id} | Admin | Один квиток |
| POST | /api/tickets | User/Admin | Купити квиток |
| PUT | /api/tickets/{id} | Admin | Оновити |
| DELETE | /api/tickets/{id} | Admin | Видалити |

---

## 7. MVC — Доступ до сторінок

| Сторінка | Роль |
|----------|------|
| /Account/Login | Публічна |
| /Account/Register | Публічна |
| /Account/Profile | Будь-який залогінений |
| /Home/Index | Будь-який залогінений |
| /Cart/* | Будь-який залогінений |
| /Admin/* | Тільки Admin |
| /AdminTicket/* | Тільки Admin |

---

## 8. Bootstrap та статичні ресурси

Для MVC-проекту потрібно встановити Bootstrap через libman:
```bash
cd BookingMvc
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman restore
```

Або додати вручну `wwwroot/lib/bootstrap` з офіційного сайту.
