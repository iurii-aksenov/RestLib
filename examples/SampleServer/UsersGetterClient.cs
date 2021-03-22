using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Core;
using Core.Infrastructure;

namespace SampleServer
{
    public class UsersGetterClient : RestClient, IUsersGetter
    {
        public UsersGetterClient(HttpClient httpClient, RestOptions restSettings) : base(ServiceName, httpClient,
            restSettings)
        {
        }

        // Указываем название сервиса, к которому будем обращаться
        public static string ServiceName => "data-server";

        public async Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter = "")
        {
            // Указываем путь до сервиса, а также входные и выходные данные
            return await CallAsync<IReadOnlyCollection<User>, string>(
                $"/{ServiceName}/rest/{nameof(IUsersGetter)}/{nameof(IUsersGetter.GetUsersAsync)}", filter);
        }
    }
}