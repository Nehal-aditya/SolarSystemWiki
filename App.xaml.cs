using Microsoft.UI.Xaml;
using SolarSystemWiki.Services;
using SolarSystemWiki.ViewModels;

namespace SolarSystemWiki;

public partial class App : Application
{
    public static MainWindow? MainWindow { get; private set; }
    public static MainViewModel ViewModel { get; private set; } = null!;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var dataService = new WikiDataService();
        ViewModel = new MainViewModel(dataService);

        MainWindow = new MainWindow();
        MainWindow.Activate();
    }
}
