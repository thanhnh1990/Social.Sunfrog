using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using HtmlAgilityPack;
using System.IO;
using RestSharp.Extensions.MonoHttp;
using System.Text.RegularExpressions;

namespace Social.Sunfrog
{
    class Crawler
    {
        public string GetAjaxData(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        public int ReadHtmlResponse(string url_sun, string category, string tag, int id_type, int dem)
        {
            int result = 0;
            Logs log = new Logs();
            try
            {
                Logic logic = new Logic();
                string html = GetAjaxData(url_sun);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var searchResults = new List<SearchResultHtml>();
                var aTags = doc.DocumentNode.SelectNodes("//a");
                var divTags = doc.DocumentNode.SelectNodes("//div[contains(@class, 'frameit')]");
                if (divTags != null && aTags != null)
                {
                    foreach (var aTag in aTags)
                    {
                        var list = new SearchResultHtml();
                        list.url = aTag.Attributes["href"].Value;
                        searchResults.Add(list);
                    }
                    if (searchResults.Count() > 0)
                    {
                        int count = 0;
                        foreach (var link in searchResults)
                        {
                            if (count > 35 && dem > 2)
                            {
                                count = -1;
                                break;
                            }

                            var data = GetHtml(link.url);
                            data.CategorySearch = category;
                            data.Tag = tag;
                            data.Type = id_type;
                            int result_count = logic.execute(data);
                            if (result_count == 0)
                                break;
                            else if (result_count == 2)
                                count++;
                            System.Threading.Thread.Sleep(5000);
                        }
                        if (count == -1)
                            result = 2;
                        else
                            result = 1;
                    }
                }

            }
            catch (Exception ex)
            {
                result = -1;
                log.IErrors("LoadHtml - ReadHtmlResponse: " + ex.Message);
            }
            return result;
        }
        public DataLoad GetHtml(string url_sun)
        {
            DataLoad data = new DataLoad();
            try
            {
                string html = GetAjaxData(url_sun);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var titleTag = doc.DocumentNode.SelectSingleNode("//title").InnerText;
                data.Title = titleTag;
                data.Url = url_sun;
                var meta = doc.DocumentNode.SelectNodes("//meta");
                foreach (var node in meta)
                {
                    //Description
                    if (node.GetAttributeValue("name", String.Empty) == "Description")
                    {
                        data.Description = node.GetAttributeValue("content", String.Empty) + " | Best T-Shirts USA are very happy to make you beutiful - Shirts as unique as you are.";
                    }
                    //Keyword
                    if (node.GetAttributeValue("name", String.Empty) == "Keywords")
                    {
                        data.Keywords = node.GetAttributeValue("content", String.Empty);
                    }
                }
                //Group ID
                var groupNote = doc.DocumentNode.SelectSingleNode("//input[@name='MockupGroup']");
                data.GroupId = groupNote.Attributes["value"].Value;

                //SKU
                var skuNote = doc.DocumentNode.SelectSingleNode("//input[@name='mockupID']");
                data.Sku = skuNote.Attributes["value"].Value;

                //Cate
                var catNote = doc.DocumentNode.SelectSingleNode("//input[@name='catName']");
                data.CategoryName = catNote.Attributes["value"].Value;

                //Cate
                var imgNote = doc.DocumentNode.SelectSingleNode("//img[@id='MainImgShow']");
                data.Image = imgNote.Attributes["src"].Value;

                //Url name
                data.UrlName = ToUrlSlug(titleTag).ToLower();

                //Price
                var priceNote = doc.DocumentNode.SelectNodes("//select[@id='shirtTypes']//option");

                string optionNote = "";
                foreach (HtmlNode node in priceNote)
                {
                    optionNote = node.NextSibling.InnerHtml;
                    break;

                }
                var text_option = optionNote.Split('$');
                data.Price = decimal.Parse(text_option[1]);
                //string type_array = text_option[0].Split(' ')[0];

            }
            catch (Exception ex)
            {

            }
            return data;
        }
        public class SearchResultHtml
        {
            public string url { get; set; }
        }

        public static string ToUrlSlug(string value)
        {

            //First to lower case
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
    }
}
