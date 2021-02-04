using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stardust.Flux.PublishApi.Models;

namespace PublishApi
{
    public class EFDataStore : IDataStore
    {

        PublishContext context;


        public string AccountId
        {
            get; set;
        }
        public string Name { get; }

        public EFDataStore(PublishContext context, string name) : this(context)
        {
            this.context = context;
            Name = name;
        }

        public EFDataStore(PublishContext context)
        {
            this.context = context;
            AccountId = Guid.NewGuid().ToString();
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

            var item = context.YoutubeAccounts.FirstOrDefault(x => x.Key == key && x.Key == AccountId);
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
            T value = item == null ? default(T) : JsonConvert.DeserializeObject<T>(item.Value);
            return Task.FromResult<T>(value);

        }

        public async Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }



            string json = JsonConvert.SerializeObject(value);

            var item = await context.YoutubeAccounts.SingleOrDefaultAsync(x => x.Key == AccountId);

            if (item == null)
            {
                item = new YoutubeAccount { Key = key, Value = json, Name = this.Name };
                context.YoutubeAccounts.Add(item);
            }
            else
            {
                item.Value = json;
            }
            await context.SaveChangesAsync();
            AccountId = item.Key;
        }
    }
}