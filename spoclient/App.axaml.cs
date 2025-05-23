using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using LocalizationManager;
using Microsoft.Data.Sqlite;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using spoclient.Pages;
using spoclient.Regions;
using spoclient.ViewModels;
using spoclient.Views;
using SpoClient.Setting;
using SpoClient.Setting.Models;
using System.Globalization;
using System.Threading.Tasks;

namespace spoclient
{
    public partial class App : PrismApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }


        protected override AvaloniaObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterSingleton<SqliteConnection?>(() =>
            {
                return Task.Run<SqliteConnection?>(async () =>
                {
                    var c = await SettingManager.Instance.OpenAsync("spoclient.setting");
                    var migrator = new Migrator(c!);
                    await migrator.ApplyMigrationsFromResourcesAsync("SpoClient.Setting.SQL.Migrations");
                    return c;
                }).Result;
            });
            containerRegistry.RegisterSingleton<SettingManager>(() => SettingManager.Instance);
            containerRegistry.RegisterSingleton<IAppSettings, AppSettings>();
            containerRegistry.RegisterSingleton<ILocalizationManager>((c) =>
            {
                var lm =  LocalizationManagerExtensions.Default!;
                var settings = c.Resolve<IAppSettings>();
                var culture = settings[AppSettingKeys.Culture];
                lm.CurrentCulture = new CultureInfo(culture ?? "en-US");
                return lm;
            });
            containerRegistry.RegisterForNavigation<MainView, MainViewViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();

            containerRegistry.RegisterDialog<ServersDialog, ServersDialogViewModel>();
            containerRegistry.RegisterDialog<ServerDialog, ServerDialogViewModel>();
            containerRegistry.RegisterDialog<AboutDialog, AboutDialogViewModel>();

            containerRegistry.RegisterDialogWindow<DialogWindow>();
        }


        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
        }


        protected override void OnInitialized()
        {
            var regionManager = Container.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("RootRegion", typeof(MainView));
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(HomePage));
            regionManager.RegisterViewWithRegion("TabView", typeof(TabView));
        }


        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            regionAdapterMappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
            regionAdapterMappings.RegisterMapping<TabView, MainTabControlRegionAdapter>();
        }



        //public override void OnFrameworkInitializationCompleted()
        //{
        //    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        //    {
        //        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        //        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        //        DisableAvaloniaDataAnnotationValidation();
        //        desktop.MainWindow = new MainWindow
        //        {
        //            DataContext = new MainWindowViewModel(),
        //        };
        //    }

        //    base.OnFrameworkInitializationCompleted();
        //}

        //private void DisableAvaloniaDataAnnotationValidation()
        //{
        //    // Get an array of plugins to remove
        //    var dataValidationPluginsToRemove =
        //        BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        //    // remove each entry found
        //    foreach (var plugin in dataValidationPluginsToRemove)
        //    {
        //        BindingPlugins.DataValidators.Remove(plugin);
        //    }
        //}
    }
}