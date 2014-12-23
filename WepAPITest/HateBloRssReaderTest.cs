using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI;
using WebAPI.Model;

namespace WepAPITest
{
    [TestClass]
    public class HateBloRssReaderTest
    {
        private HateBloRssReader _sut;

        [TestInitialize]
        public void SetUp()
        {
            var url = "http://nakaji.hatenablog.com/rss";
            _sut = new HateBloRssReader(url);
        }

        [TestMethod]
        public void テストデータから全件取得()
        {
            var xml = File.ReadAllText(@".\TestData\rss.xml");
            var items = _sut.GetAllRssItems(xml);

            Assert.AreEqual(7, items.Count());
            Assert.AreEqual("「もっと○○してからにしよう」をやめる", items[0].Title);
            Assert.AreEqual("http://nakaji.hatenablog.com/entry/2014/08/16/222644", items[0].Link);
            Assert.AreEqual(new DateTime(2014,8,16,22,26,44), items[0].PubDate);
        }

        [TestMethod]
        public void テストデータから指定日以降のもののみ取得()
        {
            var xml = File.ReadAllText(@".\TestData\rss.xml");
            var items = _sut.GetRssItemsAfterTheSpecifiedDate(xml,new DateTime(2014,8,16,9,0,0));

            Assert.AreEqual(2, items.RssItems.Count());
        }

        [TestMethod]
        public void インターネットから()
        {
            var items = _sut.GetAllRssItemsAsync().Result;
        
            Assert.AreEqual(7, items.Count());
        }
    }
}
