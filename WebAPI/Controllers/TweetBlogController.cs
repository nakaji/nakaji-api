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
        private const string MessageNoBlog = @"最近、ブログ書いてません。
なか日記
http://nakaji.hatenablog.com/
";

        // GET api/TweetBlog
        public async Task<List<RssItem>> Get()
        {
            var rssReader = new HateBloRssReader("http://nakaji.hatenablog.com/rss");

            var items = await rssReader.GetRssItemsAfterTheSpecifiedDateAsync(DateTime.Now.AddDays(-1));

            var twitterHelper = new TwitterHelper();
            try
            {
                if (items.Count == 0)
                {
                    await twitterHelper.UpdateStatusAsync(MessageNoBlog);
                    return null;
                }
                foreach (var rssItem in items)
                {
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
