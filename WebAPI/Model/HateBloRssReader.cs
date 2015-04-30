using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WebGrease.Css.Extensions;

namespace WebAPI.Model
{
    public class HateBloRssReader
    {
        public Uri Url { get; private set; }
        public string RssXml { get; private set; }

        public HateBloRssReader(Uri rssUrl)
        {
            Url = rssUrl;
        }

        public HateBloRssReader(string xml)
        {
            RssXml = xml;
        }

        /// <summary>
        /// URLからRSS情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<RssItem>> GetAllRssItemsAsync()
        {
            if (string.IsNullOrEmpty(RssXml))
            {
                using (var client = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    var xml = await client.DownloadStringTaskAsync(Url);
                    RssXml = xml;
                }
            }

            return GetItems();
        }

        /// <summary>
        /// 指定日以降のエントリと最終更新日を取得する
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<RssInfo> GetRssItemsAfterTheSpecifiedDateAsync(DateTime date)
        {
            var items = await GetAllRssItemsAsync();
            var info = new RssInfo();
            info.RssItems = items.Where(x => x.PubDate >= date).ToList();
            info.LastPubDate = items.Max(x => x.PubDate);
            return info;
        }

        public async Task<RssInfo> GetRssItemsAsync(DateTime dateFrom, DateTime dateTo)
        {
            var items = await GetAllRssItemsAsync();
            var info = new RssInfo();
            info.RssItems = items.Where(x => x.PubDate >= dateFrom && x.PubDate <= dateTo).ToList();
            info.LastPubDate = items.Max(x => x.PubDate);
            return info;
        }

        /// <summary>
        /// XMLからエントリ一覧を取得する
        /// </summary>
        /// <returns></returns>
        private List<RssItem> GetItems()
        {
            var reader = XmlReader.Create(new StringReader(RssXml));
            var doc = XDocument.Load(reader);

            var ns = doc.Root.Name.Namespace;
            var itemRoot = doc.Descendants(ns + "item");
            var items = new List<RssItem>();

            itemRoot.ForEach(x => items.Add(new RssItem()
            {
                Title = x.Descendants(ns + "title").First().Value,
                Link = x.Descendants(ns + "link").First().Value,
                Description = x.Descendants(ns + "description").First().Value,
                PubDate = DateTime.Parse(x.Descendants(ns + "pubDate").First().Value)
            }));

            return items;
        }
    }

    public class RssInfo
    {
        public DateTime? LastPubDate { get; set; }
        public List<RssItem> RssItems { get; set; }
    }

    public class RssItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }
    }
}
