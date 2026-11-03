# DVMS HTMX Dashboard

A small dashboard built with .NET 10, ASP.NET Core Minimal APIs, and HTMX. No npm build or external NuGet packages are required. HTMX is vendored locally, so the dashboard works without an internet connection.

## Run

Install the .NET 10 SDK, then run from this directory:

```powershell
dotnet run
```

Open http://localhost:5080. Stop the server with Ctrl+C. The project also opens directly in Visual Studio using `Dashboard.csproj`.

## Behavior

- **Counter:** starts at 0 whenever the page opens. HTMX polls every 10 seconds; the server calculates elapsed 10-second intervals from that page's start timestamp. Separate tabs have independent counters. If a browser suspends a tab, the counter catches up on its next request.
- **CPU:** real server CPU utilization, sampled once per second and averaged over the most recent 10 samples, refreshed in the panel every 10 seconds. During startup it averages the available samples. Uses Windows `GetSystemTimes`; other operating systems display an unavailable message. This measures host utilization, not just the web application's process. On Windows hosts with more than 64 logical processors, the Windows API reports the calling thread's processor group.
- **Clock:** server local date/time, including its UTC offset, refreshed every second.
- **Grid:** 10 synthetic device records loaded on page open. Refresh data generates new utilization, latency, and status values; these are demo values, not monitored devices.
- Panels retry on their next polling interval after a connection failure; a failed grid request can be retried with Refresh data.

## Files and routes

- `Program.cs`: Minimal API routes; HTML fragment rendering.
- `CpuSampler.cs`: shared background CPU sampler, independent of browser requests.
- `Views/index.html`: dashboard markup and HTMX polling attributes.
- `wwwroot/`: CSS, connection notice script, and local HTMX 2.0.8 distribution.
- `GET /`: dashboard.
- `GET /api/counter?started=<Unix milliseconds>`: counter fragment.
- `GET /api/cpu`: CPU fragment.
- `GET /api/time`: clock fragment.
- `GET /api/records`: table row fragments.
- `GET /health`: JSON health response.

The `/api` endpoints return HTML for HTMX to swap directly into the page. Replace the sample record generation in `/api/records` to connect a real data source.

References: [HTMX polling](https://htmx.org/docs/#polling), [Windows GetSystemTimes](https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getsystemtimes). HTMX is distributed under the Zero-Clause BSD license; see `wwwroot/vendor/HTMX-LICENSE`.
