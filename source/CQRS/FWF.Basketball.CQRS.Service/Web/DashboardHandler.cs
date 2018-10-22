using Microsoft.AspNetCore.Http;
using FWF.Configuration;
using System;
using System.Threading.Tasks;

namespace FWF.Basketball.CQRS.Service.Web
{
    internal class DashboardHandler : IHttpRequestHandler
    {

        private readonly IAppSettings _appSettings;

        public DashboardHandler(
            IAppSettings appSettings
            )
        {
            _appSettings = appSettings;
        }

        public async Task Handle(HttpContext context, Func<Task> next)
        {
            var path = context.Request.Path.ToUriComponent();
            var method = context.Request.Method;

            if (path == "/" && method.EqualsIgnoreCase("GET"))
            {
                await ShowDashboard(context);
            }
            else
            {
                await next();
            }
        }

        private async Task ShowDashboard(HttpContext context)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            var hostUri = new UriBuilder(
                "https",
                "localhost",
                _appSettings.HttpHostPort
                ).Uri;

            var body = string.Format(
            @"
<html>
  <head>
    <title>{0}</title>
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css""/>
  </head>
  <body>
    <div class=""jumbotron"">
      <div class=""container"">
        <h1 class=""display-3"">{0}</h1>
        <p class=""lead"">v{1} - {2}</p>
        <hr class=""my-4""/>
          <table class=""table table-sm table-bordered"" style=""font-size: 13px; line-height: 2;"">
            <tr>
              <th scope=""row"">Endpoint</td>
              <td>{3}</td>
            </tr>
          </table>
      </div>
    </div>
  </body>
</html>
",
                _appSettings.ClientName,
                version,
                _appSettings.EnvironmentName,
                hostUri
           );

            await context.Response.WriteAsync(body);
        }


    }
}

