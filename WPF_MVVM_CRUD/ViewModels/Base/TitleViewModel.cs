namespace WPF_MVVM_CRUD.ViewModels.Base
{
    public abstract class TitleViewModel : ViewModel
    {
        #region Title - заголовок
        private string _title;
        public string Title { get => _title; set => Set(ref _title, value); }
        #endregion
    }
}
