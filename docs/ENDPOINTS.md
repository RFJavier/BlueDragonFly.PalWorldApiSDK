# Cobertura de Endpoints

Ultima verificacion: 2026-07-16. La referencia es la [documentacion REST oficial de Palworld](https://docs.palworldgame.com/category/rest-api/). Todos los endpoints usan el prefijo `/v1/api` y HTTP Basic Authentication.

Consulta la [guia de uso](USAGE.md) para ejemplos de cada operacion.

## Implementados

| Metodo | Ruta | Metodo SDK | Respuesta |
| --- | --- | --- | --- |
| `GET` | `/info` | `Server.InfoAsync` | `ServerInfo` |
| `GET` | `/players` | `Players.ListAsync` | `PlayerList` |
| `GET` | `/metrics` | `Server.MetricsAsync` | `ServerMetrics` |
| `GET` | `/settings` | `Server.SettingsAsync` | `ServerSettings` |
| `GET` | `/game-data` | `World.GameDataAsync` | `GameDataSnapshot` |
| `POST` | `/announce` | `World.AnnounceAsync` | Sin contenido de dominio |
| `POST` | `/kick` | `Players.KickAsync` | Sin contenido de dominio |
| `POST` | `/ban` | `Players.BanAsync` | Sin contenido de dominio |
| `POST` | `/unban` | `Players.UnbanAsync` | Sin contenido de dominio |
| `POST` | `/save` | `World.SaveAsync` | Sin contenido de dominio |
| `POST` | `/shutdown` | `Server.ShutdownAsync` | Sin contenido de dominio |
| `POST` | `/stop` | `Server.StopAsync` | Sin contenido de dominio |

> `/game-data` puede no estar disponible en versiones anteriores del servidor y responder `404`.

## Criterios para agregar un endpoint

Cada endpoint nuevo debe incluir:

1. Un metodo asincrono publico en el dominio correspondiente de `PalworldClient`.
2. Modelos de solicitud y respuesta tipados cuando la API tenga un esquema definido.
3. Validacion de parametros antes de enviar la solicitud.
4. Pruebas que verifiquen metodo HTTP, ruta, autenticacion y JSON.
5. Actualizacion de este documento, `README.md` y `CHANGELOG.md`.
