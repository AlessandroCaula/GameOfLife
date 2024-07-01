using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public interface IMessageFilterReceiver
    {
        /// <summary>
        /// Notifies that a message has been received by the filter and return a value indicating whether the message has been handled
        /// (if handled the message will not be propagated to all other listeners)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool OnMessageFilterMessageReceived(ref Message message);
    }
}
