using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.Fireblocks.Wallets.Client;
using Service.Fireblocks.Wallets.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new FireblocksWalletsClientFactory("http://localhost:5001");
            var client = factory.GetWalletService();
            var assetMappings = factory.GetAssetMappingService();

            await assetMappings.UpsertAssetMappingAsync(new Service.Fireblocks.Wallets.Grpc.Models.AssetMappings.UpsertAssetMappingRequest()
            {
                AssetMapping = new MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.AssetMapping
                {
                    ActiveDepositAddessVaultAccountId = "16",
                    AssetId = "Test",
                    DepositType = MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Intermediate,
                    FireblocksAssetId = "ETH_TEST",
                    NetworkId = "Test", 
                    WithdrawalVaultAccountId = "11"
                }
            });

            var resp = await  client.GetUserWalletAsync(new ()
            {
                AssetId = "Test",
                AssetNetworkId = "Test",
                UserId = "Test"
            });

            var resp2 = await client.GetUserWalletAsync(new()
            {
                AssetId = "Test",
                AssetNetworkId = "Test",
                UserId = "Test"
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
