using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ChatClient;
using System.Threading;
using System.Media;

namespace ChatClient_WPF
{
    public partial class MainWindow : Window
    {
        private Client client;
        private bool isConnected;

        public MainWindow()
        {
            InitializeComponent();
            SendBtn.IsEnabled = false;
            ChatPage.IsReadOnly = true;
            Username.Focus();
            DisconnectBtn.Visibility = Visibility.Hidden;
            ChatPage.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
        }


        private void Send_Click(object sender, RoutedEventArgs e)
        {
            ValidateAndSendMessage(Message.Text);
        }

        private void ValidateAndSendMessage(string message)
        {
            if (!String.IsNullOrWhiteSpace(message))
                client.SendMessage(message);
            Message.Text = null;
            Message.Focus();
        }

        private void PostMessageOnChatPage(string message)
        {
            new SoundPlayer(Properties.Resources.Windows_Unlock).Play();
            ChatPage.Text += $"{message}\n";
        }
        
        private void GetMessageFromServer()
        {
            while (isConnected)
            {
                var message = client.ReceiveFromServer();
                if (ChatPage.LineCount > 20)
                    Dispatcher.Invoke(() => ChatPage.ScrollToLine(ChatPage.LineCount - 1));
                Dispatcher.Invoke(() => PostMessageOnChatPage(message));
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Username.Text))
            {
                Connect.IsEnabled = false;
                SendBtn.IsEnabled = true;
                client = new Client(Username.Text);
                isConnected = true;
                Username.IsEnabled = false;
                new Thread(GetMessageFromServer).Start();

                SuccessMsg.Text = $"Welcome {Username.Text}. You are connected !";
                SuccessMsg.Visibility = Visibility.Visible;
                DisconnectBtn.Visibility = Visibility.Visible;
                DisconnectBtn.IsEnabled = true;
            }
            else
                SuccessMsg.Text = "Connection failed. Please retry !";
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            ValidateAndSendMessage($"{Username.Text} is now offline !");
            new Thread(() => client.Disconnect()).Start();
            isConnected = false;
            DisconnectBtn.IsEnabled = false;
            SendBtn.IsEnabled = false;
            SuccessMsg.Visibility = Visibility.Hidden;
        }

        private void Message_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && SendBtn.IsEnabled == true)
            {
                ValidateAndSendMessage(Message.Text);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(isConnected)
                Disconnect_Click(this, new RoutedEventArgs());
        }
    }
}
