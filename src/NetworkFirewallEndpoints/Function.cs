// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NetworkFirewallEndpoints
{
    using System;
    using System.Linq;

    using Model;
    using Cfn;

    public class Functions: CfnBase<ResourceProperties, ReturnedData>
    {
        protected override void Create()
        {
            try
            {
                var endpointId = Request.ResourceProperties.EndpointIds
                    .Select(x => x.Split(':'))
                    .Where(x => x.Length == 2 && x[0] == Request.ResourceProperties.AvailabilityZone)
                    .Select(x => x[1])
                    .FirstOrDefault();
                Log( $"Selected endpoint: {endpointId}");

                if (endpointId == null)
                {
                    Response.Status = "FAILED";
                    Response.Reason = $"EndpointId not found for Availability Zone '{Request.ResourceProperties.AvailabilityZone}'.";
                }
                else
                {
                    Response.Data = new ReturnedData
                    {
                        AzEndpointId = endpointId
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"Exception: {ex.Message}\n{ex.StackTrace}");
                Response.Status = "FAILED";
                Response.Reason = $"Exception: {ex.Message}";
            }
        }

        protected override void Update()
        {
            Create();
        }
    }
   
}
