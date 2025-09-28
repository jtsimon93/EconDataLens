# Econ Data Lens

**Econ Data Lens** is an open-source project with the goal of making public economic data more accessible, understandable, and useful.

The platform will eventually provide a **web interface** where users can browse and visualize datasets such as:
- Consumer Price Index (CPI)
- Average Price Index
- Employment and labor market data
- Additional Bureau of Labor Statistics (BLS) and other public datasets

The data will be transformed into a clean, digestible format to help researchers, educators, journalists, and the public gain insight into the economy.

---

## Project Status

✅ The **ETL pipeline for CPI data** is complete.  
🚧 Additional ETL pipelines (e.g., Average Price Index, employment datasets) are planned.  
🌐 Web front-end for browsing and visualization is in the planning stage.

---

## For Developers

### Requirements
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [PostgreSQL](https://www.postgresql.org/) (local or containerized)
- (Optional) [Docker](https://www.docker.com/) for containerized workflows

### Setup

Clone the repo and restore dependencies:

```bash
git clone https://github.com/jtsimon93/econ-data-lens.git
cd econ-data-lens
dotnet restore
```

Apply EF Core migrations and update the database:

```bash
dotnet ef database update
```

Run the ETL (CPI ingestion):

```bash
dotnet run --project src/EconDataLens.Etl
```

### Configuration

Settings (connection strings, download paths, etc.) are managed via `appsettings.json`:

See `src/EconDataLens.Etl/appsettings.example.json` for a starter file.

---

## Contributing

Contributions are welcome! Please see the [CONTRIBUTING.md](CONTRIBUTING.md) file for guidelines.

General process:
1. Open an [issue](../../issues) to propose significant changes or new features.
2. Fork the repository and create a feature branch.
3. Submit a pull request with a clear description of your changes.


---

## License

Copyright (c) 2025 Justin Simon

This project is licensed under the terms of the **GNU Affero General Public License v3.0 (AGPL-3.0)**.  
See the [LICENSE](LICENSE) file for details.
