using System.Diagnostics;
using System.Net.Mail;
using System.Text;

namespace Store.Messages
{
    public class DebugNotificationService:INotificationService
    {
        public void SendConfirmationCode(string mobilePhone, int code)
        {
            Debug.WriteLine("Mobilephone: {0}, code: {1:0000}.", mobilePhone, code);
        }

        public void StartProcess(Order order)
        {
            using (var client = new SmtpClient())
            {
                var message = new MailMessage("from@at.my.domain", "to@at.my.domain");
                message.Subject = "Order#" + order.Id;

                var strBuilder = new StringBuilder();

                foreach (var item in order.Items)
                {
                    strBuilder.Append("{0}, {1}", item.BookId, item.Count);
                    strBuilder.AppendLine();
                }

                message.Body = strBuilder.ToString();
                client.Send(message);
            }
        }
    }
}
