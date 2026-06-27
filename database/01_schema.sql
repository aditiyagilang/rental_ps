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
