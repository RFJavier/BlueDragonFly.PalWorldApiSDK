namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Cliente principal de la API REST administrativa de un servidor Palworld.</summary>
public sealed class PalworldClient : IDisposable
{
    private readonly PalworldApiHttpClient httpClient;

    /// <summary>Inicializa un cliente que crea y administra su propio <see cref="HttpClient"/>.</summary>
    public PalworldClient(PalworldServerOptions options)
        : this(new PalworldApiHttpClient(options, new HttpClient(), disposeHttpClient: true))
    {
    }

    /// <summary>Inicializa un cliente con un <see cref="HttpClient"/> administrado por el consumidor.</summary>
    public PalworldClient(PalworldServerOptions options, HttpClient httpClient)
        : this(new PalworldApiHttpClient(options, httpClient, disposeHttpClient: false))
    {
    }

    private PalworldClient(PalworldApiHttpClient httpClient)
    {
        this.httpClient = httpClient;
        Server = new ServerDomain(httpClient);
        World = new WorldDomain(httpClient);
        Players = new PlayersDomain(httpClient);
    }

    /// <summary>Obtiene la configuracion de conexion del cliente.</summary>
    public PalworldServerOptions Options => httpClient.Options;

    /// <summary>Operaciones del dominio de servidor.</summary>
    public ServerDomain Server { get; }

    /// <summary>Operaciones del dominio de mundo.</summary>
    public WorldDomain World { get; }

    /// <summary>Operaciones del dominio de jugadores.</summary>
    public PlayersDomain Players { get; }

    /// <inheritdoc />
    public void Dispose() => httpClient.Dispose();
}
