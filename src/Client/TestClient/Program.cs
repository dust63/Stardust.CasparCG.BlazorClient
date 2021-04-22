using Refit;
using Stardust.Flux.Client.Services;
using System;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var api = RestService.For<IRecordClientApi>("https://localhost:44352");

            var response = await api.GetManualRecords(0);

            Console.WriteLine(response.StatusCode);
            Console.Read();

        }
    }
}
