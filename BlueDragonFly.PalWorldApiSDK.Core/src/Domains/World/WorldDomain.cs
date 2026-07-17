namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Operaciones REST del dominio de mundo.</summary>
public sealed class WorldDomain
{
    private readonly PalworldApiHttpClient httpClient;

    internal WorldDomain(PalworldApiHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <summary>Publica un anuncio para los jugadores.</summary>
    public Task AnnounceAsync(string message, CancellationToken cancellationToken = default) =>
        httpClient.PostAsync("announce", new AnnounceRequest(ArgumentGuard.RequireValue(message, nameof(message))), cancellationToken);

    /// <summary>Solicita guardar el mundo.</summary>
    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        httpClient.PostAsync("save", null, cancellationToken);

    /// <summary>Obtiene una instantanea de los actores presentes en el mundo.</summary>
    public Task<GameDataSnapshot> GameDataAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync<GameDataSnapshot>("game-data", cancellationToken);
}
