# CRUD MVVM
## Пример простого crud приложения с подходом code first
### В таблице `User` внешний ключ RoleId `FOREIGN KEY(RoleId) REFERENCES Role(Id)`

### Base Command.cs
```c#
using System;
using System.Windows.Input;

namespace WPF_MVVM_CRUD.Commands.Base
{
    public abstract class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public abstract bool CanExecute(object? parameter);

        public abstract void Execute(object? parameter);
    }
}
```

### LambdaCommand.cs
```c#
using System;
using WPF_MVVM_CRUD.Commands.Base;

namespace WPF_MVVM_CRUD.Commands
{
    public class LambdaCommand : Command
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public LambdaCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public override void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
```

### Models - данные
* User.cs
```c#
namespace WPF_MVVM_CRUD.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
```

* Role.cs
```c#
using System.Collections.Generic;

namespace WPF_MVVM_CRUD.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
```
Подход code first
* ApplicationContext.cs
```c#
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace WPF_MVVM_CRUD.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["ConnectionSqlite"].ToString());
        }

        //Инициализация начальными данными 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role[]
                {
                    new Role { Id=1, Name="Администратор"},
                    new Role { Id=2, Name="Менеджер"},
                    new Role { Id=3, Name="Клиент"}
                });

            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                    new User { Id=1, Name="Tom",RoleId = 1},
                    new User { Id=2, Name="Alice",RoleId = 3},
                    new User { Id=3, Name="Rob",RoleId = 2 }
                });
        }
    }
}
```

## Base ViewModels.cs
```c#
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_MVVM_CRUD.ViewModels.Base
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual bool Set<T>(ref T filed, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(filed, value)) return false;

            filed = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
    }
}
```

## MainWindowViewModel.cs
```c#
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

                        MessageBox.Show($"Книга {((User)p).Name} удален!");

                        CurrentUsers = db.Users.Include(r => r.Role).ToList();
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

                CurrentUsers = db.Users.Include(r => r.Role).ToList();
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
```

## Windows
* AddEditWindow.xaml
```c#
 Title="AddEditWindow" Height="450" Width="600">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="391*"/>
            <RowDefinition Height="43*"/>
        </Grid.RowDefinitions>

        <StackPanel>
            <TextBlock Text="Имя"/>
            <TextBox Text="{Binding CurrentUser.Name}"/>

            <TextBlock Text="Роль"/>
            <ComboBox ItemsSource="{Binding RoleName}"  
                      DisplayMemberPath="Name"                      
                      SelectedValue="{Binding CurrentUser.Role}"/>

        </StackPanel>

        <StackPanel Orientation="Horizontal" Grid.Row="1" HorizontalAlignment="Center" VerticalAlignment="Center">
            <Button 
                Content="Сохранить"              
                Command="{Binding SaveCommand}"/>
        </StackPanel>
    </Grid>
```

* MainWindow.xaml
```c#
  xmlns:vm="clr-namespace:WPF_MVVM_CRUD.ViewModels" 
        xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
        mc:Ignorable="d"
        Title="{Binding Title}" Height="450" Width="600">

    <Window.DataContext>
        <vm:MainWindowViewModel/>
    </Window.DataContext>

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <DataGrid Grid.Row="0" 
                  x:Name="userGrid"                  
                  IsReadOnly="True"
                  AutoGenerateColumns="False"
                  ItemsSource="{Binding CurrentUsers}">

            <DataGrid.Columns>
                <DataGridTextColumn Header="Имя" Binding="{Binding Name}"/>
                <DataGridTextColumn Header="Роль" Binding="{Binding Role.Name}"/>
            </DataGrid.Columns>

            <i:Interaction.Triggers>
                <i:EventTrigger EventName="MouseDoubleClick">
                    <i:InvokeCommandAction Command="{Binding EditUserCommand}" CommandParameter="{Binding ElementName=userGrid, Path=SelectedItem}"/>
                </i:EventTrigger>
            </i:Interaction.Triggers>

        </DataGrid>

        <StackPanel Orientation="Horizontal" Grid.Row="1" HorizontalAlignment="Center" VerticalAlignment="Center">
            <Button Content="Добавить"  Margin="0, 0, 10, 0" Command="{Binding AddUserCommand}"/>
            <Button Content="Изменить"  Margin="0, 0, 10, 0" Command="{Binding EditUserCommand}"
                    CommandParameter="{Binding ElementName=userGrid, Path=SelectedItem}"/>
            <Button Content="Удалить" Command="{Binding DeleteUserCommand}"
                    CommandParameter="{Binding ElementName=userGrid, Path=SelectedItem}"/>
        </StackPanel>
    </Grid>
```

### App.config
```c#
<?xml version="1.0" encoding="utf-8" ?>
<configuration>

	<connectionStrings>
		<add
			name ="ConnectionSqlite"
			connectionString="Data Source=WPF-MVVM-CRUD.db"/>
	</connectionStrings>

</configuration>
```
