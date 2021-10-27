namespace NetworkFirewallEndpoints.Cfn
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using Amazon.Lambda.Core;

    public abstract class CfnBase<TRequest, TResponse>
        where TRequest: class
        where TResponse: class
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();

        protected CfnRequest<TRequest> Request { get; private set; }
        protected CfnResponse<TResponse> Response { get; private set; }
        protected ILambdaContext Context { get; private set; }

        public void Get(CfnRequest<TRequest> request, ILambdaContext context)
        {
            Context = context;
            Request = request;
            Response = new CfnResponse<TResponse>
            {
                StackId = request.StackId,
                RequestId = request.RequestId,
                LogicalResourceId = request.LogicalResourceId,
                PhysicalResourceId = request.PhysicalResourceId ?? Guid.NewGuid().ToString(),
                Reason = string.Empty,
                NoEcho = false,
                Status = "SUCCESS"
            };
            LogJson("REQUEST:\n{0}", request);

            try
            {
                switch (request.RequestType.ToLowerInvariant())
                {
                    case "create":
                        Create();
                        break;
                    case "update":
                        Update();
                        break;
                    case "delete":
                        Delete();
                        break;
                    default:
                        Response.Status = "FAILED";
                        Response.Reason = $"RequestType '{Request.RequestType}' not implemented.";
                        break;
                }
                SendResponse();
            }
            catch (Exception ex)
            {
                Log($"Exception: {ex.Message}\n{ex.StackTrace}");
                Response.Status = "FAILED";
                Response.Reason = $"Exception: {ex.Message}";
                SendResponse();
            }            
        }

        protected abstract void Create();

        protected virtual void Update() {}

        protected virtual void Delete() {}

        protected string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, _jsonSerializerOptions);
        }

        protected void Log(string message)
        {
            Context.Logger.LogLine(message);
        }

        protected void LogJson(string message, object obj)
        {
            Log(string.Format(message, Serialize(obj)));
        }

        protected bool SendResponse()
        {
            try
            {
                string json = JsonSerializer.Serialize((object)Response, new JsonSerializerOptions());
                Log($"RESPONSE: {json}");

                var httpClient = new HttpClient();
                var httpRequest = new HttpRequestMessage(HttpMethod.Put, Request.ResponseURL)
                {
                    Content = new StringContent(json)
                };
                var httpResponse = httpClient.Send(httpRequest);
                string content = "";
                try
                {
                    var ctx = SynchronizationContext.Current;
                    SynchronizationContext.SetSynchronizationContext(null);
                    content = httpResponse.Content.ReadAsStringAsync().Result;
                    SynchronizationContext.SetSynchronizationContext(ctx);
                }
                catch {}
                Log($"PUT response: {httpResponse.StatusCode} - {httpResponse.ReasonPhrase}\n{content}");
                return true;
            }
            catch (Exception ex)
            {
                Log($"Exception: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

    }
}