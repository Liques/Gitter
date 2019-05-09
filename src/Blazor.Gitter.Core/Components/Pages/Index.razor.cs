using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Services;

namespace Blazor.Gitter.Core.Components.Pages
{
    public class IndexModel : ComponentBase, IDisposable
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IAuthApi GitterAuthApi { get; set; }
        [Inject] internal IAppState State { get; set; }
        [Inject] internal IUriHelper UriHelper { get; set; }

        internal string ErrorMessage;
        string apiKey;

        public string code { get; set; }

        internal string ApiKey
        {
            get => State.GetApiKey();
            set => apiKey = value;
        }

        protected override void OnInit()
        {
            base.OnInit();
            State.GotChatUser += State_GotChatUser;

            string code = "";

            var uri = new Uri(UriHelper.GetAbsoluteUri());
            code = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var type) ? type.First() : "";

            if (!String.IsNullOrEmpty(code))
            {
                GetAccessToken(code);
            }

            
        }

        private async Task GetAccessToken(string exchangeToken)
        {
            apiKey = await GitterAuthApi.GetAccessToken(exchangeToken);
            await SignIn(true);
        }

        private void State_GotChatUser(object sender, EventArgs e)
        {
            Invoke(StateHasChanged);
        }

        internal void OAuth()
        {
            var urlLogin = GitterAuthApi.GetLoginUrl();
            UriHelper.NavigateTo(urlLogin);
        }            

        internal async Task SignIn(bool remember)
        {
            if (!State.HasApiKey && string.IsNullOrWhiteSpace(apiKey))
            {
                ErrorMessage = "Please Enter your own Gitter API Key!";
                return;
            }

            State.SetApiKey(apiKey);

            if (remember)
            {
                await State.SaveApiKey();
            }

            ErrorMessage = "";
            StateHasChanged();
        }

        public void Dispose()
        {
            State.GotChatUser -= State_GotChatUser;
        }
    }
}
