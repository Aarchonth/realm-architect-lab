using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        service = "realm-architect-api",
        version = "0.2.0",
        timestampUtc = DateTime.UtcNow
    });
});

app.MapGet("/api/info", () =>
{
    return Results.Ok(new
    {
        project = "Realm Architect Lab",
        description = "Self-hosted infrastructure API",
        environment = app.Environment.EnvironmentName
    });
});

app.MapGet("/api/services", async () =>
{
    var services = new List<object>();

    services.Add(new
    {
        name = "Backend API",
        status = "online",
        type = "api",
        publicName = "lab.realm-architect.dev/api/health"
    });

    services.Add(new
    {
        name = "Public Lab Website",
        status = await CheckHttpAsync("https://lab.realm-architect.dev") ? "online" : "offline",
        type = "https",
        publicName = "lab.realm-architect.dev"
    });

    services.Add(new
    {
        name = "Minecraft Server",
        status = await CheckTcpAsync(
            GetEnv("MINECRAFT_TCP_HOST", "127.0.0.1"),
            GetEnvInt("MINECRAFT_TCP_PORT", 25566)
        ) ? "online" : "offline",
        type = "tcp",
        publicName = "mc.realm-architect.dev"
    });

    services.Add(new
    {
        name = "SSH Server",
        status = await CheckTcpAsync("127.0.0.1", 22) ? "online" : "offline",
        type = "tcp",
        publicName = "Private / VPN only"
    });

    services.Add(new
    {
        name = "Nginx Reverse Proxy",
        status = await CheckTcpAsync("127.0.0.1", 443) ? "online" : "offline",
        type = "https",
        publicName = "lab.realm-architect.dev"
    });

    return Results.Ok(new
    {
        generatedAtUtc = DateTime.UtcNow,
        services
    });
});

app.MapGet("/api/minecraft", async () =>
{
    var tcpOnline = await CheckTcpAsync(
        GetEnv("MINECRAFT_TCP_HOST", "127.0.0.1"),
        GetEnvInt("MINECRAFT_TCP_PORT", 25566)
    );

    var rconResult = await GetMinecraftPlayerStatusAsync();

    return Results.Ok(new
    {
        name = "Realm Architect Minecraft",
        address = "mc.realm-architect.dev",
        status = tcpOnline ? "online" : "offline",
        currentPlayers = rconResult.CurrentPlayers,
        maxPlayers = rconResult.MaxPlayers,
        players = rconResult.Players.Select(playerName => new
	{
    		name = playerName,
    		avatarUrl = $"https://mc-heads.net/avatar/{Uri.EscapeDataString(playerName)}/48"
	}),
        playerStatusAvailable = rconResult.Available,
        checkedAtUtc = DateTime.UtcNow
    });
});

app.Run("http://127.0.0.1:5050");

static string GetEnv(string name, string fallback)
{
    return Environment.GetEnvironmentVariable(name) ?? fallback;
}

static int GetEnvInt(string name, int fallback)
{
    var value = Environment.GetEnvironmentVariable(name);

    return int.TryParse(value, out var result) ? result : fallback;
}

static async Task<bool> CheckTcpAsync(string host, int port)
{
    try
    {
        using var client = new TcpClient();

        var connectTask = client.ConnectAsync(host, port);
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));

        var completedTask = await Task.WhenAny(connectTask, timeoutTask);

        return completedTask == connectTask && client.Connected;
    }
    catch
    {
        return false;
    }
}

static async Task<bool> CheckHttpAsync(string url)
{
    try
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(3)
        };

        var response = await httpClient.GetAsync(url);

        return response.IsSuccessStatusCode;
    }
    catch
    {
        return false;
    }
}

static async Task<MinecraftPlayerStatus> GetMinecraftPlayerStatusAsync()
{
    var password = Environment.GetEnvironmentVariable("MINECRAFT_RCON_PASSWORD");

    if (string.IsNullOrWhiteSpace(password))
    {
        return new MinecraftPlayerStatus(false, null, null, Array.Empty<string>());
    }

    var host = GetEnv("MINECRAFT_RCON_HOST", "127.0.0.1");
    var port = GetEnvInt("MINECRAFT_RCON_PORT", 25575);

    try
    {
        var response = await SendRconCommandAsync(host, port, password, "list");

        return ParseMinecraftListResponse(response);
    }
    catch
    {
        return new MinecraftPlayerStatus(false, null, null, Array.Empty<string>());
    }
}

static MinecraftPlayerStatus ParseMinecraftListResponse(string response)
{
    var match = Regex.Match(response, @"There are (\d+) of a max of (\d+) players online: ?(.*)");

    if (!match.Success)
    {
        return new MinecraftPlayerStatus(false, null, null, Array.Empty<string>());
    }

    var currentPlayers = int.Parse(match.Groups[1].Value);
    var maxPlayers = int.Parse(match.Groups[2].Value);

    var playerText = match.Groups[3].Value.Trim();

    var players = string.IsNullOrWhiteSpace(playerText)
        ? Array.Empty<string>()
        : playerText.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    return new MinecraftPlayerStatus(true, currentPlayers, maxPlayers, players);
}

static async Task<string> SendRconCommandAsync(string host, int port, string password, string command)
{
    using var client = new TcpClient();

    await client.ConnectAsync(host, port);

    await using var stream = client.GetStream();

    var requestId = Random.Shared.Next(1, int.MaxValue);

    await SendRconPacketAsync(stream, requestId, 3, password);

    var authResponse = await ReadRconPacketAsync(stream);

    if (authResponse.RequestId == -1)
    {
        throw new InvalidOperationException("RCON authentication failed.");
    }

    var commandRequestId = Random.Shared.Next(1, int.MaxValue);

    await SendRconPacketAsync(stream, commandRequestId, 2, command);

    var commandResponse = await ReadRconPacketAsync(stream);

    return commandResponse.Payload;
}

static async Task SendRconPacketAsync(NetworkStream stream, int requestId, int type, string payload)
{
    var payloadBytes = Encoding.UTF8.GetBytes(payload);
    var packetSize = 4 + 4 + payloadBytes.Length + 2;

    using var memoryStream = new MemoryStream();

    await memoryStream.WriteAsync(BitConverter.GetBytes(packetSize));
    await memoryStream.WriteAsync(BitConverter.GetBytes(requestId));
    await memoryStream.WriteAsync(BitConverter.GetBytes(type));
    await memoryStream.WriteAsync(payloadBytes);
    await memoryStream.WriteAsync(new byte[] { 0, 0 });

    var packet = memoryStream.ToArray();

    await stream.WriteAsync(packet);
}

static async Task<RconPacket> ReadRconPacketAsync(NetworkStream stream)
{
    var sizeBuffer = new byte[4];

    await ReadExactAsync(stream, sizeBuffer, 4);

    var packetSize = BitConverter.ToInt32(sizeBuffer, 0);
    var packetBuffer = new byte[packetSize];

    await ReadExactAsync(stream, packetBuffer, packetSize);

    var requestId = BitConverter.ToInt32(packetBuffer, 0);
    var type = BitConverter.ToInt32(packetBuffer, 4);

    var payloadLength = packetSize - 10;
    var payload = payloadLength > 0
        ? Encoding.UTF8.GetString(packetBuffer, 8, payloadLength)
        : string.Empty;

    return new RconPacket(requestId, type, payload);
}

static async Task ReadExactAsync(NetworkStream stream, byte[] buffer, int length)
{
    var offset = 0;

    while (offset < length)
    {
        var read = await stream.ReadAsync(buffer.AsMemory(offset, length - offset));

        if (read == 0)
        {
            throw new IOException("Unexpected end of stream.");
        }

        offset += read;
    }
}

record RconPacket(int RequestId, int Type, string Payload);

record MinecraftPlayerStatus(
    bool Available,
    int? CurrentPlayers,
    int? MaxPlayers,
    string[] Players
);
