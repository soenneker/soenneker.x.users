using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.X.ClientUtil.Abstract;
using Soenneker.X.OpenApiClient;
using Soenneker.X.OpenApiClient.Models;

namespace Soenneker.X.Users.Tests;

public sealed class XUsersUtilTests
{
    [Test]
    public async Task CreatePost_sends_text_and_reads_created_id()
    {
        using var handler = new RecordingHandler();
        using var provider = new ClientProvider(handler);
        var util = new XUsersUtil(provider);
        var result = await util.CreatePost("Hello X");
        await Assert.That(handler.Method).IsEqualTo(HttpMethod.Post);
        await Assert.That(handler.Url).IsEqualTo("https://api.x.com/2/tweets");
        await Assert.That(handler.Body).Contains("\"text\":\"Hello X\"");
        await Assert.That(result!.Data!.Id).IsEqualTo("123");
    }

    [Test]
    public async Task CreatePost_preserves_reply_and_media_options()
    {
        using var handler = new RecordingHandler();
        using var provider = new ClientProvider(handler);
        var util = new XUsersUtil(provider);
        await util.CreatePost(new CreatePostsRequest
        {
            Text = "A reply",
            Reply = new CreatePostsReply { InReplyToTweetId = "456" },
            Media = new CreatePostsMedia { MediaIds = ["789"] }
        });
        await Assert.That(handler.Body).Contains("\"in_reply_to_tweet_id\":\"456\"");
        await Assert.That(handler.Body).Contains("\"media_ids\":[\"789\"]");
    }

    [Test]
    public async Task CreatePost_propagates_api_failure_without_retry()
    {
        using var handler = new RecordingHandler { StatusCode = HttpStatusCode.Forbidden };
        using var provider = new ClientProvider(handler);
        var util = new XUsersUtil(provider);
        var failed = false;
        try { await util.CreatePost("Hello X"); }
        catch (Microsoft.Kiota.Abstractions.ApiException) { failed = true; }
        await Assert.That(failed).IsTrue();
        await Assert.That(handler.Calls).IsEqualTo(1);
    }

    private sealed class ClientProvider : IXClientUtil
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClientRequestAdapter _adapter;
        private readonly XOpenApiClient _client;
        public ClientProvider(HttpMessageHandler handler)
        {
            _httpClient = new HttpClient(handler);
            _adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: _httpClient);
            _client = new XOpenApiClient(_adapter);
        }
        public ValueTask<XOpenApiClient> Get(CancellationToken cancellationToken = default) => ValueTask.FromResult(_client);
        public void Dispose() { _adapter.Dispose(); _httpClient.Dispose(); }
        public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.Created;
        public HttpMethod? Method { get; private set; }
        public string? Url { get; private set; }
        public string? Body { get; private set; }
        public int Calls { get; private set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            Method = request.Method;
            Url = request.RequestUri!.ToString();
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(StatusCode)
            {
                Content = new StringContent(StatusCode == HttpStatusCode.Created
                    ? "{\"data\":{\"id\":\"123\",\"text\":\"Hello X\"}}"
                    : "{\"title\":\"Forbidden\",\"status\":403}", Encoding.UTF8, "application/json")
            };
        }
    }
}
