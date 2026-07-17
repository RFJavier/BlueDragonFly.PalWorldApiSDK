using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlueDragonFly.PalWorldApiSDK.Core;

internal sealed class PalworldApiHttpClient : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient httpClient;
    private readonly bool disposeHttpClient;

    public PalworldApiHttpClient(PalworldServerOptions options, HttpClient httpClient, bool disposeHttpClient)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.disposeHttpClient = disposeHttpClient;
    }

    public PalworldServerOptions Options { get; }

    public async Task<T> GetAsync<T>(string route, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, route);
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(content, JsonOptions) ?? throw new JsonException("La API devolvio un cuerpo JSON vacio o no valido.");
    }

    public async Task PostAsync(string route, object? body, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Post, route);
        if (body is not null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");
        }

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (disposeHttpClient)
        {
            httpClient.Dispose();
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string route)
    {
        var request = new HttpRequestMessage(method, new Uri($"{Options.BaseUrl.AbsoluteUri.TrimEnd('/')}/{route}"));
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Options.Username}:{Options.Password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }

    private static async Task<string> EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => new PalworldBadRequestException(content),
                System.Net.HttpStatusCode.Unauthorized => new PalworldUnauthorizedException(content),
                _ => new PalworldApiException(response.StatusCode, content)
            };
        }

        return content;
    }
}
