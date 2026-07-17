namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Configuración de conexión para la API REST de un servidor Palworld.</summary>
public sealed class PalworldServerOptions
{
    /// <summary>Inicializa una configuración y normaliza la URL al prefijo <c>/v1/api</c>.</summary>
    public PalworldServerOptions(string baseUrl, string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNull(password);

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var serverUrl)
            || (serverUrl.Scheme != Uri.UriSchemeHttp && serverUrl.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("La URL base debe ser una URL HTTP o HTTPS absoluta.", nameof(baseUrl));
        }

        var path = serverUrl.AbsolutePath.TrimEnd('/');
        if (!path.EndsWith("/v1/api", StringComparison.OrdinalIgnoreCase))
        {
            path = $"{path}/v1/api";
        }

        BaseUrl = new UriBuilder(serverUrl) { Path = path, Query = string.Empty, Fragment = string.Empty }.Uri;
        Username = username;
        Password = password;
    }

    /// <summary>URL base de la API, incluida la ruta <c>/v1/api</c>.</summary>
    public Uri BaseUrl { get; }
    /// <summary>Nombre de usuario para HTTP Basic Authentication.</summary>
    public string Username { get; }
    /// <summary>Contraseña para HTTP Basic Authentication.</summary>
    public string Password { get; }
}
