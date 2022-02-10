using Microsoft.Extensions.DependencyInjection;
using SahneeBotModel;

namespace SahneeBotController;

public class Class1
{
    public Class1(IServiceProvider services)
    {
        using var serviceScopoe = services.CreateScope();
        var provider = serviceScopoe.ServiceProvider;
        
        test(provider.GetRequiredService<SahneeBotModelContext>());
    }

    private void test(SahneeBotModelContext sahneeBotModelContext)
    {
        Console.Write("Dependency Injection!");
    }
    
}