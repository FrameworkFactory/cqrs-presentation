using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace FWF.Basketball.CQRS.Service.Web
{
    public interface IHttpRequestHandler
    {
        Task Handle(HttpContext context, Func<Task> next);
    }
}

