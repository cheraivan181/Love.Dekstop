using Love.Services.Auth;
using Love.Services.StateServices;
using Love.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
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
        }
    }
}
