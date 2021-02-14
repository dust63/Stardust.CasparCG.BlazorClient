using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stardust.Flux.PublishApi.Models;

namespace Stardust.Flux.PublishApi.Youtube
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
            if (item == null)
                Task.FromResult(default(T));
            var token = new TokenResponse
            {
                IssuedUtc = item.IssuedUtc.Value,
                ExpiresInSeconds = item.ExpiresInSeconds,
                IdToken = item.IdToken,
                Scope = item.Scope,
                TokenType = item.TokenType,
                RefreshToken = item.RefreshToken,
                AccessToken = item.AccessToken
            };
            return Task.FromResult<T>(JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(token)));
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
                item = new YoutubeAccount { Key = key, Name = this.Name };
                context.YoutubeAccounts.Add(item);
            }


            item.IssuedUtc = newToken.IssuedUtc;
            item.ExpiresInSeconds = newToken.ExpiresInSeconds;
            item.IdToken = newToken.IdToken;
            item.Scope = newToken.Scope;
            item.TokenType = newToken.TokenType;
            item.ModifiedOn = DateTime.UtcNow;
            item.AccessToken = newToken.AccessToken;
            if (newToken.RefreshToken != null)
                item.RefreshToken = newToken.RefreshToken;
            if (newToken.AccessToken != null)
                item.AccessToken = newToken.AccessToken;
            await context.SaveChangesAsync();
            AccountId = item.Key;
        }


    }
}