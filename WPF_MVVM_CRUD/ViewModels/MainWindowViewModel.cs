using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPF_MVVM_CRUD.Commands;
using WPF_MVVM_CRUD.Models;
using WPF_MVVM_CRUD.Services;
using WPF_MVVM_CRUD.ViewModels.Base;

namespace WPF_MVVM_CRUD.ViewModels
{
    public class MainWindowViewModel : DialogViewModel
    {
        private readonly IUserDialog _userDialog = null!;

        public MainWindowViewModel()
        {
            Title = "Главное окно";

            using (ApplicationContext db = new ApplicationContext())
            {
                CurrentUsers = db.Users.Include(r => r.Role).ToList();
            }

            EditUserCommand = new LambdaCommand(OnEditUserCommandExecuted, CanEditUserCommandExecute);
            DeleteUserCommand = new LambdaCommand(OnDeleteUserCommandExecuted, CanDeleteUserCommandExecute);
            OpenAddEditWindowCommand = new LambdaCommand(OnOpenAddEditWindowCommandExecuted, CanOpenAddEditWindowCommandExecute);

        }

        public MainWindowViewModel(IUserDialog userDialog) : this()
        {
            _userDialog = userDialog;
        }

        #region IEnumerable CurrentUsers

        private IEnumerable _currentUsers;

        public IEnumerable CurrentUsers { get => _currentUsers; set => Set(ref _currentUsers, value); }

        #endregion

        #region DeleteUserCommand

        public ICommand DeleteUserCommand { get; set; }
        private bool CanDeleteUserCommandExecute(object p)
        {
            if ((User)p != null) return true;

            return false;
        }
        private void OnDeleteUserCommandExecuted(object p)
        {
            if ((User)p != null)
            {
                if (MessageBox.Show($"Вы точно хотите удалить {((User)p).Name}", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.Users.Remove((User)p);
                        db.SaveChanges();

                        MessageBox.Show($"Книга {((User)p).Name} удален!");

                        CurrentUsers = db.Users.Include(r => r.Role).ToList();
                    }
                }
            }
        }

        #endregion


        #region Команда OpenAddEditWindowCommand

        public ICommand OpenAddEditWindowCommand { get; set; }

        private bool CanOpenAddEditWindowCommandExecute(object p) => true;

        private void OnOpenAddEditWindowCommandExecuted(object p)
        {
            _userDialog.OpenAddEditWindow();
        }

        #endregion

        #region Команда EditUserCommand
        public ICommand EditUserCommand { get; set; }

        private bool CanEditUserCommandExecute(object p)
        {
            if ((User)p != null) return true;

            return false;
        }

        private void OnEditUserCommandExecuted(object p)
        {
            AddEditWindowViewModel addEditWindowViewModel = new AddEditWindowViewModel();

            addEditWindowViewModel.CurrentUser = (User)p;

            _userDialog.UserEditWindow((User)p, addEditWindowViewModel);
        }

        #endregion
    }
}
