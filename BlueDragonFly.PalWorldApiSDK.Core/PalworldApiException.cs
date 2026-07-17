using System.Net;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Representa una respuesta no exitosa devuelta por la API de Palworld.</summary>
public class PalworldApiException(HttpStatusCode statusCode, string responseContent) : Exception($"La API de Palworld devolvió {(int)statusCode} ({statusCode}).")
{
    /// <summary>Código HTTP devuelto por el servidor.</summary>
    public HttpStatusCode StatusCode { get; } = statusCode;

    /// <summary>Contenido de la respuesta, cuando esté disponible.</summary>
    public string ResponseContent { get; } = responseContent;
}
