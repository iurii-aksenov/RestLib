using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Core;
using Core.Infrastructure;

namespace SampleClient
{
    public class UsersGetterClient : RestClient, IUsersGetter
    {
        public UsersGetterClient(HttpClient httpClient, RestOptions restSettings) : base(ServiceName, httpClient,
            restSettings)
        {
        }

        public static string ServiceName => "data-server";

        public async Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter = "")
        {
            return await CallAsync<IReadOnlyCollection<User>, string>(
                $"/{ServiceName}/rest/{nameof(IUsersGetter)}/{nameof(IUsersGetter.GetUsersAsync)}", filter);
        }
    }
}