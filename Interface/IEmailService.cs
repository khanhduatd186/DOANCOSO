namespace WebBanThu.Interface
{
    public interface IEmailService
    {
        public void SendPaymentConfirmationEmail(string toEmail, string subject, string body, List<string> productImages);

    }
}
