using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearnMsTest.GitHubIssue;

public class LearnLangService : ILearnLangService
{
    public async Task FetchData()
    {
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

        // Set variables
        var msLearnBaseAddress = "https://learn.microsoft.com/";
        var baseMsLearnApiAddress = "api/catalog?locale=";
        var ApiAddressScope = "&type=learningPaths,modules";
        var out_filename = "Learn_Catalog.txt";

        // Create an empty file
        File.Create(out_filename).Dispose();

        // Loop through all locale languages
        foreach (var locale in locales)
        {
            Console.WriteLine(locale);

            // Without retry calling
            //GetCatalog(locale, msLearnBaseAddress, baseMsLearnApiAddress, corruptedApiAddressScope, out_filename);

            // With retry calling
            Retry.Do(() => GetCatalog(locale, msLearnBaseAddress, baseMsLearnApiAddress, ApiAddressScope, out_filename), TimeSpan.FromSeconds(5), 5);
        }
    }

    public void GetCatalog(string locale, string msLearnBaseAddress, string baseMsLearnApiAddress, string ApiAddressScope, string filename)
    {
        var addressWithLocale = msLearnBaseAddress + baseMsLearnApiAddress + locale + ApiAddressScope;

        List<string> errors = new List<string>();
        // Get data
        var client = new WebClient();
        client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
        var response = client.DownloadString(addressWithLocale);
        var settings = new JsonSerializerSettings
        {
            Error = (se, ev) =>
        {
            errors.Add(se.ToString());
            ev.ErrorContext.Handled = true;
        },
            MissingMemberHandling = MissingMemberHandling.Error
        };
        dynamic parsedJson = JsonConvert.DeserializeObject(response, settings);
        JsonConvert.SerializeObject(response, Formatting.Indented);
        var reeee = Convert.ToString(response);


        //// Write to output file
        //using (StreamWriter w = File.AppendText(filename))
        //{ w.WriteLine(reeee); }
    }

}