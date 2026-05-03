# PulseMon

Real-time CPU and GPU usage monitor for Windows 11, built with WPF and .NET 10.

![Platform](https://img.shields.io/badge/platform-Windows%2011-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)

## Features

- Live CPU and GPU usage in percent
- Scrolling 60-second line graph for both
- Compact, always-on-top window
- Dark theme with custom title bar

## Requirements

- Windows 10/11
- [.NET 10 Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)
- Administrator rights (required for GPU sensor access)

## Getting Started

```bash
git clone https://github.com/michaelflarsen/pulsemon.git
cd pulsemon/PulseMon
dotnet run
```

> **Note:** Windows will show a UAC prompt on launch — click **Yes** to allow hardware sensor access.

## Tech Stack

| | |
|---|---|
| UI | WPF / .NET 10 |
| GPU sensors | [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) |
| Charts | [ScottPlot](https://scottplot.net/) |
| MVVM | [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) |

## License

MIT
