using System.Net;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Representa un error de solicitud devuelto por la API de Palworld.</summary>
public sealed class PalworldBadRequestException(string responseContent)
    : PalworldApiException(HttpStatusCode.BadRequest, responseContent)
{
}
