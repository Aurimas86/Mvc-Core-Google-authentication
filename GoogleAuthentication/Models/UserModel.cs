using System;

namespace GoogleAuthentication.Models
{
    public class UserModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
