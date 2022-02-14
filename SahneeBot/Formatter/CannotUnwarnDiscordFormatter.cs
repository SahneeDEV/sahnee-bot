using Discord;

namespace SahneeBot.Formatter;

public class CannotUnwarnDiscordFormatter: IDiscordFormatter<IUser>
{
    private readonly DefaultFormatArguments _fmt;
    
    public CannotUnwarnDiscordFormatter(DefaultFormatArguments fmt)
    {
        _fmt = fmt;
    }
    
    public Task<DiscordFormat> Format(IUser arg)
    {
        return Task.FromResult(new DiscordFormat($"Cannot unwarn {_fmt.GetMention(arg)}: No warnings left"));
    }
}