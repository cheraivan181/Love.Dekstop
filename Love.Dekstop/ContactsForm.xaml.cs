using Love.Services.Auth;
using Love.Services.StateServices;
using Love.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Love.Dekstop
{
    /// <summary>
    /// Логика взаимодействия для ContactsForm.xaml
    /// </summary>
    public partial class ContactsForm : Window
    {
        HubConnection connection;
        private readonly TokenService tokenService;
        private readonly StateContainer stateContainer;

        private ObservableCollection<TestContacts> contacts;
        private ObservableCollection<TestMessage> selectedMessages;
        private TestContacts selectedContact;

        public static string Iam = "Ilya";
        public ContactsForm()
        {
            stateContainer = StateContainer.GetStateContainer();

            tokenService = new TokenService(stateContainer.sessionStateService.UserId);

            string token = AsyncHelper.RunSync(() => tokenService.MakeAuthTokenAsync(stateContainer.sessionStateService.UserId));
            connection = new HubConnectionBuilder()
                .WithUrl(ConfigurationManager.AppSettings.Get("devUrl") + "messanger", options=> 
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            InitializeComponent();
            contacts = new ObservableCollection<TestContacts>
            {
                new TestContacts(){Name="Pendos",LastMessage = "Sosi",
                    Messages = new ObservableCollection<TestMessage>
                    {
                        new TestMessage(){Sender = "Pendos",Text = "Sosi"},
                        new TestMessage(){Sender = Iam, Text = "Sam Sosi"},
                        new TestMessage(){Sender = "Pendos",Text = "Okey("}
                    }},
                new TestContacts()
                {
                    Name="Chera",LastMessage = "Ilya sdelai chto to",
                    Messages = new ObservableCollection<TestMessage>
                    {
                        new TestMessage(){Sender = "Chera",Text = "Sosi xyi"},
                        new TestMessage(){Sender = Iam, Text = "Sam Sosi"},
                        new TestMessage(){Sender = "Chera",Text = "Okey("}
                    }
                },
                new TestContacts(){Name="Peder",LastMessage = "Ya daun"},
            };
            ContactsList.ItemsSource = contacts;
            
        }

        private void ContactsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedContact = (TestContacts) ContactsList.SelectedItem;
            selectedMessages = selectedContact.Messages;
            ItemsControl.ItemsSource = selectedMessages;
        }

        public void SendMsgBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (NewMsgTextBox.Text != null && NewMsgTextBox.Text != "")
            {
                selectedMessages.Add(new TestMessage(){Sender = Iam,Text = NewMsgTextBox.Text});
                NewMsgTextBox.Text = "";
            }
        }
    }

    public class TestContacts
    {
        public string Name { get; set; }
        public string LastMessage { get; set; }

        public ObservableCollection<TestMessage> Messages { get; set; }
    }

    public class TestMessage
    {
        public string Text { get; set; }

        public string Sender { get; set; }
    }
}
