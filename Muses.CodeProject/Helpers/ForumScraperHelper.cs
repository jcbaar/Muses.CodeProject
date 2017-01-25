using Muses.CodeProject.API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Muses.CodeProject.Helpers
{
    /// <summary>
    /// For now the CodeProject API does not supply a method for requesting the available
    /// forum ID's through the API. This class "mocks" this by scraping the forum overview
    /// web page of the main site looking for links to the forums.
    /// 
    /// This class is removed just as soon as the API provides us with a method to list the
    /// forums.
    /// </summary>
    internal static class ForumScraperHelper
    {
        private static string _baseUrl = "https://www.codeproject.com/";
        private static string _forumsUrl = "script/Forums/List.aspx";

        /// <summary>
        /// Temporary helper to scrape forum links of the
        /// </summary>
        /// <returns>The <see cref="PageData"/> object with the forums or null in case of an error.</returns>
        public static async Task<PagedData> GetForumLinks()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUrl);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/text"));

                    HttpResponseMessage response = await client.GetAsync(_forumsUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var data = Find(content);

                        // Well... Hmmm. The lounge does not have a '/Forums/' link on the
                        // forumsd overview page...
                        data.Insert(0, new ItemSummary()
                        {
                            Title = "The Lounge",
                            Id = "1159",
                            WebsiteLink = new Uri("https://www.codeproject.com/Lounge.aspx")
                        });

                        return new PagedData
                        {
                            Pagination = new Pagination
                            {
                                Page = 1,
                                PageSize = data.Count,
                                TotalItems = data.Count,
                                TotalPages = 1
                            },
                            Items = data
                        };
                    }
                }
            }
            catch (HttpRequestException)
            {
            }
            return null;
        }

        /// <summary>
        /// Simple Html scraper build from the sample at:
        /// https://www.dotnetperls.com/scraping-html
        /// </summary>
        /// <param name="htmlContent">The Html content to scrape.</param>
        /// <returns>A list of <see cref="ItemSummary"/> containing the found forum links.</returns>
        public static List<ItemSummary> Find(string htmlContent)
        {
            List<ItemSummary> result = new List<ItemSummary>();

            // First we find all links.
            MatchCollection links = Regex.Matches(htmlContent, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

            foreach (Match m in links)
            {
                string linkValue = m.Groups[1].Value;

                // Match the 'href'.
                Match hrefs = Regex.Match(linkValue, @"href=\""(.*?)\""", RegexOptions.Singleline);
                if (hrefs.Success)
                {
                    // We are only interested in the links starting with '/Forums/'. All others
                    // are simply ingnored.
                    string href = hrefs.Groups[1].Value;
                    if (href.StartsWith("/Forums/", StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        continue;
                    }

                    Match id = Regex.Match(href, @"\d+", RegexOptions.Singleline);
                    if (id.Success)
                    {
                        var item = new ItemSummary()
                        {
                            WebsiteLink = new Uri(_baseUrl + href.Substring(1)),
                            Id = id.Groups[0].Value,
                            Title = Uri.UnescapeDataString(Regex.Replace(linkValue, @"\s*<.*?>\s*|&gt;", "", RegexOptions.Singleline))//.Replace("&gt;",""))
                        };
                        result.Add(item);
                    }
                }
            }
            return result;
        }
    }
}
