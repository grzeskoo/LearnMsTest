using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LearnMsTest.Dtos;
using Newtonsoft.Json;
using LearnMsTest.GitHubIssue;

namespace LearnMsTest.MySolution;

public class MSLearn : IMSLearn
{
    private const string MsLearnBaseAddress = "https://learn.microsoft.com/";
    private const string BaseMsLearnApiAddress = "api/catalog?locale=";
    private const string CorruptedAPIAddressScope = "&type=learningPaths,modules";

    private readonly HttpClient _httpClient;

    public MSLearn(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(MsLearnBaseAddress);
    }

    public async Task FetchVendorData()
    {
        var locales = new List<MsLearnLocaleDto>
        {
            new() {LocaleCode =  "en-GB", IsActive = true},
            new() {LocaleCode = "ar-SA", IsActive = true},
            new() {LocaleCode =  "bg-BG", IsActive = true},
            new() {LocaleCode =  "bs-cyrl-BA", IsActive = true},
            new() {LocaleCode =  "bs-latn-BA", IsActive = true},
            new() {LocaleCode =  "ca-ES", IsActive = true},
            new() {LocaleCode =  "cs-CZ", IsActive = true},
            new() {LocaleCode =  "da-DK", IsActive = true},
            new() {LocaleCode = "de-AT", IsActive = true},
            new() {LocaleCode =  "de-CH", IsActive = true},
            new() {LocaleCode =  "de-DE", IsActive = true},
            new() {LocaleCode =  "el-GR", IsActive = true},
            new() {LocaleCode =  "en-AU", IsActive = true},
            new() {LocaleCode =  "en-CA", IsActive = true},
            new() {LocaleCode =  "en-IE", IsActive = true},
            new() {LocaleCode =  "en-IN", IsActive = true},
            new() {LocaleCode =  "en-MY", IsActive = true},
            new() {LocaleCode = "en-NZ", IsActive = true},
            new() {LocaleCode =  "en-SG", IsActive = true},
            new() {LocaleCode =  "en-US", IsActive = true},
            new() {LocaleCode =  "en-ZA", IsActive = true},
            new() {LocaleCode =  "es-ES", IsActive = true},
            new() {LocaleCode =  "es-MX", IsActive = true},
            new() {LocaleCode =  "et-EE", IsActive = true},
            new() {LocaleCode =  "eu-ES", IsActive = true},
            new() {LocaleCode =  "fi-FI", IsActive = true},
            new() {LocaleCode =  "fil-PH", IsActive = true},
            new() {LocaleCode = "fr-BE", IsActive = true},
            new() {LocaleCode =  "fr-CA", IsActive = true},
            new() {LocaleCode =  "fr-CH", IsActive = true},
            new() {LocaleCode =  "fr-FR", IsActive = true},
            new() {LocaleCode =  "ga-IE", IsActive = true},
            new() {LocaleCode =  "gl-ES", IsActive = true},
            new() {LocaleCode =  "he-IL", IsActive = true},
            new() {LocaleCode =  "hi-IN", IsActive = true},
            new() {LocaleCode =  "hr-HR", IsActive = true},
            new() {LocaleCode =  "hu-HU", IsActive = true},
            new() {LocaleCode = "id-ID", IsActive = true},
            new() {LocaleCode =  "is-IS", IsActive = true},
            new() {LocaleCode =  "it-CH", IsActive = true},
            new() {LocaleCode =  "it-IT", IsActive = true},
            new() {LocaleCode =  "ja-JP", IsActive = true},
            new() {LocaleCode =  "kk-KZ", IsActive = true},
            new() {LocaleCode =  "ko-KR", IsActive = true},
            new() {LocaleCode =  "lb-LU", IsActive = true},
            new() {LocaleCode =  "lt-LT", IsActive = true},
            new() {LocaleCode =  "lv-LV", IsActive = true},
            new() {LocaleCode = "ms-MY", IsActive = true},
            new() {LocaleCode =  "mt-MT", IsActive = true},
            new() {LocaleCode =  "nb-NO", IsActive = true},
            new() {LocaleCode =  "nl-BE", IsActive = true},
            new() {LocaleCode =  "nl-NL", IsActive = true},
            new() {LocaleCode =  "pl-PL", IsActive = true},
            new() {LocaleCode =  "pt-BR", IsActive = true},
            new() {LocaleCode =  "pt-PT", IsActive = true},
            new() {LocaleCode =  "ro-RO", IsActive = true},
            new() {LocaleCode =  "ru-RU", IsActive = true},
            new() {LocaleCode = "sk-SK", IsActive = true},
            new() {LocaleCode =  "sl-SI", IsActive = true},
            new() {LocaleCode =  "sr-cyrl-RS", IsActive = true},
            new() {LocaleCode =  "sr-latn-RS", IsActive = true},
            new() {LocaleCode =  "sv-SE", IsActive = true},
            new() {LocaleCode =  "th-TH", IsActive = true},
            new() {LocaleCode =  "tr-TR", IsActive = true},
            new() {LocaleCode =  "uk-UA", IsActive = true},
            new() {LocaleCode =  "vi-VN", IsActive = true},
            new() {LocaleCode = "zh-CN", IsActive = true},
            new() {LocaleCode = "zh-HK", IsActive = true},
            new() {LocaleCode =  "zh-TW", IsActive = true}
        };

        var learnDataResult = new List<MsLearnDataDto>();

        await FetchLocaleMsLearnDataModified(locales, learnDataResult);
    }

    public async Task<List<MsLearnDataDto>> FetchLocaleMsLearnDataModified(IList<MsLearnLocaleDto> msLearnLocaleDtos, List<MsLearnDataDto> msLearnDataDtos, int nrOfAttempts = 0)
    {
        var baseAddress = "api/catalog?locale=en-US&type=roles,products,levels";
        var baseResult = await GetDataAsync(baseAddress, new CancellationToken());
        var baseClassResult = JsonConvert.DeserializeObject<MsLearnDataDto>(baseResult);

        var errorList = new List<MsLearnLocaleDto>();
        List<string> errors = new List<string>();


        await Parallel.ForEachAsync(msLearnLocaleDtos.Where(x => x.IsActive), new ParallelOptions { MaxDegreeOfParallelism = 1 }, async (locale, token) =>
        {
            try
            {
                var learningModulesFilter = BaseMsLearnApiAddress + locale.LocaleCode + "&type=modules";
                var learningPathsFilter = BaseMsLearnApiAddress + locale.LocaleCode + "&type=learningPaths";
                var settings = new JsonSerializerSettings
                {
                    Error = (se, ev) =>
                    {
                        errors.Add(se.ToString());
                        ev.ErrorContext.Handled = true;
                    },
                    //MissingMemberHandling = MissingMemberHandling.Error
                };
                var learningModulesResults = JsonConvert.DeserializeObject<MsLearnDataDto>(await Retry.Do(async () => await GetDataAsync(learningModulesFilter, token), TimeSpan.FromSeconds(5), 5), settings);
                var learningPathsResults = JsonConvert.DeserializeObject<MsLearnDataDto>(await Retry.Do(async () => await GetDataAsync(learningPathsFilter, token), TimeSpan.FromSeconds(5), 5), settings);

                // var learningModulesResults = JsonConvert.DeserializeObject<MsLearnDataDto>(await GetDataAsync(learningModulesFilter, token));
                //  var LearningPathsResults = JsonConvert.DeserializeObject<MsLearnDataDto>(await GetDataAsync(LearningPathsFilter, token));

                var result = new MsLearnDataDto()
                {
                    Products = baseClassResult?.Products,
                    Levels = baseClassResult?.Levels,
                    Roles = baseClassResult?.Roles,
                    Modules = learningModulesResults?.Modules,
                    LearningPaths = learningPathsResults?.LearningPaths
                };

                msLearnDataDtos.Add(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + locale);
                errorList.Add(locale);
            }
        });

        if (errorList.Any() && nrOfAttempts <= 3)
        {
            nrOfAttempts++;
            await FetchLocaleMsLearnData(errorList, msLearnDataDtos, nrOfAttempts);
        }

        return msLearnDataDtos;
    }

    private async Task<List<MsLearnDataDto>> FetchLocaleMsLearnData(IList<MsLearnLocaleDto> msLearnLocaleDtos,
        List<MsLearnDataDto> msLearnDataDtos, int nrOfAttempts = 0)
    {
        var errorList = new List<MsLearnLocaleDto>();

        await Parallel.ForEachAsync(msLearnLocaleDtos.Where(x => x.IsActive), new ParallelOptions { MaxDegreeOfParallelism = 3 }, async (locale, token) =>
        {
            try
            {
                var addressWithLocale = BaseMsLearnApiAddress + locale.LocaleCode + CorruptedAPIAddressScope;
                var response = await GetDataAsync(addressWithLocale, token);
                var result = JsonConvert.DeserializeObject<MsLearnDataDto>(response);

                msLearnDataDtos.Add(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + locale + "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                errorList.Add(locale);
            }
        });

        if (errorList.Any() && nrOfAttempts <= 3)
        {
            nrOfAttempts++;
            await FetchLocaleMsLearnData(errorList, msLearnDataDtos, nrOfAttempts);
        }

        return msLearnDataDtos;
    }

    private async Task<string> GetDataAsync(string endpoint, CancellationToken token)
    {
        //Console.WriteLine(endpoint);

        //Get Stream

        //var response = await httpClient.GetStreamAsync(endpoint, token);
        //var reader = new StreamReader(response);
        //var data = await reader.ReadToEndAsync();
        //return data;

        //Get Response

        var response = await _httpClient.GetAsync(endpoint, token);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadAsStringAsync(token);

    }



}