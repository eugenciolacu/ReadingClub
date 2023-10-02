namespace ReadingClub.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email 
        { 
            get { return _email; }  
            set { _email = value.ToLower(); } 
        }
        public string Password { get; set; } = null!;

        public string Salt { get; set; } = null!;

        private string _email = string.Empty;
    }
}
