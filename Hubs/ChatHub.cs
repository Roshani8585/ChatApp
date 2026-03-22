using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    // All connected users
    private static ConcurrentDictionary<string, string> Users = new();

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        await Clients.Caller.SendAsync("AskUserName", connectionId);
    }

    public async Task SetUserName(string userName)
    {
        Users[userName] = Context.ConnectionId;

        // Send updated user list to all
        await Clients.All.SendAsync("UserList", Users.Keys);
    }

    // Private message
    public async Task SendPrivateMessage(string toUser, string message)
    {
        var fromUser = Users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

        if (Users.ContainsKey(toUser))
        {
            var toConnectionId = Users[toUser];

            await Clients.Client(toConnectionId)
                .SendAsync("ReceivePrivateMessage", fromUser, message);

            await Clients.Caller
                .SendAsync("ReceivePrivateMessage", fromUser, message);
        }
    }
}