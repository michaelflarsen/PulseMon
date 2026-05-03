using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using PulseMon.Services;
using PulseMon.ViewModels;
using ScottPlot;

namespace PulseMon.Views;

public partial class MainWindow : Window
{
    private MainViewModel _viewModel = null!;
    private DispatcherTimer? _refreshTimer;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel(new HardwareMonitorService());
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ConfigurePlot(CpuPlot, "#4FC3F7", isCpu: true);
        ConfigurePlot(GpuPlot, "#81C784", isCpu: false);

        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _refreshTimer.Tick += (_, _) =>
        {
            if (_viewModel.IsAutoScale)
            {
                CpuPlot.Plot.Axes.AutoScale();
                GpuPlot.Plot.Axes.AutoScale();
            }
            else
            {
                CpuPlot.Plot.Axes.SetLimitsY(0, 100);
                GpuPlot.Plot.Axes.SetLimitsY(0, 100);
            }
            CpuPlot.Refresh();
            GpuPlot.Refresh();
        };
        _refreshTimer.Start();
    }

    private void ConfigurePlot(ScottPlot.WPF.WpfPlot wpfPlot, string hexColor, bool isCpu)
    {
        var plot = wpfPlot.Plot;

        // Baggrundsfarver matcher Border
        plot.FigureBackground.Color = Color.FromHex("#242424");
        plot.DataBackground.Color = Color.FromHex("#242424");

        // Opret DataStreamer med 60 datapunkter (= 60 sekunders rullende vindue)
        var streamer = plot.Add.DataStreamer(60);
        streamer.LineStyle.Color = Color.FromHex(hexColor);
        streamer.LineStyle.Width = 2;
        streamer.ViewScrollLeft();

        // Lås Y-aksen til 0-100%
        plot.Axes.SetLimitsY(0, 100);

        // Stil på akser
        plot.Axes.Left.TickLabelStyle.ForeColor = Color.FromHex("#9E9E9E");
        plot.Axes.Left.FrameLineStyle.Color = Color.FromHex("#333333");
        plot.Axes.Left.MajorTickStyle.Color = Color.FromHex("#333333");
        plot.Axes.Left.MinorTickStyle.Color = Color.FromHex("#2A2A2A");
        plot.Axes.Bottom.IsVisible = false;
        plot.Axes.Right.IsVisible = false;
        plot.Axes.Top.IsVisible = false;

        // Grid
        plot.Grid.MajorLineColor = Color.FromHex("#2D2D2D");
        plot.Grid.MinorLineColor = Color.FromHex("#252525");

        // Tilpassede Y-ticks: 0, 25, 50, 75, 100
        plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            new double[] { 0, 25, 50, 75, 100 },
            new string[] { "0%", "25%", "50%", "75%", "100%" });

        // Grid konfiguration
        plot.Grid.MajorLineColor = Color.FromHex("#2D2D2D");

        if (isCpu)
            _viewModel.CpuStreamer = streamer;
        else
            _viewModel.GpuStreamer = streamer;

        wpfPlot.Refresh();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
        else
        {
            DragMove();
        }
    }

    private void ResizeGrip_DragDelta(object sender, DragDeltaEventArgs e)
    {
        Width  = Math.Max(MinWidth,  Width  + e.HorizontalChange);
        Height = Math.Max(MinHeight, Height + e.VerticalChange);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        _refreshTimer?.Stop();
        _viewModel.Dispose();
        base.OnClosed(e);
    }
}
