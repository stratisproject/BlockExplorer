using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Stratis.Bitcoin.Utilities.JsonErrors;

namespace AzureIndexer.Api.Infrastructure
{
    public class WebApiExceptionActionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var result = this.BuildErrorResult(context.Exception, context.HttpContext.Request);

            if (result != null)
            {
                context.Result = result;
            }

            return Task.CompletedTask;
        }

        private IActionResult BuildErrorResult(Exception exception, HttpRequest request)
        {
            switch (exception)
            {
                case HttpResponseException httpResponseException:
                    return this.HandleHttpResponseException(httpResponseException, request);
                default:
                    return null;
            }
        }

        private IActionResult HandleHttpResponseException(
            HttpResponseException exception,
            HttpRequest request,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = new ErrorResponse
            {
                Errors = new List<ErrorModel>
                {
                    new ErrorModel {Message = exception.Message, Status = (int) exception.StatusCode}
                }
            };

            return new ObjectResult(response)
            {
                StatusCode = (int) statusCode
            };
        }
    }
}
