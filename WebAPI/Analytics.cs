using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace WebAPI
{
    public class Analytics
    {
        public async static Task<int> GetPvAsync()
        {
            // Azure Web サイトで動かす場合には WEBSITE_LOAD_USER_PROFILE = 1 必須
            var file = ConfigurationManager.AppSettings["analyticsKeyFile"];
            var analyticsKeyFile = file[0] == '~' ? HttpContext.Current.Server.MapPath(file) : file;
            var certificate = new X509Certificate2(analyticsKeyFile, "notasecret", X509KeyStorageFlags.Exportable);

            // Scopes は指定しないとエラーになる
            var analyticsCredentialId = ConfigurationManager.AppSettings["analyticsCredentialId"];
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(analyticsCredentialId)
            {
                Scopes = new[] { AnalyticsService.Scope.Analytics, AnalyticsService.Scope.AnalyticsReadonly }
            }.FromCertificate(certificate));

            // HttpClientInitializer に credential 入れるのは違和感あるけど正しいらしい
            var service = new AnalyticsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "TweetPV",
            });

            // Azure は UTC なので +9 時間して -1 日
            var date = DateTime.UtcNow.AddHours(9).AddDays(-1).ToString("yyyy-MM-dd");

            // ****** はメモしたビューの ID
            var analyticsViewId = ConfigurationManager.AppSettings["analyticsViewId"];
            var data = await service.Data.Ga.Get("ga:" + analyticsViewId, date, date, "ga:pageviews").ExecuteAsync();

            return int.Parse(data.Rows[0][0]);
        }
    }
}