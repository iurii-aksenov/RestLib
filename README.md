


# Rest Lib - библиотека для общения сервисов между собой при помощи HTTP запросов / Rest Lib - a library for communication between services using HTTP requests  

(ENG is below)

Rus

Rest Lib позволяет одному сервису вызывать функции другого сервиса с помощью HTTP запроса.

## Подготовка к работе

Рассмотрим пример использования, находится в папке **Examples**

1. Common - общий код
2. SampleDataServer - сервер, который содержит данные и которые вызывают другие сервисы
3. SampleClient - десктоп клиент, который осуществляет запрос за данными к SampleDataServer
4. SampleServer  - сервер, который осуществляет запрос за данными к SampleDataServer

Схема взаимодействия следующая: 
![Sample of service interactions](/RestLibSample.png  "Пример взаимодействия")

Порядок действий:
1. ### Определить интерфейс общения. В нашем случае это *IUsersGetter**
```csharp 
public interface IUsersGetter
{
    Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter);
}
```
2. ### У поставщика данных (сервер SampleDataServer) реализуем этот интерфейс (часть кода опущена)
```csharp 
[Route("data-server" + "/rest/" + nameof(IUsersGetter))]
[RestAuth]
public class UsersGetterController : IUsersGetter
{
    private readonly IReadOnlyCollection<User> _users = ...

    [HttpPost(nameof(IUsersGetter.GetUsersAsync))]
    public async Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter)
    {
        ...
        users = _users;
        var result = new RestResponse<IReadOnlyCollection<User>>(users);
        return await Task.FromResult(result);
    }
}
```

В **Startup** классе необходимо добавить middlewares для обработок ошибок и для проверки аунтефикации запрос, а также добавить использование самой либы

```csharp 
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddRest();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRestExceptionHandling();
    app.UseRestAuthentication();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

3. ### У потребителя данных (Sample Server) также реализуем IUsersGetter интерфейс, но только для потребления

```csharp
public class UsersGetterClient : RestClient, IUsersGetter
{
    public UsersGetterClient(HttpClient httpClient, RestOptions restSettings) : base(ServiceName, httpClient, restSettings){}

    public static string ServiceName => "data-server";

    public async Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter = "")
    {
        return await CallAsync<IReadOnlyCollection<User>, string>(
            $"/{ServiceName}/rest/{nameof(IUsersGetter)}/{nameof(IUsersGetter.GetUsersAsync)}", filter);
    }
}
```

В дальнейшем, чтобы использовать данный клиент, мы будет с помощью Dependency Injection лишь просить **IUsersGetter**

В классе **Startup** Добавим использование либы и использование **UsersGetterClient**

```csharp
 public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddRest();
    services.AddRestClient<IUsersGetter, UsersGetterClient>();
}
```

И далее в месте использования просим реализацию **IUsersGetter** и вызываем нужные метод
```csharp
var response = await _client.GetUsersAsync();
```

4. ### Добавляем данные в конфиг
Также необходимо добавить данные о сервисах и о токене в appsettings.json

```json
"Rest": {
    "RestToken": {
      "Issuer": "Issuer",
      "Audience": "Audience",
      "IssuerSigningKey": "IssuerSigningKey",
      "TokenDecryptionKey": "TokenDecryptionKey"
    },
    "Services": [
      {
        "Name": "data-server",
        "Scheme": "http",
        "Host": "localhost",
        "Port": "10000"
      }
    ]
  },
```

Всё работает

## Структура проекта
Бибилиотека состоит из следующих модулей:

### Core
Является базовым модулем, содежащий весь код.

### Examples
Соедржит примеры работы с библиотекой.

## Contributing и Поддержка
- Contributing приветствуеется, вы можете начать через создание issue в GitHub и открыть pull request после обсуждения в issue
- По возникающим вопросам просьба обращаться на [iurii.aksenov@yandex.ru][support-email]
- Баги и feature-реквесты можно направлять в раздел [issues][issues]

---

Eng

Rest Lib позволяет одному сервису вызывать функции другого сервиса с помощью HTTP запроса.

## Подготовка к работе

You might wish to take a look at the sample application for the details 
Let's look at an example of use, located in the **Examples** folder

1. Common - common code
2. SampleDataServer - a server that contains data and which is called by other services
3. SampleClient - a desktop client that makes a request for data to SampleDataServer
4. SampleServer - a server that makes a request for data to SampleDataServer

The interaction scheme is as follows:
![Sample of service interactions](/RestLibSample.png  "Пример взаимодействия")

Steps:
1. ### Define the communication interface. In our case, this is **IUsersGetter**
```csharp 
public interface IUsersGetter
{
    Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter);
}
```
2. ### At the data provider (SampleDataServer), we implement this interface (part of the code is omitted)
```csharp 
[Route("data-server" + "/rest/" + nameof(IUsersGetter))]
[RestAuth]
public class UsersGetterController : IUsersGetter
{
    private readonly IReadOnlyCollection<User> _users = ...

    [HttpPost(nameof(IUsersGetter.GetUsersAsync))]
    public async Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter)
    {
        ...
        users = _users;
        var result = new RestResponse<IReadOnlyCollection<User>>(users);
        return await Task.FromResult(result);
    }
}
```

In the **Startup** class, you need to add middlewares for error handling and for checking the authentication of the request, as well as add the use of the lib itself

```csharp 
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddRest();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRestExceptionHandling();
    app.UseRestAuthentication();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

3. ### The data consumer (Sample Server) also implements the **IUsersGetter** interface, but only for consumption

```csharp
public class UsersGetterClient : RestClient, IUsersGetter
{
    public UsersGetterClient(HttpClient httpClient, RestOptions restSettings) : base(ServiceName, httpClient, restSettings){}

    public static string ServiceName => "data-server";

    public async Task<RestResponse<IReadOnlyCollection<User>>> GetUsersAsync(string filter = "")
    {
        return await CallAsync<IReadOnlyCollection<User>, string>(
            $"/{ServiceName}/rest/{nameof(IUsersGetter)}/{nameof(IUsersGetter.GetUsersAsync)}", filter);
    }
}
```

In order to use this client we are using Dependency Injection, we inject  **IUsersGetter**

In the **Startup** class, add the use of RestLib and the use of **UsersGetterClient**

```csharp
 public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddRest();
    services.AddRestClient<IUsersGetter, UsersGetterClient>();
    ...
}
```

And then, at the place of use, ask for the **IUsersGetter** implementation and call the necessary methods
```csharp
var response = await _client.GetUsersAsync();
```

4. ### Add data to the config
You also need to add service and token data to appsettings.json

```json
"Rest": {
    "RestToken": {
      "Issuer": "Issuer",
      "Audience": "Audience",
      "IssuerSigningKey": "IssuerSigningKey",
      "TokenDecryptionKey": "TokenDecryptionKey"
    },
    "Services": [
      {
        "Name": "data-server",
        "Scheme": "http",
        "Host": "localhost",
        "Port": "10000"
      }
    ]
  },
```

Everything works!

## Project structure
The library consists of the following modules:

### Core
It is the base module that contains all the code.

### Examples
Contains examples of working with the library.

## Support
- Contributions are welcome, you can start by creating an issue on GitHub and opening a pull request after discussing the issue.
- If you have any questions, please contact [iurii.aksenov@yandex.ru][support-email]
- Bugs and feature requests can be sent to the [issues][issues] section

[support-email]: mailto:iurii.aksenov@yandex.ru
[issues]: https://github.com/IuriiAksenov/RestLib/issues
