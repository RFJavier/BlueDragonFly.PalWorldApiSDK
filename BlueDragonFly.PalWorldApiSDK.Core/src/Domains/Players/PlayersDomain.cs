namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Operaciones REST del dominio de jugadores.</summary>
public sealed class PlayersDomain
{
    private readonly PalworldApiHttpClient httpClient;

    internal PlayersDomain(PalworldApiHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <summary>Obtiene la lista de jugadores.</summary>
    public Task<PlayerList> ListAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync<PlayerList>("players", cancellationToken);

    /// <summary>Expulsa un jugador del servidor.</summary>
    public Task KickAsync(string steamId, string? message = null, CancellationToken cancellationToken = default) =>
        httpClient.PostAsync("kick", new KickPlayerRequest(ArgumentGuard.RequireValue(steamId, nameof(steamId)), message), cancellationToken);

    /// <summary>Bloquea a un jugador en el servidor.</summary>
    public Task BanAsync(string steamId, string? message = null, CancellationToken cancellationToken = default) =>
        httpClient.PostAsync("ban", new BanPlayerRequest(ArgumentGuard.RequireValue(steamId, nameof(steamId)), message), cancellationToken);

    /// <summary>Elimina el bloqueo de un jugador.</summary>
    public Task UnbanAsync(string steamId, CancellationToken cancellationToken = default) =>
        httpClient.PostAsync("unban", new UnbanPlayerRequest(ArgumentGuard.RequireValue(steamId, nameof(steamId))), cancellationToken);
}
