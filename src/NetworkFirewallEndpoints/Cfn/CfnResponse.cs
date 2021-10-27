namespace NetworkFirewallEndpoints.Cfn
{
    public class CfnResponse
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public string PhysicalResourceId { get; set; }
        public string StackId { get; set; }
        public string RequestId { get; set; }
        public string LogicalResourceId { get; set; }
        public bool NoEcho { get; set; }
    }

    public class CfnResponse<T>: CfnResponse
        where T:class
    {
        public T Data { get; set; }
    }
}