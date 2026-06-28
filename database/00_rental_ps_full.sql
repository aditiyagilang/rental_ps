IF DB_ID(N'rental_ps') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE rental_ps');
END
GO

USE rental_ps;
GO

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
GO

CREATE TABLE customers (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(120) NOT NULL,
  phone NVARCHAR(30) NULL,
  address NVARCHAR(MAX) NULL,
  identity_number NVARCHAR(60) NULL,
  notes NVARCHAR(MAX) NULL,
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE room_types (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(80) NOT NULL,
  hourly_rate DECIMAL(14,2) NOT NULL DEFAULT 0,
  overtime_rate DECIMAL(14,2) NOT NULL DEFAULT 0,
  description NVARCHAR(MAX) NULL,
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE rooms (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  room_type_id BIGINT NOT NULL,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(80) NOT NULL,
  capacity INT NOT NULL DEFAULT 1,
  status NVARCHAR(30) NOT NULL DEFAULT 'available',
  notes NVARCHAR(MAX) NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_rooms_room_type FOREIGN KEY (room_type_id) REFERENCES room_types(id),
  CONSTRAINT chk_rooms_status CHECK (status IN ('available','occupied','maintenance','inactive'))
);

CREATE TABLE consoles (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(100) NOT NULL,
  console_type NVARCHAR(50) NOT NULL,
  serial_number NVARCHAR(100) NULL,
  purchase_date DATE NULL,
  condition_status NVARCHAR(30) NOT NULL DEFAULT 'good',
  availability_status NVARCHAR(30) NOT NULL DEFAULT 'available',
  notes NVARCHAR(MAX) NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT chk_consoles_condition CHECK (condition_status IN ('good','minor_issue','broken','service')),
  CONSTRAINT chk_consoles_availability CHECK (availability_status IN ('available','rented','maintenance','inactive'))
);

CREATE TABLE fnb_items (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(120) NOT NULL,
  category NVARCHAR(80) NULL,
  purchase_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  selling_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  stock_qty INT NOT NULL DEFAULT 0,
  minimum_stock INT NOT NULL DEFAULT 0,
  unit NVARCHAR(30) NOT NULL DEFAULT 'pcs',
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE games (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  title NVARCHAR(150) NOT NULL,
  platform NVARCHAR(50) NOT NULL,
  genre NVARCHAR(80) NULL,
  size_gb DECIMAL(8,2) NULL,
  install_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE services (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(120) NOT NULL,
  default_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  description NVARCHAR(MAX) NULL,
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE spareparts (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(120) NOT NULL,
  category NVARCHAR(80) NULL,
  purchase_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  selling_price DECIMAL(14,2) NOT NULL DEFAULT 0,
  stock_qty INT NOT NULL DEFAULT 0,
  minimum_stock INT NOT NULL DEFAULT 0,
  unit NVARCHAR(30) NOT NULL DEFAULT 'pcs',
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE suppliers (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(120) NOT NULL,
  phone NVARCHAR(30) NULL,
  address NVARCHAR(MAX) NULL,
  notes NVARCHAR(MAX) NULL,
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE payment_methods (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  code NVARCHAR(30) NOT NULL UNIQUE,
  name NVARCHAR(80) NOT NULL,
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE roles (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  name NVARCHAR(60) NOT NULL UNIQUE,
  description NVARCHAR(MAX) NULL
);

CREATE TABLE users (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  username NVARCHAR(60) NOT NULL UNIQUE,
  password_hash NVARCHAR(255) NOT NULL,
  full_name NVARCHAR(120) NOT NULL,
  is_active BIT NOT NULL DEFAULT 1,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE user_roles (
  user_id BIGINT NOT NULL,
  role_id BIGINT NOT NULL,
  PRIMARY KEY (user_id, role_id),
  CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES users(id),
  CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES roles(id)
);

CREATE TABLE bookings (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  booking_no NVARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT NOT NULL,
  room_id BIGINT NULL,
  console_id BIGINT NULL,
  start_time DATETIME2 NOT NULL,
  end_time DATETIME2 NOT NULL,
  deposit_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status NVARCHAR(30) NOT NULL DEFAULT 'booked',
  notes NVARCHAR(MAX) NULL,
  created_by BIGINT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_bookings_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_bookings_room FOREIGN KEY (room_id) REFERENCES rooms(id),
  CONSTRAINT fk_bookings_console FOREIGN KEY (console_id) REFERENCES consoles(id),
  CONSTRAINT fk_bookings_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_bookings_time CHECK (end_time > start_time),
  CONSTRAINT chk_bookings_status CHECK (status IN ('booked','checked_in','completed','cancelled'))
);

CREATE TABLE rentals (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  rental_no NVARCHAR(40) NOT NULL UNIQUE,
  booking_id BIGINT NULL,
  customer_id BIGINT NOT NULL,
  room_id BIGINT NOT NULL,
  console_id BIGINT NOT NULL,
  start_time DATETIME2 NOT NULL,
  planned_end_time DATETIME2 NULL,
  actual_end_time DATETIME2 NULL,
  hourly_rate DECIMAL(14,2) NOT NULL DEFAULT 0,
  duration_minutes INT NOT NULL DEFAULT 0,
  rental_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  discount_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  paid_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status NVARCHAR(30) NOT NULL DEFAULT 'running',
  notes NVARCHAR(MAX) NULL,
  created_by BIGINT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_rentals_booking FOREIGN KEY (booking_id) REFERENCES bookings(id),
  CONSTRAINT fk_rentals_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_rentals_room FOREIGN KEY (room_id) REFERENCES rooms(id),
  CONSTRAINT fk_rentals_console FOREIGN KEY (console_id) REFERENCES consoles(id),
  CONSTRAINT fk_rentals_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_rentals_status CHECK (status IN ('running','completed','cancelled'))
);

CREATE TABLE rental_fnb_items (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  rental_id BIGINT NOT NULL,
  fnb_item_id BIGINT NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_rental_fnb_rental FOREIGN KEY (rental_id) REFERENCES rentals(id),
  CONSTRAINT fk_rental_fnb_item FOREIGN KEY (fnb_item_id) REFERENCES fnb_items(id),
  CONSTRAINT chk_rental_fnb_qty CHECK (qty > 0)
);

CREATE TABLE game_installs (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  install_no NVARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT NOT NULL,
  device_name NVARCHAR(120) NOT NULL,
  device_capacity_gb DECIMAL(10,2) NULL,
  service_fee DECIMAL(14,2) NOT NULL DEFAULT 0,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status NVARCHAR(30) NOT NULL DEFAULT 'received',
  notes NVARCHAR(MAX) NULL,
  received_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  completed_at DATETIME2 NULL,
  created_by BIGINT NULL,
  CONSTRAINT fk_game_installs_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_game_installs_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_game_installs_status CHECK (status IN ('received','processing','completed','picked_up','cancelled'))
);

CREATE TABLE game_install_items (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  game_install_id BIGINT NOT NULL,
  game_id BIGINT NOT NULL,
  price DECIMAL(14,2) NOT NULL DEFAULT 0,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_game_install_items_install FOREIGN KEY (game_install_id) REFERENCES game_installs(id),
  CONSTRAINT fk_game_install_items_game FOREIGN KEY (game_id) REFERENCES games(id)
);

CREATE TABLE service_jobs (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  service_no NVARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT NOT NULL,
  service_id BIGINT NULL,
  item_name NVARCHAR(120) NOT NULL,
  problem_description NVARCHAR(MAX) NOT NULL,
  technician_notes NVARCHAR(MAX) NULL,
  labor_cost DECIMAL(14,2) NOT NULL DEFAULT 0,
  sparepart_cost DECIMAL(14,2) NOT NULL DEFAULT 0,
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status NVARCHAR(30) NOT NULL DEFAULT 'received',
  received_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  completed_at DATETIME2 NULL,
  picked_up_at DATETIME2 NULL,
  created_by BIGINT NULL,
  CONSTRAINT fk_service_jobs_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_service_jobs_service FOREIGN KEY (service_id) REFERENCES services(id),
  CONSTRAINT fk_service_jobs_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_service_jobs_status CHECK (status IN ('received','diagnosis','processing','completed','picked_up','cancelled'))
);

CREATE TABLE service_spareparts (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  service_job_id BIGINT NOT NULL,
  sparepart_id BIGINT NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_service_spareparts_job FOREIGN KEY (service_job_id) REFERENCES service_jobs(id),
  CONSTRAINT fk_service_spareparts_part FOREIGN KEY (sparepart_id) REFERENCES spareparts(id),
  CONSTRAINT chk_service_spareparts_qty CHECK (qty > 0)
);

CREATE TABLE fnb_sales (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  sale_no NVARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT NULL,
  sale_date DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  paid_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status NVARCHAR(30) NOT NULL DEFAULT 'draft',
  notes NVARCHAR(MAX) NULL,
  created_by BIGINT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_fnb_sales_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_fnb_sales_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_fnb_sales_status CHECK (status IN ('draft','paid','void'))
);

CREATE TABLE fnb_sale_items (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  fnb_sale_id BIGINT NOT NULL,
  fnb_item_id BIGINT NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  CONSTRAINT fk_fnb_sale_items_sale FOREIGN KEY (fnb_sale_id) REFERENCES fnb_sales(id),
  CONSTRAINT fk_fnb_sale_items_item FOREIGN KEY (fnb_item_id) REFERENCES fnb_items(id),
  CONSTRAINT chk_fnb_sale_items_qty CHECK (qty > 0)
);

CREATE TABLE sparepart_purchases (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  purchase_no NVARCHAR(40) NOT NULL UNIQUE,
  supplier_id BIGINT NULL,
  purchase_date DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  notes NVARCHAR(MAX) NULL,
  created_by BIGINT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_sparepart_purchases_supplier FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
  CONSTRAINT fk_sparepart_purchases_created_by FOREIGN KEY (created_by) REFERENCES users(id)
);

CREATE TABLE sparepart_purchase_items (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  sparepart_purchase_id BIGINT NOT NULL,
  sparepart_id BIGINT NOT NULL,
  qty INT NOT NULL,
  purchase_price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  CONSTRAINT fk_sparepart_purchase_items_purchase FOREIGN KEY (sparepart_purchase_id) REFERENCES sparepart_purchases(id),
  CONSTRAINT fk_sparepart_purchase_items_part FOREIGN KEY (sparepart_id) REFERENCES spareparts(id),
  CONSTRAINT chk_sparepart_purchase_items_qty CHECK (qty > 0)
);

CREATE TABLE sparepart_sales (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  sale_no NVARCHAR(40) NOT NULL UNIQUE,
  customer_id BIGINT NULL,
  sale_date DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  total_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  paid_amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status NVARCHAR(30) NOT NULL DEFAULT 'draft',
  notes NVARCHAR(MAX) NULL,
  created_by BIGINT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_sparepart_sales_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_sparepart_sales_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_sparepart_sales_status CHECK (status IN ('draft','paid','void'))
);

CREATE TABLE sparepart_sale_items (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  sparepart_sale_id BIGINT NOT NULL,
  sparepart_id BIGINT NOT NULL,
  qty INT NOT NULL,
  price DECIMAL(14,2) NOT NULL,
  subtotal DECIMAL(14,2) NOT NULL,
  CONSTRAINT fk_sparepart_sale_items_sale FOREIGN KEY (sparepart_sale_id) REFERENCES sparepart_sales(id),
  CONSTRAINT fk_sparepart_sale_items_part FOREIGN KEY (sparepart_id) REFERENCES spareparts(id),
  CONSTRAINT chk_sparepart_sale_items_qty CHECK (qty > 0)
);

CREATE TABLE fines (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  fine_no NVARCHAR(40) NOT NULL UNIQUE,
  rental_id BIGINT NULL,
  customer_id BIGINT NOT NULL,
  fine_type NVARCHAR(30) NOT NULL,
  description NVARCHAR(MAX) NOT NULL,
  amount DECIMAL(14,2) NOT NULL DEFAULT 0,
  status NVARCHAR(30) NOT NULL DEFAULT 'unpaid',
  created_by BIGINT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_fines_rental FOREIGN KEY (rental_id) REFERENCES rentals(id),
  CONSTRAINT fk_fines_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
  CONSTRAINT fk_fines_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_fines_type CHECK (fine_type IN ('late','damage','lost_item','manual')),
  CONSTRAINT chk_fines_status CHECK (status IN ('unpaid','paid','void'))
);

CREATE TABLE payments (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  payment_no NVARCHAR(40) NOT NULL UNIQUE,
  payment_method_id BIGINT NOT NULL,
  reference_type NVARCHAR(40) NOT NULL,
  reference_id BIGINT NOT NULL,
  amount DECIMAL(14,2) NOT NULL,
  paid_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  notes NVARCHAR(MAX) NULL,
  created_by BIGINT NULL,
  CONSTRAINT fk_payments_method FOREIGN KEY (payment_method_id) REFERENCES payment_methods(id),
  CONSTRAINT fk_payments_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_payments_reference_type CHECK (reference_type IN ('rental','booking','fine','game_install','service_job','fnb_sale','sparepart_purchase','sparepart_sale'))
);

CREATE TABLE stock_movements (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  item_type NVARCHAR(30) NOT NULL,
  item_id BIGINT NOT NULL,
  movement_type NVARCHAR(30) NOT NULL,
  qty INT NOT NULL,
  reference_type NVARCHAR(60) NULL,
  reference_id BIGINT NULL,
  notes NVARCHAR(MAX) NULL,
  created_by BIGINT NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_stock_movements_created_by FOREIGN KEY (created_by) REFERENCES users(id),
  CONSTRAINT chk_stock_movements_item_type CHECK (item_type IN ('fnb','sparepart')),
  CONSTRAINT chk_stock_movements_type CHECK (movement_type IN ('in','out','adjustment'))
);

CREATE TABLE settings (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  setting_key NVARCHAR(80) NOT NULL UNIQUE,
  setting_value NVARCHAR(MAX) NULL,
  description NVARCHAR(MAX) NULL,
  updated_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE audit_logs (
  id BIGINT IDENTITY(1,1) PRIMARY KEY,
  user_id BIGINT NULL,
  action NVARCHAR(80) NOT NULL,
  table_name NVARCHAR(80) NOT NULL,
  record_id BIGINT NULL,
  old_data NVARCHAR(MAX) NULL,
  new_data NVARCHAR(MAX) NULL,
  created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  CONSTRAINT fk_audit_logs_user FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE INDEX idx_bookings_schedule ON bookings (start_time, end_time, status);
CREATE INDEX idx_rentals_status ON rentals (status, start_time);
CREATE INDEX idx_service_jobs_status ON service_jobs (status, received_at);
CREATE INDEX idx_game_installs_status ON game_installs (status, received_at);
CREATE INDEX idx_payments_reference ON payments (reference_type, reference_id);
CREATE INDEX idx_stock_movements_item ON stock_movements (item_type, item_id);
CREATE INDEX idx_stock_movements_reference ON stock_movements (reference_type, reference_id);
GO

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
('admin', 'admin', 'Administrator'),
('kasir', 'kasir', 'Kasir Rental');

INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id FROM users u JOIN roles r ON r.name = 'admin' WHERE u.username = 'admin';

INSERT INTO user_roles (user_id, role_id)
SELECT u.id, r.id FROM users u JOIN roles r ON r.name = 'kasir' WHERE u.username = 'kasir';

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
((SELECT id FROM users WHERE username = 'admin'), 'seed', 'database', 1, NULL, '{"message":"Initial seed completed"}');
