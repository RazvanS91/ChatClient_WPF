using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ChatClient;
using System.Threading;

namespace ChatClient_WPF
{
    public partial class MainWindow : Window
    {
        private Client client;

        public MainWindow()
        {
            InitializeComponent();
            SendBtn.IsEnabled = false;
            DisconnectBtn.Visibility = Visibility.Hidden;
            ChatPage.IsEnabled = false;
        }

        private void Send(object sender, RoutedEventArgs e)
        {
            client.SendMessage(Message.Text);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Username.Text))
            {
                Connect.IsEnabled = false;
                SendBtn.IsEnabled = true;
                client = new Client(Username.Text);

                SuccessMsg.Text = $"Welcome {Username.Text}. You are connected !";
                DisconnectBtn.Visibility = Visibility.Visible;
            }
            else
                SuccessMsg.Text = "Connection failed. Please retry !";
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
            DisconnectBtn.IsEnabled = false;
            Connect.IsEnabled = true;
            SendBtn.IsEnabled = false;
            SuccessMsg.Visibility = Visibility.Hidden;
        }
    }
}
