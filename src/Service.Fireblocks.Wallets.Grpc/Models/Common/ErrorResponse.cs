using System.Runtime.Serialization;

namespace Service.Fireblocks.Wallets.Grpc.Models
{
    [DataContract]
    public class ErrorResponse
    {
        [DataMember(Order = 1)]
        public string Error { get; set; }

        [DataMember(Order = 2)]
        public ErrorCode ErrorCode { get; set; }
    }
}