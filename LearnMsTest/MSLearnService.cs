using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using LearnMsTest.GitHubIssue;
using LearnMsTest.MySolution;

namespace LearnMsTest;

public class MSLearnService : BackgroundService
{
    private readonly IMSLearn _msLearn;
    private readonly ILearnLangService _learnLangService;

    public MSLearnService(IMSLearn msLearn, ILearnLangService learnLangService)
    {
        _msLearn = msLearn;
        _learnLangService = learnLangService;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //fetch data using solution form Github
    //     await  _learnLangService.FetchData();

        //fetch data using my solution
        await _msLearn.FetchVendorData();
    }

}