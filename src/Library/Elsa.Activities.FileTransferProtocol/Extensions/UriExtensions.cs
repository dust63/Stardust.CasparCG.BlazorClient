using System.Linq;


namespace Elsa.Activities.FileTransferProtocol.Extensions
{
    public static class UriExtensions
    {
        public static string Append(this string url, params string[] paths)
        {
            return paths.Where(x => !string.IsNullOrWhiteSpace(x)).Aggregate(url, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/')));
        }
    }
}

