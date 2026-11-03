using System.Globalization;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CpuSampler>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<CpuSampler>());
var app = builder.Build();

app.UseStaticFiles();
app.Use(async (context, next) =>
{
    context.Response.Headers.CacheControl = "no-store";
    await next(context);
});

app.MapGet("/", async (IWebHostEnvironment environment) =>
{
    var html = await File.ReadAllTextAsync(Path.Combine(environment.ContentRootPath, "Views", "index.html"));
    return Results.Content(html.Replace("{{started}}", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)), "text/html");
});

app.MapGet("/api/counter", (long started) =>
{
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    if (started < 0 || started > now) return Results.BadRequest("Invalid start time.");
    return Results.Content($"<span class=\"metric-value\">{(now - started) / 10_000:N0}</span><span class=\"metric-unit\">ticks</span>", "text/html");
});

app.MapGet("/api/cpu", (CpuSampler cpu) =>
{
    var value = cpu.Average;
    var number = value?.ToString("F1", CultureInfo.InvariantCulture) ?? "—";
    var caption = value.HasValue ? "Average utilization · last 10 seconds" : cpu.IsSupported ? "Collecting CPU samples…" : "CPU sampling requires Windows";
    return Results.Content($"""
        <div class="metric-line"><span class="metric-value">{number}</span><span class="metric-unit">%</span></div>
        <div class="meter" role="meter" aria-label="Average CPU utilization" aria-valuemin="0" aria-valuemax="100" {(value.HasValue ? $"aria-valuenow=\"{number}\"" : "")}><span style="width:{number.Replace("—", "0")}%"></span></div>
        <p class="metric-note">{caption}</p>
        """, "text/html");
});

app.MapGet("/api/time", () =>
{
    var now = DateTimeOffset.Now;
    return Results.Content($"""
        <time datetime="{now:O}"><span class="clock-value">{now:HH:mm:ss}</span><span class="date-value">{now.ToString("dddd, MMMM d, yyyy", CultureInfo.InvariantCulture)}</span></time>
        <p class="metric-note">Server time · UTC{now:zzz}</p>
        """, "text/html");
});

app.MapGet("/api/records", () =>
{
    string[] names = ["North gateway", "Main entrance", "Warehouse east", "Loading bay", "Office network", "South gateway", "Parking level 1", "Reception", "Workshop", "Storage cluster"];
    string[] types = ["Gateway", "Camera", "Sensor", "Controller"];
    var html = new StringBuilder();
    for (var i = 0; i < names.Length; i++)
    {
        var warning = Random.Shared.Next(5) == 0;
        var utilization = Random.Shared.Next(8, 87);
        html.Append(CultureInfo.InvariantCulture, $"""
            <tr><td class="record-id">DV-{1001 + i}</td><td><span class="device-name">{WebUtility.HtmlEncode(names[i])}</span></td><td>{types[i % types.Length]}</td>
            <td><span class="status {(warning ? "warning" : "healthy")}"><span></span>{(warning ? "Attention" : "Healthy")}</span></td>
            <td><div class="table-load"><span class="mini-meter"><span style="width:{utilization}%"></span></span><span>{utilization}%</span></div></td>
            <td class="numeric">{Random.Shared.Next(12, 145)} ms</td><td class="last-seen">{DateTimeOffset.Now:HH:mm:ss}</td></tr>
            """);
    }
    return Results.Content(html.ToString(), "text/html");
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();
