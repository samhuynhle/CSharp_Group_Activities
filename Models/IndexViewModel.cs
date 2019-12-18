namespace qwerty.Models
{
    // Wrapper model for handling login, registration, and creating new activities
    public class IndexViewModel
    {
        public User newUser {get;set;}
        public LoginUser loginUser {get;set;}
        public DojoActivity newDojoActivity {get;set;}
    }
}