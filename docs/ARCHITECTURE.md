# Arquitectura

## Objetivo

`BlueDragonFly.PalWorldApiSDK.Core` es una biblioteca .NET para consumir la API REST administrativa de un servidor Palworld. No usa dependencias de terceros: utiliza `HttpClient` y `System.Text.Json` incluidos en .NET.

## Proyectos

| Proyecto | Responsabilidad |
| --- | --- |
| `BlueDragonFLy.PalWorldApiSDK.Core` | Biblioteca publicable: cliente REST por dominios, opciones, modelos y excepciones. |
| `BlueDragonFLy.PalWorldApiSDK.Demo` | Aplicacion de consola que muestra el uso seguro mediante variables de entorno o un archivo `.env`. |
| `BlueDragonFLy.PalWorldApiSDK.Tests` | Pruebas unitarias de la composicion de peticiones y serializacion. |

## Componentes principales

### `PalworldServerOptions`

Recibe la URL del servidor, usuario y contrasena. Valida que la URL sea HTTP/HTTPS y normaliza la ruta al prefijo `/v1/api`. La configuracion solo vive en memoria; el consumidor decide si obtiene los valores de un JSON, secretos o variables de entorno.

### `PalworldClient`

Es la fachada publica para llamar endpoints por dominio. Expone `Server`, `World` y `Players`, cada uno con metodos asincronos de su contexto.

- Cada solicitud agrega `Accept: application/json`.
- La autenticacion se envia como HTTP Basic Auth en cada solicitud.
- Las respuestas de error generan `PalworldApiException` con el codigo HTTP y el cuerpo de respuesta.
- Las respuestas JSON se deserializan con `System.Text.Json`.
- El constructor que recibe `HttpClient` no lo libera; el constructor simple crea y libera su propio cliente.

### Modelos

`ServerInfo`, `PlayerList`, `PlayerInfo`, `ServerMetrics`, `ServerSettings` y `GameDataSnapshot` representan las respuestas tipadas ya soportadas. `GameDataSnapshot` usa un convertidor polimorfico para materializar `CharacterActor`, `PalBoxActor` o `UnknownGameActor` cuando el servidor devuelva un tipo futuro. Sus propiedades se asocian explicitamente a los nombres JSON de Palworld mediante `JsonPropertyName` cuando es necesario.

### `PalworldApiException`

Expone `StatusCode` y `ResponseContent` para que el consumidor pueda manejar respuestas como `400` y `401` sin perder el detalle enviado por el servidor.

## Flujo de una operacion

1. El consumidor crea `PalworldServerOptions` y `PalworldClient`.
2. El dominio (`Server`, `World` o `Players`) combina la URL normalizada con la ruta del endpoint.
3. La capa HTTP agrega autenticacion, encabezados y, si aplica, un cuerpo JSON.
4. `HttpClient` envia la solicitud al servidor Palworld.
5. Una respuesta correcta se deserializa o finaliza sin valor para operaciones de comando.
6. Una respuesta no exitosa lanza `PalworldApiException`.

Consulta [docs/PLATFORM.md](PLATFORM.md) para los requisitos de la plataforma de destino.

## Versionado

El versionado sigue [Semantic Versioning](https://semver.org/lang/es/):

## Seguridad

La API de Palworld usa Basic Auth y permite acciones administrativas destructivas. No se deben almacenar credenciales en codigo fuente ni exponer el puerto REST directamente a Internet. Se recomienda usar secretos del entorno, una red privada o un proxy con controles de acceso adicionales.
