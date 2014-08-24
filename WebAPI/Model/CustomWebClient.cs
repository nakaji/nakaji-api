using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace WebAPI.Model
{
    /// <summary>
    /// Cookieが使えるWebClient
    /// 下記ブログのまるパクリ
    /// neue cc - C#のWebRequestとWebClientでCookie認証をする方法(と、mixiボイスへの投稿)
    /// http://neue.cc/2009/12/17_230.html
    /// </summary>
    public class CustomWebClient : WebClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        // WebClientはWebRequestのラッパーにすぎないので、
        // GetWebRequestのところの動作をちょっと横取りして書き換える
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = _cookieContainer;
            }
            return request;
        }
    }
}