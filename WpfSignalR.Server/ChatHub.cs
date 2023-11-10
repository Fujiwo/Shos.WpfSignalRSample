using Microsoft.AspNetCore.SignalR;

namespace WpfSignalR.Server;

using Models;

public class ChatHub : Hub
{
    public const string Name = "chathub";
    public const string ReceiveMessage = "ReceiveMessage";
    public const string MessageDeleted = "MessageDeleted";

    MessageContext context = new();

    public async Task SendMessage(Message message)
    {
        await AddAsync(message);
        await Clients.All.SendAsync(ReceiveMessage, message);
        Console.WriteLine($"{nameof(SendMessage)}: {message}");
    }

    public async Task DeleteMessage(int messageId)
    {
        var result = await DeleteMessageAsync(messageId);
        if (result) {
            await Clients.All.SendAsync(MessageDeleted, messageId);
            Console.WriteLine($"{nameof(DeleteMessage)}: {messageId}");
        }
    }

    async Task AddAsync(Message message)
    {
        var user = message.User is null ? null : context.Users.FirstOrDefault(user => user.Name == message.User.Name);
        if (user is null && message.User is not null) {
            user = new User { Name = message.User.Name };
            context.Users.Add(user);
        }
        context.Add(message);
        await context.SaveChangesAsync();
    }

    async Task<bool> DeleteMessageAsync(int messageId)
    {
        if (context.Messages is null)
            return false;

        var message = await context.Messages.FindAsync(messageId);
        if (message is null)
            return false;

        context.Messages.Remove(message);
        await context.SaveChangesAsync();
        return true;
    }
}
