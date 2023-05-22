using LearnMsTest.Dtos;
using MSLearnCatalogAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace LearnMsTest;

public class ExternalLibs
{
    public const string Url = "https://learn.microsoft.com/api/catalog";

    public async Task<LearnCatalog> GetCatalogAsync(string? locale = null, CatalogFilter? filters = null)
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


    private string ConvertExpression(string expression)
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

}