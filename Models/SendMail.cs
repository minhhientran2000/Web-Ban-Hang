using Microsoft.Build.Framework;

namespace Project.Models
{
    public class SendMail
    {
        
        public string Email { get; set; }
        
        public string Subject { get; set; }
        
        public string Message { get; set; }
    }
}
