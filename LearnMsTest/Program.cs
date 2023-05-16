using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using LearnMsTest.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Net;
using System.Net.Http.Headers;
using MSLearnCatalogAPI;
using CatalogTypes = MSLearnCatalogAPI.CatalogTypes;

namespace LearnMsTest;

public class Program
{
    private const string MsLearnBaseAddress = "https://learn.microsoft.com/";
    private const string BaseMsLearnApiAddress = "api/catalog?locale=";
    private const string APIAddressScope = "&type=roles,modules,products,levels,learningPaths";
    private const string CorruptedAPIAddressScope = "&type=learningPaths,modules";

    static async Task Main(string[] args)
    {

        await FetchVendorData();
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

    public static async Task<List<LearnCatalog>> FetchLocaleMsLearnData()
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
                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                var response = client.DownloadString(addressWithLocale);
                dynamic parsedJson = JsonConvert.DeserializeObject(response);
                JsonConvert.SerializeObject(response, Formatting.Indented);
                var reeee = Convert.ToString(response);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        return listOfLearnData;
    }
    public const string Url = "https://learn.microsoft.com/api/catalog";

    private static string ConvertExpression(string expression)
    {
        expression = expression.Trim()
            .Replace(">=", "gte ")
            .Replace("<=", "lte ")
            .Replace("=", "eq ")
            .Replace(">", "gt ")
            .Replace("<", "lt ")
            .Replace("  ", " ");

        return WebUtility.HtmlEncode(expression);

    }

    public static async Task<LearnCatalog> GetCatalogAsync(string? locale = null, CatalogFilter? filters = null)
    {
        var endpoint = Url;
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(locale))
            parameters.Add($"locale={WebUtility.HtmlEncode(locale)}");
        if (filters?.Types != null)
        {
            var value = filters.Types.ToString() ?? string.Empty;
            var values = value.Split(',');
            value = string.Join(',', values.Select(v =>
            {
                v = v.Trim();
                v = char.ToLower(v[0]) + v[1..];
                return v;
            }));
            if (!string.IsNullOrWhiteSpace(value))
                parameters.Add($"type={WebUtility.HtmlEncode(value)}");
        }
        if (filters?.Uids?.Count > 0)
            parameters.Add($"uid={WebUtility.HtmlEncode(string.Join(',', filters.Uids.Where(s => !string.IsNullOrWhiteSpace(s))))}");
        if (!string.IsNullOrWhiteSpace(filters?.LastModifiedExpression))
            parameters.Add($"last_modified={ConvertExpression(filters.LastModifiedExpression)}");
        if (!string.IsNullOrWhiteSpace(filters?.PopularityExpression))
            parameters.Add($"popularity={ConvertExpression(filters.PopularityExpression)}");
        if (filters?.Levels?.Count > 0)
            parameters.Add($"level={WebUtility.HtmlEncode(string.Join(',', filters.Levels.Where(s => !string.IsNullOrWhiteSpace(s))))}");
        if (filters?.Roles?.Count > 0)
            parameters.Add($"role={WebUtility.HtmlEncode(string.Join(',', filters.Roles.Where(s => !string.IsNullOrWhiteSpace(s))))}");
        if (filters?.Products?.Count > 0)
            parameters.Add($"product={WebUtility.HtmlEncode(string.Join(',', filters.Products.Where(s => !string.IsNullOrWhiteSpace(s))))}");
        if (filters?.Subjects?.Count > 0)
            parameters.Add($"subject={WebUtility.HtmlEncode(string.Join(',', filters.Subjects.Where(s => !string.IsNullOrWhiteSpace(s))))}");

        if (parameters.Count > 0)
        {
            endpoint += "?" + string.Join('&', parameters);
        }

        using var client = new HttpClient();
        var response = await client.GetAsync(endpoint).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (errorText.Contains("message"))
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(errorText);
                if (error != null)
                {
                    throw new InvalidOperationException($"{error.ErrorCode}: {error.Message}");
                }
            }

            throw new InvalidOperationException(
                $"Failed to retrieve catalog information - {response.StatusCode}: {errorText}");
        }

        var jsonText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var catalog = JsonConvert.DeserializeObject<LearnCatalog>(jsonText,
            new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
        if (catalog == null)
            throw new InvalidOperationException(
                "Unable to parse results from Learn Catalog - possibly outdated schema?");

        // Fill in path ratings if the system didn't supply it.
        foreach (var path in catalog.LearningPaths.Where(p => p.Rating?.Count == 0))
        {
            var modules = catalog.ModulesForPath(path).ToList();
            if (modules.Any(m => m.Rating?.Count > 0))
            {
                path.Rating = new Rating
                {
                    Count = modules.Sum(m => m.Rating?.Count ?? 0),
                    Average = modules.Where(m => m.Rating != null)
                                     .Average(m => m.Rating!.Average)
                };
            }
        }

        return catalog;
    }
    private static async Task<string> GetData(string endpoint)
    {


        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(MsLearnBaseAddress)
        };

        var response = await httpClient.GetAsync(endpoint);

        var jsonText = await response.Content.ReadAsStringAsync();
        var catalog = JsonConvert.DeserializeObject<LearnCatalog>(jsonText,
            new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });

        //if (!response.IsSuccessStatusCode)
        //{
        //    return null;
        //}

        return "aaaa";
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