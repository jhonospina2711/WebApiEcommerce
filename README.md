# README — Migraciones Entity Framework Core (Proyecto nuevo)

Este README describe el paso a paso para crear y aplicar migraciones en un proyecto C# con .NET 8 y EF Core 9.x.

**Resumen rápido**
- Requisitos: .NET 8 SDK, paquetes EF Core en el proyecto.
- Opciones: instalar `dotnet-ef` globalmente o usarlo como herramienta local.
- Ejecutar `dotnet ef migrations add <Nombre>` y `dotnet ef database update` desde la carpeta del proyecto.

---

## 1. Requisitos
- .NET SDK 8 instalado. Verifica con:

```powershell
dotnet --version
```
- En tu `*.csproj` incluye al menos:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.5" />
```
- Asegúrate de estar en la carpeta donde está el `.csproj` que contiene tu `DbContext` antes de ejecutar comandos `dotnet ef`.

## 2. Preparar la base de datos (opcional: Docker)
Si usas el `docker-compose.yaml` del proyecto, levanta SQL Server:

```powershell
docker-compose up -d
```

Ejemplo de connection string para `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=WebApiEcommerceDb;User Id=sa;Password=MyStrongPass123;TrustServerCertificate=True;"
}
```

Reemplaza contraseña y nombre de BD según necesites.

## 3. Restaurar paquetes

```powershell
dotnet restore
```

## 4. Instalar `dotnet-ef` (opcional)
Tienes dos opciones:

A) Instalar globalmente (requiere PATH):

```powershell
# Versión recomendada para EF Core 9
dotnet tool install --global dotnet-ef --version 9.0.5
```

B) Instalar como herramienta local (recomendado para proyectos compartidos):

```powershell
# Desde la carpeta raíz del proyecto donde ejecutarás los comandos
dotnet new tool-manifest # crea .config/dotnet-tools.json
dotnet tool install dotnet-ef --version 9.0.5
# Ejecutar luego como:
dotnet tool run dotnet-ef -- migrations add InitialMigration
```

Si instalas globalmente, asegúrate que `%USERPROFILE%\.dotnet\tools` esté en tu `PATH`. Si no se reconoce `dotnet-ef`, reinicia la terminal o añade la ruta al PATH.

## 5. Crear la migración
Sitúate en la carpeta del proyecto (donde está el `.csproj` con `DbContext`) y ejecuta:

```powershell
# Si dotnet-ef está global
dotnet ef migrations add InitialMigration

# Si usas herramienta local
dotnet tool run dotnet-ef -- migrations add InitialMigration
```

Si trabajas con una solución donde el proyecto de inicio (`Startup`) es distinto, pasa los parámetros `-p` (project) y `-s` (startup project):

```powershell
dotnet ef migrations add InitialMigration -p ./WebApiEcommerce -s ./WebApiEcommerce
```

## 6. Aplicar la migración (crear la BD/ajustar esquema)

```powershell
# Global
dotnet ef database update

# Local
dotnet tool run dotnet-ef -- database update
```

## 7. Aplicar migraciones automáticamente al iniciar la app (opcional)
En `Program.cs` puedes agregar (ejemplo mínimo):

```csharp
using Microsoft.EntityFrameworkCore;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
```

Esto aplica cualquier migración pendiente al iniciar la aplicación (útil en entornos controlados).

## 8. Operaciones comunes
- Eliminar la última migración (si no fue aplicada a la DB):

```powershell
dotnet ef migrations remove
```

- Regresar la base de datos a una migración anterior:

```powershell
dotnet ef database update NombreMigracionAnterior
```

## 9. Resolución de problemas comunes
- Error al instalar `dotnet-ef` con mensaje sobre `DotnetToolSettings.xml` o paquete inválido:
  - Limpia caché NuGet y vuelve a intentar:

```powershell
dotnet nuget locals all --clear
```

  - Elimina carpeta de herramientas globales (si vas a reinstalar globalmente):

```powershell
Remove-Item "$env:USERPROFILE\.dotnet\tools" -Recurse -Force
```

  - Intenta instalar una versión explícita:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.5
```

- `dotnet-ef` instalado pero comando no encontrado: añade `%USERPROFILE%\.dotnet\tools` al `PATH` y reinicia la terminal.

- `dotnet ef` falla por no encontrar `DbContext`: asegúrate de ejecutar desde el proyecto correcto o usar `-p`/`-s`.

## 10. Notas finales
- Mantén las versiones de `Microsoft.EntityFrameworkCore.*` compatibles con la versión de `dotnet-ef` que uses.
- Para proyectos en equipo, prefiero la instalación como herramienta local (`tool-manifest`) para evitar diferencias entre máquinas.

---

Si quieres, puedo:
- Añadir el `README.md` al repositorio (ya lo estoy creando aquí) con personalizaciones para este proyecto.
- Generar un ejemplo de `Program.cs` con `db.Database.Migrate()` integrado.
- Ejecutar los comandos en tu máquina (si me das permiso para correr terminales).