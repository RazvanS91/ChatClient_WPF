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
            DisconnectBtn.Visibility = Visibility.Hidden;
            ChatPage.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
            ChatPage.IsReadOnly = true;
        }


        private void Send_Click(object sender, RoutedEventArgs e)
        {
            ValidateAndSendMessage();
        }

        private void ValidateAndSendMessage()
        {
            if (!String.IsNullOrWhiteSpace(Message.Text))
                client.SendMessage(Message.Text);
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
            new Thread(() => client.Disconnect()).Start();
            isConnected = false;
            DisconnectBtn.IsEnabled = false;
            Connect.IsEnabled = true;
            SendBtn.IsEnabled = false;
            SuccessMsg.Visibility = Visibility.Hidden;
        }

        private void Message_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && SendBtn.IsEnabled == true)
            {
                ValidateAndSendMessage();
            }
        }
    }
}
