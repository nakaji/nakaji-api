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
        private const string MessageNoBlog = @"最近、ブログ書いてません。（{0}日目）
なか日記
http://nakaji.hatenablog.com/
";

        // GET api/TweetBlog
        public async Task<List<RssItem>> Get()
        {
            var rssReader = new HateBloRssReader(new Uri("http://nakaji.hatenablog.com/rss"));

            var items = await rssReader.GetRssItemsAfterTheSpecifiedDateAsync(DateTime.Now.AddDays(-1));

            var twitterHelper = new TwitterHelper();
            try
            {
                if (items.RssItems.Count == 0)
                {
                    var message = string.Format(MessageNoBlog, (DateTime.Now-items.LastPubDate.Value).Days);
                    await twitterHelper.UpdateStatusAsync(message);
                    return null;
                }

                foreach (var rssItem in items.RssItems)
                {
                    var message = string.Format(MessageTemplate, rssItem.Title, rssItem.Link);
                    await twitterHelper.UpdateStatusAsync(message);
                }
                return items.RssItems;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
