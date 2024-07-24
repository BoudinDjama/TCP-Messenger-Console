using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Chatbox.Model
{
    internal class ChatManager
    {
        public Dictionary<User, string> chatHistory { get; set; }

        public ChatManager() { }

        public void sendMessage(User user, string message)
        {

            chatHistory.Add(user, message);
        }
    }
}
