using PulseMon.Models;

namespace PulseMon.Services;

public interface IHardwareMonitorService : IDisposable
{
    event EventHandler<HardwareReading> ReadingAvailable;
    void Start(TimeSpan interval);
    void Stop();
}
