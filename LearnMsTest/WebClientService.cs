using System;
using MSLearnCatalogAPI;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearnMsTest;

public class WebClientService : IWebClientService
{
    public async Task<List<LearnCatalog>> FetchLocaleMsLearnDataUsingWebClient()
    {
        var listOfLearnData = new List<LearnCatalog>();
        var locales = new List<string>
        {
            "ar-SA", "bg-BG", "bs-cyrl-BA", "bs-latn-BA", "ca-ES", "cs-CZ", "da-DK",
            "de-AT", "de-CH", "de-DE", "el-GR", "en-AU", "en-CA", "en-GB", "en-IE", "en-IN", "en-MY",
            "en-NZ", "en-SG", "en-US", "en-ZA", "es-ES", "es-MX", "et-EE", "eu-ES", "fi-FI", "fil-PH",
            "fr-BE", "fr-CA", "fr-CH", "fr-FR", "ga-IE", "gl-ES", "he-IL", "hi-IN", "hr-HR", "hu-HU",
            "id-ID", "is-IS", "it-CH", "it-IT", "ja-JP", "kk-KZ", "ko-KR", "lb-LU", "lt-LT", "lv-LV",
            "ms-MY", "mt-MT", "nb-NO", "nl-BE", "nl-NL", "pl-PL", "pt-BR", "pt-PT", "ro-RO", "ru-RU",
            "sk-SK", "sl-SI", "sr-cyrl-RS", "sr-latn-RS", "th-TH", "tr-TR", "uk-UA", "vi-VN", "zh-CN",
            "zh-HK", "zh-TW"
        };

        foreach (var locale in locales)
        {
            Console.WriteLine(locale);
            var msLearnBaseAddress = "https://learn.microsoft.com/";
            var baseMsLearnApiAddress = "api/catalog?locale=";
            var corruptedApiAddressScope = "&type=learningPaths,modules";

            try
            {
                var addressWithLocale = msLearnBaseAddress + baseMsLearnApiAddress + locale + corruptedApiAddressScope;

                var client = new WebClient();
                client.Headers.Add(HttpRequestHeader.UserAgent,
                    "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                var response = client.DownloadString(addressWithLocale);
                dynamic parsedJson = JsonConvert.DeserializeObject(response);
                JsonConvert.SerializeObject(response, Formatting.Indented);
                // var converted = Convert.ToString(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return listOfLearnData;
    }
}

public interface IWebClientService
{
    Task<List<LearnCatalog>> FetchLocaleMsLearnDataUsingWebClient();
}