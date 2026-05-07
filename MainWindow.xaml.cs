using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SolarSystemWiki.Views;
using System;
using Windows.Graphics;
using WinRT;

namespace SolarSystemWiki;

public sealed partial class MainWindow : Window
{
    private MicaController? _micaController;
    private SystemBackdropConfiguration? _backdropConfig;
    private AppWindow DeepWindow;

    public MainWindow()
    {
        this.InitializeComponent();

        // Get AppWindow reference
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        DeepWindow = AppWindow.GetFromWindowId(windowId);

        SetupWindowChrome();
        SetupMicaBackdrop();

        ContentFrame.Navigate(typeof(HomePage));        // hook Loaded in code-behind

        DeepWindow.Title = "Solar System Wiki";
        DeepWindow.SetIcon("Assets/AppIcon.ico");
    }

    private void SetupWindowChrome()
    {
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        DeepWindow.Resize(new SizeInt32(1200, 800));

        if (DeepWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = true;
            presenter.IsMaximizable = true;
        }
    }

    private void SetupMicaBackdrop()
    {
        if (!MicaController.IsSupported())
            return;

        _backdropConfig = new SystemBackdropConfiguration
        {
            IsInputActive = true
        };

        _micaController = new MicaController
        {
            Kind = MicaKind.Base
        };

        this.Activated += (_, e) =>
        {
            if (_backdropConfig != null)
                _backdropConfig.IsInputActive =
                    e.WindowActivationState != WindowActivationState.Deactivated;
        };

        this.Closed += (_, _) =>
        {
            _micaController?.Dispose();
            _micaController = null;
        };

        _micaController.AddSystemBackdropTarget(
            this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
        _micaController.SetSystemBackdropConfiguration(_backdropConfig);
    }
}
