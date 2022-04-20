namespace Service.Blockchain.Wallets.Grpc.Models
{
    public enum ErrorCode
    {
        Unknown,
        AssetDoNotFound,
        AssetIsDisabled,
        BlockchainIsNotConfigured,
        BlockchainIsNotSupported,
        PaymentIsNotConfigured,
        AssetIsNotSupported,
        AddressPoolIsEmpty,
        NoKey
    }
}