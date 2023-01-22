
using System.Windows.Input;
using System.Windows;
using System;
using WPF_MVVM_CRUD.Models;
using WPF_MVVM_CRUD.ViewModels.Base;
using WPF_MVVM_CRUD.Commands;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace WPF_MVVM_CRUD.ViewModels
{
    public class AddEditWindowViewModel : ViewModel
    {
        public AddEditWindowViewModel()
        {
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);

            CurrentUser = new User();

            using (ApplicationContext db = new ApplicationContext())
            {
                CurrentUsers = db.Users.Include(r => r.Role).ToList();

                RoleName = db.Roles.ToList();
            }
        }


        #region RoleName
        private List<Role> _roleName;
        public List<Role> RoleName { get => _roleName; set => Set(ref _roleName, value); }
        #endregion

        #region IEnumerable CurrentUsers
        private IEnumerable _currentUsers;
        public IEnumerable CurrentUsers { get => _currentUsers; set => Set(ref _currentUsers, value); }
        #endregion

        #region User CurrentUser
        private User _currentUser;
        public User CurrentUser { get => _currentUser; set => Set(ref _currentUser, value); }
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
