namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Operaciones REST del dominio de servidor.</summary>
public sealed class ServerDomain
{
    private readonly PalworldApiHttpClient httpClient;

    internal ServerDomain(PalworldApiHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <summary>Obtiene la informacion del servidor.</summary>
    public Task<ServerInfo> InfoAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync<ServerInfo>("info", cancellationToken);

    /// <summary>Obtiene las metricas de rendimiento y capacidad del servidor.</summary>
    public Task<ServerMetrics> MetricsAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync<ServerMetrics>("metrics", cancellationToken);

    /// <summary>Obtiene la configuracion activa del servidor.</summary>
    public Task<ServerSettings> SettingsAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync<ServerSettings>("settings", cancellationToken);

    /// <summary>Programa el apagado ordenado del servidor.</summary>
    public Task ShutdownAsync(int waitTimeSeconds, string? message = null, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(waitTimeSeconds);
        return httpClient.PostAsync("shutdown", new ShutdownRequest(waitTimeSeconds, message), cancellationToken);
    }

    /// <summary>Fuerza la detencion inmediata del servidor.</summary>
    public Task StopAsync(CancellationToken cancellationToken = default) =>
        httpClient.PostAsync("stop", null, cancellationToken);
}
