using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CoreTweet;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    public class TweetBlogController : ApiController
    {
        private const string MessageTemplate = @"ブログ書きました。
{0} - なか日記
{1}
";

        // GET api/TweetBlog
        public async Task<List<RssItem>> Get()
        {
            var rssReader = new HateBloRssReader("http://nakaji.hatenablog.com/rss");

            var items = await rssReader.GetRssItemsAfterTheSpecifiedDateAsync(DateTime.Now.AddDays(-1));

            var consumerKey = ConfigurationManager.AppSettings["consumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["consumerSecret"];
            var accessToken = ConfigurationManager.AppSettings["accessToken"];
            var accessSecret = ConfigurationManager.AppSettings["accessSecret"];

            var token = Tokens.Create(consumerKey, consumerSecret, accessToken, accessSecret);
            string tweets;
            try
            {
                foreach (var rssItem in items)
                {
                    var pv = await Analytics.GetPvAsync();
                    var message = string.Format(MessageTemplate, rssItem.Title, rssItem.Link);
                    await token.Statuses.UpdateAsync(status => message);
                }

                return items;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
