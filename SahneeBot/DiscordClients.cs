using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;

namespace SahneeBot;

/// <summary>
/// The error webhook used to report errors.
/// </summary>
/// <param name="Client">The webhook client.</param>
public record ErrorWebhook(DiscordWebhookClient? Client);

/// <summary>
/// The actual bot. We develop against the `IDiscordClient` interface instead of an implementation to be able to easily
/// switch if the need arises.
/// </summary>
/// <param name="Client">The discord client.</param>
public record Bot(IDiscordClient Client)
{
    /// <summary>
    /// Gets the client as a socket client. Only use this when ready needed.
    /// </summary>
    public DiscordSocketClient? Socket => Client as DiscordSocketClient;
    /// <summary>
    /// Gets the client as a REST client. Only use this when ready needed.
    /// </summary>
    public DiscordRestClient? Rest => Client as DiscordRestClient;

    /// <summary>
    /// Runs a client specific implementation.
    /// </summary>
    /// <param name="socketDel">The implementation for the socket client.</param>
    /// <param name="restDel">The implementation for the rest client.</param>
    /// <exception cref="NotImplementedException">A non rest/socket client is used.</exception>
    public void Impl(Action<DiscordSocketClient> socketDel, Action<DiscordRestClient> restDel)
    {
        switch (Client)
        {
            case DiscordSocketClient socket:
                socketDel(socket);
                break;
            case DiscordRestClient rest:
                restDel(rest);
                break;
            default:
                throw new NotImplementedException("Non supported discord client type.");
        }
    }

    /// <summary>
    /// Runs a client specific implementation that returns something.
    /// </summary>
    /// <param name="socketDel">The implementation for the socket client.</param>
    /// <param name="restDel">The implementation for the rest client.</param>
    /// <exception cref="NotImplementedException">A non rest/socket client is used.</exception>
    public T Impl<T>(Func<DiscordSocketClient, T> socketDel, Func<DiscordRestClient, T> restDel)
    {
        return Client switch
        {
            DiscordSocketClient socket => socketDel(socket),
            DiscordRestClient rest => restDel(rest),
            _ => throw new NotImplementedException("Non supported discord client type.")
        };
    }

    /// <summary>
    /// Runs a client specific async implementation.
    /// </summary>
    /// <param name="socketDel">The async implementation for the socket client.</param>
    /// <param name="restDel">The async implementation for the rest client.</param>
    /// <exception cref="NotImplementedException">A non rest/socket client is used.</exception>
    public Task ImplAsync(Func<DiscordSocketClient, Task> socketDel, Func<DiscordRestClient, Task> restDel)
    {
        return Client switch
        {
            DiscordSocketClient socket => socketDel(socket),
            DiscordRestClient rest => restDel(rest),
            _ => throw new NotImplementedException("Non supported discord client type.")
        };
    }

    /// <summary>
    /// Runs a client specific async implementation that returns something.
    /// </summary>
    /// <param name="socketDel">The async implementation for the socket client.</param>
    /// <param name="restDel">The async implementation for the rest client.</param>
    /// <exception cref="NotImplementedException">A non rest/socket client is used.</exception>
    public Task<T> ImplAsync<T>(Func<DiscordSocketClient, Task<T>> socketDel, Func<DiscordRestClient, Task<T>> restDel)
    {
        return Client switch
        {
            DiscordSocketClient socket => socketDel(socket),
            DiscordRestClient rest => restDel(rest),
            _ => throw new NotImplementedException("Non supported discord client type.")
        };
    }
}
