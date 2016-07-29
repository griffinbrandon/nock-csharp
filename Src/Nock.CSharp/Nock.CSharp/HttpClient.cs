namespace NockCSharp
{
    public class HttpClient : System.Net.Http.HttpClient
    {
        public HttpClient() : base(new NockMessageHandler())
        {            
        }
    }
}