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
        public async Task SendMessage(string message)
        {
            var user = Context.User.Identity;
            var role = "User"; // Default to user role

            if (Context.User.IsInRole("Admin"))
            {
                role = "Admin"; // If the sender is an admin
            }

            // Send the message to all clients
            await Clients.All.SendAsync("ReceiveMessage", user.Name, message, role);
        }
    }
}
