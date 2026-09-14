-- seed_planes.sql
-- Aplica por SQL la misma lógica idempotente de MimaCare.infrastructure\Seed\SubscriptionPlanSeeder:
--   1) garantiza los 3 planes por defecto
--   2) asigna plan a toda clínica (Básico) y Enterprise a la clínica demo mimacare@gmail.com
--   3) crea una sucursal por defecto para clínicas sin sucursales
--   4) vincula cada doctor a la sucursal de su clínica
--   5) backfill: atribuye las citas existentes a la sucursal de su clínica
-- Idempotente: se puede ejecutar varias veces.

INSERT INTO "SubscriptionPlans" ("Id","Name","Description","MaxBranches","MaxDoctors","AllowsUnlimitedAppointments","IsActive","CreatedAt")
VALUES
 ('c3a00000-0000-4000-8000-000000000001','Básico','1 sucursal y hasta 3 doctores, citas ilimitadas.',1,3,true,true,'2025-01-01 00:00:00+00'),
 ('c3a00000-0000-4000-8000-000000000002','Profesional','Hasta 3 sucursales y doctores ilimitados, citas ilimitadas.',3,NULL,true,true,'2025-01-01 00:00:01+00'),
 ('c3a00000-0000-4000-8000-000000000003','Enterprise','Sucursales y doctores ilimitados, citas ilimitadas, soporte custom.',NULL,NULL,true,true,'2025-01-01 00:00:02+00')
ON CONFLICT ("Id") DO NOTHING;

-- 2) Plan por clínica
UPDATE "Clinics" SET "PlanId" = 'c3a00000-0000-4000-8000-000000000001' WHERE "PlanId" IS NULL;

UPDATE "Clinics" c
SET "PlanId" = 'c3a00000-0000-4000-8000-000000000003'
FROM "AspNetUsers" u
WHERE u."ClinicId" = c."Id" AND u."Email" = 'mimacare@gmail.com';

-- 3) Sucursal por defecto
INSERT INTO "Branches" ("Id","ClinicId","Name","Address","Phone","IsActive","CreatedAt")
SELECT gen_random_uuid(), c."Id", 'Sucursal Principal', NULL, NULL, true, '2025-01-01 00:00:00+00'
FROM "Clinics" c
WHERE NOT EXISTS (SELECT 1 FROM "Branches" b WHERE b."ClinicId" = c."Id");

-- 4) Vincular doctores a la primera sucursal de su clínica
INSERT INTO "DoctorBranches" ("DoctorId","BranchId")
SELECT d."Id", fb."Id"
FROM "Doctors" d
JOIN LATERAL (
    SELECT b."Id" FROM "Branches" b
    WHERE b."ClinicId" = d."ClinicId"
    ORDER BY b."CreatedAt", b."Id"
    LIMIT 1
) fb ON true
ON CONFLICT ("DoctorId","BranchId") DO NOTHING;

-- 5) Backfill citas existentes -> sucursal de su clínica
UPDATE "Appointments" a
SET "BranchId" = fb."Id", "CreatedBranchId" = fb."Id"
FROM (
    SELECT DISTINCT ON (b."ClinicId") b."ClinicId", b."Id"
    FROM "Branches" b
    ORDER BY b."ClinicId", b."CreatedAt", b."Id"
) fb
WHERE a."ClinicId" = fb."ClinicId";