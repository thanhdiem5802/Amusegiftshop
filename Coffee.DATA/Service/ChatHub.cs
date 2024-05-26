using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DATA.Service
{
    public class ChatHub : Hub
    {
        public async Task SendToAdmin(string message)
        {
            // Gửi tin nhắn từ user đến admin
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
