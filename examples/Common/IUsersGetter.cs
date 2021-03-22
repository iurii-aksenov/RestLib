using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure;

namespace Common
{
    public interface IUsersGetter
    {
        Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter = "");
    }
}