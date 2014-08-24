using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Sgml;

namespace WebAPI.Model
{
    public static class HatenaCounterHelper
    {
        private const string LoginParamBase = "name={0}&password={1}&persistent=1";
        private const string CounterUrlBase = "http://counter.hatena.ne.jp/{0}/report?cid={1}&date={2}&mode=access";
        public static int GetPv(int cid, DateTime date)
        {
            var hatenaId = ConfigurationManager.AppSettings["hatenaId"];
            var hatenaPassword = ConfigurationManager.AppSettings["hatenaPassword"];


            var wc = new CustomWebClient() { Encoding = Encoding.UTF8 };
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var data = string.Format(LoginParamBase, hatenaId, hatenaPassword);
            wc.UploadString("https://www.hatena.ne.jp/login", "POST", data);

            var url = string.Format(CounterUrlBase, hatenaId, cid, date.ToString("yyyy-MM-dd"));
            var res = wc.DownloadString(url);

            XDocument xml;
            using (var sgml = new SgmlReader() { IgnoreDtd = true })
            {
                sgml.InputStream = new StringReader(res);
                xml = XDocument.Load(sgml);
            }
            var ns = xml.Root.Name.Namespace;
            var count = xml.Descendants(ns + "table")
                .Where(x => x.FirstAttribute.Value == "totalcount")
                .Descendants(ns + "strong")
                .First().Value;
            return int.Parse(count);
        }
    }
}