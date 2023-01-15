using Microsoft.Extensions.DependencyInjection;
using System;
using WPF_MVVM_CRUD.Services.Implementations;
using WPF_MVVM_CRUD.Services;
using WPF_MVVM_CRUD.ViewModels;
using WPF_MVVM_CRUD.Views.Windows;
using System.Windows;

namespace WPF_MVVM_CRUD
{
    public partial class App
    {
        private static IServiceProvider? _services;

        public static IServiceProvider Services => _services ??= InitializeServices().BuildServiceProvider();
        private static IServiceCollection InitializeServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MainWindowViewModel>();
            services.AddScoped<AddEditWindowViewModel>();

            services.AddSingleton<IUserDialog, UserDialogService>();

            services.AddTransient(
                s =>
                {
                    var model = s.GetRequiredService<MainWindowViewModel>();
                    var window = new MainWindow { DataContext = model };

                    return window;
                });

            services.AddTransient(
                s =>
                {
                    var scope = s.CreateScope();
                    var model = scope.ServiceProvider.GetRequiredService<AddEditWindowViewModel>();
                    var window = new AddEditWindow { DataContext = model };

                    window.Closed += (_, _) => scope.Dispose();

                    return window;
                });

            return services;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Services.GetRequiredService<MainWindow>().Show();
            Services.GetRequiredService<IUserDialog>().OpenMainWindow();
        }
    }
}
