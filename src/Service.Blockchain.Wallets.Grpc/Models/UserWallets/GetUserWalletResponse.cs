using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using System;
using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.UserWallets
{
    [DataContract]
    public class GetUserWalletResponse
    {
        /// <summary>
        /// WalletId
        /// </summary>
        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string AssetId { get; set; }

        [DataMember(Order = 3)]
        public string AssetNetworkId { get; set; }

        [DataMember(Order = 4)]
        public VaultAddress VaultAddress { get; set; }

        [DataMember(Order = 5)]
        public string ClientId { get; set; }

        [DataMember(Order = 6)]
        public string BrokerId { get; set; }

        [DataMember(Order = 7)]
        public long SignatureIssuedAt { get; set; }

        [DataMember(Order = 8)]
        public string SigningKeyId { get; set; }

        [DataMember(Order = 9)]
        public string Signature { get; set; }


        [DataMember(Order = 99)]
        public ErrorResponse Error { get; set; }

        public static GetUserWalletResponse CreateErrorResponse(string message, ErrorCode errorCode)
        {
            return new GetUserWalletResponse
            {
                Error = new ErrorResponse
                {
                    Error = message,
                    ErrorCode = errorCode
                }
            };
        }
    }
}