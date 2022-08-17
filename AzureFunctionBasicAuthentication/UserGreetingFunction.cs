using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzureFunctionBasicAuthentication
{
    public class UserGreetingFunction
    {
        private readonly ILogger<UserGreetingFunction> _logger;

        public UserGreetingFunction(ILogger<UserGreetingFunction> log)
        {
            _logger = log;
        }

        [FunctionName("UserGreeting")]
        [OpenApiOperation(operationId: "Run", 
            tags: new[] { "UserGreeting Function" })]
        [OpenApiSecurity("basic_auth", 
            SecuritySchemeType.Http, 
            Scheme = OpenApiSecuritySchemeType.Basic, 
            Description = "Please enter user credentials [valid username:Salman Password 12345]")]
        [OpenApiParameter(name: "name", 
            In = ParameterLocation.Query, 
            Required = true, 
            Type = typeof(string), 
            Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, 
            contentType: "application/json", 
            bodyType: typeof(string), 
            Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest,
            Summary = "If the URL is missing or invalid URL",
            Description = "If the URL is missing or invalid URL")]
        public async Task<IActionResult> GreetUser(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var headers = req.Headers["Authorization"];
            if (!ValidateToken(headers)) return new UnauthorizedResult();

            _logger.LogInformation("User is Authenticated!");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "Please enter your name."
                : $"Hello, {name}.";

            _logger.LogInformation("Returning the result");
            return new JsonResult(responseMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private bool ValidateToken(string header)
        {
            //Checking the header
            if (!string.IsNullOrEmpty(header) && header.StartsWith("Basic"))
            {
                //Extracting credentials
                // Removing "Basic " Substring
                string encodedUsernamePassword = header.Substring("Basic ".Length).Trim();
                //Decoding Base64
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                //Splitting Username:Password
                int seperatorIndex = usernamePassword.IndexOf(':');
                // Extracting the individual username and password
                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);
                //Validating the credentials
                if (username is "Salman" && password is "12345") return true;
                else return false;
            }
            else
            {
                return false;
            }
        }
    }
}


