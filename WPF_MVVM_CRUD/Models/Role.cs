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
