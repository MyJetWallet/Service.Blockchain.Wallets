namespace Service.Blockchain.Wallets.Domain.Models
{
    public enum AddressStatus
    {
        New = 0,
        InProcess = 1,
        Assigned = 2,
        WaitingForSignature = 3,

    }
}
