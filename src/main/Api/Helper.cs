using Nancy;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace neurUL.Common.Api
{
    public static class Helper
    {
        public static async Task<Response> ProcessRequest(Func<string> requestValidator, Func<Task> requestTrigger, Func<Exception, HttpStatusCode> httpStatusCodeExceptionOverride)
        {
            var result = new Response { StatusCode = HttpStatusCode.OK };

            var exception = await Helper.ProcessRequestCore(requestValidator, requestTrigger);

            if (exception != null)
            {
                HttpStatusCode hsc = HttpStatusCode.BadRequest;

                if (httpStatusCodeExceptionOverride != null)
                    hsc = httpStatusCodeExceptionOverride.Invoke(exception);

                result = new TextResponse(hsc, exception.ToString());
            }

            return result;
        }

        public static async Task<Exception> ProcessRequestCore(Func<string> requestValidator, Func<Task> requestTrigger)
        {
            Exception result = null;

            var requestResult = requestValidator != null ? requestValidator.Invoke() : null;
            if (string.IsNullOrEmpty(requestResult))
            {
                try
                {
                    await requestTrigger.Invoke();
                }
                catch (Exception ex)
                {
                    result = ex;
                }
            }
            else
                result = new ApplicationException(requestResult);

            return result;
        }
    }
}
