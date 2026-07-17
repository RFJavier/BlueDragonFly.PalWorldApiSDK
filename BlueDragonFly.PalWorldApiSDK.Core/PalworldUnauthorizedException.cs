using System.Net;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Representa una respuesta sin autorizacion de la API de Palworld.</summary>
public sealed class PalworldUnauthorizedException(string responseContent)
    : PalworldApiException(HttpStatusCode.Unauthorized, responseContent)
{
}
