using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI;
using WebAPI.Model;

namespace WepAPITest
{
    [TestClass]
    public class HateBloRssReaderTest
    {
        private HateBloRssReader _sut;

        [TestMethod]
        public void テストデータから全件取得()
        {
            var xml = File.ReadAllText(@".\TestData\rss.xml");
            _sut = new HateBloRssReader(xml);

            var items = _sut.AsDynamic().GetAllRssItemsAsync().Result as List<RssItem>;

            Assert.AreEqual(7, items.Count());
            Assert.AreEqual("「もっと○○してからにしよう」をやめる", items[0].Title);
            Assert.AreEqual("http://nakaji.hatenablog.com/entry/2014/08/16/222644", items[0].Link);
            Assert.AreEqual(new DateTime(2014, 8, 16, 22, 26, 44), items[0].PubDate);
        }

        [TestMethod]
        public void テストデータから指定日以降のもののみ取得()
        {
            var xml = File.ReadAllText(@".\TestData\rss.xml");
            _sut = new HateBloRssReader(xml);

            var items = _sut.GetRssItemsAfterTheSpecifiedDateAsync(new DateTime(2014, 8, 16, 9, 0, 0)).Result as RssInfo;

            Assert.AreEqual(2, items.RssItems.Count());
        }

        [TestMethod]
        public void 指定期間内のエントリを取得()
        {
            var xml = File.ReadAllText(@".\TestData\rss.xml");
            _sut = new HateBloRssReader(xml);

            var items = _sut.GetRssItemsAsync(new DateTime(2014, 8, 16, 9, 0, 0), new DateTime(2014, 8, 16, 10, 0, 0)).Result as RssInfo;
            
            Assert.AreEqual(1, items.RssItems.Count());
        }

        [TestMethod]
        public void インターネットから()
        {
            _sut = new HateBloRssReader(new Uri("http://nakaji.hatenablog.com/rss"));

            var items = _sut.GetAllRssItemsAsync().Result;

            Assert.AreEqual(7, items.Count());
        }
    }
}
