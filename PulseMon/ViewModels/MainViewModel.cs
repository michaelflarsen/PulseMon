using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PulseMon.Models;
using PulseMon.Services;
using ScottPlot.Plottables;

namespace PulseMon.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly IHardwareMonitorService _service;

    [ObservableProperty] private float _cpuPercent;
    [ObservableProperty] private float _gpuPercent;
    [ObservableProperty] private string _cpuLabel = "CPU: --%";
    [ObservableProperty] private string _gpuLabel = "GPU: --%";
    [ObservableProperty] private bool _isAlwaysOnTop;

    // Sættes fra MainWindow.xaml.cs efter ScottPlot-kontrollerne er konfigurerede
    public DataStreamer? CpuStreamer { get; set; }
    public DataStreamer? GpuStreamer { get; set; }

    public MainViewModel(IHardwareMonitorService service)
    {
        _service = service;
        _service.ReadingAvailable += OnReadingAvailable;
        _service.Start(TimeSpan.FromSeconds(1));
    }

    private void OnReadingAvailable(object? sender, HardwareReading reading)
    {
        Application.Current?.Dispatcher.InvokeAsync(() =>
        {
            CpuPercent = reading.CpuPercent;
            GpuPercent = reading.GpuPercent;
            CpuLabel = $"CPU: {reading.CpuPercent:F0}%";
            GpuLabel = $"GPU: {reading.GpuPercent:F0}%";

            CpuStreamer?.Add(reading.CpuPercent);
            GpuStreamer?.Add(reading.GpuPercent);
        });
    }

    [RelayCommand]
    private void ToggleAlwaysOnTop() => IsAlwaysOnTop = !IsAlwaysOnTop;

    public void Dispose()
    {
        _service.ReadingAvailable -= OnReadingAvailable;
        _service.Stop();
        _service.Dispose();
    }
}
