using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SignalR.OrderTrade
{
    
    public static class UserHandler //this static class is to store the number of users conected at the same time
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public static class OrderHandler //this static class is to store the number of users conected at the same time
    {
        public static Dictionary<string, double> Orders = new Dictionary<string, double>();
        public static bool itensAdded = false;
    }

    [HubName("tradeOrder")]   //this is for use a name to use in the client
    public class MoveShapeHub : Hub
    {
        public void sendOrder(string x, double y) // this method will be called from the client
        {
            OrderHandler.Orders.Add(x, y);
            Clients.All.newOrder(x, y);       
        }

        public void cleanOrders()
        {
            OrderHandler.Orders.Clear();
        }

        public override Task OnConnected() //override OnConnect, OnReconnected and OnDisconnected to know if a user is connected or disconnected
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId); //add a connection id to the list
            Clients.All.usersConnected(UserHandler.ConnectedIds.Count()); //this will send to ALL the clients the number of users connected
            fillOrderTableInfo();
            return base.OnConnected();
        }

        private void fillOrderTableInfo(){
            foreach(string key in OrderHandler.Orders.Keys)
                Clients.All.newOrder(key, OrderHandler.Orders[key]); //this will send to ALL the clients 
        }

        public override Task OnReconnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            Clients.All.usersConnected(UserHandler.ConnectedIds.Count());
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            Clients.All.usersConnected(UserHandler.ConnectedIds.Count());
            return base.OnDisconnected();
        }        
    }
}