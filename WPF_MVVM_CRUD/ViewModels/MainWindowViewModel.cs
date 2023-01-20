using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPF_MVVM_CRUD.Commands;
using WPF_MVVM_CRUD.Models;
using WPF_MVVM_CRUD.ViewModels.Base;
using WPF_MVVM_CRUD.Views.Windows;

namespace WPF_MVVM_CRUD.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Title
        private string _title = "Главное окно";
        public string Title { get => _title; set => Set(ref _title, value); }
        #endregion

        #region IEnumerable CurrentUsers
        private IEnumerable _currentUsers;
        public IEnumerable CurrentUsers { get => _currentUsers; set => Set(ref _currentUsers, value); }
        #endregion

        #region User CurrentUser
        private User _currentUser;
        public User CurrentUser { get => _currentUser; set => Set(ref _currentUser, value); }
        #endregion

        #region RoleName
        private List<Role> _roleName;
        public List<Role> RoleName { get => _roleName; set => Set(ref _roleName, value); }
        #endregion

        #region Command AddUserCommand

        public ICommand AddUserCommand { get; set; }

        private bool CanAddUserCommandExecute(object p) => true;
        private void OnAddUserCommandExecuted(object p)
        {
            AddEditWindow addEditWindow = new AddEditWindow();

            addEditWindow.DataContext = this;
            CurrentUser = new User();

            addEditWindow.Title = "Добавление нового пользователя";
            addEditWindow.ShowDialog();

            using (ApplicationContext db = new ApplicationContext())
            {
                CurrentUsers = db.Users.Include(r => r.Role).ToList();
                RoleName = db.Roles.ToList();
            }
        }

        #endregion

        #region Comand EditUserCommand

        public ICommand EditUserCommand { get; set; }

        private bool CanEditUserCommandExecute(object p)
        {
            if ((User)p != null) return true;

            return false;
        }

        private void OnEditUserCommandExecuted(object p)
        {
            if ((User)p != null)
            {
                AddEditWindow addEditWindow = new AddEditWindow();

                addEditWindow.DataContext = this;
                CurrentUser = (User)p;

                addEditWindow.Title = $"Данные пользователя {((User)p).Name}";
                addEditWindow.ShowDialog();

                using (ApplicationContext db = new ApplicationContext())
                {
                    CurrentUsers = db.Users.Include(r => r.Role).ToList();
                    RoleName = db.Roles.ToList();
                }
            }
        }

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

                        MessageBox.Show($"Пользователь {((User)p).Name} удален!");

                        CurrentUsers = db.Users.Include(r => r.Role).ToList();
                        RoleName = db.Roles.ToList();
                    }
                }
            }
        }

        #endregion

        #region Command SaveCommand

        public ICommand SaveCommand { get; set; }

        private bool CanSaveCommandExecute(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (CurrentUser.Id == 0)
                {
                    User user = new User()
                    {
                        Name = CurrentUser.Name,
                        RoleId = CurrentUser.Role.Id
                    };

                    db.Users.Add(user);
                }
                else
                {
                    db.Users.Update(CurrentUser);
                }

                try
                {
                    db.SaveChanges();

                    MessageBox.Show("Данные сохранены");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                CurrentUsers = db.Users.Include(r => r.Role).ToList();

                RoleName = db.Roles.ToList();
            }

            AddUserCommand = new LambdaCommand(OnAddUserCommandExecuted, CanAddUserCommandExecute);
            EditUserCommand = new LambdaCommand(OnEditUserCommandExecuted, CanEditUserCommandExecute);
            DeleteUserCommand = new LambdaCommand(OnDeleteUserCommandExecuted, CanDeleteUserCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
        }
    }
}
