namespace PulseMon.Models;

public record HardwareReading(float CpuPercent, float GpuPercent, DateTime Timestamp);
