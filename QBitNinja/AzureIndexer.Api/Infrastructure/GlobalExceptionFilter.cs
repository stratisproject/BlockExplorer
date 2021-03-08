using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using AzureIndexer.Api.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Stratis.Bitcoin.Utilities.JsonConverters;

namespace AzureIndexer.Api.Infrastructure
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is FormatException)
            {
                actionExecutedContext.Exception = new QBitNinjaException(400, actionExecutedContext.Exception.Message);
            }

            if (actionExecutedContext.Exception is JsonObjectException)
            {
                actionExecutedContext.Exception = new QBitNinjaException(400, actionExecutedContext.Exception.Message)
                {
                    Location = ((JsonObjectException)actionExecutedContext.Exception).Path
                };
            }

            if (actionExecutedContext.Exception is JsonReaderException)
            {
                actionExecutedContext.Exception = new QBitNinjaException(400, actionExecutedContext.Exception.Message)
                {
                    Location = ((JsonReaderException)actionExecutedContext.Exception).Path
                };
            }

            if (actionExecutedContext.Exception is QBitNinjaException)
            {
                var rapidEx = actionExecutedContext.Exception as QBitNinjaException;
                actionExecutedContext.Exception = new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = (HttpStatusCode)rapidEx.StatusCode,
                    ReasonPhrase = rapidEx.Message + (rapidEx.Location == null ? string.Empty : " at " + rapidEx.Location),
                    Content = new ObjectContent<QBitNinjaError>(rapidEx.ToError(), new JsonMediaTypeFormatter(), "application/json")
                });
            }

            if (actionExecutedContext.Exception is StorageException)
            {
                var storageEx = actionExecutedContext.Exception as StorageException;
                if (storageEx.RequestInformation != null && storageEx.RequestInformation.HttpStatusCode == 404)
                {
                    actionExecutedContext.Exception = new HttpResponseException(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
            }

            base.OnException(actionExecutedContext);
        }
    }
}
