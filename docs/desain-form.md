# Desain Form Aplikasi Rental PS

Dokumen ini menjadi panduan UI untuk project VB.NET Windows Forms. Aplikasi dibuat model operasional: cepat dipakai kasir, mudah dibaca admin, dan tetap lengkap untuk owner.

## Navigasi Utama

Gunakan layout 2 kolom:

- Sidebar kiri: menu utama.
- Area kanan: konten aktif.

Menu:

- Dashboard
- Transaksi
  - Sewa
  - Booking
  - Denda
  - Isi Game
  - Service
  - Beli/Jual Sparepart
  - Penjualan FNB
- Data Master
  - Pelanggan
  - Stok PS
  - Jenis Ruang
  - Ruang
  - Stok FNB
  - Game
  - Jasa
  - Sparepart
  - Supplier
- Laporan
  - Pendapatan
  - Sewa
  - Booking
  - Stok
  - Service
  - Sparepart
- Pengaturan
  - User
  - Role
  - Metode Pembayaran
  - Profil Toko

## Dashboard

Komponen:

- Kartu ringkas: sewa berjalan, booking hari ini, pendapatan hari ini, stok menipis.
- Tabel sewa berjalan.
- Tabel booking terdekat.
- Shortcut tombol: Sewa Baru, Booking Baru, Service Baru.

## Form Data Master

Semua form master memakai pola yang sama:

- Bagian atas: search, filter status, tombol Tambah.
- Bagian tengah: DataGridView.
- Bagian kanan atau dialog: form input.
- Tombol aksi: Simpan, Batal, Edit, Nonaktifkan.

### Pelanggan

Field:

- Kode pelanggan
- Nama
- No HP
- Alamat
- No identitas
- Catatan
- Status aktif

Validasi:

- Nama wajib.
- No HP sebaiknya unik jika diisi.

### Stok PS

Field:

- Kode PS
- Nama
- Tipe console
- Serial number
- Tanggal beli
- Kondisi
- Status ketersediaan
- Catatan

Status:

- Available
- Rented
- Maintenance
- Inactive

### Jenis Ruang

Field:

- Kode jenis
- Nama jenis
- Tarif per jam
- Tarif overtime
- Deskripsi
- Status aktif

### Ruang

Field:

- Kode ruang
- Nama ruang
- Jenis ruang
- Kapasitas
- Status
- Catatan

### Stok FNB

Field:

- Kode item
- Nama item
- Kategori
- Harga beli
- Harga jual
- Stok
- Minimum stok
- Satuan
- Status aktif

Fitur tambahan:

- Tombol stok masuk.
- Tombol koreksi stok.
- Riwayat pergerakan stok.

### Game

Field:

- Kode game
- Judul
- Platform
- Genre
- Ukuran GB
- Harga install
- Status aktif

### Jasa

Field:

- Kode jasa
- Nama jasa
- Harga default
- Deskripsi
- Status aktif

### Sparepart

Field:

- Kode sparepart
- Nama
- Kategori
- Harga beli
- Harga jual
- Stok
- Minimum stok
- Satuan
- Status aktif

## Form Transaksi

### Sewa

Layout:

- Panel kiri: data pelanggan, ruang, PS, jam mulai, rencana selesai.
- Panel kanan: ringkasan biaya.
- Tab bawah: tambahan FNB, pembayaran, catatan.

Field:

- Nomor transaksi
- Pelanggan
- Ruang
- PS
- Jam mulai
- Rencana selesai
- Jam selesai aktual
- Tarif per jam
- Durasi
- Biaya sewa
- Diskon
- Tambahan FNB
- Total
- Dibayar
- Status

Tombol:

- Mulai Sewa
- Tambah FNB
- Selesaikan
- Bayar
- Cetak Struk
- Batal

Validasi:

- Ruang dan PS tidak boleh sedang dipakai.
- Jika dari booking, status booking berubah menjadi checked_in.
- Saat selesai, hitung durasi dan denda jika lewat toleransi.

### Booking

Field:

- Nomor booking
- Pelanggan
- Tanggal dan jam mulai
- Tanggal dan jam selesai
- Ruang
- PS
- DP
- Status
- Catatan

Tombol:

- Simpan Booking
- Check-in
- Batalkan
- Cetak Bukti

Validasi:

- Jam selesai harus lebih besar dari jam mulai.
- Tidak boleh bentrok dengan booking aktif lain pada ruang/PS yang sama.

### Denda

Field:

- Nomor denda
- Pelanggan
- Transaksi sewa
- Jenis denda
- Deskripsi
- Nominal
- Status bayar

Jenis:

- Terlambat
- Kerusakan
- Barang hilang
- Manual

### Isi Game

Field:

- Nomor transaksi
- Pelanggan
- Nama device
- Kapasitas device
- List game
- Biaya jasa
- Total
- Status
- Catatan

Status:

- Received
- Processing
- Completed
- Picked Up
- Cancelled

### Service

Field:

- Nomor service
- Pelanggan
- Barang
- Keluhan
- Jenis jasa
- Catatan teknisi
- Sparepart dipakai
- Biaya jasa
- Biaya sparepart
- Total
- Status

Status:

- Received
- Diagnosis
- Processing
- Completed
- Picked Up
- Cancelled

### Beli Sparepart

Form ini dipakai saat toko membeli stok dari supplier.

Field:

- Nomor pembelian
- Supplier
- Tanggal
- Item sparepart
- Qty
- Harga beli
- Subtotal
- Total
- Catatan

Efek:

- Stok sparepart bertambah.
- Masuk ke riwayat stock_movements.
- Detail item pembelian disimpan di `sparepart_purchase_items`.

### Jual Sparepart

Form ini dipakai saat pelanggan membeli sparepart.

Field:

- Nomor penjualan
- Pelanggan
- Tanggal
- Item sparepart
- Qty
- Harga jual
- Subtotal
- Total
- Dibayar
- Catatan

Efek:

- Stok sparepart berkurang.
- Masuk ke riwayat stock_movements.
- Detail item penjualan disimpan di `sparepart_sale_items`.

### Penjualan FNB

Form ini dipakai kalau pelanggan membeli FNB tanpa masuk ke transaksi sewa.

Field:

- Nomor penjualan
- Pelanggan
- Tanggal
- Item FNB
- Qty
- Harga jual
- Subtotal
- Total
- Dibayar
- Catatan

Efek:

- Stok FNB berkurang.
- Masuk ke riwayat stock_movements.
- Detail item penjualan disimpan di `fnb_sale_items`.

### Koreksi Stok

Form ini dipakai untuk stok awal, stok opname, barang rusak, selisih hitung, atau koreksi manual.

Field:

- Jenis item: FNB atau sparepart
- Barang
- Jenis mutasi: in, out, adjustment
- Qty
- Catatan

Efek:

- Jika `in` atau `adjustment`, stok bertambah.
- Jika `out`, stok berkurang dan tidak boleh melebihi stok tersedia.
- Semua perubahan dicatat ke `stock_movements`.

## Laporan

Filter standar:

- Tanggal mulai
- Tanggal selesai
- Status
- Metode pembayaran
- Tombol tampilkan
- Tombol cetak/export

Laporan utama:

- Pendapatan harian/bulanan
- Detail transaksi sewa
- Booking
- Denda
- Isi game
- Service
- Pembelian sparepart
- Stok menipis
- Riwayat stok

## Aturan Desain Visual

- Gunakan warna dasar terang: putih, abu muda, biru tua untuk aksen.
- Tombol utama: biru.
- Tombol bahaya: merah.
- Tombol selesai/sukses: hijau.
- DataGridView harus punya zebra row dan header tebal.
- Form transaksi harus menampilkan total besar dan jelas.
- Hindari terlalu banyak popup kecil; gunakan dialog hanya untuk tambah/edit.

## Struktur Form WinForms yang Disarankan

- FrmLogin
- FrmMain
- FrmDashboard
- FrmCustomers
- FrmConsoles
- FrmRoomTypes
- FrmRooms
- FrmFnbItems
- FrmGames
- FrmServices
- FrmSpareparts
- FrmSuppliers
- FrmRental
- FrmBooking
- FrmFine
- FrmGameInstall
- FrmServiceJob
- FrmSparepartPurchase
- FrmSparepartSale
- FrmFnbSale
- FrmReports
- FrmSettings
