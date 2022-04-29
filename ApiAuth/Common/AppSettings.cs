namespace ApiAuth.Common;

public class AppSettings
{
    public string Secret { get; set; }
    public int RefreshTokenDaysToLive { get; set; }
    
    public EmailServiceSettings EmailService { get; set; }


    public class EmailServiceSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public string SenderEmail { get; set; }
    }
}