-- CleanRoute uses its own database. This script never touches TradeTrack.
CREATE DATABASE IF NOT EXISTS CleanRouteDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE CleanRouteDb;

CREATE TABLE IF NOT EXISTS users (
    id INT NOT NULL AUTO_INCREMENT,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    phone_number VARCHAR(25) NOT NULL,
    address VARCHAR(200) NOT NULL,
    role VARCHAR(20) NOT NULL,
    is_active TINYINT(1) NOT NULL DEFAULT 1,
    created_at_utc DATETIME NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY ux_users_email (email)
);

CREATE TABLE IF NOT EXISTS collection_requests (
    id INT NOT NULL AUTO_INCREMENT,
    resident_id INT NOT NULL,
    waste_type VARCHAR(30) NOT NULL,
    collection_address VARCHAR(500) NOT NULL,
    notes VARCHAR(500) NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'Submitted',
    preferred_date DATETIME NOT NULL,
    created_at_utc DATETIME NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT fk_requests_resident FOREIGN KEY (resident_id) REFERENCES users (id),
    KEY ix_requests_status_date (status, preferred_date)
);

CREATE TABLE IF NOT EXISTS collection_schedules (
    id INT NOT NULL AUTO_INCREMENT,
    collection_request_id INT NOT NULL,
    collector_id INT NOT NULL,
    scheduled_for DATETIME NOT NULL,
    route_area VARCHAR(250) NULL,
    completion_notes VARCHAR(500) NULL,
    completed_at_utc DATETIME NULL,
    PRIMARY KEY (id),
    UNIQUE KEY ux_schedules_request (collection_request_id),
    CONSTRAINT fk_schedules_request FOREIGN KEY (collection_request_id) REFERENCES collection_requests (id) ON DELETE CASCADE,
    CONSTRAINT fk_schedules_collector FOREIGN KEY (collector_id) REFERENCES users (id),
    KEY ix_schedules_collector_date (collector_id, scheduled_for)
);