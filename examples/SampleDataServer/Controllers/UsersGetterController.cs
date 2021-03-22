using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Core.Authentication;
using Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace SampleDataServer.Controllers
{
    [Route("data-server" + "/rest/" + nameof(IUsersGetter))]
    // Атрибут, который позволит удостовериться, что запрос из нашей системы
    [RestAuth] 
    public class UsersGetterController : IUsersGetter
    {
        private readonly IReadOnlyCollection<User> _users = new List<User>
        {
            new()
            {
                Id = 1,
                FirstName = "Tom"
            },
            new()
            {
                Id = 2,
                FirstName = "Brad"
            },
            new()
            {
                Id = 3,
                FirstName = "Sam"
            },
            new()
            {
                Id = 4,
                FirstName = "Bob"
            }
        };


        [HttpPost(nameof(IUsersGetter.GetUsersAsync))]
        public async Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter)
        {
            var users = _users;
            if (!string.IsNullOrEmpty(filter))
            {
                users = _users.Where(x =>
                        string.Equals(x.FirstName, filter, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var result = new RestResponse<IReadOnlyCollection<User>>(users);
            return await Task.FromResult(result);
        }
    }
}