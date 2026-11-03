using System.Runtime.InteropServices;

/// <summary>Samples host CPU usage across processors, independently of dashboard requests.</summary>
public sealed class CpuSampler(ILogger<CpuSampler> logger) : BackgroundService
{
    private readonly object gate = new();
    private readonly Queue<(ulong Busy, ulong Total)> samples = new();
    public bool IsSupported => OperatingSystem.IsWindows();

    public double? Average
    {
        get
        {
            lock (gate)
            {
                var total = samples.Sum(sample => (double)sample.Total);
                return total == 0 ? null : Math.Clamp(samples.Sum(sample => (double)sample.Busy) / total * 100, 0, 100);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsSupported) return;
        var previous = ReadTimes();
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var current = ReadTimes();
                lock (gate)
                {
                    if (previous.HasValue && current.HasValue && current.Value.Total >= previous.Value.Total && current.Value.Idle >= previous.Value.Idle)
                    {
                        var total = current.Value.Total - previous.Value.Total;
                        var idle = current.Value.Idle - previous.Value.Idle;
                        samples.Enqueue((total - Math.Min(idle, total), total));
                        while (samples.Count > 10) samples.Dequeue();
                    }
                    else samples.Clear();
                }
                previous = current;
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
    }

    private (ulong Idle, ulong Total)? ReadTimes()
    {
        if (GetSystemTimes(out var idle, out var kernel, out var user))
            // Windows kernel time includes idle time; subtract it when calculating busy time.
            return (idle.Value, kernel.Value + user.Value);
        logger.LogWarning("Unable to sample CPU utilization. Win32 error: {Error}", Marshal.GetLastWin32Error());
        return null;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FileTime
    {
        public uint Low;
        public uint High;
        public readonly ulong Value => ((ulong)High << 32) | Low;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemTimes(out FileTime idle, out FileTime kernel, out FileTime user);
}
