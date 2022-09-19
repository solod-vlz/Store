using System.Threading.Tasks;

namespace Store.Messages
{
    public interface INotificationService
    {
        void SendConfirmationCode(string mobilePhone, int code);

        Task SendConfirmationCodeAsync(string mobilePhone, int confirmationCode);

        void StartProcess(Order order);

        Task StartProcessAsync(Order order);
    }

}
