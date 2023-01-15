namespace WPF_MVVM_CRUD.Services
{
    public interface IUserDialog
    {
        void OpenMainWindow();
        void OpenAddEditWindow();
        void UserEditWindow(object p, object dataContext);
    }
}
