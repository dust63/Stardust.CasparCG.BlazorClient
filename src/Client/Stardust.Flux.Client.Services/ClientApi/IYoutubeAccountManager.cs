using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Services
{
   public interface IYoutubeAccountManager
    {

        Task<HttpResponseMessage> AddAccount();
    }


    public class YoutubeAccountManager : IYoutubeAccountManager
    {
        private readonly HttpClient httpClient;

        public YoutubeAccountManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> AddAccount()
        {  
            var response = await httpClient.PostAsync("/Youtube/Account/Grant", null);
            return response;

        }
    }
}
