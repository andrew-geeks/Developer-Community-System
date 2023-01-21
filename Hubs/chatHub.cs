using System;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DCS_new
{
    public class chatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}