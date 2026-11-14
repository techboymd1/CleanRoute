-- Safe to run repeatedly: existing sample rows with these IDs are updated.
USE CleanRouteDb;

-- All sample accounts use the application password: CleanRoute123!
-- Change or remove sample accounts before using this outside local development.
INSERT INTO
    users (
        id,
        full_name,
        email,
        password_hash,
        phone_number,
        address,
        role,
        is_active,
        created_at_utc
    )
VALUES (
        1,
        'Ada Okafor',
        'ada@example.com',
        '100000:lh0sKFrderqTd75VadBcaA==:q0H38jYmIiT5XIn5SlSGxigtU07uwGktdEJsWMBPzwc=',
        '0803 555 0101',
        '12 Unity Close, Surulere',
        'Resident',
        1,
        UTC_TIMESTAMP()
    ),
    (
        2,
        'Musa Bello',
        'musa@example.com',
        '100000:lh0sKFrderqTd75VadBcaA==:q0H38jYmIiT5XIn5SlSGxigtU07uwGktdEJsWMBPzwc=',
        '0803 555 0102',
        '4 Alade Street, Yaba',
        'Resident',
        1,
        UTC_TIMESTAMP()
    ),
    (
        3,
        'Chinedu Nwosu',
        'chinedu@example.com',
        '100000:lh0sKFrderqTd75VadBcaA==:q0H38jYmIiT5XIn5SlSGxigtU07uwGktdEJsWMBPzwc=',
        '0803 555 0103',
        'CleanRoute Operations',
        'Collector',
        1,
        UTC_TIMESTAMP()
    ),
    (
        4,
        'Aisha Ibrahim',
        'aisha@example.com',
        '100000:lh0sKFrderqTd75VadBcaA==:q0H38jYmIiT5XIn5SlSGxigtU07uwGktdEJsWMBPzwc=',
        '0803 555 0104',
        'CleanRoute Operations',
        'Collector',
        1,
        UTC_TIMESTAMP()
    ),
    (
        5,
        'CleanRoute Admin',
        'admin@cleanroute.local',
        '100000:lh0sKFrderqTd75VadBcaA==:q0H38jYmIiT5XIn5SlSGxigtU07uwGktdEJsWMBPzwc=',
        '0803 555 0100',
        'CleanRoute Operations',
        'Admin',
        1,
        UTC_TIMESTAMP()
    )
ON DUPLICATE KEY UPDATE
    full_name = VALUES(full_name),
    password_hash = VALUES(password_hash),
    role = VALUES(role),
    is_active = VALUES(is_active);

INSERT INTO
    collection_requests (
        id,
        resident_id,
        waste_type,
        collection_address,
        notes,
        status,
        preferred_date,
        created_at_utc
    )
VALUES (
        1,
        1,
        'Household',
        '12 Unity Close, Surulere',
        'Two black bags by the gate.',
        'Scheduled',
        DATE_ADD(CURDATE(), INTERVAL 1 DAY),
        UTC_TIMESTAMP()
    ),
    (
        2,
        2,
        'Recyclable',
        '4 Alade Street, Yaba',
        'Separated plastic bottles and cardboard.',
        'Submitted',
        DATE_ADD(CURDATE(), INTERVAL 2 DAY),
        UTC_TIMESTAMP()
    ),
    (
        3,
        1,
        'Bulk',
        '12 Unity Close, Surulere',
        'One damaged chair and small table.',
        'Collected',
        DATE_SUB(CURDATE(), INTERVAL 3 DAY),
        DATE_SUB(
            UTC_TIMESTAMP(),
            INTERVAL 5 DAY
        )
    )
ON DUPLICATE KEY UPDATE
    status = VALUES(status),
    preferred_date = VALUES(preferred_date),
    notes = VALUES(notes);

INSERT INTO
    collection_schedules (
        id,
        collection_request_id,
        collector_id,
        scheduled_for,
        route_area,
        completion_notes,
        completed_at_utc
    )
VALUES (
        1,
        1,
        3,
        DATE_ADD(
            DATE_ADD(CURDATE(), INTERVAL 1 DAY),
            INTERVAL 9 HOUR
        ),
        'Surulere Central',
        NULL,
        NULL
    ),
    (
        2,
        3,
        4,
        DATE_ADD(
            DATE_SUB(CURDATE(), INTERVAL 3 DAY),
            INTERVAL 10 HOUR
        ),
        'Surulere Central',
        'Collected successfully.',
        DATE_SUB(
            UTC_TIMESTAMP(),
            INTERVAL 3 DAY
        )
    )
ON DUPLICATE KEY UPDATE
    scheduled_for = VALUES(scheduled_for),
    route_area = VALUES(route_area),
    completion_notes = VALUES(completion_notes),
    completed_at_utc = VALUES(completed_at_utc);