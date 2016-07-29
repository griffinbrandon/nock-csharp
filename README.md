#Nock-CSharp

Nock-CSharp is an HTTP mocking library that was inspired by <a href="https://github.com/node-nock/nock">node-nock</a>.

It is used to mimic the flow you would normally expect using the standard HttpClient available in .net, without having to stray away from the standard objects already built in.

#Install
Package is hosted in <a href="https://www.nuget.org/packages/nock-csharp">nuget.org</a>. Or run
```
PM> Install-Package nock-csharp
```
from the <a href="https://docs.nuget.org/consume/package-manager-console">Package Manager Console</a>

#Setup
There are a couple of steps that need to be followed in order to use this library. To intercept requests using the standard `System.Net.Http.HttpClient` object, you need to initialize it with an object that inherits from `System.Net.Http.DelegatingHandler`.

Included is `Nock.CSharp.HttpClient`. It inherits from `System.Net.Http.HttpClient` and initilizes it using `TestMessageHandler`. This is where all of the dirty work is performed. `TestMessageHandler` checks for the url and method you are calling and either returns the response you setup or throws an error.

To implement this, you will need to be set up for dependency injection. This will allow you to use `System.Net.Http.HttpClient` in your project, but use `Nock.CSharp.HttpClient` when you are running your tests.

Following is a small example on how to do this using the nuget package Unity (http://unity.codeplex.com/).

IFactory.cs
```csharp
public interface IFactory
{
  IUnityContainer Container { get; }
  IFactory Refresh();
  T Resolve<T>();
  IFactory SetUp();  
}
```

PeopleService.cs
```csharp
private IFactory _factory;

public Service(IFactory factory)
{
  _factory = factory;
}

public Task<HttpResponseMessage> Post(string uri, string json)
{
  var httpClient = _factory.Resolve<HttpClient>();

  var content = new StringContent(json, Encoding.UTF8, "application/json");
  var response = await httpClient.PostAsync(uri, content);

  return response;
}
```

Factory.cs (from unit tests)
```csharp
using Microsoft.Practices.Unity;

internal class Factory : IFactory
{
  private bool _disposed;

  public Factory()
  {
    Container = new UnityContainer();
  }

  public void Dispose()
  {
    if (!_disposed)
    {
        return;
    }

    _disposed = true;

    Container?.Dispose();
  }

  public IUnityContainer Container { get; }

  public IFactory Refresh()
  {
    return this;
  }

  public T Resolve<T>()
  {
    return Container.Resolve<T>();
  }

  public IFactory SetUp()
  {
    this.Container.RegisterType<HttpClient, Nock.CSharp.HttpClient>();
    return this;
  }
}
```

Factory.cs (from project) Notice the only change being in `SetUp`.
```csharp
using Microsoft.Practices.Unity;

internal class Factory : IFactory
{
  private bool _disposed;

  public Factory()
  {
    Container = new UnityContainer();
  }

  public void Dispose()
  {
    if (!_disposed)
    {
        return;
    }

    _disposed = true;

    Container?.Dispose();
  }

  public IUnityContainer Container { get; }

  public IFactory Refresh()
  {
    return this;
  }

  public T Resolve<T>()
  {
    return Container.Resolve<T>();
  }

  public IFactory SetUp()
  {
    // ensure the parameterless constructor is used
    this.Container.RegisterType<HttpClient>(new InjectionConstructor());
    return this;
  }
}
```

Finally, all you need to do is instantiate `PersonService` with the respective `Factory` and all of your calls will be either routed as usual or intercepted by Nock-CSharp.

#Interceptors
Once you have successfully wired up `Nock.CSharp.HttpClient`, you are ready to start using it. To mock a request, you can create a mocking object like this:
```csharp
new Nock("http://localhost:8080").Get("/?id=1").Reply(HttpStatusCode.Ok, "{\"id\": 1, \"firstName\":\"brandon\"}");
```
This will intercept every HTTP call to `http://localhost:8080/?id=1`. It will then return an HttpResponseMessage with a status 200 and the body returned will be a json object:
```csharp
{
  "id": 1,
  "firstName": "brandon"
}
```
