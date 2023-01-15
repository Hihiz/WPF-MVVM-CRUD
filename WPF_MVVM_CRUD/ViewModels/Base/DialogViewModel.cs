using System;

namespace WPF_MVVM_CRUD.ViewModels.Base
{
    public class DialogViewModel : TitleViewModel
    {
        //Событие
        public event EventHandler DialogComplete;

        //Метод которое событие генерирует
        protected virtual void OnDialogComlete(EventArgs e) => DialogComplete.Invoke(this, e);
    }
}
