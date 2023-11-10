using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace WpfSignalR.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public DateTime? DeletedAt { get; set; } = null;

    public override string ToString() => $"{Name}({Id})";
}

public class Message
{
    public int Id { get; set; }
    public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();

    public int UserId { get; set; }
    public virtual User? User { get; set; }

    public override string ToString() => $"{User?.ToString() ?? ""}: {Body} ({CreatedAt.ToShortDateString()} {CreatedAt.ToShortTimeString()})";
}

public class MessageList : IEnumerable<Message>, INotifyCollectionChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    ObservableCollection<Message> messages = new();

    public int Count => messages.Count;

    public MessageList()
        => messages.CollectionChanged +=
           (sender, e) => CollectionChanged?.Invoke(sender, e);

    public void Add(Message message) => messages.Add(message);

    public static MessageList From(IEnumerable<Message> messages)
    {
        var messageList = new MessageList();
        messages.ForEach(message => messageList.Add(message));
        return messageList;
    }

    public IEnumerator<Message> GetEnumerator() => messages.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class MessageContext : DbContext
{
    const string connectionString = @"Server=(localdb)\mssqllocaldb;Database=WpfSignalR-2023111001;Trusted_Connection=True;MultipleActiveResultSets=true";

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }

    public MessageContext()
    {}

    //public MessageContext(DbContextOptions<MessageContext> options)
    //    : base(options)
    //{}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(message => Debug.WriteLine(message),
                                new[] { DbLoggerCategory.Database.Name },
                                LogLevel.Debug,
                                DbContextLoggerOptions.LocalTime)
                         .UseSqlServer(connectionString);
}
