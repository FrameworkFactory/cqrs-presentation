using FWF.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FWF.Basketball.RESTFul.Service.v1 
{
    public class HomeController : ControllerBase 
    {

        private readonly IAppSettings _appSettings;

        public HomeController(
            IAppSettings appSettings
            )
        {
            _appSettings = appSettings;
        }

        public ActionResult Index()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            var hostUri = new UriBuilder(
                "https",
                "localhost",
                _appSettings.HttpHostPort
                ).Uri;

            var content = @"
<html>
  <head>
    <title>CQRS Service</title>
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css"" />
  </head>
  <body>
    <div class=""jumbotron"">
      <div class=""container"">
        <h1 class=""display-3"">RESTFul Service</h1>
        <p class=""lead"">v" + version + @" - DEBUG</p>
        <hr class=""my-4"" />
          <table class=""table table-sm table-bordered"" style=""font -size: 13px; line-height: 2;"">
            <tr>
              <th scope=""row"">Endpoint</td>
              <td>" + hostUri + @"</td>
            </tr>
          </table>
      </div>
    </div>
  </body>
</html>
";

            return Content(content, "text/html");
        }

    }
}