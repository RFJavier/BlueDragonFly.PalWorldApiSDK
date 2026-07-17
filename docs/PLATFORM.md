# Plataforma de destino y compatibilidad

Este documento describe los requisitos de la plataforma de destino para consumir `BlueDragonFly.PalWorldApiSDK.Core` desde NuGet.org.

## Requisitos del proyecto consumidor

- **.NET 10 SDK o runtime** (o superior).
- El paquete se compila para el `TargetFramework` `net10.0` y no expone `netstandard`.
- Cualquier sistema operativo compatible con .NET 10: Windows, Linux y macOS.

Tu proyecto debe declarar al menos `net10.0`:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
</PropertyGroup>
```

Si tu proyecto apunta a `net10.0-windows` u otro perfil de .NET 10, tambien puede referenciar el paquete; la biblioteca misma no depende de APIs especificas de Windows.

## Instalar desde NuGet.org

```powershell
dotnet add package BlueDragonFly.PalWorldApiSDK.Core
```

O directamente en tu archivo de proyecto:

```xml
<ItemGroup>
    <PackageReference Include="BlueDragonFly.PalWorldApiSDK.Core" Version="1.0.0" />
</ItemGroup>
```

Para instalar una version concreta:

```powershell
dotnet add package BlueDragonFly.PalWorldApiSDK.Core --version 1.0.0
```

## Verificar el entorno local

Asegurate de que tienes el runtime .NET 10 disponible:

```powershell
# Windows
dotnet --list-runtimes | findstr "Microsoft.NETCore.App 10."
dotnet --list-sdks | findstr "10."
```

```bash
# Linux / macOS
dotnet --list-runtimes | grep "Microsoft.NETCore.App 10."
dotnet --list-sdks | grep "10."
```

## Requisitos del servidor Palworld

- Servidor dedicado con la **API REST habilitada**:

  ```ini
  RESTAPIEnabled=True
  ```

- Puerto REST accesible desde la aplicacion consumidora (por defecto `8212`).
- Credenciales de administrador configuradas (`AdminPassword` y usuario REST; el SDK usa `admin` como predeterminado).
- Conexion HTTP o HTTPS; el SDK normaliza automaticamente la ruta a `/v1/api`.

> El endpoint `/v1/api/game-data` puede responder `404` si la version del servidor no lo implementa. En ese caso, ignora esa operacion o actualiza el servidor.

Para actualizar:

```powershell
dotnet add package BlueDragonFly.PalWorldApiSDK.Core
```

## Limites conocidos

- No se puede usar directamente en proyectos que apunten a .NET 8 o .NET 9 porque el TFM del paquete es `net10.0`. Si necesitas soportar versiones anteriores, puedes clonar el repositorio y recompilar el proyecto Core cambiando `<TargetFramework>`.
- El SDK refleja la API REST oficial de Palworld; si Pocketpair modifica los nombres o tipos de los campos, es posible que algunas propiedades aparezcan en `ServerSettings.AdditionalSettings` hasta que se publique una nueva version del SDK.
