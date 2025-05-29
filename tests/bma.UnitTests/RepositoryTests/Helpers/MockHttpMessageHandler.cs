namespace bma.UnitTests.RepositoryTests.Helpers;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _sendHandler;

    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> sendHandler)
    {
        _sendHandler = sendHandler;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_sendHandler(request));
    }
}
