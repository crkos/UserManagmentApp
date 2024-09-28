namespace UserManagmentApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime RegistrationTime { get; set; }
        public string Status { get; set; }

    }
}
