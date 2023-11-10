using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfSignalR.Models;

namespace WpfSignalR.Client.Views;

public partial class MainWindow : Window
{
    const string      serverUrl  = "https://localhost:7236/";
    const string      chatHubUrl = $"{serverUrl}chathub";
    const string      apiUrl     = $"{serverUrl}api/Messages/";

    HubConnection     connection = null!;
    static HttpClient httpClient = new();

    MessageList       messageList = null!;

    public MainWindow()
    {
        InitializeComponent();
        InitializeOthers();
    }

    async void InitializeOthers()
    {
        try {
            await InitializeMessageList();
            await InitializeHubConnection();
        } catch (Exception ex) {
            ShowError(ex);
        }
    }

    async Task InitializeMessageList()
    {
        var messages = await GetMessagesAsync() ?? new Message[] {};
        messageList = MessageList.From(messages);
        DataContext = messageList;
    }

    async Task InitializeHubConnection()
    {
        connection = new HubConnectionBuilder()
                        .WithUrl(chatHubUrl)
                        .Build();

        connection.On<Message>(
            "ReceiveMessage",
            message => Dispatcher.Invoke(() => messageList.Add(message))
        );

        connection.On<int>(
            "MessageDeleted",
            _ => Dispatcher.Invoke(async () => await InitializeMessageList())
        );

        await connection.StartAsync();
    }

    async void OnMessageTextBoxKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            await SendMessageAsync();
    }

    async Task SendMessageAsync()
    {
        var messageContent = messageTextBox.Text;
        if (string.IsNullOrEmpty(messageContent))
            return;

        await SendMessageAsync(messageContent);
        messageTextBox.Text = string.Empty;
        messageTextBox.Focus();
    }

    async Task SendMessageAsync(string messageContent)
    {
        var user = new User { Name = Environment.UserName };

        var message = new Message {
            Body = messageContent,
            User    = user
        };

        try {
            await connection.InvokeAsync("SendMessage", message);
        } catch (Exception ex) {
            ShowError(ex);
        }
    }

    async void OnDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var messageId = (int?)button?.DataContext;
        if (messageId is null)
            return;

        try {
            await connection.InvokeAsync("DeleteMessage", messageId.Value);
        } catch (Exception ex) {
            ShowError(ex);
        }
    }

    static async Task<Message[]?> GetMessagesAsync()
        => await httpClient.GetFromJsonAsync<Message[]>(apiUrl);

    void ShowError(Exception? exception)
        => Dispatcher.Invoke(() => ShowError("サーバー接続エラー", exception));

    static void ShowError(string caption, Exception? exception)
        => MessageBox.Show(messageBoxText: exception?.Message ?? "", caption: caption, button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
}
