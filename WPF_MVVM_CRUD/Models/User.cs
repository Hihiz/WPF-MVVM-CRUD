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
