# ============================================================================
# MimaCare :: Siembra de datos VIA API (flujo completo de la app)
# Doctores -> especialidades -> horarios -> pacientes -> citas -> estados
# ============================================================================
# Uso:  powershell -File Scripts\seed_via_api.ps1
# ============================================================================

$ErrorActionPreference = 'Stop'
$base = "http://localhost:5097"

function Invoke-Api {
    param($Method, $Uri, $Body = $null, $Token = $null)
    $headers = @{}
    if ($Token) { $headers.Authorization = "Bearer $Token" }
    $bytes = $null
    if ($null -ne $Body) {
        $json = $Body | ConvertTo-Json -Depth 8
        # PS5.1 envia strings en Latin-1; bytes UTF-8 garantizan acentos correctos
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($json)
    }
    try {
        return Invoke-RestMethod -Uri "$base$Uri" -Method $Method `
            -Headers $headers -ContentType "application/json; charset=utf-8" -Body $bytes
    } catch {
        Write-Host "ERROR $Method $uri -> $($_.Exception.Message)" -ForegroundColor Red
        Write-Host $_.ErrorDetails.Message -ForegroundColor Red
        throw
    }
}

function Resolve-Date {
    # Devuelve la fecha mas cercana a hoy+offset cuyo dia caiga en $AllowedDow
    param([int]$OffsetDays, [int[]]$AllowedDow)
    $step = if ($OffsetDays -ge 0) { 1 } else { -1 }
    $d = (Get-Date).Date.AddDays($OffsetDays)
    for ($i = 0; $i -lt 7; $i++) {
        if ($AllowedDow -contains [int]$d.DayOfWeek) { return $d }
        $d = $d.AddDays($step)
    }
    throw "No se encontro fecha valida"
}

Write-Host "== 1. Login clinica ==" -ForegroundColor Cyan
$tok = (Invoke-Api Post "/api/Auth/login" @{ email = "mimacare@gmail.com"; password = "Egerman-17" }).token
Write-Host "   OK"

Write-Host "== 2. Catalogo de especialidades odontologicas ==" -ForegroundColor Cyan
$r = Invoke-Api Get "/api/Specialty" -Token $tok
$cat = @($r.data)
$specIds = @{}
foreach ($s in $cat) {
    if ($null -ne $s -and $s.name) { $specIds[$s.name] = $s.id }
}
Write-Host ("   {0} especialidades: {1}" -f $cat.Count, (($cat | ForEach-Object name) -join ", "))

Write-Host "== 3. Crear doctores ==" -ForegroundColor Cyan
$doctorsIn = @(
    @{ f = "Estarlin"; l = "Germán";  e = "estarlin@gmail.com";  p = "Estarlin2026*";   ph = "8095551001"; spec = "Cirugía Oral";    lic = "DOC-1990-001"; dows = @(1,2,3,4,5); extra = @("Implantología Oral") },
    @{ f = "Sweet";    l = "Pain";    e = "sweet@gmail.com";     p = "Sweetpain2026*";  ph = "8095551002"; spec = "Ortodoncia";      lic = "DOC-1992-002"; dows = @(1,2,3,4,5,6); extra = @("Estética Dental") },
    @{ f = "Karla";    l = "García";  e = "kgarcia@gmail.com";   p = "Karla2026*";      ph = "8095551003"; spec = "Endodoncia";      lic = "DOC-1988-003"; dows = @(2,3,4,5,6); extra = @("Rehabilitación Oral") },
    @{ f = "Dora";     l = "Medrano"; e = "wmedrano@gmail.com";  p = "Dora2026*";       ph = "8095551004"; spec = "Periodoncia";     lic = "DOC-1995-004"; dows = @(1,2,3,4,5); extra = @("Prostodoncia") }
)

$docList = (Invoke-Api Get "/api/Doctor?page=1&pageSize=50" -Token $tok).data.items
$docIds = @{}
foreach ($x in $docList) { $docIds[$x.email] = $x.id }

$created = 0
foreach ($d in $doctorsIn) {
    if ($docIds.ContainsKey($d.e)) { Write-Host "   $($d.e) ya existe, omito"; continue }
    try {
        Invoke-Api Post "/api/Doctor" @{
            firstName    = $d.f
            lastName     = $d.l
            email        = $d.e
            password     = $d.p
            phoneNumber  = $d.ph
            specialty    = $d.spec
            licenseNumber= $d.lic
        } -Token $tok | Out-Null
        $created++
    } catch { Write-Host "   (fallo creando $($d.e))" -ForegroundColor Yellow }
}
Write-Host "   doctores creados: $created"

$docList = (Invoke-Api Get "/api/Doctor?page=1&pageSize=50" -Token $tok).data.items
$docIds = @{}
foreach ($x in $docList) { $docIds[$x.email] = $x.id }

Write-Host "== 4. Asignar especialidades por doctor ==" -ForegroundColor Cyan
foreach ($d in $doctorsIn) {
    $did = $docIds[$d.e]
    if (-not $did) { Write-Host "   SIN doctor $($d.e), omito especialidades" -ForegroundColor Yellow; continue }
    $ids = @($specIds[$d.spec]) + @($d.extra | ForEach-Object { $specIds[$_] })
    Invoke-Api Put "/api/doctors/$did/specialties" @{ specialtyIds = @($ids | Where-Object { $_ }) } -Token $tok | Out-Null
    Write-Host ("   {0} {1}: {2}" -f $d.f, $d.spec, ($d.extra -join ", "))
}

Write-Host "== 5. Horarios por doctor ==" -ForegroundColor Cyan
$schedMap = @{
    "estarlin@gmail.com" = @(
        @{ dayOfWeek=1; startTime="08:00:00"; endTime="12:00:00"; isActive=$true },
        @{ dayOfWeek=2; startTime="08:00:00"; endTime="12:00:00"; isActive=$true },
        @{ dayOfWeek=3; startTime="08:00:00"; endTime="12:00:00"; isActive=$true },
        @{ dayOfWeek=4; startTime="08:00:00"; endTime="12:00:00"; isActive=$true },
        @{ dayOfWeek=5; startTime="08:00:00"; endTime="12:00:00"; isActive=$true },
        @{ dayOfWeek=1; startTime="14:00:00"; endTime="18:00:00"; isActive=$true },
        @{ dayOfWeek=3; startTime="14:00:00"; endTime="18:00:00"; isActive=$true },
        @{ dayOfWeek=5; startTime="14:00:00"; endTime="18:00:00"; isActive=$true }
    )
    "sweet@gmail.com" = @(1..6 | ForEach-Object { @{ dayOfWeek=$_; startTime="09:00:00"; endTime="17:00:00"; isActive=$true } })
    "kgarcia@gmail.com" = @(2..6 | ForEach-Object { @{ dayOfWeek=$_; startTime="10:00:00"; endTime="18:00:00"; isActive=$true } })
    "wmedrano@gmail.com" = @(1..5 | ForEach-Object { @{ dayOfWeek=$_; startTime="07:00:00"; endTime="15:00:00"; isActive=$true } })
}
foreach ($k in $schedMap.Keys) {
    $did = $docIds[$k]
    if (-not $did) { Write-Host "   SIN doctor $k, omito horario" -ForegroundColor Yellow; continue }
    Invoke-Api Put "/api/doctors/$did/schedule" @($schedMap[$k]) -Token $tok | Out-Null
}
Write-Host "   horarios configurados (4 doctores)"

Write-Host "== 6. Crear pacientes ==" -ForegroundColor Cyan
$patientsIn = @(
    @{ fn="María";  ln="Pérez Santos";   ph="8095550101"; ad="C/ Duarte #15, Los Prados";              al="Penicilina"; nt="Hipertensión controlada - precaución anestésicos con epinefrina." },
    @{ fn="José";   ln="Rodríguez Díaz"; ph="8095550102"; ad="Av. Independencia #502, Gazcue";         al=$null;        nt="Bruxismo nocturno. Usa férula de descarga." },
    @{ fn="Ana";    ln="Torres Cruz";    ph="8095550103"; ad="C/ Máximo Gómez #89";                    al="Aspirina";   nt="Paciente pediátrica - acompañada por su madre." },
    @{ fn="Luis";   ln="Gómez Reyes";    ph="8095550104"; ad="C/ José Martí #210, Herrera";            al=$null;        nt="Gingivitis - sangrado al cepillarse. Endodoncia pieza 36 finalizada." },
    @{ fn="Carmen"; ln="Sánchez Matos";  ph="8095550105"; ad="Res. Palma Real Apto 3B, Villa Mella";   al=$null;        nt="Limpieza y revisión cada 6 meses." },
    @{ fn="Pedro";  ln="Ramírez Nova";   ph="8095550106"; ad="C/ Primera #33, Ens. Ozama";             al=$null;        nt="Post-extracción tercer molar en seguimiento." },
    @{ fn="Laura";  ln="Díaz Fernández"; ph="8095550107"; ad="Av. 27 de Febrero #1000";                al="Látex";      nt="Usar guantes sin látex. Ortodoncia en planificación." },
    @{ fn="Miguel"; ln="Castillo Peña";  ph="8095550108"; ad="C/ Caracol #7, Los Alcarrizos";          al=$null;        nt="Caries múltiples - plan por fases." },
    @{ fn="Rosa";   ln="Flores Jiménez"; ph="8095550109"; ad="C/ Restauración #45";                    al="Ibuprofeno"; nt="Ortodoncia en curso. Sensibilidad con fríos." },
    @{ fn="Sofía";  ln="Valdez Polanco"; ph="8095550110"; ad="C/ El Sol #4, Naco";                     al=$null;        nt="Paciente nueva - blanqueamiento interesada." }
)
foreach ($p in $patientsIn) {
    try {
        Invoke-Api Post "/api/Patient" @{
            firstName = $p.fn; lastName = $p.ln; phone = $p.ph; address = $p.ad
            allergies = $p.al; notes = $p.nt
        } -Token $tok | Out-Null
    } catch { Write-Host "   paciente $($p.fn) $($p.ln) fallo (duplicado?)" -ForegroundColor Yellow }
}
$patList = (Invoke-Api Get "/api/Patient?page=1&pageSize=50" -Token $tok).data.items
$patIds = @{}
foreach ($x in $patList) { $patIds["$($x.firstName) $($x.lastName)"] = $x.id }
Write-Host "   pacientes totales: $($patList.Count)"

function Pat([string]$name) { return $patIds[$name] }
function Doc([string]$email) { return $docIds[$email] }

Write-Host "== 7. Crear citas odontologicas ==" -ForegroundColor Cyan
# [emailDoctor, paciente, offsetDias, start, end, notas, estadoFinal]
$slots = @(
    ,@("estarlin@gmail.com","María Pérez Santos",   -14,"09:00","09:45","Evaluación quirúrgica. Panorámica: terceros molares inferiores retenidos.",3)
    ,@("estarlin@gmail.com","Pedro Ramírez Nova",    -7,"10:00","10:45","Extracción de tercer molar superior derecho. Sin complicaciones.",3)
    ,@("estarlin@gmail.com","José Rodríguez Díaz",   -3,"11:00","11:30","Valoración para exodoncia múltiple.",4)
    ,@("estarlin@gmail.com","Pedro Ramírez Nova",    +3,"09:00","09:30","Control post-quirúrgico - retirar puntos.",2)
    ,@("estarlin@gmail.com","Sofía Valdez Polanco",  +8,"14:00","15:00","Cirugía oral bajo sedación - premedicación indicada.",1)
    ,@("kgarcia@gmail.com","Luis Gómez Reyes",      -21,"10:00","11:00","Pulpitis irreversible pieza 36 - endodoncia sesión 1.",3)
    ,@("kgarcia@gmail.com","Luis Gómez Reyes",      -12,"10:00","11:00","Endodoncia pieza 36 - sesión 2, obturación con gutapercha.",3)
    ,@("kgarcia@gmail.com","Carmen Sánchez Matos",   -2,"15:00","15:45","Obturación con resina pieza 46 (caries oclusal).",3)
    ,@("kgarcia@gmail.com","Rosa Flores Jiménez",    +2,"16:00","16:45","Urgencia: dolor agudo pieza 21.",2)
    ,@("kgarcia@gmail.com","Miguel Castillo Peña",   +9,"11:00","11:45","Evaluación endodóntica pieza 46.",1)
    ,@("sweet@gmail.com","Laura Díaz Fernández",    -10,"09:30","10:15","Estudio de ortodoncia: fotografías, impresiones y cefalometría.",3)
    ,@("sweet@gmail.com","Rosa Flores Jiménez",      -5,"11:00","11:45","Instalación de brackets arco superior.",3)
    ,@("sweet@gmail.com","Ana Torres Cruz",           -1,"09:00","09:45","Ortodoncia interceptiva infantil.",5)
    ,@("sweet@gmail.com","Rosa Flores Jiménez",      +4,"11:00","11:30","Activación mensual de brackets.",2)
    ,@("sweet@gmail.com","Laura Díaz Fernández",    +12,"10:30","11:15","Colocación de brackets - presupuesto aprobado.",1)
    ,@("wmedrano@gmail.com","María Pérez Santos",     -6,"08:00","09:00","Raspado y alisado radicular cuadrantes superiores.",3)
    ,@("wmedrano@gmail.com","Carmen Sánchez Matos",   -1,"13:00","13:45","Mantenimiento periodontal.",3)
    ,@("wmedrano@gmail.com","Rosa Flores Jiménez",    +6,"08:00","08:45","Control de bolsas periodontales - sondaje.",2)
    ,@("wmedrano@gmail.com","José Rodríguez Díaz",   +10,"09:00","09:45","Evaluación para injerto gingival.",1)
    ,@("wmedrano@gmail.com","Sofía Valdez Polanco",   +5,"12:00","12:30","Limpieza y valoración general.",1)
)

$apptIds = @()
$i = 0
foreach ($s in $slots) {
    $i++
    $email = $s[0]
    $did = Doc $email
    $pid = Pat $s[1]
    if (-not $did -or -not $pid) { Write-Host "   cita #$i omitida (doctor/paciente faltante)" -ForegroundColor Yellow; continue }
    $dow = ($doctorsIn | Where-Object { $_.e -eq $email }).dows
    if (-not $dow) { continue }
    try { $date = Resolve-Date -OffsetDays $s[2] -AllowedDow $dow } catch { Write-Host "   cita #$i sin fecha valida" -ForegroundColor Yellow; continue }
    $body = @{
        doctorId        = $did
        patientId       = $pid
        appointmentDate = $date.ToString("yyyy-MM-dd")
        startTime       = "$($s[3]):00"
        endTime         = "$($s[4]):00"
        notes           = $s[5]
    }
    try {
        $res = Invoke-Api Post "/api/Appointment" $body -Token $tok
        $apptIds += ,@($res.data.id, $s[6], $email, $s[1], $date.ToString("yyyy-MM-dd"), $s[3])
    } catch {
        Write-Host "   cita #$i fallo ($($s[1]) $($s[3]))" -ForegroundColor Yellow
        Write-Host $_.ErrorDetails.Message -ForegroundColor DarkYellow
    }
}
Write-Host "   citas creadas: $($apptIds.Count)"

Write-Host "== 8. Transiciones de estado (flujo real) ==" -ForegroundColor Cyan
$done = 0
foreach ($a in $apptIds) {
    if ($a[1] -ne 1) {
        Invoke-Api Put "/api/Appointment/$($a[0])/status" @{ status = $a[1] } -Token $tok | Out-Null
        $done++
    }
}
Write-Host "   citas con estado aplicado: $done"

Write-Host "== 8b. Flujo asignar-doctor (reasignar limpieza a kgarcia) ==" -ForegroundColor Cyan
$reassign = $apptIds | Where-Object { $_[2] -eq "wmedrano@gmail.com" -and $_[5] -eq "12:00" } | Select-Object -First 1
if ($reassign) {
    try {
        Invoke-Api Put "/api/Appointment/$($reassign[0])/assign-doctor" @{ doctorId = (Doc "kgarcia@gmail.com") } -Token $tok | Out-Null
        Write-Host "   cita reasignada a Karla García (Endodoncia)"
    } catch { Write-Host "   reasignacion fallo (validacion horario destino?)" -ForegroundColor Yellow }
}

Write-Host "== 8c. Flujo editar cita (control post-quirurgico) ==" -ForegroundColor Cyan
$editTarget = $apptIds | Where-Object { $_[2] -eq "estarlin@gmail.com" -and $_[4] -eq (Resolve-Date -OffsetDays 3 -AllowedDow @(1,2,3,4,5)).ToString("yyyy-MM-dd") } | Select-Object -First 1
if ($editTarget) {
    try {
        Invoke-Api Put "/api/Appointment/$($editTarget[0])" @{
            patientId       = Pat "Pedro Ramírez Nova"
            appointmentDate = $editTarget[4]
            startTime       = "09:00:00"
            endTime         = "09:45:00"
            notes           = "Control post-quirúrgico - retirar puntos + radiografía de control."
        } -Token $tok | Out-Null
        Write-Host "   cita editada (duracion extendida + nota)"
    } catch { Write-Host "   edicion fallo" -ForegroundColor Yellow }
}

Write-Host "== 9. Flujo del DOCTOR (login propio) ==" -ForegroundColor Cyan
$stok = (Invoke-Api Post "/api/Auth/login" @{ email = "sweet@gmail.com"; password = "Sweetpain2026*" }).token
$mine = (Invoke-Api Get "/api/Appointment/my?page=1&pageSize=20" -Token $stok).data
Write-Host "   citas del Dr. Sweet: total=$($mine.totalCount)"
$toConfirm = $apptIds | Where-Object { $_[2] -eq "sweet@gmail.com" -and $_[1] -eq 1 } | Select-Object -First 1
if ($toConfirm) {
    Invoke-Api Put "/api/Appointment/$($toConfirm[0])/status" @{ status = 2 } -Token $stok | Out-Null
    Write-Host "   el doctor confirmo su cita pendiente (flujo doctor OK)"
}

Write-Host "== 10. Resumen final ==" -ForegroundColor Green
$all = @(Invoke-Api Get "/api/Appointment?page=1&pageSize=100" -Token $tok).data.items
$statusNames = @{ 1 = "Pending"; 2 = "Confirmed"; 3 = "Completed"; 4 = "Cancelled"; 5 = "Rejected" }
$groups = $all | Group-Object status | Sort-Object Name
foreach ($g in $groups) {
    $label = if ($statusNames[[int]$g.Name]) { $statusNames[[int]$g.Name] } else { $g.Name }
    Write-Host ("   {0}: {1}" -f $label, $g.Count)
}
Write-Host ("   TOTAL: {0} citas | {1} pacientes | {2} doctores" -f $all.Count, $patList.Count, $docList.Count)
