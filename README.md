# FastFood

FastFood is a cross-platform restaurant point-of-sale and management application built with .NET MAUI. It helps restaurant teams manage a menu, create customer orders, track supplier purchases, and generate receipts from one app.

## Features

- Role-based access for administrators and cashiers
- Menu browsing with categories, item images, prices, and cart management
- Order checkout with cash or online payment modes
- Tax and total calculation
- Order history with item-level details
- Menu item and category management
- Supplier and supplier-order management
- PDF receipt generation
- Local SQLite data storage with seed data for development

## Technology

- .NET 9
- .NET MAUI
- C# and XAML
- CommunityToolkit.Maui and CommunityToolkit.Mvvm
- SQLite via `sqlite-net-pcl`
- QuestPDF for receipts

## Requirements

- .NET 9 SDK
- .NET MAUI workload installed for your target platform
- Visual Studio 2022 with the .NET MAUI workload, or the .NET CLI

Install the MAUI workload if needed:

```bash
dotnet workload install maui
```

## Getting started

Clone the repository and restore its dependencies:

```bash
git clone https://github.com/Aboubakar2/fastfood.git
cd fastfood
dotnet restore
```

Open `fastfood.sln` in Visual Studio and select an available MAUI target, or run the project from the command line with a target framework supported by your environment. For example:

```bash
dotnet build
```

The application initializes a local SQLite database and loads development seed data on first use. Seeded users and sample data are defined in `Data/SeedData.cs`.

## Project structure

| Directory | Purpose |
| --- | --- |
| `Pages/` | Application screens and navigation pages |
| `ViewModels/` | MVVM presentation and command logic |
| `Data/` | SQLite service, entities, and seed data |
| `Controls/` | Reusable XAML controls |
| `Utils/` | Receipt and document services |
| `Resources/` | Images, fonts, icons, and styles |
| `Platforms/` | Platform-specific application configuration |

## Supported platforms

The project is configured for Android, iOS, Mac Catalyst, and Windows. Tizen support is present in the project configuration as an optional target.

## License

No license has been specified yet.