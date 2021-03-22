using System;
using System.Collections.Generic;
using System.Net.Http;
using Core.Infrastructure;
using SampleClient;

var httpClient = new HttpClient();
var restClient = new UsersGetterClient(httpClient, CreateOptions());

Console.WriteLine("Press Any Key to get users");
Console.ReadLine();

var users = await restClient.GetUsersAsync(string.Empty);
if (users.HasError)
{
    Console.WriteLine(users.Error!.Message);
}
else
{
    foreach (var user in users.Data!)
    {
        Console.WriteLine($"{user.Id} : {user.FirstName}");
    }
}

static RestOptions CreateOptions()
{
    return new()
    {
        RestToken = new RestToken
        {
            Issuer = "Issuer",
            Audience = "Audience",
            IssuerSigningKey = "IssuerSigningKey"
        },
        Services = new List<ServiceRoute>
        {
            new()
            {
                Scheme = "http",
                Host = "localhost",
                Port = 10000,
                Name = "data-server"
            }
        }
    };
}