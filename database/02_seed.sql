USE rental_ps;

INSERT INTO roles (name, description) VALUES
('admin', 'Akses penuh aplikasi'),
('kasir', 'Transaksi harian dan data pelanggan'),
('owner', 'Laporan dan monitoring');

INSERT INTO users (username, password_hash, full_name) VALUES
('admin', '$2y$10$replace_with_real_hash_before_production', 'Administrator');

INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id
FROM users u
JOIN roles r ON r.name = 'admin'
WHERE u.username = 'admin';

INSERT INTO payment_methods (code, name) VALUES
('CASH', 'Cash'),
('TRANSFER', 'Transfer Bank'),
('QRIS', 'QRIS'),
('DEBIT', 'Kartu Debit');

INSERT INTO room_types (code, name, hourly_rate, overtime_rate, description) VALUES
('REG', 'Regular', 10000, 12000, 'Ruang standar'),
('VIP', 'VIP', 20000, 25000, 'Ruang premium');

INSERT INTO rooms (room_type_id, code, name, capacity) VALUES
((SELECT id FROM room_types WHERE code = 'REG'), 'R001', 'Ruang 1', 2),
((SELECT id FROM room_types WHERE code = 'REG'), 'R002', 'Ruang 2', 2),
((SELECT id FROM room_types WHERE code = 'VIP'), 'VIP01', 'VIP 1', 4);

INSERT INTO consoles (code, name, console_type, serial_number) VALUES
('PS001', 'PlayStation 4 - 1', 'PS4', 'PS4-001'),
('PS002', 'PlayStation 4 - 2', 'PS4', 'PS4-002'),
('PS501', 'PlayStation 5 - 1', 'PS5', 'PS5-001');

INSERT INTO fnb_items (code, name, category, purchase_price, selling_price, stock_qty, minimum_stock, unit) VALUES
('FNB001', 'Air Mineral', 'Minuman', 2500, 5000, 24, 6, 'botol'),
('FNB002', 'Mie Instan Cup', 'Makanan', 4500, 8000, 12, 4, 'cup');

INSERT INTO games (code, title, platform, genre, size_gb, install_price) VALUES
('G001', 'EA Sports FC', 'PS4/PS5', 'Sports', 50.00, 5000),
('G002', 'Tekken', 'PS4/PS5', 'Fighting', 80.00, 5000);

INSERT INTO services (code, name, default_price, description) VALUES
('JS001', 'Bersih PS', 50000, 'Pembersihan console dan controller'),
('JS002', 'Install Ulang Sistem', 75000, 'Install ulang software/sistem');

INSERT INTO spareparts (code, name, category, purchase_price, selling_price, stock_qty, minimum_stock, unit) VALUES
('SP001', 'Analog Stick', 'Controller', 15000, 35000, 10, 3, 'pcs'),
('SP002', 'Kabel HDMI', 'Kabel', 20000, 45000, 5, 2, 'pcs');

INSERT INTO suppliers (code, name, phone) VALUES
('SUP001', 'Supplier Umum', '081234567890');

INSERT INTO settings (setting_key, setting_value, description) VALUES
('shop_name', 'Rental PS', 'Nama toko yang tampil di struk'),
('currency', 'IDR', 'Mata uang aplikasi'),
('late_tolerance_minutes', '10', 'Toleransi keterlambatan sebelum denda');
