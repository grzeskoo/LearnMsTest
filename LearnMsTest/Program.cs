using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using LearnMsTest.Dtos;
using Newtonsoft.Json;

namespace LearnMsTest;

public class Program
{
    private const string MsLearnBaseAddress = "https://learn.microsoft.com/";
    private const string BaseMsLearnApiAddress = "api/catalog?locale=";
    private const string APIAddressScope = "&type=roles,modules,products,levels,learningPaths";
    private const string CorruptedAPIAddressScope = "&type=roles,products,levels,learningPaths,modules";

    static void Main(string[] args)
    {

        FetchVendorData().Wait();
    }

    static async Task FetchVendorData()
    {
        var locales = new List<MsLearnLocaleDto>
        {
            new() {LocaleCode = "ar-SA", IsActive = true},
            new() {LocaleCode = "ru-RU", IsActive = true},
            new() {LocaleCode = "nl-BE", IsActive = true},
            new() {LocaleCode = "bs-cyrl-BA", IsActive = true},
            new() {LocaleCode =  "es-ES", IsActive = true},
            new() {LocaleCode = "de-DE", IsActive = true},
            new() {LocaleCode = "fr-BE", IsActive = true}

            //"ar-SA", "bg-BG", "bs-cyrl-BA", "bs-latn-BA", "ca-ES", "cs-CZ", "da-DK",
            //"de-AT", "de-CH", "de-DE", "el-GR", "en-AU", "en-CA", "en-GB", "en-IE", "en-IN", "en-MY",
            //"en-NZ", "en-SG", "en-US", "en-ZA", "es-ES", "es-MX", "et-EE", "eu-ES", "fi-FI", "fil-PH",
            //"fr-BE", "fr-CA", "fr-CH", "fr-FR", "ga-IE", "gl-ES", "he-IL", "hi-IN", "hr-HR", "hu-HU",
            //"id-ID", "is-IS", "it-CH", "it-IT", "ja-JP", "kk-KZ", "ko-KR", "lb-LU", "lt-LT", "lv-LV",
            //"ms-MY", "mt-MT", "nb-NO", "nl-BE", "nl-NL", "pl-PL", "pt-BR", "pt-PT", "ro-RO", "ru-RU",
            //"sk-SK", "sl-SI", "sr-cyrl-RS", "sr-latn-RS", "sv-SE", "th-TH", "tr-TR", "uk-UA", "vi-VN",
            //"zh-CN",
            //"zh-HK", "zh-TW"
        };

        // parallel 
        // await FetchLocaleMsLearnData(locales);

        await FetchLocaleMsLearnData();
    }

    public static async Task<List<MsLearnDataDto>> FetchLocaleMsLearnData()
    {
        var locales = new List<string>
        { "sv-SE"
            //,
            //"ar-SA", "bg-BG", "bs-cyrl-BA", "bs-latn-BA", "ca-ES", "cs-CZ", "da-DK",
            //"de-AT", "de-CH", "de-DE", "el-GR", "en-AU", "en-CA", "en-GB", "en-IE", "en-IN", "en-MY",
            //"en-NZ", "en-SG", "en-US", "en-ZA", "es-ES", "es-MX", "et-EE", "eu-ES", "fi-FI", "fil-PH",
            //"fr-BE", "fr-CA", "fr-CH", "fr-FR", "ga-IE", "gl-ES", "he-IL", "hi-IN", "hr-HR", "hu-HU",
            //"id-ID", "is-IS", "it-CH", "it-IT", "ja-JP", "kk-KZ", "ko-KR", "lb-LU", "lt-LT", "lv-LV",
            //"ms-MY", "mt-MT", "nb-NO", "nl-BE", "nl-NL", "pl-PL", "pt-BR", "pt-PT", "ro-RO", "ru-RU",
            //"sk-SK", "sl-SI", "sr-cyrl-RS", "sr-latn-RS", "th-TH", "tr-TR", "uk-UA", "vi-VN", "zh-CN",
            //"zh-HK", "zh-TW"
        };
        var listOfLearnData = new List<MsLearnDataDto>();

        foreach (var locale in locales)
        {
            try
            {
                var addressWithLocale = BaseMsLearnApiAddress + locale + CorruptedAPIAddressScope;
                var response = await GetData(addressWithLocale);
                //check data
                var result = JsonConvert.DeserializeObject<MsLearnDataDto>(response);
                listOfLearnData.Add(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return listOfLearnData;
    }

    private static async Task<string> GetData(string endpoint)
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(MsLearnBaseAddress)
        };

        var response = await httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }

    private static async Task<List<MsLearnDataDto>> FetchLocaleMsLearnData(IList<MsLearnLocaleDto> msLearnLocaleDtos, int nrOfAttempts = 0)
    {
        var errorList = new List<MsLearnLocaleDto>();
        var learnDataResult = new List<MsLearnDataDto>();

        await Parallel.ForEachAsync(msLearnLocaleDtos.Where(x => x.IsActive), new ParallelOptions { MaxDegreeOfParallelism = 2 }, async (locale, token) =>
            {
                try
                {
                    var addressWithLocale = BaseMsLearnApiAddress + locale.LocaleCode + CorruptedAPIAddressScope;
                    var response = await GetDataAsync(addressWithLocale, token);
                    var result = JsonConvert.DeserializeObject<MsLearnDataDto>(response);

                    learnDataResult.Add(result);
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
            await FetchLocaleMsLearnData(errorList, nrOfAttempts);
        }

        return learnDataResult;
    }

    private static async Task<string> GetDataAsync(string endpoint, CancellationToken token)
    {
        Console.WriteLine(endpoint);

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(MsLearnBaseAddress)
        };

        var response = await httpClient.GetAsync(endpoint, token);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadAsStringAsync(token);
    }
}