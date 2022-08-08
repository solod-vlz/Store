using System.Diagnostics;

namespace Store.Messages
{
    public class DebugNotificationService:INotificationService
    {
        public void SendConfirmationCode(string mobilePhone, int code)
        {
            Debug.WriteLine("Mobilephone: {0}, code: {1:0000}.", mobilePhone, code);
        }
    }
}
