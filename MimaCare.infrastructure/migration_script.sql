CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "AspNetRoles" (
        "Id" TEXT NOT NULL,
        "Name" TEXT,
        "NormalizedName" TEXT,
        "ConcurrencyStamp" TEXT,
        CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "Clinics" (
        "Id" TEXT NOT NULL,
        "Name" TEXT,
        "Email" TEXT,
        "Phone" TEXT,
        "Address" TEXT,
        "LogoUrl" TEXT,
        "IsActive" INTEGER NOT NULL,
        "CreatedAt" TEXT NOT NULL,
        CONSTRAINT "PK_Clinics" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "AspNetRoleClaims" (
        "Id" INTEGER NOT NULL,
        "RoleId" TEXT NOT NULL,
        "ClaimType" TEXT,
        "ClaimValue" TEXT,
        CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "AspNetUsers" (
        "Id" TEXT NOT NULL,
        "FirstName" TEXT,
        "LastName" TEXT,
        "ClinicId" TEXT NOT NULL,
        "IsActive" INTEGER NOT NULL,
        "CreatedAt" TEXT NOT NULL,
        "UserName" TEXT,
        "NormalizedUserName" TEXT,
        "Email" TEXT,
        "NormalizedEmail" TEXT,
        "EmailConfirmed" INTEGER NOT NULL,
        "PasswordHash" TEXT,
        "SecurityStamp" TEXT,
        "ConcurrencyStamp" TEXT,
        "PhoneNumber" TEXT,
        "PhoneNumberConfirmed" INTEGER NOT NULL,
        "TwoFactorEnabled" INTEGER NOT NULL,
        "LockoutEnd" TEXT,
        "LockoutEnabled" INTEGER NOT NULL,
        "AccessFailedCount" INTEGER NOT NULL,
        CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AspNetUsers_Clinics_ClinicId" FOREIGN KEY ("ClinicId") REFERENCES "Clinics" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "Patients" (
        "Id" TEXT NOT NULL,
        "ClinicId" TEXT NOT NULL,
        "FirstName" TEXT,
        "LastName" TEXT,
        "Phone" TEXT,
        "Address" TEXT,
        "Allergies" TEXT,
        "CreatedAt" TEXT NOT NULL,
        CONSTRAINT "PK_Patients" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Patients_Clinics_ClinicId" FOREIGN KEY ("ClinicId") REFERENCES "Clinics" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "AspNetUserClaims" (
        "Id" INTEGER NOT NULL,
        "UserId" TEXT NOT NULL,
        "ClaimType" TEXT,
        "ClaimValue" TEXT,
        CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "AspNetUserLogins" (
        "LoginProvider" TEXT NOT NULL,
        "ProviderKey" TEXT NOT NULL,
        "ProviderDisplayName" TEXT,
        "UserId" TEXT NOT NULL,
        CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
        CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "AspNetUserRoles" (
        "UserId" TEXT NOT NULL,
        "RoleId" TEXT NOT NULL,
        CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
        CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "AspNetUserTokens" (
        "UserId" TEXT NOT NULL,
        "LoginProvider" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Value" TEXT,
        CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
        CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "Doctors" (
        "Id" TEXT NOT NULL,
        "UserId" TEXT NOT NULL,
        "ClinicId" TEXT NOT NULL,
        "Specialty" TEXT,
        "LicenseNumber" TEXT,
        "ProfileImageUrl" TEXT,
        "IsAvailable" INTEGER NOT NULL,
        "CreatedAt" TEXT NOT NULL,
        CONSTRAINT "PK_Doctors" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Doctors_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Doctors_Clinics_ClinicId" FOREIGN KEY ("ClinicId") REFERENCES "Clinics" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "PatientFiles" (
        "Id" TEXT NOT NULL,
        "PatientId" TEXT NOT NULL,
        "UploadedByUserId" TEXT NOT NULL,
        "FileName" TEXT,
        "FileUrl" TEXT,
        "ContentType" TEXT,
        "UploadedAt" TEXT NOT NULL,
        CONSTRAINT "PK_PatientFiles" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PatientFiles_AspNetUsers_UploadedByUserId" FOREIGN KEY ("UploadedByUserId") REFERENCES "AspNetUsers" ("Id"),
        CONSTRAINT "FK_PatientFiles_Patients_PatientId" FOREIGN KEY ("PatientId") REFERENCES "Patients" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE TABLE "Appointments" (
        "Id" TEXT NOT NULL,
        "ClinicId" TEXT NOT NULL,
        "PatientId" TEXT NOT NULL,
        "DoctorId" TEXT NOT NULL,
        "AppointmentDate" TEXT NOT NULL,
        "StartTime" TEXT,
        "EndTime" TEXT,
        "Status" INTEGER NOT NULL,
        "Notes" TEXT,
        "CreatedAt" TEXT NOT NULL,
        CONSTRAINT "PK_Appointments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Appointments_Clinics_ClinicId" FOREIGN KEY ("ClinicId") REFERENCES "Clinics" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Appointments_Doctors_DoctorId" FOREIGN KEY ("DoctorId") REFERENCES "Doctors" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_Appointments_Patients_PatientId" FOREIGN KEY ("PatientId") REFERENCES "Patients" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_ClinicId" ON "Appointments" ("ClinicId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_DoctorId" ON "Appointments" ("DoctorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_Appointments_PatientId" ON "Appointments" ("PatientId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_AspNetUsers_ClinicId" ON "AspNetUsers" ("ClinicId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_Doctors_ClinicId" ON "Doctors" ("ClinicId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Doctors_UserId" ON "Doctors" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_PatientFiles_PatientId" ON "PatientFiles" ("PatientId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_PatientFiles_UploadedByUserId" ON "PatientFiles" ("UploadedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    CREATE INDEX "IX_Patients_ClinicId" ON "Patients" ("ClinicId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260523164337_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260523164337_InitialCreate', '10.0.7');
    END IF;
END $EF$;
COMMIT;

