using WPF_MVVM_CRUD.Models;
using WPF_MVVM_CRUD.Services;
using WPF_MVVM_CRUD.ViewModels.Base;
using System.Windows.Input;
using WPF_MVVM_CRUD.Commands;
using System.Windows;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WPF_MVVM_CRUD.ViewModels
{
    public class AddEditWindowViewModel : DialogViewModel
    {
        private readonly IUserDialog _userDialog = null!;

        public AddEditWindowViewModel()
        {
            Title = "Окно с данными пользователя";

            using (ApplicationContext db = new ApplicationContext())
            {
                //RoleName = db.Roles.Select(r => r.Name).ToList();

                CurrentUser = new User();

                RoleName = db.Roles.ToList();
            }

            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
        }

        public AddEditWindowViewModel(IUserDialog userDialog) : this()
        {
            _userDialog = userDialog;
        }


        #region User CurrentUser

        private User _currentUser;

        public User CurrentUser { get => _currentUser; set => Set(ref _currentUser, value); }

        #endregion

        #region RoleName

        private List<Role> _roleName;
        public List<Role> RoleName { get => _roleName; set => Set(ref _roleName, value); }

        //private List<string> _roleName;
        //public List<string> RoleName { get => _roleName; set => Set(ref _roleName, value); }

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
    }
}
