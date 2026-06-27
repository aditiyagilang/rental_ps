# Rancangan Project VB.NET Windows Forms

Project dibuat agar bisa dipakai di VS Code dan tetap bisa dibuka di Visual Studio.

## File Utama

- `RentalPS.sln`
- `src/RentalPS.WinForms/RentalPS.WinForms.vbproj`
- `src/RentalPS.WinForms/Program.vb`

## Layer Aplikasi

- `Infrastructure`
  - `DbConnectionFactory`: membuat koneksi MySQL.
  - `DatabaseHealth`: tes koneksi.
- `Repositories`
  - akses database per modul.
- `UI`
  - form Windows Forms.

## Form Yang Sudah Dibuat

- `FrmLogin`
  - login awal.
  - tombol tes koneksi MySQL.
- `FrmMain`
  - layout utama dengan sidebar.
- `FrmDashboard`
  - membaca ringkasan dari MySQL.
  - grafik pendapatan 7 hari.
  - grafik komposisi transaksi hari ini.
  - grafik risiko operasional.
- `FrmCustomers`
  - contoh master data yang sudah CRUD dasar.
- `FrmMasterData`
  - form generik untuk master dan transaksi sederhana.
- `FrmPlaceholder`
  - placeholder untuk modul lain.

## Form Master Aktif

- Pelanggan
- Stok PS
- Jenis Ruang
- Ruang
- Stok FNB
- Game
- Jasa
- Sparepart
- Supplier

## Form Transaksi Aktif

- Sewa
- Booking
- Denda
- Isi Game
- Service
- Beli Sparepart
- Jual Sparepart
- Penjualan FNB

Catatan: form transaksi detail item seperti list game, item FNB, dan item sparepart masih tahap berikutnya. Tahap ini sudah bisa menyimpan data transaksi utama/header ke database.

## Koneksi MySQL

Default connection string saat ini cocok untuk Laragon:

```text
Server=localhost;Port=3306;Database=rental_ps;User ID=root;Password=;
```

Bisa dioverride dengan environment variable:

- `RENTAL_PS_DB_HOST`
- `RENTAL_PS_DB_PORT`
- `RENTAL_PS_DB_NAME`
- `RENTAL_PS_DB_USER`
- `RENTAL_PS_DB_PASSWORD`

## Modul Berikutnya

Urutan implementasi yang disarankan:

1. Detail item transaksi FNB/sparepart/game.
2. Auto generate nomor transaksi.
3. Perhitungan otomatis total sewa/durasi.
4. Pembayaran dan cetak struk.
5. Laporan.

## Catatan Visual Studio Designer

Form di project ini dibuat lewat kode VB.NET, bukan drag-and-drop Designer. Jadi tampilan utama yang benar dilihat dengan menjalankan aplikasi, bukan hanya membuka file `.vb` di Designer.

Jika Designer terlihat kosong, jalankan aplikasi dengan:

```bash
dotnet run --project src/RentalPS.WinForms/RentalPS.WinForms.vbproj
```

Atau dari Visual Studio klik Start.
