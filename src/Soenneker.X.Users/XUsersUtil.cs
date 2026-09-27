using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.X.ClientUtil.Abstract;
using Soenneker.X.OpenApiClient.Models;
using Soenneker.X.Users.Abstract;

namespace Soenneker.X.Users;

public sealed class XUsersUtil(IXClientUtil clientUtil) : IXUsersUtil
{
    public async ValueTask<GetUsersMeResponse?> GetMe(CancellationToken cancellationToken = default)
    {
        var client = await clientUtil.Get(cancellationToken).ConfigureAwait(false);
        return await client.Two.Users.Me.GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<CreatePostsResponse?> CreatePost(string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        return CreatePost(new CreatePostsRequest { Text = text }, cancellationToken);
    }

    public async ValueTask<CreatePostsResponse?> CreatePost(CreatePostsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var client = await clientUtil.Get(cancellationToken).ConfigureAwait(false);
        return await client.Two.Tweets.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
