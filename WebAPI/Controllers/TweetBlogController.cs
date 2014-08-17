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

            var twitterHelper = new TwitterHelper();
            try
            {
                foreach (var rssItem in items)
                {
                    var pv = await Analytics.GetPvAsync();
                    var message = string.Format(MessageTemplate, rssItem.Title, rssItem.Link);
                    await twitterHelper.UpdateStatusAsync(message);
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
