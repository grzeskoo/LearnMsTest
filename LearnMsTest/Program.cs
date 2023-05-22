using LearnMsTest.GitHubIssue;
using LearnMsTest.MySolution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LearnMsTest;

internal class Program
{

    static void Main(string[] args)
    {
        CreateHostBuilder().Run();
    }

    private static IHost CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((services) =>
                {
                    services.AddHttpClient();
                    services.AddHostedService<MSLearnService>();
                    services.AddTransient<ILearnLangService, LearnLangService>();
                    services.AddTransient<IMSLearn, MSLearn>();
                })
            .Build();
    }
}