namespace SahneeBot.Jobs;

public interface IJob
{
    public Task Perform();
}