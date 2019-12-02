namespace ResourceServer.Models
{
    public class User
    {
        public string ID_User { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public decimal? Rate { get; set; }
        public bool isBlocked { get; set; }
        public string PhoneNumber { get; set; }
        public int IDRole { get; set; }
    }
}