using Microsoft.Extensions.DependencyInjection;
using System;
using WPF_MVVM_CRUD.Models;
using WPF_MVVM_CRUD.Views.Windows;

namespace WPF_MVVM_CRUD.Services.Implementations
{
    public class UserDialogService : IUserDialog
    {
        private readonly IServiceProvider _services;

        public UserDialogService(IServiceProvider services) => _services = services;

        public void OpenMainWindow()
        {
            MainWindow window = new MainWindow();

            window = _services.GetRequiredService<MainWindow>();

            window.Show();
        }

        public void OpenAddEditWindow()
        {
            AddEditWindow window = new AddEditWindow();
            
            window = _services.GetRequiredService<AddEditWindow>();
          
            window.Show();
        }

        public void UserEditWindow(object p, object dataContext)
        {
            AddEditWindow addEditWindow = new AddEditWindow();

            addEditWindow.DataContext = dataContext;

            addEditWindow.Title = $"Данные пользователя {((User)p).Name}";

            addEditWindow.ShowDialog();
        }
    }
}
