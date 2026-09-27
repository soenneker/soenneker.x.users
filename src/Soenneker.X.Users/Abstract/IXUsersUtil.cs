using System.Threading;
using System.Threading.Tasks;
using Soenneker.X.OpenApiClient.Models;

namespace Soenneker.X.Users.Abstract;

/// <summary>Retrieves the authenticated X user and creates posts on their behalf.</summary>
/// <remarks>
/// Configure X:BearerToken with an OAuth 2.0 user access token authorized for tweet.read, tweet.write,
/// and users.read. An application-only bearer token cannot create posts. The client provider uses one
/// configured identity; token acquisition and refresh are the caller's responsibility.
/// </remarks>
public interface IXUsersUtil
{
    /// <summary>Retrieves the user associated with the configured access token.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user response, including any API errors, or null for an empty response.</returns>
    ValueTask<GetUsersMeResponse?> GetMe(CancellationToken cancellationToken = default);

    /// <summary>Creates a text post as the authenticated user.</summary>
    /// <param name="text">Nonblank post text. X validates account-specific length limits.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created post response, including its ID and any API errors, or null for an empty response.</returns>
    /// <exception cref="System.ArgumentException">The text is null, empty, or whitespace.</exception>
    ValueTask<CreatePostsResponse?> CreatePost(string text, CancellationToken cancellationToken = default);

    /// <summary>Creates a post using the full X request model, including replies, polls, and uploaded media IDs.</summary>
    /// <param name="request">The post options. Upload media separately before supplying media IDs. X validates supported combinations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created post response, including its ID and any API errors, or null for an empty response.</returns>
    /// <exception cref="System.ArgumentNullException">The request is null.</exception>
    /// <remarks>API failures propagate to the caller. Requests are not retried here to avoid duplicate posts.</remarks>
    ValueTask<CreatePostsResponse?> CreatePost(CreatePostsRequest request, CancellationToken cancellationToken = default);
}
