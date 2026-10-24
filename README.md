# Mnemo

Ứng dụng ghi nhớ thông minh dạng thẻ học (Flashcard) dựa trên thuật toán Spaced Repetition, xây dựng trên nền tảng **WPF (.NET 10)** và cơ sở dữ liệu **SQLite**.

---

## 🏛️ Cấu trúc dự án

Dự án được tổ chức theo kiến trúc phân tầng (Clean / Layered Architecture):

```
Mnemo/
├── Mnemo.slnx                   # Solution file
├── src/
│   ├── Mnemo.Core/              # Domain Models, Enums & Service Interfaces
│   ├── Mnemo.Data/              # EF Core, SQLite DbContext & Data Access
│   └── Mnemo.Wpf/               # WPF UI, ViewModels (MVVM) & Dependency Injection
```

---

## 🚀 Công nghệ sử dụng

- **.NET 10 (WPF)**
- **SQLite** (`Microsoft.EntityFrameworkCore.Sqlite`)
- **CommunityToolkit.Mvvm**
- **Microsoft.Extensions.DependencyInjection**

---

## 💻 Hướng dẫn chạy

```powershell
# Chạy ứng dụng trực tiếp từ terminal
dotnet run --project src/Mnemo.Wpf
```
hoặc mở file `Mnemo.slnx` bằng Visual Studio / JetBrains Rider / VS Code và nhấn **F5**.
