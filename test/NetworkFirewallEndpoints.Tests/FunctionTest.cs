using System;
using System.Collections.Generic;
using Xunit;
using Amazon.Lambda.TestUtilities;
using NetworkFirewallEndpoints.Cfn;
using NetworkFirewallEndpoints.Model;

namespace NetworkFirewallEndpoints.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
        }

        [Fact]
        public void TetGetMethod()
        {
   
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Functions();
            var context = new TestLambdaContext();
            var request = new CfnRequest<ResourceProperties>
            {
                RequestType = "Create",
                LogicalResourceId = "NwFwEp",
                RequestId = Guid.NewGuid().ToString(),
                ResourceType = "RadarMed::Helpers:NetworkFirewallEndpoint",
                StackId = "unit-test",
                ResponseURL = "https://webhook.site/5b5dff12-5c44-4e1a-b0e7-8fef51e7cca0",
                ResourceProperties = new ResourceProperties
                {
                    EndpointIds = new string[]
                    { 
                        "us-west-1:vpce-1234567890", 
                        "us-west-2:vpce-0987654321",
                        "us-east-1:vpce-0000000000", 
                        "us-east-2:vpce-1111111111" 
                    },
                    AvailabilityZone = "us-east-1"
                }
            };
            function.Get(request, context);
        }

    }
}
