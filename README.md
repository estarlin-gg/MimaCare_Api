# MimaCare API

Backend de **MimaCare**, un sistema para clínicas odontológicas que permite gestionar clínicas, médicos (doctores), pacientes, especialidades, horarios de atención y citas, con la lógica de negocio del consultorio (validación de horarios, solapamientos de citas y cambios de estado) resuelta del lado del servidor.

- **Stack:** .NET 10 (ASP.NET Core Web API) · Entity Framework Core 10 · PostgreSQL (Npgsql) · ASP.NET Core Identity · JWT Bearer
- **Librerías clave:** FluentValidation (validación), Mapster (mapeo entidad→DTO), Scalar (documentación OpenAPI)
- **Arquitectura:** Clean Architecture en capas (Dominio → Aplicación → Infraestructura → Presentación)

---

## Características principales

- **Multi-clínica con aislamiento de datos**: cada clínica registrada administra sus propios doctores, pacientes, citas y horarios. Un usuario solo puede operar sobre los datos de la clínica a la que pertenece (aislamiento por claim `clinicId` en el JWT).
- **Planes de suscripción por clínica**: toda clínica tiene un plan obligatorio (`Básico`, `Profesional`, `Enterprise`, o planes custom creados por `Super`). Los límites se aplican sobre sucursales y doctores; las citas son ilimitadas.
- **Multi-sucursal**: cada clínica administra sucursales. Las sucursales se limitan según el plan (Básico = 1, Profesional = 3, Enterprise = ilimitadas). Los doctores pueden trabajar en **varias sucursales** (relación N–N).
- **Citas con sucursal**: cada cita registra la sucursal donde se atiende (`BranchId`) y la sucursal donde se creó (`CreatedBranchId`, por defecto igual a la de atención). Se valida que el doctor trabaje en la sucursal de la cita.
- **Roles**: `Clinic` (administrador de la clínica), `Doctor` (médico), `Super` (rol global/admin). Los endpoints se protegen por rol.
- **Autenticación completa**: registro de clínica, login, cambio de contraseña, "olvidé mi contraseña", reset de contraseña (autoservicio y por administración), y **logout con revocación de tokens** (blacklist por `jti`).
- **Gestión de doctores**: crear (con o sin contraseña; por defecto `123456`), actualizar, eliminar, consultar, resetear contraseña, asignar especialidades y configurar horarios por día de la semana.
- **Catálogo de especialidades odontológicas** (se siembra automáticamente) y asignación de especialidades por doctor.
- **Citas con reglas de negocio**:
  - Se valida que el doctor trabaje ese día y dentro de su horario (si tiene horario configurado; sin horario = disponibilidad libre).
  - No se permiten citas **solapadas** del mismo doctor (las canceladas/rechazadas no bloquean).
  - Estados: `Pending (1)`, `Confirmed (2)`, `Completed (3)`, `Cancelled (4)`, `Rejected (5)`.
  - Reasignación de cita a otro doctor, edición de cita y consultas por doctor/paciente/clínica o "mis citas" para el doctor.
- **Paginación y búsqueda** en todos los endpoints de listado (`Page`, `PageSize` máx. 100, por defecto 10; `Search` en doctores y pacientes).
- **Respuestas uniformes** `{ statusCode, message, data, success }` gracias a un middleware global de excepciones.

---

## Arquitectura

| Capa | Proyecto | Carpeta | Responsabilidad |
|---|---|---|---|
| **Dominio** | `MimaCare.Domain` | `MimaCare.Entities/` | Entidades (`Clinic`, `SubscriptionPlan`, `Branch`, `Doctor`, `Patient`, `Appointment`, `DoctorSchedule`, `DoctorSpecialty`, `DoctorBranch`, `Specialty`, `PatientFile`, `RevokedToken`, `ApplicationUser`), enums (`AppointmentStatus`) e interfaces de repositorios. |
| **Aplicación** | `MimaCare.Application` | `MimaCare.Application/` | DTOs, servicios de negocio (`*Service`), validadores FluentValidation (`Validators/`), mapeos Mapster (`Mappers/`) y excepciones de dominio (`Common/Exceptions`). |
| **Infraestructura** | `MimaCare.Infrastructure` | `MimaCare.infrastructure/` | `MimaCareContext` (DbContext), repositorios EF Core, `UnitOfWork`, migraciones, constantes (`Roles`) y seeders (roles, especialidades, planes/sucursales). |
| **Presentación** | `MimaCare.Presentation` | `MimaCare.Presentation/` | Controladores (API), middleware de excepciones y `Program.cs` (arranque, DI, JWT, migraciones). |

> Nota: la carpeta `MimaCare.Entities/` contiene el proyecto **`MimaCare.Domain`** (los namespaces usan `MimaCare.Domain.*`), y la carpeta `MimaCare.infrastructure/` contiene el proyecto `MimaCare.Infrastructure`.

---

## Modelo de datos

Relaciones principales:

- `Clinic` 1—N `ApplicationUser`, `Doctor`, `Patient`, `Appointment`, `Branch`.
- `SubscriptionPlan` 1—N `Clinic` (`Clinic.PlanId`); toda clínica tiene plan.
- `Clinic` 1—N `Branch` (sucursales); `Doctor` N—N `Branch` (tabla puente `DoctorBranch`).
- `Appointment` → `Branch` (sucursal de **atención**) y → `Branch` (sucursal de **creación**), vía `BranchId` / `CreatedBranchId`; `Patient` puede atenderse en cualquier sucursal de la clínica.
- `ApplicationUser` (Identity, GUID) 1—1 `Doctor` (por `Doctor.UserId`).
- `Doctor` N—N `Specialty` (tabla puente `DoctorSpecialty`).
- `Doctor` 1—N `DoctorSchedule` (bloques por `DayOfWeek` con `StartTime`/`EndTime`).
- `Doctor` 1—N `Appointment`; `Patient` 1—N `Appointment`; `Patient` 1—N `PatientFile` (expedientes/carillas subidas por un usuario).
- `RevokedToken` guarda el `Jti` de los JWT revocados en logout.

---

## Autenticación y seguridad

- **Registro** (`register-clinic`): crea la clínica **y** su usuario administrador con rol `Clinic` de forma transaccional, además de asignarle el plan **Básico** por defecto y crear su **sucursal principal** inicial.
- **Login**: valida credenciales (y que la cuenta esté **activa**), emite un JWT con claims `Sub`, `Email`, `NameIdentifier`, `clinicId`, `Jti` y los roles del usuario. Expira a los **2 meses**. Devuelve `{ fullName, clinicName, role, token, message }`.
- **JWT**: firmado con HMAC-SHA256 usando `Jwt:Key`; valida lifetime sin tolerancia de reloj (`ClockSkew = 0`). No usa HTTPS redirect (se eliminó: el 307 hacía perder el header `Authorization`).
- **Logout / revocación**: al hacer logout se guarda el `jti` del token en `RevokedTokens`; cada request valida en `OnTokenValidated` que el token no haya sido revocado.
- **Contraseñas**:
  - Política Identity mínima: largo mínimo 6, sin requisitos de mayúsculas/dígitos/símbolos.
  - `change-password` (autenticado): cambia la contraseña con la actual.
  - `forgot-password` + `reset-password` (flujo por token): como no hay proveedor de email configurado, el token se devuelve **solo en Development** (`PasswordResetOptions.ExposeResetToken`).
  - `reset-password/{userId}` (Clinic/Super): reseteo administrativo **solo de usuarios de la misma clínica**.
  - `PUT api/Doctor/{id}/reset-password` (Clinic/Super): fuerza la contraseña `123456` y **desbloquea** la cuenta (limpia `AccessFailedCount`/`LockoutEnd`).

---

## Lógica de negocio de citas (`AppointmentService`)

1. **Crear cita** (`POST api/Appointment`):
   - El paciente **debe** pertenecer a la clínica.
   - `branchId` es **obligatorio** y debe ser una sucursal de la clínica; el doctor de la cita **debe trabajar en esa sucursal** (relación `DoctorBranch`). `createdBranchId` es opcional (por defecto = `branchId`).
   - Rol `Clinic`/`Super` debe indicar `doctorId`; rol `Doctor` crea la cita sobre su propio perfil (resuelto por `UserId` vía JWT).
   - Se valida el **horario del doctor** si existe: el día de la cita debe ser laboral (por `DayOfWeek`) y la franja debe estar dentro del bloque. Sin horario configurado = "open schedule" (sin restricción).
   - Se rechaza con `409` si el doctor ya tiene una cita **solapada** (misma fecha y franjas que se cruzan), ignorando citas Cancelled/Rejected.
   - La cita nace siempre con estado `Pending`.
2. **Cambiar estado** (`PUT {id}/status`): un doctor solo puede cambiar el estado de **sus propias** citas.
3. **Editar cita** (`PUT {id}`): re-valida paciente, horario y solapamiento (excluyendo la cita en curso).
4. **Asignar doctor** (`PUT {id}/assign-doctor`, solo Clinic/Super): reasigna la cita validando el horario del doctor destino y que este trabaje en la sucursal de la cita.
5. **Consultas**: todas por clínica con paginación; `my` devuelve las citas del doctor autenticado (o las de su clínica si el rol es Clinic/Super).

---

## API Endpoints

Convención: `Bearer` token en el header `Authorization`. Los roles entre paréntesis restringen el acceso.

### Auth — `api/Auth`

| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| POST | `/register-clinic` | Público | Registra clínica + usuario `Clinic` |
| POST | `/login` | Público | Login, devuelve JWT |
| PUT | `/change-password` | Autenticado | Cambia contraseña (con la actual) |
| POST | `/forgot-password` | Público | Genera token de reset (expuesto solo en Development) |
| POST | `/reset-password` | Público | Aplica el token + nueva contraseña |
| PUT | `/reset-password/{userId}` | Clinic, Super | Reset administrativo (misma clínica) |
| POST | `/logout` | Autenticado | Revoca el token actual |

### Doctores — `api/Doctor` (acceso Clinic/Super en todo el controlador)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/` | Lista paginada (+`search` por nombre/email/teléfono/especialidad/licencia) |
| GET | `/specialties` | Catálogo de especialidades |
| POST | `/` | Crea doctor (password opcional, default `123456`) |
| GET | `/{doctorId}` | Detalle de doctor |
| PUT | `/{id}` | Actualiza doctor |
| DELETE | `/{id}` | Elimina doctor |
| PUT | `/{id}/reset-password` | Resetea a `123456` y desbloquea |

### Pacientes — `api/Patient` (autenticado)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/` | Lista paginada de pacientes de la clínica |
| GET | `/{id}` | Detalle |
| POST | `/` | Crea paciente |
| PUT | `/{id}` | Actualiza |
| DELETE | `/{id}` | Elimina |

### Citas — `api/Appointment` (autenticado)

| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| GET | `/` | — | Todas las citas de la clínica (paginadas) |
| GET | `/my` | — | Citas del doctor autenticado (o de la clínica) |
| GET | `/{id}` | — | Detalle |
| POST | `/` | — | Crea cita (reglas de horario/solapamiento) |
| PUT | `/{id}` | — | Edita cita |
| PUT | `/{id}/status` | — | Cambia estado (doctor solo sus citas) |
| PUT | `/{id}/assign-doctor` | Clinic, Super | Reasigna doctor |
| GET | `/by-doctor/{doctorId}` | — | Citas de un doctor (misma clínica) |
| GET | `/by-patient/{patientId}` | — | Citas de un paciente (misma clínica) |

### Planes de suscripción — `api/SubscriptionPlan`

| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| GET | `/` | Clinic, Super | Catálogo de planes activos |
| GET | `/my` | Clinic, Super | Plan de mi clínica + uso actual (`plan`, `branchCount`, `doctorCount`, `canCreateBranch`, `canCreateDoctor`) |
| GET | `/clinic/{clinicId}/usage` | Super | Uso de plan de **cualquier** clínica |
| POST | `/` | Super | Crea un plan (`maxBranches`/`maxDoctors` nulos = ilimitado) |
| PUT | `/{planId}` | Super | Actualiza un plan |
| PUT | `/{clinicId}/assign` | Super | Asigna un plan a una clínica (`{ planId }`) |

### Sucursales — `api/Branch` (Clinic/Super)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/` | Sucursales de mi clínica |
| POST | `/` | Crea sucursal (valida límite de sucursales según plan) |
| PUT | `/{branchId}` | Actualiza sucursal |
| DELETE | `/{branchId}` | Elimina (bloqueado si tiene citas asociadas) |

### Sucursales por doctor — `api/doctors/{doctorId}/branches` (Clinic/Super)

| Método | Descripción |
|---|---|
| GET | Sucursales donde trabaja el doctor |
| PUT | Reemplaza la asignación (`{ branchIds: [uuid…] }`) — valida que pertenezcan a la clínica |

Los planes por defecto: **Básico** (1 sucursal, 3 doctores), **Profesional** (3 sucursales, doctores ilimitados) y **Enterprise** (ilimitado). Al crear un doctor se puede indicar opcionalmente `branchIds` para asignarlo a sus sucursales iniciales.

### Especialidades — `api/Specialty` (Clinic/Super)

GET `/` · POST `/` · GET `/{id}` · PUT `/{id}` · DELETE `/{id}`

### Horarios — `api/doctors/{doctorId}/schedule`

| Método | Acceso | Descripción |
|---|---|---|
| GET | Autenticado | Horarios del doctor |
| PUT | Clinic, Super | Reemplaza la parrilla (cuerpo: array de `{ dayOfWeek, startTime, endTime, isActive }`) |
| DELETE | Clinic, Super | Limpia horarios → doctor en "open schedule" |

### Especialidades por doctor — `api/doctors/{doctorId}/specialties`

| Método | Acceso | Descripción |
|---|---|---|
| GET | Autenticado | Especialidades del doctor |
| PUT | Clinic, Super | Asigna especialidades (`{ specialtyIds: [uuid…] }`) |

---

## Paginación y formato de respuesta

- Parámetros query en los endpoints de listado: `page` (default 1), `pageSize` (default 10, máx 100) y `search` (doctores/pacientes).
- Respuesta paginada en `data`: `{ items, page, pageSize, totalCount, totalPages }`.
- Envoltorio de respuestas (controladores `BaseController` y middleware):

```jsonc
// Éxito
{ "statusCode": 200, "message": "Success", "data": { ... }, "success": false }

// Error (middleware global)
{ "statusCode": 400, "message": "...", "data": null, "success": false }
```

> Los endpoints del módulo Auth devuelven el DTO directamente (`AuthResponseDto`, p. ej. `{ fullName, clinicName, role, token, message }`).

Días de semana (horarios/citas): `DayOfWeek` como `0 = Sunday … 6 = Saturday`.

---

## Configuración

La configuración se resuelve por capas (ASP.NET Core: `appsettings.json` → `appsettings.{Environment}.json` → variables de entorno, con separador `__`).

**`appsettings.json` no contiene credenciales** (van vacías), para que nunca se filtren al repositorio:

```jsonc
{
  "ConnectionStrings": { "DefaultConnection": "" },
  "Jwt": { "Key": "" },
  "AllowedHosts": "*"
}
```

| Entorno | Origen de `ConnectionStrings:DefaultConnection` y `Jwt:Key` |
|---|---|
| **Desarrollo local** (`dotnet run`) | `appsettings.Development.json` (PostgreSQL local `localhost:5432`) |
| **Desarrollo con Docker** | `.env` local (nunca commiteado) → interpola el `docker-compose.yml` |
| **Producción (Supabase)** | **Variables de entorno** de la plataforma de despliegue (no del repo) |

### Producción: Supabase (PostgreSQL gestionado)

La meta es usar **[Supabase](https://supabase.com)** como base de datos en producción; el de desarrollo/local se mantiene como está (PostgreSQL local o el contenedor de Docker). Como Supabase es PostgreSQL, la API funciona igual: solo cambia la cadena de conexión (y los parámetros de seguridad).

En la plataforma de despliegue (Railway, Render, Azure, etc.) define:

- `ConnectionStrings__DefaultConnection` — cadena de conexión de Supabase (conexión directa o pooler, puerto 6543 en modo pooler), por ejemplo:
  `Host=aws-0-xx-xx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<db-password>;SSL Mode=Require;Trust Server Certificate=true`
- `Jwt__Key` — el **JWT Secret** del proyecto Supabase (Configuración → API → JWT Secret). Si en el futuro se autentican usuarios con Supabase Auth, se usará esa misma clave para validar los tokens.
- (Opcional) `Supabase__Url` y `Supabase__AnonKey` para integrar el SDK de Supabase.

Consideraciones para producción:
- La migraciones se aplican automáticamente al arrancar (`MigrateAsync`); se recomienda correr `dotnet ef database update` en el proceso de deploy para no abrir la app con migraciones pendientes.
- `ASPNETCORE_ENVIRONMENT` debe ser `Production` para no exponer `/scalar` ni el token de reset de contraseña.
- RLS en Supabase: el aislamiento multi-clínica ya lo hace la API (claim `clinicId`); si además habilitas RLS en tablas, configura las policies con la misma regla.

En `Program.cs`:
- Identity con requisitos mínimos de contraseña.
- `Jwt:Key` se usa para firmar/validar el token.
- En **Development** se exponen OpenAPI y **Scalar** (`/scalar`) y el token de `forgot-password`.

---

## Ejecución

Requisitos: .NET SDK 10 y PostgreSQL corriendo.

```bash
# 1. Compilar
dotnet build MimaCare.sln

# 2. Ejecutar la API (aplica migraciones automáticamente)
dotnet run --project MimaCare.Presentation
```

Al arrancar, la aplicación:
1. Aplica las migraciones pendientes (`Database.MigrateAsync()`). Si la BD ya tiene tablas creadas por otro medio, hay un fallback que marca la migración inicial como aplicada (evita el error `42P07`).
2. Siembra los roles `Super`, `Clinic` y `Doctor` (`RoleSeeder`).
3. Siembra el catálogo de **10 especialidades odontológicas** (`SpecialtySeeder`).
4. `SubscriptionPlanSeeder`: garantiza los 3 planes por defecto, asigna un plan a toda clínica sin plan (y **Enterprise** a la clínica demo `mimacare@gmail.com`), crea una **sucursal por defecto** para clínicas sin sucursales y vincula cada doctor a la sucursal de su clínica. Idempotente.

La API queda en **`http://localhost:5097`** y la documentación en **`http://localhost:5097/scalar`** (Development).

> No hay redirección HTTPS por diseño (ver sección de seguridad).

---

## Scripts de datos (locales)

La carpeta `Scripts/` (sin subir al repositorio) contiene:

- `seed_directo.sql` — siembra directa por SQL: especialidades por doctor, horarios, 10 pacientes y 20 citas odontológicas con fechas en días laborables. Idempotente.
- `seed_via_api.ps1` — misma siembra pero **completamente por API** (flujo real: login → especialidades → doctores → horarios → pacientes → citas → estados → reasignación → edición → login de doctor).
- `seed_planes.sql` — aplicación de la lógica de `SubscriptionPlanSeeder` por SQL (planes, plan Enterprise para la clínica demo, sucursal por defecto y vinculación de doctores a sucursal). Idempotente.
- `seed_test_data.sql` — versión anterior de siembra SQL.

---

## Docker

El repositorio incluye `Dockerfile` (multi-stage .NET 10) y `docker-compose.yml` con dos servicios: **`db`** (PostgreSQL 17) y **`api`** (la API, que aplica migraciones y seeders al arrancar).

Las credenciales **no van en el YAML**: el compose interpola variables de entorno de un archivo **`.env`** local.

1. Crear el entorno local desde la plantilla (editar con valores reales):

   ```bash
   copy .env.example .env
   ```

   Mínimo: `POSTGRES_PASSWORD` y `JWT_KEY` (clave de firma del JWT, se usa `Jwt__Key` en el contenedor). Si ya tienes PostgreSQL local en el puerto 5432, el contenedor de BDs expone el `5433` (`POSTGRES_PORT_EXTERNO`) para no chocar; los servicios internos se hablan por la red de compose (`db:5432`).

2. Levantar (postgres + API en `http://localhost:5097`, docs en `/scalar`):

   ```bash
   docker compose up --build -d
   ```

3. Detener / ver logs:

   ```bash
   docker compose down
   docker compose logs -f api
   ```

> `.env` está en `.gitignore` (nunca se sube); `.env.example` es la plantilla. La API en el contenedor lee `ConnectionStrings__DefaultConnection` y `Jwt__Key` desde el entorno (convención de configuración de ASP.NET, separador `__`), por lo que `appsettings.json` conserva los valores locales de desarrollo.

---

## Repositorio

- GitHub: `https://github.com/estarlin-gg/MimaCare_Api.git`
- Rama por defecto: `master`