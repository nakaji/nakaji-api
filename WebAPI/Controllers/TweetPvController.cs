using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CoreTweet;

namespace WebAPI.Controllers
{
    public class TweetPvController : ApiController
    {
        // GET api/values
        public async Task<string> Get()
        {
            // アプリケーションの設定方法の詳細については、http://go.microsoft.com/fwlink/?LinkID=316888 を参照してください

            var consumerKey = ConfigurationManager.AppSettings["consumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["consumerSecret"];
            var accessToken = ConfigurationManager.AppSettings["accessToken"];
            var accessSecret = ConfigurationManager.AppSettings["accessSecret"];

            var token = Tokens.Create(consumerKey,consumerSecret,accessToken,accessSecret);
            try
            {
                var pv = await Analytics.GetPvAsync();
                var message = string.Format("昨日のなか日記のPVは{0}でした http://nakaji.hatenablog.com/", pv);
                await token.Statuses.UpdateAsync(status => message);

                return message;
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}