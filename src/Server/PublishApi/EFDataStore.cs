using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stardust.Flux.PublishApi.Models;

namespace Stardust.Flux.PublishApi
{
    public class EFDataStore : IDataStore
    {

        PublishContext context;


        public string AccountId
        {
            get; private set;
        }
        public string Name { get; }

        public EFDataStore(PublishContext context, string name) : this(context)
        {
            Name = name;
        }

        public EFDataStore(PublishContext context)
        {
            this.context = context;
        }

        public async Task ClearAsync()
        {
            context.YoutubeAccounts.RemoveRange(context.YoutubeAccounts.Where(x => x.Key == AccountId));
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var item = context.YoutubeAccounts.FirstOrDefault(x => x.Key == AccountId);
            if (item != null)
            {
                context.YoutubeAccounts.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }


            var item = context.YoutubeAccounts.FirstOrDefault(x => x.Key == key);
            T value = item?.Value == null ? default(T) : JsonConvert.DeserializeObject<T>(item.Value);
            return Task.FromResult<T>(value);

        }

        public async Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var newToken = value as TokenResponse;

            var item = await context.YoutubeAccounts.SingleOrDefaultAsync(x => x.Key == key);

            if (item == null)
            {
                item = new YoutubeAccount { Key = key, Value = JsonConvert.SerializeObject(newToken), Name = this.Name };
                context.YoutubeAccounts.Add(item);
            }
            else
            {
                TransitRefreshToken(newToken, item.Value);
                item.Value = JsonConvert.SerializeObject(newToken);
                item.ModifiedOn = DateTime.UtcNow;
            }
            await context.SaveChangesAsync();
            AccountId = item.Key;
        }

        /// <summary>
        /// Used to replace refresh token to be sure to allow process to refresh token
        /// Refresh tolen is sent only once
        /// </summary>
        /// <param name="newResponse">new token response sent by google</param>
        /// <param name="previousJsonResponse">the previous toke in JSON string</param>
        public static void TransitRefreshToken(TokenResponse newResponse, string previousJsonResponse)
        {
            if (newResponse.RefreshToken != null || string.IsNullOrWhiteSpace(previousJsonResponse))
                return;

            var oldUserToken = JsonConvert.DeserializeObject<TokenResponse>(previousJsonResponse);
            newResponse.RefreshToken = oldUserToken?.RefreshToken;
        }
    }
}