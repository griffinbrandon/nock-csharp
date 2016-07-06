namespace Nock.CSharp
{
    public class HttpClient : System.Net.Http.HttpClient
    {
        public HttpClient() : base(new TestMessageHandler())
        {            
        }
    }
}