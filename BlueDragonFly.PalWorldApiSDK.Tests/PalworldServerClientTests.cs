using BlueDragonFly.PalWorldApiSDK.Core;

namespace BlueDragonFly.PalWorldApiSDK.Tests;

public sealed class PalworldClientTests
{
    [Fact]
    public async Task GetServerInfoAsync_UsesApiPathAndBasicAuthentication()
    {
        using var handler = new RecordingHandler("""{ "version":"1.0", "servername":"Test", "description":"Server", "worldguid":"world" }""");
        using var httpClient = new HttpClient(handler);
        using var client = new PalworldClient(new PalworldServerOptions("http://localhost:8212", "admin", "secret"), httpClient);

        var info = await client.Server.InfoAsync();

        Assert.Equal("Test", info.ServerName);
        Assert.Equal("http://localhost:8212/v1/api/info", handler.Request!.RequestUri!.ToString());
        Assert.Equal("Basic", handler.Request.Headers.Authorization!.Scheme);
    }

    [Fact]
    public async Task GetPlayersAsync_DeserializesPlayerList()
    {
        using var handler = new RecordingHandler("""{ "players":[{ "name":"Player", "accountName":"account", "playerId":"player-1", "userId":"steam_1", "ip":"127.0.0.1", "ping":3.14, "location_x":1.0, "location_y":2.0, "level":5, "building_count":4 }] }""");
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        var players = await client.Players.ListAsync();

        Assert.Equal("Player", Assert.Single(players.Players).Name);
        Assert.Equal("http://localhost:8212/v1/api/players", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public async Task BanPlayerAsync_UsesOfficialRequestSchema()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = new PalworldClient(new PalworldServerOptions("http://localhost:8212/v1/api", "admin", "secret"), httpClient);

        await client.Players.BanAsync("steam_123", "Rule violation");

        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal("http://localhost:8212/v1/api/ban", handler.Request.RequestUri!.ToString());
        Assert.Equal("{\"userid\":\"steam_123\",\"message\":\"Rule violation\"}", handler.Body);
    }

    [Fact]
    public async Task AnnounceAsync_UsesOfficialRequestSchema()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.World.AnnounceAsync("Server message");

        AssertRequest(handler, "announce", "{\"message\":\"Server message\"}");
    }

    [Fact]
    public async Task KickPlayerAsync_UsesOfficialRequestSchema()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.Players.KickAsync("steam_123", "Disconnect");

        AssertRequest(handler, "kick", "{\"userid\":\"steam_123\",\"message\":\"Disconnect\"}");
    }

    [Fact]
    public async Task UnbanPlayerAsync_UsesOfficialRequestSchema()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.Players.UnbanAsync("steam_123");

        AssertRequest(handler, "unban", "{\"userid\":\"steam_123\"}");
    }

    [Fact]
    public async Task SaveWorldAsync_UsesPostWithoutBody()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.World.SaveAsync();

        AssertRequest(handler, "save", null);
    }

    [Fact]
    public async Task ShutdownAsync_UsesOfficialRequestSchema()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.Server.ShutdownAsync(30, "Restarting");

        AssertRequest(handler, "shutdown", "{\"waittime\":30,\"message\":\"Restarting\"}");
    }

    [Fact]
    public async Task ForceStopAsync_UsesPostWithoutBody()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.Server.StopAsync();

        AssertRequest(handler, "stop", null);
    }

    [Fact]
    public async Task GetServerMetricsAsync_DeserializesOfficialResponse()
    {
        using var handler = new RecordingHandler("""{ "serverfps":57, "currentplayernum":10, "serverframetime":16.7671, "maxplayernum":32, "uptime":3600, "basecampnum":32, "days":1 }""");
        using var httpClient = new HttpClient(handler);
        using var client = new PalworldClient(new PalworldServerOptions("http://localhost:8212", "admin", "secret"), httpClient);

        var metrics = await client.Server.MetricsAsync();

        Assert.Equal(57, metrics.ServerFps);
        Assert.Equal(16.7671, metrics.ServerFrameTimeMilliseconds);
        Assert.Equal(32, metrics.BaseCampCount);
        Assert.Equal("http://localhost:8212/v1/api/metrics", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetServerSettingsAsync_DeserializesSettingsAndPreservesUnknownProperties()
    {
        using var handler = new RecordingHandler("""{ "Difficulty":"Normal", "ExpRate":2.0, "bIsPvP":true, "ServerName":"Test", "RESTAPIEnabled":true, "FutureSetting":"value" }""");
        using var httpClient = new HttpClient(handler);
        using var client = new PalworldClient(new PalworldServerOptions("http://localhost:8212", "admin", "secret"), httpClient);

        var settings = await client.Server.SettingsAsync();

        Assert.Equal("Normal", settings.Difficulty);
        Assert.Equal(2.0, settings.ExpRate);
        Assert.True(settings.BIsPvP);
        Assert.True(settings.RESTAPIEnabled);
        Assert.True(settings.AdditionalSettings!.ContainsKey("FutureSetting"));
        Assert.Equal("http://localhost:8212/v1/api/settings", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetWorldActorSnapshotAsync_DeserializesKnownAndUnknownActors()
    {
        using var handler = new RecordingHandler("""
            {
              "Time":"2026-06-17 13:00:40", "FPS":91.71, "AverageFPS":33.78,
              "ActorData":[
                { "Type":"Character", "InstanceID":"player-1", "UnitType":"Player", "userid":"steam_1", "HP":100, "MaxHP":100, "LocationX":1.0, "LocationY":2.0, "LocationZ":3.0 },
                { "Type":"PalBox", "GuildID":"guild-1", "LocationX":4.0, "LocationY":5.0, "LocationZ":6.0 },
                { "Type":"FutureActor", "Value":1 }
              ]
            }
            """);
        using var httpClient = new HttpClient(handler);
        using var client = new PalworldClient(new PalworldServerOptions("http://localhost:8212", "admin", "secret"), httpClient);

        var snapshot = await client.World.GameDataAsync();

        var character = Assert.IsType<CharacterActor>(snapshot.Actors[0]);
        Assert.Equal("steam_1", character.SteamId);
        Assert.Equal(100, character.HealthPoints);
        Assert.IsType<PalBoxActor>(snapshot.Actors[1]);
        Assert.IsType<UnknownGameActor>(snapshot.Actors[2]);
        Assert.Equal("http://localhost:8212/v1/api/game-data", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetServerInfoAsync_ThrowsBadRequestException_ForStatus400()
    {
        using var handler = new RecordingHandler("Request error", System.Net.HttpStatusCode.BadRequest);
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        var exception = await Assert.ThrowsAsync<PalworldBadRequestException>(async () => await client.Server.InfoAsync());

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Request error", exception.ResponseContent);
    }

    [Fact]
    public async Task GetServerInfoAsync_ThrowsUnauthorizedException_ForStatus401()
    {
        using var handler = new RecordingHandler("Unauthorized", System.Net.HttpStatusCode.Unauthorized);
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        var exception = await Assert.ThrowsAsync<PalworldUnauthorizedException>(async () => await client.Server.InfoAsync());

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("Unauthorized", exception.ResponseContent);
    }

    [Fact]
    public async Task PlayerCommands_RequireSteamId()
    {
        using var httpClient = new HttpClient(new RecordingHandler());
        using var client = CreateClient(httpClient);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => client.Players.BanAsync(" ", "Rule violation"));

        Assert.Equal("steamId", exception.ParamName);
    }

    [Fact]
    public async Task AnnounceAsync_RequiresMessage()
    {
        using var httpClient = new HttpClient(new RecordingHandler());
        using var client = CreateClient(httpClient);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => client.World.AnnounceAsync(" "));

        Assert.Equal("message", exception.ParamName);
    }

    [Fact]
    public async Task GetPlayersAsync_DeserializesAllPlayerFields()
    {
        using var handler = new RecordingHandler("""{ "players":[{"name":"Player", "accountName":"account", "playerId":"player-1", "userId":"steam_1", "ip":"127.0.0.1", "ping":3.14, "location_x":1.0, "location_y":2.0, "level":5, "building_count":4 }] }""");
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        var players = await client.Players.ListAsync();
        var player = Assert.Single(players.Players);

        Assert.Equal("Player", player.Name);
        Assert.Equal("account", player.AccountName);
        Assert.Equal("player-1", player.PlayerId);
        Assert.Equal("steam_1", player.SteamId);
        Assert.Equal("127.0.0.1", player.IpAddress);
        Assert.Equal(3.14, player.Ping);
        Assert.Equal(1.0, player.LocationX);
        Assert.Equal(2.0, player.LocationY);
        Assert.Equal(5, player.Level);
        Assert.Equal(4, player.BuildingCount);
    }

    [Fact]
    public async Task GetServerInfoAsync_KeepsApiPathWhenAlreadyNormalized()
    {
        using var handler = new RecordingHandler("""{ "version":"1.0", "servername":"Test", "description":"Server", "worldguid":"world" }""");
        using var httpClient = new HttpClient(handler);
        using var client = new PalworldClient(new PalworldServerOptions("http://localhost:8212/v1/api", "admin", "secret"), httpClient);

        await client.Server.InfoAsync();

        Assert.Equal("http://localhost:8212/v1/api/info", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public async Task KickPlayerAsync_WithoutMessage_SendsUserIdOnly()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.Players.KickAsync("steam_123");

        AssertRequest(handler, "kick", "{\"userid\":\"steam_123\",\"message\":null}");
    }

    [Fact]
    public async Task ShutdownAsync_WithZeroSeconds_SendsWaittimeZero()
    {
        using var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        await client.Server.ShutdownAsync(0);

        AssertRequest(handler, "shutdown", "{\"waittime\":0,\"message\":null}");
    }

    [Fact]
    public async Task ShutdownAsync_ThrowsWhenWaitTimeIsNegative()
    {
        using var httpClient = new HttpClient(new RecordingHandler());
        using var client = CreateClient(httpClient);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.Server.ShutdownAsync(-1));
    }

    [Fact]
    public async Task GetServerInfoAsync_ThrowsPalworldApiException_ForStatus500()
    {
        using var handler = new RecordingHandler("Internal error", System.Net.HttpStatusCode.InternalServerError);
        using var httpClient = new HttpClient(handler);
        using var client = CreateClient(httpClient);

        var exception = await Assert.ThrowsAsync<PalworldApiException>(async () => await client.Server.InfoAsync());

        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Equal("Internal error", exception.ResponseContent);
    }

    private static PalworldClient CreateClient(HttpClient httpClient) =>
        new(new PalworldServerOptions("http://localhost:8212", "admin", "secret"), httpClient);

    private static void AssertRequest(RecordingHandler handler, string route, string? body)
    {
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal($"http://localhost:8212/v1/api/{route}", handler.Request.RequestUri!.ToString());
        Assert.Equal(body, handler.Body);
    }

    private sealed class RecordingHandler(string response = "{}", System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(statusCode) { Content = new StringContent(response) };
        }
    }
}
