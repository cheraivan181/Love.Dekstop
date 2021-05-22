using Love.Models.Responses;
using Love.Providers;
using Love.Services.Auth;
using Love.Services.Crypt;
using Love.Services.Http;
using Love.Services.StateServices;
using Love.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Love.Dekstop
{
    public partial class MainWindow : Window
    {
        private readonly UserProvider userProvider;
		private TokenService tokenService;
		private readonly BaseHttpRequest baseHttpRequest;

        public MainWindow()
        {
            InitializeComponent();
            userProvider = new UserProvider();
			baseHttpRequest = new BaseHttpRequest();

			HaveAccount.Visibility = Visibility.Visible;
        }

		private void Open(Border screen)
		{
			LoginScreen.Visibility = Visibility.Hidden;
			Accounts.Visibility = Visibility.Hidden;

			screen.Visibility = Visibility.Visible;
		}

        private async void LoginButton_Click_1(object sender, RoutedEventArgs e)
        {
			var stateContainer = StateContainer.GetStateContainer();

			string errorMessage = "";
			if (stateContainer.registrationStateService.IsLogin)
				errorMessage = ValidateLogin(LoginBox.Text, PasswordBox.Password, EmailBox.Text, false);
            else
				errorMessage = ValidateLogin(LoginBox.Text, PasswordBox.Password, EmailBox.Text, true);
			
			if (!string.IsNullOrEmpty(errorMessage))
			{
				LoginMessageBlock.Text = errorMessage;
				LoginMessageBlock.Visibility = Visibility.Visible;

				return;
			}

			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("userName", LoginBox.Text);
			parameters.Add("password", PasswordBox.Password);

			if (stateContainer.registrationStateService.IsLogin)
            {
				var users = await userProvider.GetAllUsersAsync();
				var currentUser = users.FirstOrDefault(x => x.Login == LoginBox.Text && x.Password == HashService.GetHash(PasswordBox.Password));
				
				if (currentUser != null)
				{
					var strongKey = await userProvider.GetStrongKeyAsync(currentUser.Id);
					var session = await userProvider.GetSessionAsync(currentUser.Id);

					tokenService = new TokenService(currentUser.Id);

					string acessToken = await tokenService.MakeAuthTokenAsync(currentUser.Id);

					stateContainer.sessionStateService.SetStateAsync(strongKey: strongKey.Key, clientPrivateKey: session.ClientPrivateKey,
						clientPublicKey: session.ClientPublicKey, serverPublicKey: session.ServerPublicKey,
						acessToken: acessToken, userId: currentUser.Id);

					var sessionService = new SessionService(currentUser.Id);
					await sessionService.SetSessionAsync(acessToken);

					ContactsForm contactForm = new ContactsForm();
					contactForm.Show();
					Close();
				}
				else
                {
					var httpRequestMessage = new HttpRequestMessage();
					httpRequestMessage.Method = HttpMethod.Post;
					httpRequestMessage.RequestUri = new Uri(ConfigurationManager.AppSettings.Get("devUrl") + Urls.AuthUrl);

					var form = new MultipartFormDataContent();

					foreach (var parameter in parameters)
						form.Add(new StringContent(parameter.Value), parameter.Key);

					httpRequestMessage.Content = form;

					var authResponse = await baseHttpRequest.httpClient.SendAsync(httpRequestMessage);
					switch (authResponse.StatusCode)
                    {
						case HttpStatusCode.BadRequest:
							LoginMessageBlock.Text = "Wrong login or password!";
							LoginMessageBlock.Visibility = Visibility.Visible;
							break;
						case HttpStatusCode.Unauthorized:

							LoginMessageBlock.Text = "Wrong login or password!";
							LoginMessageBlock.Visibility = Visibility.Visible;

							break;
						case HttpStatusCode.InternalServerError:
							LoginMessageBlock.Text = "Internal server error!";
							LoginMessageBlock.Visibility = Visibility.Visible;
							break;
						case HttpStatusCode.OK:

							string content = await authResponse.Content.ReadAsStringAsync();
							var authResult = JsonConvert.DeserializeObject<AuthResult>(content);

							await userProvider.СreateOrUpdateAuthStorageAsync(authResult.UserId, authResult.AccessToken, authResult.RefreshToken);

							var sessionService = new SessionService(authResult.UserId);
							await sessionService.MakeSessionAsync(authResult.AccessToken, authResult.RefreshToken);

							await userProvider.CreateUserAsync(authResult.UserId, LoginBox.Text, HashService.GetHash(PasswordBox.Password));

							var strongKey = await userProvider.GetStrongKeyAsync(authResult.UserId);
							var session = await userProvider.GetSessionAsync(authResult.UserId);
							var authStorage = await userProvider.GetAuthStorageAsync(authResult.UserId);

							stateContainer.sessionStateService.SetStateAsync(strongKey: strongKey.Key, clientPrivateKey: session.ClientPrivateKey,
								clientPublicKey: session.ClientPublicKey, serverPublicKey: session.ServerPublicKey,
								acessToken: authStorage.AcessToken, userId: currentUser.Id);

							await sessionService.SetSessionAsync(authStorage.AcessToken);

							ContactsForm contactForm = new ContactsForm();
							contactForm.Show();
							Close();
						
							break;
					}
				}
            }
            else
            {
				var httpRequest = new HttpRequestMessage();
				httpRequest.Method = HttpMethod.Post;
				httpRequest.RequestUri = new Uri(ConfigurationManager.AppSettings.Get("devUrl") + Urls.RegisterUrl);

				var form = new MultipartFormDataContent();

				foreach (var parameter in parameters)
					form.Add(new StringContent(parameter.Value), parameter.Key);

				httpRequest.Content = form;
				
				var registerResult = await baseHttpRequest.httpClient.SendAsync(httpRequest);
				

				switch (registerResult.StatusCode)
                {
					case HttpStatusCode.OK:

						string testerToken = ConfigurationManager.AppSettings.Get("testerToken");

						var authUserInfo = JsonConvert.DeserializeObject<AuthResult>(
							await registerResult.Content.ReadAsStringAsync()
						);

						var phoneConfirmRequestMessage = new HttpRequestMessage();
						phoneConfirmRequestMessage.Method = HttpMethod.Get;
						phoneConfirmRequestMessage.RequestUri = new Uri(ConfigurationManager.AppSettings.Get("devUrl") + $"{Urls.ConfirmPhoneTestUrl}/{authUserInfo.UserId}");
						phoneConfirmRequestMessage.Headers.Add("TesterToken", testerToken);

						var phoneConfirmResult = await baseHttpRequest.httpClient.SendAsync(phoneConfirmRequestMessage);
						if (phoneConfirmResult.StatusCode == HttpStatusCode.OK)
						{
							var sessionService = new SessionService(authUserInfo.UserId);

							await userProvider.СreateOrUpdateAuthStorageAsync(authUserInfo.UserId, authUserInfo.AccessToken, authUserInfo.RefreshToken);

							await sessionService.MakeSessionAsync(authUserInfo.AccessToken, authUserInfo.RefreshToken);
							await userProvider.CreateUserAsync(authUserInfo.UserId, LoginBox.Text, HashService.GetHash(PasswordBox.Password));

							var strongKey = await userProvider.GetStrongKeyAsync(authUserInfo.UserId);
							var session = await userProvider.GetSessionAsync(authUserInfo.UserId);
							var authStorage = await userProvider.GetAuthStorageAsync(authUserInfo.UserId);

							stateContainer.sessionStateService.SetStateAsync(strongKey: strongKey.Key, clientPrivateKey: session.ClientPrivateKey,
								clientPublicKey: session.ClientPublicKey, serverPublicKey: session.ServerPublicKey,
								acessToken: authStorage.AcessToken, userId: authUserInfo.UserId);


							await sessionService.SetSessionAsync(authUserInfo.UserId);

							ContactsForm contact = new ContactsForm();
							contact.Show();
							Close();
						}
						else
							MessageBox.Show($"Something was error");

						break;
					case HttpStatusCode.BadRequest:
						break;
					case HttpStatusCode.InternalServerError:
						break;
                }				
            }

			//LoginMessageBlock.Text = "Wrong login or password!";
			//LoginMessageBlock.Visibility = Visibility.Visible;
		}


        private void Reigstration_Click(object sender, RoutedEventArgs e)
        {
			var stateContainer = StateContainer.GetStateContainer();
			stateContainer.registrationStateService.ChangeState();
            Thickness margin = Registration.Margin;
			if (!stateContainer.registrationStateService.IsLogin)
            {
				HaveAccount.Visibility = Visibility.Hidden;
				EmailBox.Visibility = Visibility.Visible;
				Registration.Content = "Login";
                margin.Left = 35;
                Registration.Margin = margin;
				EnterType.Text = "Registration";
				LoginButton.Content = "Registration";
            }
            else
            {
				HaveAccount.Visibility = Visibility.Visible;
				EmailBox.Visibility = Visibility.Hidden;
				Registration.Content = "Registration";
                margin.Left = 0;
                Registration.Margin = margin;
				EnterType.Text = "Login";
				LoginButton.Content = "Login";
            }

			ChangeTypeSign();

        }
        private async void HaveAccount_Click(object sender, RoutedEventArgs e)
        {
			var users = await userProvider.GetAllUsersAsync();
        }


		private void ChangeTypeSign()
        {
			LoginBox.Text = "";
			PasswordBox.Password = "";
			EmailBox.Text = "";
        }

		private string ValidateLogin(string login, string password, string email, bool isValidEmail)
        {
			if (string.IsNullOrEmpty(login))
				return "Login must be filled";
			if (string.IsNullOrEmpty(password))
				return "Password must be filled";
			if (isValidEmail && string.IsNullOrEmpty(email))
				return "Email must be filed";

			if (isValidEmail)
			{
				Regex mailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match mailMatch = mailRegex.Match(email);

				if (!mailMatch.Success)
					return "Incorrect email";
			}

			Regex loginPattern = new Regex(@"(?s)^([^a-zA-Z]*[A-Za-z]){4}.*");
			Match loginMatch = loginPattern.Match(login);

			if (!loginMatch.Success)
				return "Login must be contains 4 symbhol and latin alphavite";

			return null;
        }
    }
}

