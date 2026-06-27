# Rancangan Database MySQL

Database utama: `rental_ps`

File SQL utama:

- `database/00_rental_ps_full.sql`

File ini sudah berisi:

- `CREATE DATABASE`
- `DROP TABLE`
- `CREATE TABLE`
- index
- seed data untuk semua tabel

Jika memakai `docker-compose.yml` di repo ini, file SQL otomatis dijalankan saat container MySQL pertama kali dibuat.

## Cara Menjalankan Dengan Docker

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
Root password: root
```

Jika container sudah pernah dibuat dan ingin menjalankan ulang init SQL dari awal, hapus volume MySQL dulu:

```bash
docker compose down -v
docker compose up -d
```

## Cara Import Dengan Laragon

Lewat HeidiSQL/phpMyAdmin/MySQL client, import file:

```text
database/00_rental_ps_full.sql
```

Atau lewat terminal MySQL:

```bash
mysql -u root < database/00_rental_ps_full.sql
```

## Kelompok Tabel

Master data:

- `customers`
- `room_types`
- `rooms`
- `consoles`
- `fnb_items`
- `games`
- `services`
- `spareparts`
- `suppliers`
- `payment_methods`

User dan akses:

- `users`
- `roles`
- `user_roles`

Transaksi:

- `bookings`
- `rentals`
- `rental_fnb_items`
- `fines`
- `game_installs`
- `game_install_items`
- `service_jobs`
- `service_spareparts`
- `fnb_sales`
- `fnb_sale_items`
- `sparepart_purchases`
- `sparepart_purchase_items`
- `sparepart_sales`
- `sparepart_sale_items`
- `payments`

Pendukung:

- `stock_movements`
- `settings`
- `audit_logs`

## Catatan Penting

- Kolom uang memakai `DECIMAL(14,2)`.
- Nomor transaksi disimpan sebagai string agar format seperti `SW-20260627-001` mudah dipakai.
- `payments` memakai pola `reference_type` dan `reference_id` agar satu tabel pembayaran bisa dipakai untuk sewa, denda, service, isi game, dan transaksi lain.
- `stock_movements` mencatat riwayat stok FNB dan sparepart.
- Password seed admin masih placeholder. Saat aplikasi dibuat, ganti dengan hash asli.
