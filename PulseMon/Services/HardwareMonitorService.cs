using System.Diagnostics;
using System.Timers;
using LibreHardwareMonitor.Hardware;
using PulseMon.Models;

namespace PulseMon.Services;

public sealed class HardwareMonitorService : IHardwareMonitorService
{
    private PerformanceCounter? _cpuCounter;
    private Computer? _computer;
    private ISensor? _gpuLoadSensor;
    private System.Timers.Timer? _timer;
    private bool _disposed;

    public event EventHandler<HardwareReading>? ReadingAvailable;

    public void Start(TimeSpan interval)
    {
        Initialize();
        _timer = new System.Timers.Timer(interval.TotalMilliseconds);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Start();
    }

    public void Stop()
    {
        _timer?.Stop();
    }

    private void Initialize()
    {
        _cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        _ = _cpuCounter.NextValue(); // Første opkald returnerer altid 0 — kassér det

        _computer = new Computer
        {
            IsCpuEnabled = false,
            IsGpuEnabled = true
        };
        _computer.Open();

        foreach (var hardware in _computer.Hardware)
        {
            if (hardware.HardwareType is HardwareType.GpuNvidia
                                      or HardwareType.GpuAmd
                                      or HardwareType.GpuIntel)
            {
                hardware.Update();
                _gpuLoadSensor = hardware.Sensors
                    .FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name == "GPU Core");
                if (_gpuLoadSensor is not null)
                    break;
            }
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_cpuCounter is null) return;

        float cpu = Math.Clamp(_cpuCounter.NextValue(), 0f, 100f);

        float gpu = 0f;
        if (_gpuLoadSensor is not null)
        {
            _gpuLoadSensor.Hardware.Update();
            gpu = Math.Clamp(_gpuLoadSensor.Value ?? 0f, 0f, 100f);
        }

        ReadingAvailable?.Invoke(this, new HardwareReading(cpu, gpu, DateTime.Now));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _timer?.Stop();
        _timer?.Dispose();
        _cpuCounter?.Dispose();
        _computer?.Close();
    }
}
