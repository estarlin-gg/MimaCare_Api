-- ============================================================
-- MimaCare :: Siembra directa por SQL (idempotente)
-- Requiere: los 4 doctores ya creados + catalogo de especialidades
-- ============================================================

-- 1. Especialidades por doctor
INSERT INTO "DoctorSpecialties" ("DoctorId", "SpecialtyId")
SELECT d."Id", s."Id"
FROM "Doctors" d
JOIN "AspNetUsers" u ON u."Id" = d."UserId"
CROSS JOIN (VALUES
    ('estarlin@gmail.com',   'Cirugía Oral'),
    ('estarlin@gmail.com',   'Implantología Oral'),
    ('sweet@gmail.com',      'Ortodoncia'),
    ('sweet@gmail.com',      'Estética Dental'),
    ('kgarcia@gmail.com',    'Endodoncia'),
    ('kgarcia@gmail.com',    'Rehabilitación Oral'),
    ('wmedrano@gmail.com',   'Periodoncia'),
    ('wmedrano@gmail.com',   'Prostodoncia')
) AS x(email, spec)
JOIN "Specialties" s ON s."Name" = x.spec
WHERE u."Email" = x.email
ON CONFLICT DO NOTHING;

-- 2. Horarios
INSERT INTO "DoctorSchedules" ("Id", "DoctorId", "DayOfWeek", "StartTime", "EndTime", "IsActive")
SELECT x.sid::uuid, d."Id", x.dow::int, x.st::time, x.et::time, true
FROM "Doctors" d
JOIN "AspNetUsers" u ON u."Id" = d."UserId"
CROSS JOIN (VALUES
    ('c1000001-0000-0000-0000-000000000001','estarlin@gmail.com', 1,'08:00','12:00'),
    ('c1000002-0000-0000-0000-000000000002','estarlin@gmail.com', 2,'08:00','12:00'),
    ('c1000003-0000-0000-0000-000000000003','estarlin@gmail.com', 3,'08:00','12:00'),
    ('c1000004-0000-0000-0000-000000000004','estarlin@gmail.com', 4,'08:00','12:00'),
    ('c1000005-0000-0000-0000-000000000005','estarlin@gmail.com', 5,'08:00','12:00'),
    ('c1000006-0000-0000-0000-000000000006','estarlin@gmail.com', 1,'14:00','18:00'),
    ('c1000007-0000-0000-0000-000000000007','estarlin@gmail.com', 3,'14:00','18:00'),
    ('c1000008-0000-0000-0000-000000000008','estarlin@gmail.com', 5,'14:00','18:00'),
    ('c1000009-0000-0000-0000-000000000009','sweet@gmail.com',    1,'09:00','17:00'),
    ('c1000010-0000-0000-0000-000000000010','sweet@gmail.com',    2,'09:00','17:00'),
    ('c1000011-0000-0000-0000-000000000011','sweet@gmail.com',    3,'09:00','17:00'),
    ('c1000012-0000-0000-0000-000000000012','sweet@gmail.com',    4,'09:00','17:00'),
    ('c1000013-0000-0000-0000-000000000013','sweet@gmail.com',    5,'09:00','17:00'),
    ('c1000014-0000-0000-0000-000000000014','sweet@gmail.com',    6,'09:00','17:00'),
    ('c1000015-0000-0000-0000-000000000015','kgarcia@gmail.com',  2,'10:00','18:00'),
    ('c1000016-0000-0000-0000-000000000016','kgarcia@gmail.com',  3,'10:00','18:00'),
    ('c1000017-0000-0000-0000-000000000017','kgarcia@gmail.com',  4,'10:00','18:00'),
    ('c1000018-0000-0000-0000-000000000018','kgarcia@gmail.com',  5,'10:00','18:00'),
    ('c1000019-0000-0000-0000-000000000019','kgarcia@gmail.com',  6,'10:00','18:00'),
    ('c1000020-0000-0000-0000-000000000020','wmedrano@gmail.com', 1,'07:00','15:00'),
    ('c1000021-0000-0000-0000-000000000021','wmedrano@gmail.com', 2,'07:00','15:00'),
    ('c1000022-0000-0000-0000-000000000022','wmedrano@gmail.com', 3,'07:00','15:00'),
    ('c1000023-0000-0000-0000-000000000023','wmedrano@gmail.com', 4,'07:00','15:00'),
    ('c1000024-0000-0000-0000-000000000024','wmedrano@gmail.com', 5,'07:00','15:00')
) AS x(sid, email, dow, st, et)
WHERE u."Email" = x.email
ON CONFLICT ("Id") DO NOTHING;

-- 3. Pacientes
INSERT INTO "Patients" ("Id", "ClinicId", "FirstName", "LastName", "Phone", "Address", "Allergies", "Notes", "CreatedAt")
SELECT x.pid::uuid,
       (SELECT "ClinicId" FROM "AspNetUsers" WHERE "Email" = 'mimacare@gmail.com'),
       x.fn, x.ln, x.ph, x.ad, x.al, x.nt, NOW()
FROM (VALUES
    ('aa000000-0000-0000-0000-000000000001','María', 'Pérez Santos',  '8095550101','C/ Duarte #15, Los Prados','Penicilina','Hipertensión controlada - precaución anestésicos con epinefrina.'),
    ('aa000000-0000-0000-0000-000000000002','José',  'Rodríguez Díaz','8095550102','Av. Independencia #502, Gazcue',NULL,'Bruxismo nocturno. Usa férula de descarga.'),
    ('aa000000-0000-0000-0000-000000000003','Ana',   'Torres Cruz',   '8095550103','C/ Máximo Gómez #89','Aspirina','Paciente pediátrica - acompañada por su madre.'),
    ('aa000000-0000-0000-0000-000000000004','Luis',  'Gómez Reyes',   '8095550104','C/ José Martí #210, Herrera',NULL,'Gingivitis - sangrado al cepillarse. Endodoncia pieza 36 finalizada.'),
    ('aa000000-0000-0000-0000-000000000005','Carmen','Sánchez Matos', '8095550105','Res. Palma Real Apto 3B, Villa Mella',NULL,'Limpieza y revisión cada 6 meses.'),
    ('aa000000-0000-0000-0000-000000000006','Pedro', 'Ramírez Nova',  '8095550106','C/ Primera #33, Ens. Ozama',NULL,'Post-extracción tercer molar en seguimiento.'),
    ('aa000000-0000-0000-0000-000000000007','Laura', 'Díaz Fernández','8095550107','Av. 27 de Febrero #1000','Látex','Usar guantes sin látex. Ortodoncia en planificación.'),
    ('aa000000-0000-0000-0000-000000000008','Miguel','Castillo Peña', '8095550108','C/ Caracol #7, Los Alcarrizos',NULL,'Caries múltiples - plan por fases.'),
    ('aa000000-0000-0000-0000-000000000009','Rosa',  'Flores Jiménez','8095550109','C/ Restauración #45','Ibuprofeno','Ortodoncia en curso. Sensibilidad con fríos.'),
    ('aa000000-0000-0000-0000-000000000010','Sofía', 'Valdez Polanco','8095550110','C/ El Sol #4, Naco',NULL,'Paciente nueva - interesada en blanqueamiento.')
) AS x(pid, fn, ln, ph, ad, al, nt)
ON CONFLICT ("Id") DO NOTHING;

-- 4. Citas (fechas resueltas a dias laborables de cada doctor)
INSERT INTO "Appointments" ("Id", "ClinicId", "DoctorId", "PatientId", "AppointmentDate", "StartTime", "EndTime", "Status", "Notes", "CreatedAt")
SELECT x.aid::uuid,
       d."ClinicId",
       d."Id",
       p."Id",
       x.fecha::date, x.hi::time, x.hf::time, x.status::int, x.nota, NOW()
FROM (VALUES
    ('bb000000-0000-0000-0000-000000000001','estarlin@gmail.com','8095550101','2026-08-07','09:00','09:45',3,'Evaluación quirúrgica. Panorámica: terceros molares inferiores retenidos.'),
    ('bb000000-0000-0000-0000-000000000002','estarlin@gmail.com','8095550106','2026-08-14','10:00','10:45',3,'Extracción de tercer molar superior derecho. Sin complicaciones.'),
    ('bb000000-0000-0000-0000-000000000003','estarlin@gmail.com','8095550102','2026-08-20','11:00','11:30',4,'Valoración para exodoncia múltiple.'),
    ('bb000000-0000-0000-0000-000000000004','estarlin@gmail.com','8095550106','2026-08-26','09:00','09:30',2,'Control post-quirúrgico - retirar puntos.'),
    ('bb000000-0000-0000-0000-000000000005','estarlin@gmail.com','8095550110','2026-08-31','14:00','15:00',1,'Cirugía oral bajo sedación - premedicación indicada.'),
    ('bb000000-0000-0000-0000-000000000006','kgarcia@gmail.com','8095550104','2026-08-01','10:00','11:00',3,'Pulpitis irreversible pieza 36 - endodoncia sesión 1.'),
    ('bb000000-0000-0000-0000-000000000007','kgarcia@gmail.com','8095550104','2026-08-11','10:00','11:00',3,'Endodoncia pieza 36 - sesión 2, obturación con gutapercha.'),
    ('bb000000-0000-0000-0000-000000000008','kgarcia@gmail.com','8095550105','2026-08-21','15:00','15:45',3,'Obturación con resina pieza 46 (caries oclusal).'),
    ('bb000000-0000-0000-0000-000000000009','kgarcia@gmail.com','8095550109','2026-08-25','16:00','16:45',2,'Urgencia: dolor agudo pieza 21.'),
    ('bb000000-0000-0000-0000-000000000010','kgarcia@gmail.com','8095550108','2026-09-01','11:00','11:45',1,'Evaluación endodóntica pieza 46.'),
    ('bb000000-0000-0000-0000-000000000011','sweet@gmail.com','8095550107','2026-08-13','09:30','10:15',3,'Estudio de ortodoncia: fotografías, impresiones y cefalometría.'),
    ('bb000000-0000-0000-0000-000000000012','sweet@gmail.com','8095550109','2026-08-18','11:00','11:45',3,'Instalación de brackets arco superior.'),
    ('bb000000-0000-0000-0000-000000000013','sweet@gmail.com','8095550103','2026-08-22','09:00','09:45',5,'Ortodoncia interceptiva infantil.'),
    ('bb000000-0000-0000-0000-000000000014','sweet@gmail.com','8095550109','2026-08-27','11:00','11:30',2,'Activación mensual de brackets.'),
    ('bb000000-0000-0000-0000-000000000015','sweet@gmail.com','8095550107','2026-09-04','10:30','11:15',1,'Colocación de brackets - presupuesto aprobado.'),
    ('bb000000-0000-0000-0000-000000000016','wmedrano@gmail.com','8095550101','2026-08-17','08:00','09:00',3,'Raspado y alisado radicular cuadrantes superiores.'),
    ('bb000000-0000-0000-0000-000000000017','wmedrano@gmail.com','8095550105','2026-08-21','13:00','13:45',3,'Mantenimiento periodontal.'),
    ('bb000000-0000-0000-0000-000000000018','wmedrano@gmail.com','8095550109','2026-08-31','08:00','08:45',2,'Control de bolsas periodontales - sondaje.'),
    ('bb000000-0000-0000-0000-000000000019','wmedrano@gmail.com','8095550102','2026-09-02','09:00','09:45',1,'Evaluación para injerto gingival.'),
    ('bb000000-0000-0000-0000-000000000020','wmedrano@gmail.com','8095550110','2026-08-28','12:00','12:30',1,'Limpieza y valoración general.')
) AS x(aid, email, tel, fecha, hi, hf, status, nota)
JOIN "Doctors" d ON d."Id" = (SELECT dd."Id" FROM "Doctors" dd JOIN "AspNetUsers" uu ON uu."Id" = dd."UserId" WHERE uu."Email" = x.email)
JOIN "Patients" p ON p."Phone" = x.tel AND p."ClinicId" = d."ClinicId"
ON CONFLICT ("Id") DO NOTHING;
