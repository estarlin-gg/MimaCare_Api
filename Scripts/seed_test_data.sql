-- ============================================================================
-- MimaCare :: Datos de prueba ODONTOLÓGICOS (pacientes, citas, historial)
-- ============================================================================
-- CUENTAS DE PRUEBA (este script también normaliza sus contraseñas):
--
--   CLINICA : mimacare@gmail.com   / Egerman-17
--   DOCTOR  : estarlin@gmail.com   / Estarlin2026*   (Cirugía Oral)
--   DOCTOR  : sweet@gmail.com      / Sweetpain2026*  (Ortodoncia)
--   DOCTOR  : kgarcia@gmail.com    / Karla2026*      (Endodoncia)
--   DOCTOR  : wmedrano@gmail.com   / Dora2026*       (Periodoncia)
--
-- NOTA: la cuenta real del segundo doctor es sweet@gmail.com
--       (NO existe sweetpain1725@gmail.com).
--
-- El script es IDEMPOTENTE: elimina las filas sembradas por una corrida
-- anterior (IDs fijos 9f000000-...) y vuelve a insertar todo con fechas
-- relativas a CURRENT_DATE. También REEMPLAZA el catálogo de especialidades
-- por uno odontológico.
--
-- Ejecutar: psql -U postgres -d MimaCare -f Scripts/seed_test_data.sql
-- ============================================================================

BEGIN;

-- ============================================================================
-- 1. CONTRASEÑAS CONOCIDAS + DESBLOQUEO DE CUENTAS
-- ============================================================================

UPDATE "AspNetUsers" SET
    "PasswordHash"      = 'AQAAAAIAAYagAAAAENU8CIdQbO/36C2XPUiE2rkqlM0OFFS1GnUAUoke2SjPAoAXDjkUiYzEvSiR42b2Jg==',
    "AccessFailedCount" = 0,
    "LockoutEnd"        = NULL,
    "IsActive"          = TRUE,
    "EmailConfirmed"    = TRUE
WHERE "Email" = 'mimacare@gmail.com';

UPDATE "AspNetUsers" SET
    "PasswordHash"      = 'AQAAAAIAAYagAAAAECxjaUO1Iw7AW0mbGfSJnBLLjBJofBM5BNZcnh/Cw9rcywMhRm6LQ787ZZ8yZcByTQ==',
    "AccessFailedCount" = 0,
    "LockoutEnd"        = NULL,
    "IsActive"          = TRUE,
    "EmailConfirmed"    = TRUE
WHERE "Email" = 'estarlin@gmail.com';

UPDATE "AspNetUsers" SET
    "PasswordHash"      = 'AQAAAAIAAYagAAAAEHVc/62PjtRAaH1DjuHVXn4eKV1T7ewSgz2rIZunFqwiEyRmhevyD+WmXTYnFH8Ikg==',
    "AccessFailedCount" = 0,
    "LockoutEnd"        = NULL,
    "IsActive"          = TRUE,
    "EmailConfirmed"    = TRUE
WHERE "Email" = 'sweet@gmail.com';

UPDATE "AspNetUsers" SET
    "PasswordHash"      = 'AQAAAAIAAYagAAAAEBZ6VRbiwLpXk0/UYXAaSFWAwW6U8cX9I13Y9jNycf5hd7zYHOrombKlfgaxPT4c/g==',
    "AccessFailedCount" = 0,
    "LockoutEnd"        = NULL,
    "IsActive"          = TRUE,
    "EmailConfirmed"    = TRUE
WHERE "Email" = 'kgarcia@gmail.com';

UPDATE "AspNetUsers" SET
    "PasswordHash"      = 'AQAAAAIAAYagAAAAEB+1AFRWXX76dGF2AECkO8Tsl0tg34WYKg1Q43OPJ6J8uqx+ShRqJ4n+APb+gjRKyg==',
    "AccessFailedCount" = 0,
    "LockoutEnd"        = NULL,
    "IsActive"          = TRUE,
    "EmailConfirmed"    = TRUE
WHERE "Email" = 'wmedrano@gmail.com';

-- Especialidad clínica de cada doctor (contexto odontológico)
UPDATE "Doctors" SET "Specialty" = 'Cirugía Oral'
WHERE "Id" = '019fe959-15d4-7a72-9889-fdbf4629d1d1';  -- estarlin

UPDATE "Doctors" SET "Specialty" = 'Ortodoncia'
WHERE "Id" = '019e3e44-5293-705d-9101-4592f16f60e9';  -- sweet

UPDATE "Doctors" SET "Specialty" = 'Endodoncia'
WHERE "Id" = '019e57df-b547-75dc-84cf-04b3ab8acc1c';  -- kgarcia

UPDATE "Doctors" SET "Specialty" = 'Periodoncia'
WHERE "Id" = '019fe4ea-cd63-73ad-8361-36bfaa759f30';  -- wmedrano

-- Limpieza de restos de pruebas previas en el perfil de Karla García
UPDATE "AspNetUsers" SET "FirstName" = 'Karla', "LastName" = 'García'
WHERE "Id" = '019e57df-a16b-7632-a506-8558ace295cb';

-- ============================================================================
-- 2. LIMPIEZA DE CORRIDAS ANTERIORES + CATÁLOGO ODONTOLÓGICO
-- ============================================================================

DELETE FROM "PatientFiles"
WHERE "Id"::text LIKE '9f000000-0000-0000-0000-%';

DELETE FROM "Appointments"
WHERE "Id"::text LIKE '9f000000-0000-0000-0000-%';

DELETE FROM "Patients"
WHERE "Id"::text LIKE '9f000000-0000-0000-0000-%';

DELETE FROM "DoctorSchedules"
WHERE "Id"::text LIKE '9f000000-0000-0000-0000-%';

-- Reemplazo completo del catálogo por uno odontológico
DELETE FROM "DoctorSpecialties";

DELETE FROM "Specialties";

INSERT INTO "Specialties" ("Id", "Name", "Description", "IsActive", "CreatedAt") VALUES
('9f000000-0000-0000-0000-000000009001', 'Odontología General',   'Atención integral: diagnóstico, prevención y tratamientos básicos.', TRUE, NOW()),
('9f000000-0000-0000-0000-000000009002', 'Ortodoncia',            'Corrección de maloclusiones y alineación dental.',                  TRUE, NOW()),
('9f000000-0000-0000-0000-000000009003', 'Endodoncia',            'Tratamientos de conducto y patologías pulpar.',                      TRUE, NOW()),
('9f000000-0000-0000-0000-000000009004', 'Periodoncia',           'Tratamiento de encías y tejidos de soporte del diente.',            TRUE, NOW()),
('9f000000-0000-0000-0000-000000009005', 'Cirugía Oral',          'Extracciones, terceros molares y cirugía bucal.',                   TRUE, NOW()),
('9f000000-0000-0000-0000-000000009006', 'Odontopediatría',       'Salud dental infantil y adolescentes.',                             TRUE, NOW()),
('9f000000-0000-0000-0000-000000009007', 'Rehabilitación Oral',   'Coronas, puentes y prótesis.',                                      TRUE, NOW()),
('9f000000-0000-0000-0000-000000009008', 'Estética Dental',       'Blanqueamientos, carillas y diseño de sonrisa.',                    TRUE, NOW()),
('9f000000-0000-0000-0000-000000009009', 'Implantología Oral',    'Colocación de implantes dentales.',                                 TRUE, NOW()),
('9f000000-0000-0000-0000-000000009010', 'Prostodoncia',          'Prótesis fijas, removibles e implanto-soportadas.',                 TRUE, NOW());

-- ============================================================================
-- 3. REFERENCIAS
-- ============================================================================
-- Clinica  MimaCare Plus : 019e3dd6-0da4-7662-9a3a-9e6209398da6
-- Usuario clinica        : 019e3dd6-0ede-73c1-b25d-daca155aaa95 (mimacare)
-- Doctor estarlin        : 019fe959-15d4-7a72-9889-fdbf4629d1d1 (Cirugía Oral)
-- Doctor kgarcia         : 019e57df-b547-75dc-84cf-04b3ab8acc1c (Endodoncia)
-- Doctor sweet           : 019e3e44-5293-705d-9101-4592f16f60e9 (Ortodoncia)
-- Doctor wmedrano        : 019fe4ea-cd63-73ad-8361-36bfaa759f30 (Periodoncia)

-- ============================================================================
-- 4. PACIENTES (9) - historial odontológico
-- ============================================================================

INSERT INTO "Patients" ("Id", "ClinicId", "FirstName", "LastName", "Phone", "Address", "Allergies", "Notes", "CreatedAt") VALUES
('9f000000-0000-0000-0000-000000000001', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'María',  'Pérez Santos',   '8095550101', 'C/ Duarte #15, Los Prados, Santo Domingo',      'Penicilina', 'Hipertensión controlada - precaución con anestésicos con epinefrina. Terceros molares en evaluación.', NOW() - INTERVAL '120 days'),
('9f000000-0000-0000-0000-000000000002', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'José',   'Rodríguez Díaz', '8095550102', 'Av. Independencia #502, Gazcue, Santo Domingo', NULL,         'Bruxismo nocturno. Usa férula de descarga.',                                                          NOW() - INTERVAL '110 days'),
('9f000000-0000-0000-0000-000000000003', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'Ana',    'Torres Cruz',    '8095550103', 'C/ Máximo Gómez #89, Santo Domingo',            'Aspirina',   'Paciente pediátrica - acompañada por su madre. Flúor cada 6 meses.',                                  NOW() - INTERVAL '100 days'),
('9f000000-0000-0000-0000-000000000004', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'Luis',   'Gómez Reyes',    '8095550104', 'C/ José Martí #210, Herrera',                   NULL,         'Gingivitis - refiere sangrado al cepillarse. Endodoncia pieza 36 finalizada.',                        NOW() - INTERVAL '95 days'),
('9f000000-0000-0000-0000-000000000005', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'Carmen', 'Sánchez Matos',  '8095550105', 'Res. Palma Real, Apto 3B, Villa Mella',         NULL,         'Limpieza y revisión cada 6 meses. Corona pendiente pieza 36.',                                        NOW() - INTERVAL '90 days'),
('9f000000-0000-0000-0000-000000000006', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'Pedro',  'Ramírez Nova',   '8095550106', 'C/ Primera #33, Ens. Ozama',                    NULL,         'Extracción de tercer molar realizada. En control post-quirúrgico.',                                   NOW() - INTERVAL '85 days'),
('9f000000-0000-0000-0000-000000000007', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'Laura',  'Díaz Fernández', '8095550107', 'Av. 27 de Febrero #1000, Santo Domingo',        'Látex',      'Usar guantes sin látex. Tratamiento de ortodoncia en planificación.',                                 NOW() - INTERVAL '80 days'),
('9f000000-0000-0000-0000-000000000008', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'Miguel', 'Castillo Peña',  '8095550108', 'C/ Caracol #7, Los Alcarrizos',                 NULL,         'Caries múltiples - plan de tratamiento por fases.',                                                   NOW() - INTERVAL '75 days'),
('9f000000-0000-0000-0000-000000000009', '019e3dd6-0da4-7662-9a3a-9e6209398da6', 'Rosa',   'Flores Jiménez', '8095550109', 'C/ Restauración #45, Santo Domingo',            'Ibuprofeno', 'Ortodoncia en curso. Sensibilidad dental ocasional con fríos.',                                       NOW() - INTERVAL '70 days');

-- ============================================================================
-- 5. CITAS (19) - Estados: 1=Pending 2=Confirmed 3=Completed 4=Cancelled 5=Rejected
-- ============================================================================

-- ---- Dr. estarlin (Cirugía Oral) --------------------------------------------
INSERT INTO "Appointments" ("Id", "ClinicId", "PatientId", "DoctorId", "AppointmentDate", "StartTime", "EndTime", "Status", "Notes", "CreatedAt") VALUES
('9f000000-0000-0000-0000-000000000101', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000001', '019fe959-15d4-7a72-9889-fdbf4629d1d1', CURRENT_DATE - 21, '09:00', '09:45', 3, 'Evaluación quirúrgica. Radiografía panorámica: terceros molares inferiores retenidos.', NOW() - INTERVAL '22 days'),
('9f000000-0000-0000-0000-000000000102', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000006', '019fe959-15d4-7a72-9889-fdbf4629d1d1', CURRENT_DATE - 7,  '10:00', '10:45', 3, 'Extracción de tercer molar superior derecho bajo anestesia local. Sin complicaciones.', NOW() - INTERVAL '8 days'),
('9f000000-0000-0000-0000-000000000103', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000002', '019fe959-15d4-7a72-9889-fdbf4629d1d1', CURRENT_DATE - 3,  '11:00', '11:45', 4, 'Cancelada por el paciente - viaje de trabajo.',                                         NOW() - INTERVAL '10 days'),
('9f000000-0000-0000-0000-000000000104', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000006', '019fe959-15d4-7a72-9889-fdbf4629d1d1', CURRENT_DATE + 3,  '09:00', '09:30', 2, 'Control post-quirúrgico - retirar puntos.',                                             NOW() - INTERVAL '2 days'),
('9f000000-0000-0000-0000-000000000105', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000003', '019fe959-15d4-7a72-9889-fdbf4629d1d1', CURRENT_DATE + 5,  '08:30', '09:00', 1, 'Valoración odontopediátrica - primera cita.',                                           NOW() - INTERVAL '1 days'),

-- ---- Dra. kgarcia (Endodoncia) ----------------------------------------------
('9f000000-0000-0000-0000-000000000106', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000004', '019e57df-b547-75dc-84cf-04b3ab8acc1c', CURRENT_DATE - 30, '10:00', '11:00', 3, 'Pulpitis irreversible pieza 36. Se inicia endodoncia - sesión 1.',                      NOW() - INTERVAL '32 days'),
('9f000000-0000-0000-0000-000000000107', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000004', '019e57df-b547-75dc-84cf-04b3ab8acc1c', CURRENT_DATE - 14, '10:00', '11:00', 3, 'Endodoncia pieza 36 - sesión 2. Obturación definitiva con gutapercha.',                 NOW() - INTERVAL '15 days'),
('9f000000-0000-0000-0000-000000000108', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000005', '019e57df-b547-75dc-84cf-04b3ab8acc1c', CURRENT_DATE - 1,  '11:30', '12:15', 3, 'Obturación con resina pieza 46 (caries oclusal profunda).',                             NOW() - INTERVAL '3 days'),
('9f000000-0000-0000-0000-000000000109', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000005', '019e57df-b547-75dc-84cf-04b3ab8acc1c', CURRENT_DATE + 2,  '15:00', '15:45', 1, 'Corona metal-porcelana pieza 36 - toma de impresión.',                                   NOW() - INTERVAL '1 days'),
('9f000000-0000-0000-0000-000000000110', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000008', '019e57df-b547-75dc-84cf-04b3ab8acc1c', CURRENT_DATE + 7,  '16:00', '16:45', 2, 'Evaluación endodóntica pieza 21 - confirmada.',                                         NOW() - INTERVAL '2 days'),

-- ---- Dr. sweet (Ortodoncia) -------------------------------------------------
('9f000000-0000-0000-0000-000000000111', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000007', '019e3e44-5293-705d-9101-4592f16f60e9', CURRENT_DATE - 10, '09:30', '10:15', 3, 'Estudio de ortodoncia: fotografías, impresiones y cefalometría.',                       NOW() - INTERVAL '11 days'),
('9f000000-0000-0000-0000-000000000112', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000008', '019e3e44-5293-705d-9101-4592f16f60e9', CURRENT_DATE - 2,  '14:00', '14:45', 5, 'Rechazada: solicitada fuera del horario del especialista.',                             NOW() - INTERVAL '4 days'),
('9f000000-0000-0000-0000-000000000113', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000008', '019e3e44-5293-705d-9101-4592f16f60e9', CURRENT_DATE + 1,  '09:00', '09:45', 1, 'Instalación de brackets arco superior - reagendada.',                                   NOW() - INTERVAL '1 days'),
('9f000000-0000-0000-0000-000000000114', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000009', '019e3e44-5293-705d-9101-4592f16f60e9', CURRENT_DATE + 4,  '11:00', '11:30', 2, 'Activación mensual de brackets - confirmada.',                                          NOW() - INTERVAL '1 days'),
('9f000000-0000-0000-0000-000000000115', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000007', '019e3e44-5293-705d-9101-4592f16f60e9', CURRENT_DATE + 12, '10:30', '11:15', 1, 'Colocación de brackets - traer estudio completo y presupuesto aprobado.',               NOW() - INTERVAL '1 days'),

-- ---- Dr. wmedrano (Periodoncia) ---------------------------------------------
('9f000000-0000-0000-0000-000000000116', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000001', '019fe4ea-cd63-73ad-8361-36bfaa759f30', CURRENT_DATE - 5,  '08:00', '09:00', 3, 'Raspado y alisado radicular cuadrantes superiores. Sin contraindicaciones.',            NOW() - INTERVAL '6 days'),
('9f000000-0000-0000-0000-000000000117', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000006', '019fe4ea-cd63-73ad-8361-36bfaa759f30', CURRENT_DATE + 3,  '09:30', '10:15', 1, 'Evaluación periodontal - nuevo paciente.',                                              NOW() - INTERVAL '1 days'),
('9f000000-0000-0000-0000-000000000118', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '9f000000-0000-0000-0000-000000000009', '019fe4ea-cd63-73ad-8361-36bfaa759f30', CURRENT_DATE + 6,  '13:00', '13:45', 2, 'Control de bolsas periodontales - sondaje completo.',                                   NOW() - INTERVAL '2 days'),

-- ---- Paciente preexistente Juan German con estarlin -------------------------
('9f000000-0000-0000-0000-000000000119', '019e3dd6-0da4-7662-9a3a-9e6209398da6', '01a0301a-59de-7067-9eb7-17bec07823d2', '019fe959-15d4-7a72-9889-fdbf4629d1d1', CURRENT_DATE + 2,  '12:00', '12:30', 1, 'Valoración general y limpieza.',                                                        NOW());

-- ============================================================================
-- 6. HORARIOS DE DOCTORES (DayOfWeek: 0=Domingo .. 6=Sábado)
-- ============================================================================

-- estarlin: Lun-Vie 08:00-12:00 + Lun/Mie/Vie 14:00-18:00 (quirúrgico)
INSERT INTO "DoctorSchedules" ("Id", "DoctorId", "DayOfWeek", "StartTime", "EndTime", "IsActive") VALUES
('9f000000-0000-0000-0000-000000000201', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 1, '08:00', '12:00', TRUE),
('9f000000-0000-0000-0000-000000000202', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 2, '08:00', '12:00', TRUE),
('9f000000-0000-0000-0000-000000000203', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 3, '08:00', '12:00', TRUE),
('9f000000-0000-0000-0000-000000000204', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 4, '08:00', '12:00', TRUE),
('9f000000-0000-0000-0000-000000000205', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 5, '08:00', '12:00', TRUE),
('9f000000-0000-0000-0000-000000000206', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 1, '14:00', '18:00', TRUE),
('9f000000-0000-0000-0000-000000000207', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 3, '14:00', '18:00', TRUE),
('9f000000-0000-0000-0000-000000000208', '019fe959-15d4-7a72-9889-fdbf4629d1d1', 5, '14:00', '18:00', TRUE);

-- sweet: Lun-Sáb 09:00-17:00
INSERT INTO "DoctorSchedules" ("Id", "DoctorId", "DayOfWeek", "StartTime", "EndTime", "IsActive") VALUES
('9f000000-0000-0000-0000-000000000209', '019e3e44-5293-705d-9101-4592f16f60e9', 1, '09:00', '17:00', TRUE),
('9f000000-0000-0000-0000-000000000210', '019e3e44-5293-705d-9101-4592f16f60e9', 2, '09:00', '17:00', TRUE),
('9f000000-0000-0000-0000-000000000211', '019e3e44-5293-705d-9101-4592f16f60e9', 3, '09:00', '17:00', TRUE),
('9f000000-0000-0000-0000-000000000212', '019e3e44-5293-705d-9101-4592f16f60e9', 4, '09:00', '17:00', TRUE),
('9f000000-0000-0000-0000-000000000213', '019e3e44-5293-705d-9101-4592f16f60e9', 5, '09:00', '17:00', TRUE),
('9f000000-0000-0000-0000-000000000214', '019e3e44-5293-705d-9101-4592f16f60e9', 6, '09:00', '17:00', TRUE);

-- kgarcia: Mar-Sáb 10:00-18:00
INSERT INTO "DoctorSchedules" ("Id", "DoctorId", "DayOfWeek", "StartTime", "EndTime", "IsActive") VALUES
('9f000000-0000-0000-0000-000000000215', '019e57df-b547-75dc-84cf-04b3ab8acc1c', 2, '10:00', '18:00', TRUE),
('9f000000-0000-0000-0000-000000000216', '019e57df-b547-75dc-84cf-04b3ab8acc1c', 3, '10:00', '18:00', TRUE),
('9f000000-0000-0000-0000-000000000217', '019e57df-b547-75dc-84cf-04b3ab8acc1c', 4, '10:00', '18:00', TRUE),
('9f000000-0000-0000-0000-000000000218', '019e57df-b547-75dc-84cf-04b3ab8acc1c', 5, '10:00', '18:00', TRUE),
('9f000000-0000-0000-0000-000000000219', '019e57df-b547-75dc-84cf-04b3ab8acc1c', 6, '10:00', '18:00', TRUE);

-- wmedrano: Lun-Vie 07:00-15:00
INSERT INTO "DoctorSchedules" ("Id", "DoctorId", "DayOfWeek", "StartTime", "EndTime", "IsActive") VALUES
('9f000000-0000-0000-0000-000000000220', '019fe4ea-cd63-73ad-8361-36bfaa759f30', 1, '07:00', '15:00', TRUE),
('9f000000-0000-0000-0000-000000000221', '019fe4ea-cd63-73ad-8361-36bfaa759f30', 2, '07:00', '15:00', TRUE),
('9f000000-0000-0000-0000-000000000222', '019fe4ea-cd63-73ad-8361-36bfaa759f30', 3, '07:00', '15:00', TRUE),
('9f000000-0000-0000-0000-000000000223', '019fe4ea-cd63-73ad-8361-36bfaa759f30', 4, '07:00', '15:00', TRUE),
('9f000000-0000-0000-0000-000000000224', '019fe4ea-cd63-73ad-8361-36bfaa759f30', 5, '07:00', '15:00', TRUE);

-- ============================================================================
-- 7. RELACIÓN DOCTOR <-> ESPECIALIDADES (catálogo odontológico)
-- ============================================================================

-- Odontología General para todos los doctores
INSERT INTO "DoctorSpecialties" ("DoctorId", "SpecialtyId")
SELECT d."Id", s."Id"
FROM "Doctors" d
JOIN "AspNetUsers" u ON u."Id" = d."UserId"
JOIN "Specialties" s ON s."Name" = 'Odontología General'
WHERE u."Email" IN ('estarlin@gmail.com', 'sweet@gmail.com', 'kgarcia@gmail.com', 'wmedrano@gmail.com')
ON CONFLICT DO NOTHING;

-- estarlin: Cirugía Oral + Implante
INSERT INTO "DoctorSpecialties" ("DoctorId", "SpecialtyId")
SELECT d."Id", s."Id"
FROM "Doctors" d
JOIN "AspNetUsers" u ON u."Id" = d."UserId"
JOIN "Specialties" s ON s."Name" IN ('Cirugía Oral', 'Implantología Oral')
WHERE u."Email" = 'estarlin@gmail.com'
ON CONFLICT DO NOTHING;

-- sweet: Ortodoncia + Estética
INSERT INTO "DoctorSpecialties" ("DoctorId", "SpecialtyId")
SELECT d."Id", s."Id"
FROM "Doctors" d
JOIN "AspNetUsers" u ON u."Id" = d."UserId"
JOIN "Specialties" s ON s."Name" IN ('Ortodoncia', 'Estética Dental')
WHERE u."Email" = 'sweet@gmail.com'
ON CONFLICT DO NOTHING;

-- kgarcia: Endodoncia + Rehabilitación
INSERT INTO "DoctorSpecialties" ("DoctorId", "SpecialtyId")
SELECT d."Id", s."Id"
FROM "Doctors" d
JOIN "AspNetUsers" u ON u."Id" = d."UserId"
JOIN "Specialties" s ON s."Name" IN ('Endodoncia', 'Rehabilitación Oral')
WHERE u."Email" = 'kgarcia@gmail.com'
ON CONFLICT DO NOTHING;

-- wmedrano: Periodoncia + Prostodoncia
INSERT INTO "DoctorSpecialties" ("DoctorId", "SpecialtyId")
SELECT d."Id", s."Id"
FROM "Doctors" d
JOIN "AspNetUsers" u ON u."Id" = d."UserId"
JOIN "Specialties" s ON s."Name" IN ('Periodoncia', 'Prostodoncia')
WHERE u."Email" = 'wmedrano@gmail.com'
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 8. ARCHIVOS / HISTORIAL CLÍNICO DENTAL
-- ============================================================================

INSERT INTO "PatientFiles" ("Id", "PatientId", "UploadedByUserId", "FileName", "FileUrl", "ContentType", "UploadedAt") VALUES
('9f000000-0000-0000-0000-000000000301', '9f000000-0000-0000-0000-000000000001', '019e3dd6-0ede-73c1-b25d-daca155aaa95', 'plan-tratamiento-maria.pdf',      '/uploads/patients/plan-tratamiento-maria.pdf',      'application/pdf', NOW() - INTERVAL '20 days'),
('9f000000-0000-0000-0000-000000000302', '9f000000-0000-0000-0000-000000000004', '019e3dd6-0ede-73c1-b25d-daca155aaa95', 'radiografia-panoramica-luis.jpg', '/uploads/patients/radiografia-panoramica-luis.jpg', 'image/jpeg',      NOW() - INTERVAL '29 days'),
('9f000000-0000-0000-0000-000000000303', '9f000000-0000-0000-0000-000000000007', '019e3dd6-0ede-73c1-b25d-daca155aaa95', 'fotografia-intraoral-laura.jpg',  '/uploads/patients/fotografia-intraoral-laura.jpg',  'image/jpeg',      NOW() - INTERVAL '9 days'),
('9f000000-0000-0000-0000-000000000304', '9f000000-0000-0000-0000-000000000002', '019e3dd6-0ede-73c1-b25d-daca155aaa95', 'periapical-jose.pdf',             '/uploads/patients/periapical-jose.pdf',             'application/pdf', NOW() - INTERVAL '7 days');

COMMIT;

-- ============================================================================
-- VERIFICACIÓN RÁPIDA
-- ============================================================================
SELECT 'Pacientes sembrados' AS concepto, COUNT(*) AS total FROM "Patients" WHERE "Id"::text LIKE '9f000000-%'
UNION ALL SELECT 'Citas sembradas',           COUNT(*) FROM "Appointments"    WHERE "Id"::text LIKE '9f000000-%'
UNION ALL SELECT 'Horarios sembrados',        COUNT(*) FROM "DoctorSchedules" WHERE "Id"::text LIKE '9f000000-%'
UNION ALL SELECT 'Especialidades en catálogo', COUNT(*) FROM "Specialties"
UNION ALL SELECT 'Doctores con especialidad', COUNT(*) FROM "DoctorSpecialties"
UNION ALL SELECT 'Archivos sembrados',        COUNT(*) FROM "PatientFiles"    WHERE "Id"::text LIKE '9f000000-%';
