using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.Responses;
using neurUL.Common.Domain.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace neurUL.Common.Api
{
    public static class NancyExtensions
    {
        public static async Task<Response> ProcessCommand(
            this Request request, 
            Func<dynamic, Dictionary<string, object>, int, Task> commandProcessor, 
            Func<Exception, HttpStatusCode, HttpStatusCode> httpStatusCodeExceptionOverride, 
            string[] alternativeRequiredFields, 
            params string[] requiredFields
            )
        {
            return await ProcessCommand(request, true, commandProcessor, httpStatusCodeExceptionOverride, alternativeRequiredFields, requiredFields);
        }

        public static async Task<Response> ProcessCommand(
            this Request request, 
            bool versionRequired, 
            Func<dynamic, Dictionary<string, object>, int, Task> commandProcessor, 
            Func<Exception, HttpStatusCode, HttpStatusCode> httpStatusCodeExceptionOverride, 
            string[] alternativeRequiredFields, 
            params string[] requiredFields
            )
        {
            AssertionConcern.AssertArgumentNotNull(request, nameof(request));
            AssertionConcern.AssertArgumentNotNull(commandProcessor, nameof(commandProcessor));

            dynamic bodyAsObject = null;
            Dictionary<string, object> bodyAsDictionary = null;

            var jsonString = RequestStream.FromStream(request.Body).AsString();

            string[] missingRequiredFields = null;
            bool alternativeRequiredFieldNotFound = false;

            if (!string.IsNullOrEmpty(jsonString))
            {
                bodyAsObject = JsonConvert.DeserializeObject(jsonString);
                bodyAsDictionary = JObject.Parse(jsonString).ToObject<Dictionary<string, object>>();
                missingRequiredFields = requiredFields.Where(s => !bodyAsDictionary.ContainsKey(s)).ToArray();
                alternativeRequiredFieldNotFound = !alternativeRequiredFields.Any(arf => bodyAsDictionary.ContainsKey(arf));
            }
            else
                missingRequiredFields = requiredFields;

            var result = new Response { StatusCode = HttpStatusCode.OK };

            // validate required body fields

            int expectedVersion = -1;
            if (versionRequired)
            {
                var rh = request.Headers["ETag"];
                if (!(rh.Any() && int.TryParse(rh.First(), out expectedVersion)))
                    missingRequiredFields = missingRequiredFields.Concat(new string[] { "ExpectedVersion" }).ToArray();
            }

            var errorInfo = string.Empty;
            if (missingRequiredFields.Count() > 0)
                errorInfo = $"Required field(s) '{ string.Join("', '", missingRequiredFields) }' not found. ";
            if (alternativeRequiredFieldNotFound)
                errorInfo += $"No alternative required field found: '{string.Join("', '", alternativeRequiredFields)}'";
                
            if (!string.IsNullOrEmpty(errorInfo))
                result = new TextResponse(HttpStatusCode.BadRequest, errorInfo);

            if (result.StatusCode != HttpStatusCode.BadRequest)
            {
                try
                {
                    await commandProcessor.Invoke(bodyAsObject, bodyAsDictionary, expectedVersion);                    
                }
                catch (Exception ex)
                {
                    HttpStatusCode hsc = HttpStatusCode.BadRequest;

                    if (httpStatusCodeExceptionOverride != null)
                        hsc = httpStatusCodeExceptionOverride.Invoke(ex, hsc);

                    result = new TextResponse(hsc, ex.ToString());
                }
            }

            return result;
        }
    }
}