using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Messages
{
    public interface INotificationService
    {
        void SendConfirmationCode(string mobilePhone, int code);
    }
}
