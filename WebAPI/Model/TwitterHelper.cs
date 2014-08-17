using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CoreTweet;

namespace WebAPI.Model
{
    public class TwitterHelper
    {
        private readonly Tokens _tokens;

        public TwitterHelper()
        {
            var consumerKey = ConfigurationManager.AppSettings["consumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["consumerSecret"];
            var accessToken = ConfigurationManager.AppSettings["accessToken"];
            var accessSecret = ConfigurationManager.AppSettings["accessSecret"];

            _tokens = Tokens.Create(consumerKey, consumerSecret, accessToken, accessSecret);
        }

        public async Task<Status> UpdateStatusAsync(string message)
        {
            return await _tokens.Statuses.UpdateAsync(status => message);
        }
    }
}