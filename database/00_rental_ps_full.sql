CREATE DATABASE IF NOT EXISTS rental_ps
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE rental_ps;

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS audit_logs;
DROP TABLE IF EXISTS payments;
DROP TABLE IF EXISTS fines;
DROP TABLE IF EXISTS sparepart_sale_items;
DROP TABLE IF EXISTS sparepart_sales;
DROP TABLE IF EXISTS sparepart_purchase_items;
DROP TABLE IF EXISTS sparepart_purchases;
DROP TABLE IF EXISTS fnb_sale_items;
DROP TABLE IF EXISTS fnb_sales;
DROP TABLE IF EXISTS service_spareparts;
DROP TABLE IF EXISTS service_jobs;
DROP TABLE IF EXISTS game_install_items;
DROP TABLE IF EXISTS game_installs;
DROP TABLE IF EXISTS rental_fnb_items;
DROP TABLE IF EXISTS rentals;
DROP TABLE IF EXISTS bookings;
DROP TABLE IF EXISTS stock_movements;
DROP TABLE IF EXISTS user_roles;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS spareparts;
DROP TABLE IF EXISTS services;
DROP TABLE IF EXISTS games;
DROP TABLE IF EXISTS fnb_items;
DROP TABLE IF EXISTS consoles;
DROP TABLE IF EXISTS rooms;
DROP TABLE IF EXISTS room_types;
DROP TABLE IF EXISTS customers;
DROP TABLE IF EXISTS payment_methods;
DROP TABLE IF EXISTS suppliers;
DROP TABLE IF EXISTS settings;

SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE customers (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(120) NOT NULL,
  phone VARCHAR(30) NULL,
  address TEXT NULL,
  identity_number VARCHAR(60) NULL,
  notes TEXT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE room_types (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(80) NOT NULL,
  hourly_rate DECIMAL(14,2) NOT NULL DEFAULT 0,
  overtime_rate DECIMAL(14,2) NOT NULL DEFAULT 0,
  description TEXT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE rooms (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  room_type_id BIGINT UNSIGNED NOT NULL,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(80) NOT NULL,
  capacity INT NOT NULL DEFAULT 1,
  status ENUM('available','occupied','maintenance','inactive') NOT NULL DEFAULT 'available',
  notes TEXT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_rooms_room_type FOREIGN KEY (room_type_id) REFERENCES room_types(id)
) ENGINE=InnoDB;

CREATE TABLE consoles (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(100) NOT NULL,
  console_type VARCHAR(50) NOT NULL,
  serial_number VARCHAR(100) NULL,
  purchase_date DATE NULL,
  condition_status ENUM('good','minor_issue','broken','service') NOT NULL DEFAULT 'good',
  availability_status ENUM('available','rented','maintenance','inactive') NOT NULL DEFAULT 'available',
  notes TEXT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE fnb_items (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(120) NOT NULL,
  category VARCHAR(80) NULL,
  purchase_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  selling_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  stock_qty INT NOT NULL DEFAULT 0,
  minimum_stock INT NOT NULL DEFAULT 0,
  unit VARCHAR(30) NOT NULL DEFAULT 'pcs',
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE games (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  title VARCHAR(150) NOT NULL,
  platform VARCHAR(50) NOT NULL,
  genre VARCHAR(80) NULL,
  size_gb DECIMAL(8,2) NULL,
  install_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE services (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(120) NOT NULL,
  default_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  description TEXT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE spareparts (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(120) NOT NULL,
  category VARCHAR(80) NULL,
  purchase_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  selling_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  stock_qty INT NOT NULL DEFAULT 0,
  minimum_stock INT NOT NULL DEFAULT 0,
  unit VARCHAR(30) NOT NULL DEFAULT 'pcs',
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE suppliers (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(120) NOT NULL,
  phone VARCHAR(30) NULL,
  address TEXT NULL,
  notes TEXT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE payment_methods (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(30) NOT NULL UNIQUE,
  name VARCHAR(80) NOT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE roles (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(60) NOT NULL UNIQUE,
  description TEXT NULL
) ENGINE=InnoDB;

CREATE TABLE users (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(60) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  full_name VARCHAR(120) NOT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE user_roles (
  user_id BIGINT UNSIGNED NOT NULL,
  role_id BIGINT UNSIGNED NOT NULL,
  PRIMARY KEY (user_id, role_id),
  CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users(id),
  CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES roles(id)
) ENGINE=InnoDB;

CREATE TABLE bookings (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  booking_no VARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT UNSIGNED NOT NULL,
  room_id BIGINT UNSIGNED NULL,
  console_id BIGINT UNSIGNED NULL,
  start_time DATETIME NOT NULL,
  end_time DATETIME NOT NULL,
  deposit_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status ENUM('booked','checked_in','completed','cancelled') NOT NULL DEFAULT 'booked',
  notes TEXT NULL,
  created_by BIGINT UNSIGNED NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_bookings_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_bookings_room FOREIGN KEY (room_id) REFERENCES rooms(id),
  CONSTRAINT fk_bookings_console FOREIGN KEY (console_id) REFERENCES consoles(id),
  CONSTRAINT fk_bookings_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_bookings_time CHECK (end_time > start_time)
) ENGINE=InnoDB;

CREATE TABLE rentals (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  rental_no VARCHAR(40) NOT NULL UNIQUE,
  booking_id BIGINT UNSIGNED NULL,
  customer_id BIGINT UNSIGNED NOT NULL,
  room_id BIGINT UNSIGNED NOT NULL,
  console_id BIGINT UNSIGNED NOT NULL,
  start_time DATETIME NOT NULL,
  planned_end_time DATETIME NULL,
  actual_end_time DATETIME NULL,
  hourly_rate DECIMAL(14,2) NOT NULL DEFAULT 0,
  duration_minutes INT NOT NULL DEFAULT 0,
  rental_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  discount_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  paid_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status ENUM('running','completed','cancelled') NOT NULL DEFAULT 'running',
  notes TEXT NULL,
  created_by BIGINT UNSIGNED NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_rentals_booking FOREIGN KEY (booking_id) REFERENCES bookings(id),
  CONSTRAINT fk_rentals_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_rentals_room FOREIGN KEY (room_id) REFERENCES rooms(id),
  CONSTRAINT fk_rentals_console FOREIGN KEY (console_id) REFERENCES consoles(id),
  CONSTRAINT fk_rentals_created_by FOREIGN KEY (created_by) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE TABLE rental_fnb_items (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  rental_id BIGINT UNSIGNED NOT NULL,
  fnb_item_id BIGINT UNSIGNED NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_rental_fnb_rental FOREIGN KEY (rental_id) REFERENCES rentals(id),
  CONSTRAINT fk_rental_fnb_item FOREIGN KEY (fnb_item_id) REFERENCES fnb_items(id),
  CONSTRAINT chk_rental_fnb_qty CHECK (qty > 0)
) ENGINE=InnoDB;

CREATE TABLE game_installs (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  install_no VARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT UNSIGNED NOT NULL,
  device_name VARCHAR(120) NOT NULL,
  device_capacity_gb DECIMAL(10,2) NULL,
  service_fee DECIMAL(14,2) NOT NULL DEFAULT 0,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status ENUM('received','processing','completed','picked_up','cancelled') NOT NULL DEFAULT 'received',
  notes TEXT NULL,
  received_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  completed_at DATETIME NULL,
  created_by BIGINT UNSIGNED NULL,
  CONSTRAINT fk_game_installs_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_game_installs_created_by FOREIGN KEY (created_by) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE TABLE game_install_items (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  game_install_id BIGINT UNSIGNED NOT NULL,
  game_id BIGINT UNSIGNED NOT NULL,
  price DECIMAL(14,2) NOT NULL DEFAULT 0,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_game_install_items_install FOREIGN KEY (game_install_id) REFERENCES game_installs(id),
  CONSTRAINT fk_game_install_items_game FOREIGN KEY (game_id) REFERENCES games(id)
) ENGINE=InnoDB;

CREATE TABLE service_jobs (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  service_no VARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT UNSIGNED NOT NULL,
  service_id BIGINT UNSIGNED NULL,
  item_name VARCHAR(120) NOT NULL,
  problem_description TEXT NOT NULL,
  technician_notes TEXT NULL,
  labor_cost DECIMAL(14,2) NOT NULL DEFAULT 0,
  sparepart_cost DECIMAL(14,2) NOT NULL DEFAULT 0,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status ENUM('received','diagnosis','processing','completed','picked_up','cancelled') NOT NULL DEFAULT 'received',
  received_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  completed_at DATETIME NULL,
  picked_up_at DATETIME NULL,
  created_by BIGINT UNSIGNED NULL,
  CONSTRAINT fk_service_jobs_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_service_jobs_service FOREIGN KEY (service_id) REFERENCES services(id),
  CONSTRAINT fk_service_jobs_created_by FOREIGN KEY (created_by) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE TABLE service_spareparts (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  service_job_id BIGINT UNSIGNED NOT NULL,
  sparepart_id BIGINT UNSIGNED NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_service_spareparts_job FOREIGN KEY (service_job_id) REFERENCES service_jobs(id),
  CONSTRAINT fk_service_spareparts_part FOREIGN KEY (sparepart_id) REFERENCES spareparts(id),
  CONSTRAINT chk_service_spareparts_qty CHECK (qty > 0)
) ENGINE=InnoDB;

CREATE TABLE fnb_sales (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  sale_no VARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT UNSIGNED NULL,
  sale_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  paid_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status ENUM('draft','paid','void') NOT NULL DEFAULT 'draft',
  notes TEXT NULL,
  created_by BIGINT UNSIGNED NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_fnb_sales_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_fnb_sales_created_by FOREIGN KEY (created_by) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE TABLE fnb_sale_items (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  fnb_sale_id BIGINT UNSIGNED NOT NULL,
  fnb_item_id BIGINT UNSIGNED NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  CONSTRAINT fk_fnb_sale_items_sale FOREIGN KEY (fnb_sale_id) REFERENCES fnb_sales(id),
  CONSTRAINT fk_fnb_sale_items_item FOREIGN KEY (fnb_item_id) REFERENCES fnb_items(id),
  CONSTRAINT chk_fnb_sale_items_qty CHECK (qty > 0)
) ENGINE=InnoDB;

CREATE TABLE sparepart_purchases (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  purchase_no VARCHAR(40) NOT NULL UNIQUE,
  supplier_id BIGINT UNSIGNED NULL,
  purchase_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  notes TEXT NULL,
  created_by BIGINT UNSIGNED NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_sparepart_purchases_supplier FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
  CONSTRAINT fk_sparepart_purchases_created_by FOREIGN KEY (created_by) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE TABLE sparepart_purchase_items (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  sparepart_purchase_id BIGINT UNSIGNED NOT NULL,
  sparepart_id BIGINT UNSIGNED NOT NULL,
  qty INT NOT NULL,
  purchase_price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  CONSTRAINT fk_sparepart_purchase_items_purchase FOREIGN KEY (sparepart_purchase_id) REFERENCES sparepart_purchases(id),
  CONSTRAINT fk_sparepart_purchase_items_part FOREIGN KEY (sparepart_id) REFERENCES spareparts(id),
  CONSTRAINT chk_sparepart_purchase_items_qty CHECK (qty > 0)
) ENGINE=InnoDB;

CREATE TABLE sparepart_sales (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  sale_no VARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT UNSIGNED NULL,
  sale_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  paid_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status ENUM('draft','paid','void') NOT NULL DEFAULT 'draft',
  notes TEXT NULL,
  created_by BIGINT UNSIGNED NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_sparepart_sales_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_sparepart_sales_created_by FOREIGN KEY (created_by) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE TABLE sparepart_sale_items (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  sparepart_sale_id BIGINT UNSIGNED NOT NULL,
  sparepart_id BIGINT UNSIGNED NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  CONSTRAINT fk_sparepart_sale_items_sale FOREIGN KEY (sparepart_sale_id) REFERENCES sparepart_sales(id),
  CONSTRAINT fk_sparepart_sale_items_part FOREIGN KEY (sparepart_id) REFERENCES spareparts(id),
  CONSTRAINT chk_sparepart_sale_items_qty CHECK (qty > 0)
) ENGINE=InnoDB;

CREATE TABLE fines (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  fine_no VARCHAR(40) NOT NULL UNIQUE,
  rental_id BIGINT UNSIGNED NULL,
  customer_id BIGINT UNSIGNED NOT NULL,
  fine_type ENUM('late','damage','lost_item','manual') NOT NULL,
  description TEXT NOT NULL,
  amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status ENUM('unpaid','paid','void') NOT NULL DEFAULT 'unpaid',
  created_by BIGINT UNSIGNED NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_fines_rental FOREIGN KEY (rental_id) REFERENCES rentals(id),
  CONSTRAINT fk_fines_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_fines_created_by FOREIGN KEY (created_by) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE TABLE payments (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  payment_no VARCHAR(40) NOT NULL UNIQUE,
  payment_method_id BIGINT UNSIGNED NOT NULL,
  reference_type ENUM('rental','booking','fine','game_install','service_job','fnb_sale','sparepart_purchase','sparepart_sale') NOT NULL,
  reference_id BIGINT UNSIGNED NOT NULL,
  amount DECIMAL(14,2) NOT NULL,
  paid_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  notes TEXT NULL,
  created_by BIGINT UNSIGNED NULL,
  CONSTRAINT fk_payments_method FOREIGN KEY (payment_method_id) REFERENCES payment_methods(id),
  CONSTRAINT fk_payments_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  INDEX idx_payments_reference (reference_type, reference_id)
) ENGINE=InnoDB;

CREATE TABLE stock_movements (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  item_type ENUM('fnb','sparepart') NOT NULL,
  item_id BIGINT UNSIGNED NOT NULL,
  movement_type ENUM('in','out','adjustment') NOT NULL,
  qty INT NOT NULL,
  reference_type VARCHAR(60) NULL,
  reference_id BIGINT UNSIGNED NULL,
  notes TEXT NULL,
  created_by BIGINT UNSIGNED NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_stock_movements_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  INDEX idx_stock_movements_item (item_type, item_id),
  INDEX idx_stock_movements_reference (reference_type, reference_id)
) ENGINE=InnoDB;

CREATE TABLE settings (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  setting_key VARCHAR(80) NOT NULL UNIQUE,
  setting_value TEXT NULL,
  description TEXT NULL,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE audit_logs (
  id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  user_id BIGINT UNSIGNED NULL,
  action VARCHAR(80) NOT NULL,
  table_name VARCHAR(80) NOT NULL,
  record_id BIGINT UNSIGNED NULL,
  old_data JSON NULL,
  new_data JSON NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_audit_logs_user FOREIGN KEY (user_id) REFERENCES users(id)
) ENGINE=InnoDB;

CREATE INDEX idx_bookings_schedule ON bookings (start_time, end_time, status);
CREATE INDEX idx_rentals_status ON rentals (status, start_time);
CREATE INDEX idx_service_jobs_status ON service_jobs (status, received_at);
CREATE INDEX idx_game_installs_status ON game_installs (status, received_at);

INSERT INTO customers (code, name, phone, address, identity_number, notes) VALUES
('CUST001', 'Budi Santoso', '081234567001', 'Jl. Melati No. 1', '3273010101010001', 'Pelanggan reguler'),
('CUST002', 'Siti Aminah', '081234567002', 'Jl. Kenanga No. 2', '3273010202020002', 'Sering booking VIP'),
('CUST003', 'Raka Pratama', '081234567003', 'Jl. Anggrek No. 3', NULL, 'Pelanggan service');

INSERT INTO room_types (code, name, hourly_rate, overtime_rate, description) VALUES
('REG', 'Regular', 10000, 12000, 'Ruang standar'),
('VIP', 'VIP', 20000, 25000, 'Ruang premium'),
('DUO', 'Duo', 15000, 18000, 'Ruang untuk dua pemain');

INSERT INTO rooms (room_type_id, code, name, capacity, status, notes) VALUES
((SELECT id FROM room_types WHERE code = 'REG'), 'R001', 'Ruang 1', 2, 'available', 'Dekat kasir'),
((SELECT id FROM room_types WHERE code = 'REG'), 'R002', 'Ruang 2', 2, 'available', NULL),
((SELECT id FROM room_types WHERE code = 'VIP'), 'VIP01', 'VIP 1', 4, 'available', 'TV besar'),
((SELECT id FROM room_types WHERE code = 'DUO'), 'D001', 'Duo 1', 2, 'maintenance', 'Perlu cek AC');

INSERT INTO consoles (code, name, console_type, serial_number, purchase_date, condition_status, availability_status, notes) VALUES
('PS001', 'PlayStation 4 - 1', 'PS4', 'PS4-001', '2024-01-10', 'good', 'available', NULL),
('PS002', 'PlayStation 4 - 2', 'PS4', 'PS4-002', '2024-02-10', 'good', 'available', NULL),
('PS501', 'PlayStation 5 - 1', 'PS5', 'PS5-001', '2025-03-15', 'good', 'available', 'Console utama VIP'),
('PS502', 'PlayStation 5 - 2', 'PS5', 'PS5-002', '2025-05-20', 'minor_issue', 'maintenance', 'Controller perlu dicek');

INSERT INTO fnb_items (code, name, category, purchase_price, selling_price, stock_qty, minimum_stock, unit) VALUES
('FNB001', 'Air Mineral', 'Minuman', 2500, 5000, 22, 6, 'botol'),
('FNB002', 'Mie Instan Cup', 'Makanan', 4500, 8000, 10, 4, 'cup'),
('FNB003', 'Teh Botol', 'Minuman', 3500, 7000, 18, 5, 'botol');

INSERT INTO games (code, title, platform, genre, size_gb, install_price) VALUES
('G001', 'EA Sports FC', 'PS4/PS5', 'Sports', 50.00, 5000),
('G002', 'Tekken', 'PS4/PS5', 'Fighting', 80.00, 5000),
('G003', 'Gran Turismo', 'PS5', 'Racing', 110.00, 7500);

INSERT INTO services (code, name, default_price, description) VALUES
('JS001', 'Bersih PS', 50000, 'Pembersihan console dan controller'),
('JS002', 'Install Ulang Sistem', 75000, 'Install ulang software/sistem'),
('JS003', 'Ganti Analog Controller', 30000, 'Jasa penggantian analog');

INSERT INTO spareparts (code, name, category, purchase_price, selling_price, stock_qty, minimum_stock, unit) VALUES
('SP001', 'Analog Stick', 'Controller', 15000, 35000, 14, 3, 'pcs'),
('SP002', 'Kabel HDMI', 'Kabel', 20000, 45000, 4, 2, 'pcs'),
('SP003', 'Thermal Paste', 'Maintenance', 10000, 25000, 7, 2, 'pcs');

INSERT INTO suppliers (code, name, phone, address, notes) VALUES
('SUP001', 'Supplier Umum', '081234567890', 'Jl. Pasar Baru No. 10', 'Supplier sparepart umum'),
('SUP002', 'Game Parts Store', '081234567891', 'Jl. Elektronik No. 5', 'Supplier controller dan kabel');

INSERT INTO payment_methods (code, name) VALUES
('CASH', 'Cash'),
('TRANSFER', 'Transfer Bank'),
('QRIS', 'QRIS'),
('DEBIT', 'Kartu Debit');

INSERT INTO roles (name, description) VALUES
('admin', 'Akses penuh aplikasi'),
('kasir', 'Transaksi harian dan data pelanggan'),
('owner', 'Laporan dan monitoring');

INSERT INTO users (username, password_hash, full_name) VALUES
('admin', '$2y$10$replace_with_real_hash_before_production', 'Administrator'),
('kasir', '$2y$10$replace_with_real_hash_before_production', 'Kasir Rental');

INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id
FROM users u
JOIN roles r ON r.name = 'admin'
WHERE u.username = 'admin';

INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id
FROM users u
JOIN roles r ON r.name = 'kasir'
WHERE u.username = 'kasir';

INSERT INTO bookings (booking_no, customer_id, room_id, console_id, start_time, end_time, deposit_amount, status, notes, created_by) VALUES
('BK-20260627-001', (SELECT id FROM customers WHERE code = 'CUST002'), (SELECT id FROM rooms WHERE code = 'VIP01'), (SELECT id FROM consoles WHERE code = 'PS501'), '2026-06-27 18:00:00', '2026-06-27 20:00:00', 20000, 'booked', 'Booking malam minggu', (SELECT id FROM users WHERE username = 'admin')),
('BK-20260627-002', (SELECT id FROM customers WHERE code = 'CUST001'), (SELECT id FROM rooms WHERE code = 'R001'), (SELECT id FROM consoles WHERE code = 'PS001'), '2026-06-27 13:00:00', '2026-06-27 15:00:00', 10000, 'checked_in', 'Sudah datang', (SELECT id FROM users WHERE username = 'kasir'));

INSERT INTO rentals (rental_no, booking_id, customer_id, room_id, console_id, start_time, planned_end_time, actual_end_time, hourly_rate, duration_minutes, rental_amount, discount_amount, total_amount, paid_amount, status, notes, created_by) VALUES
('SW-20260627-001', (SELECT id FROM bookings WHERE booking_no = 'BK-20260627-002'), (SELECT id FROM customers WHERE code = 'CUST001'), (SELECT id FROM rooms WHERE code = 'R001'), (SELECT id FROM consoles WHERE code = 'PS001'), '2026-06-27 13:00:00', '2026-06-27 15:00:00', '2026-06-27 15:05:00', 10000, 125, 20833, 0, 20833, 21000, 'completed', 'Sewa dari booking', (SELECT id FROM users WHERE username = 'kasir')),
('SW-20260627-002', NULL, (SELECT id FROM customers WHERE code = 'CUST002'), (SELECT id FROM rooms WHERE code = 'VIP01'), (SELECT id FROM consoles WHERE code = 'PS501'), '2026-06-27 16:00:00', '2026-06-27 18:00:00', NULL, 20000, 120, 40000, 4000, 36000, 36000, 'running', 'Diskon 10 persen', (SELECT id FROM users WHERE username = 'admin'));

INSERT INTO rental_fnb_items (rental_id, fnb_item_id, qty, price, subtotal) VALUES
((SELECT id FROM rentals WHERE rental_no = 'SW-20260627-001'), (SELECT id FROM fnb_items WHERE code = 'FNB001'), 2, 5000, 10000);

INSERT INTO game_installs (install_no, customer_id, device_name, device_capacity_gb, service_fee, total_amount, status, notes, received_at, completed_at, created_by) VALUES
('IG-20260627-001', (SELECT id FROM customers WHERE code = 'CUST003'), 'HDD External 1TB', 1000, 15000, 25000, 'completed', 'Install 2 game', '2026-06-27 10:00:00', '2026-06-27 12:00:00', (SELECT id FROM users WHERE username = 'kasir'));

INSERT INTO game_install_items (game_install_id, game_id, price) VALUES
((SELECT id FROM game_installs WHERE install_no = 'IG-20260627-001'), (SELECT id FROM games WHERE code = 'G001'), 5000),
((SELECT id FROM game_installs WHERE install_no = 'IG-20260627-001'), (SELECT id FROM games WHERE code = 'G002'), 5000);

INSERT INTO service_jobs (service_no, customer_id, service_id, item_name, problem_description, technician_notes, labor_cost, sparepart_cost, total_amount, status, received_at, completed_at, picked_up_at, created_by) VALUES
('SV-20260627-001', (SELECT id FROM customers WHERE code = 'CUST003'), (SELECT id FROM services WHERE code = 'JS003'), 'Controller DualShock', 'Analog kiri drift', 'Ganti analog kiri', 30000, 35000, 65000, 'completed', '2026-06-27 09:00:00', '2026-06-27 11:00:00', NULL, (SELECT id FROM users WHERE username = 'admin'));

INSERT INTO service_spareparts (service_job_id, sparepart_id, qty, price, subtotal) VALUES
((SELECT id FROM service_jobs WHERE service_no = 'SV-20260627-001'), (SELECT id FROM spareparts WHERE code = 'SP001'), 1, 35000, 35000);

INSERT INTO fnb_sales (sale_no, customer_id, sale_date, total_amount, paid_amount, status, notes, created_by) VALUES
('FNB-20260627-001', (SELECT id FROM customers WHERE code = 'CUST001'), '2026-06-27 14:00:00', 15000, 15000, 'paid', 'Penjualan langsung', (SELECT id FROM users WHERE username = 'kasir'));

INSERT INTO fnb_sale_items (fnb_sale_id, fnb_item_id, qty, price, subtotal) VALUES
((SELECT id FROM fnb_sales WHERE sale_no = 'FNB-20260627-001'), (SELECT id FROM fnb_items WHERE code = 'FNB001'), 1, 5000, 5000),
((SELECT id FROM fnb_sales WHERE sale_no = 'FNB-20260627-001'), (SELECT id FROM fnb_items WHERE code = 'FNB003'), 1, 7000, 7000),
((SELECT id FROM fnb_sales WHERE sale_no = 'FNB-20260627-001'), (SELECT id FROM fnb_items WHERE code = 'FNB002'), 1, 3000, 3000);

INSERT INTO sparepart_purchases (purchase_no, supplier_id, purchase_date, total_amount, notes, created_by) VALUES
('PB-20260627-001', (SELECT id FROM suppliers WHERE code = 'SUP001'), '2026-06-27 08:30:00', 50000, 'Restock sparepart', (SELECT id FROM users WHERE username = 'admin'));

INSERT INTO sparepart_purchase_items (sparepart_purchase_id, sparepart_id, qty, purchase_price, subtotal) VALUES
((SELECT id FROM sparepart_purchases WHERE purchase_no = 'PB-20260627-001'), (SELECT id FROM spareparts WHERE code = 'SP001'), 2, 15000, 30000),
((SELECT id FROM sparepart_purchases WHERE purchase_no = 'PB-20260627-001'), (SELECT id FROM spareparts WHERE code = 'SP003'), 2, 10000, 20000);

INSERT INTO sparepart_sales (sale_no, customer_id, sale_date, total_amount, paid_amount, status, notes, created_by) VALUES
('JSP-20260627-001', (SELECT id FROM customers WHERE code = 'CUST002'), '2026-06-27 15:30:00', 45000, 45000, 'paid', 'Jual kabel HDMI', (SELECT id FROM users WHERE username = 'kasir'));

INSERT INTO sparepart_sale_items (sparepart_sale_id, sparepart_id, qty, price, subtotal) VALUES
((SELECT id FROM sparepart_sales WHERE sale_no = 'JSP-20260627-001'), (SELECT id FROM spareparts WHERE code = 'SP002'), 1, 45000, 45000);

INSERT INTO fines (fine_no, rental_id, customer_id, fine_type, description, amount, status, created_by) VALUES
('DN-20260627-001', (SELECT id FROM rentals WHERE rental_no = 'SW-20260627-001'), (SELECT id FROM customers WHERE code = 'CUST001'), 'late', 'Terlambat 5 menit', 5000, 'paid', (SELECT id FROM users WHERE username = 'kasir'));

INSERT INTO payments (payment_no, payment_method_id, reference_type, reference_id, amount, paid_at, notes, created_by) VALUES
('PAY-20260627-001', (SELECT id FROM payment_methods WHERE code = 'CASH'), 'rental', (SELECT id FROM rentals WHERE rental_no = 'SW-20260627-001'), 21000, '2026-06-27 15:06:00', 'Pembayaran sewa', (SELECT id FROM users WHERE username = 'kasir')),
('PAY-20260627-002', (SELECT id FROM payment_methods WHERE code = 'QRIS'), 'fnb_sale', (SELECT id FROM fnb_sales WHERE sale_no = 'FNB-20260627-001'), 15000, '2026-06-27 14:00:00', 'Pembayaran FNB', (SELECT id FROM users WHERE username = 'kasir')),
('PAY-20260627-003', (SELECT id FROM payment_methods WHERE code = 'CASH'), 'fine', (SELECT id FROM fines WHERE fine_no = 'DN-20260627-001'), 5000, '2026-06-27 15:07:00', 'Pembayaran denda', (SELECT id FROM users WHERE username = 'kasir'));

INSERT INTO stock_movements (item_type, item_id, movement_type, qty, reference_type, reference_id, notes, created_by) VALUES
('sparepart', (SELECT id FROM spareparts WHERE code = 'SP001'), 'in', 2, 'sparepart_purchase', (SELECT id FROM sparepart_purchases WHERE purchase_no = 'PB-20260627-001'), 'Pembelian sparepart PB-20260627-001', (SELECT id FROM users WHERE username = 'admin')),
('sparepart', (SELECT id FROM spareparts WHERE code = 'SP003'), 'in', 2, 'sparepart_purchase', (SELECT id FROM sparepart_purchases WHERE purchase_no = 'PB-20260627-001'), 'Pembelian sparepart PB-20260627-001', (SELECT id FROM users WHERE username = 'admin')),
('sparepart', (SELECT id FROM spareparts WHERE code = 'SP002'), 'out', 1, 'sparepart_sale', (SELECT id FROM sparepart_sales WHERE sale_no = 'JSP-20260627-001'), 'Penjualan sparepart JSP-20260627-001', (SELECT id FROM users WHERE username = 'kasir')),
('sparepart', (SELECT id FROM spareparts WHERE code = 'SP001'), 'out', 1, 'service_job', (SELECT id FROM service_jobs WHERE service_no = 'SV-20260627-001'), 'Pemakaian sparepart service SV-20260627-001', (SELECT id FROM users WHERE username = 'admin')),
('fnb', (SELECT id FROM fnb_items WHERE code = 'FNB001'), 'out', 1, 'fnb_sale', (SELECT id FROM fnb_sales WHERE sale_no = 'FNB-20260627-001'), 'Penjualan FNB', (SELECT id FROM users WHERE username = 'kasir')),
('fnb', (SELECT id FROM fnb_items WHERE code = 'FNB002'), 'out', 1, 'fnb_sale', (SELECT id FROM fnb_sales WHERE sale_no = 'FNB-20260627-001'), 'Penjualan FNB', (SELECT id FROM users WHERE username = 'kasir')),
('fnb', (SELECT id FROM fnb_items WHERE code = 'FNB003'), 'out', 1, 'fnb_sale', (SELECT id FROM fnb_sales WHERE sale_no = 'FNB-20260627-001'), 'Penjualan FNB', (SELECT id FROM users WHERE username = 'kasir')),
('fnb', (SELECT id FROM fnb_items WHERE code = 'FNB001'), 'out', 2, 'rental', (SELECT id FROM rentals WHERE rental_no = 'SW-20260627-001'), 'Tambahan FNB di transaksi sewa', (SELECT id FROM users WHERE username = 'kasir'));

INSERT INTO settings (setting_key, setting_value, description) VALUES
('shop_name', 'Rental PS', 'Nama toko yang tampil di struk'),
('shop_address', 'Jl. Contoh Rental No. 99', 'Alamat toko'),
('shop_phone', '081234567890', 'No HP toko'),
('currency', 'IDR', 'Mata uang aplikasi'),
('late_tolerance_minutes', '10', 'Toleransi keterlambatan sebelum denda'),
('default_discount_percent', '0', 'Diskon default transaksi sewa');

INSERT INTO audit_logs (user_id, action, table_name, record_id, old_data, new_data) VALUES
((SELECT id FROM users WHERE username = 'admin'), 'seed', 'database', 1, NULL, JSON_OBJECT('message', 'Initial seed completed'));
