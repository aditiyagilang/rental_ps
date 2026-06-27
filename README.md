# Rental PS

Aplikasi desktop VB.NET Windows Forms untuk operasional rental PlayStation, booking ruang, FNB, service, isi game, dan sparepart.

## Struktur

- `RentalPS.sln`: solution untuk dibuka di Visual Studio.
- `src/RentalPS.WinForms`: project VB.NET Windows Forms.
- `database`: file SQL MySQL.
- `docs`: rancangan database dan desain form.
- `docker-compose.yml`: MySQL lokal.

## Jalankan Database

```bash
docker compose up -d
```

Koneksi default:

```text
Host: localhost
Port: 3306
Database: rental_ps
User: rental_user
Password: rental_pass
```

## Jalankan Aplikasi

Di Windows dengan .NET SDK:

```bash
dotnet restore RentalPS.sln
dotnet run --project src/RentalPS.WinForms/RentalPS.WinForms.vbproj
```

Atau buka `RentalPS.sln` langsung di Visual Studio.

Login awal:

```text
Username: admin
Password: admin
```

Password ini hanya untuk tahap awal development. Nanti perlu diganti ke sistem hash password yang benar.
