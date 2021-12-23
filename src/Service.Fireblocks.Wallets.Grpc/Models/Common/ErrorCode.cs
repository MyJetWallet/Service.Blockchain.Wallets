namespace Service.Fireblocks.Wallets.Grpc.Models
{
    public enum ErrorCode
    {
        Unknown,
        AssetDoNotFound,
        AssetIsDisabled,
        BlockchainIsNotConfigured,
        BlockchainIsNotSupported,
        PaymentIsNotConfigured
    }
}