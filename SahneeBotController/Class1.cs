using Microsoft.Extensions.DependencyInjection;
using SahneeBotModel;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController;

public class Class1
{
    public Class1(IServiceProvider services)
    {
        using var serviceScopoe = services.CreateScope();
        var provider = serviceScopoe.ServiceProvider;
    }
}
