using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.Blockchain.Wallets.Client;
using Service.Blockchain.Wallets.Grpc.Models;
using static Service.Blockchain.Wallets.Grpc.Models.UserWallets.GetUserByAddressRequest;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new BlockchainWalletsClientFactory("http://localhost:5001");
            var client = factory.GetWalletService();
            var assetMappings = factory.GetAssetMappingService();

            //await assetMappings.UpsertAssetMappingAsync(new Service.Blockchain.Wallets.Grpc.Models.AssetMappings.UpsertAssetMappingRequest()
            //{
            //    AssetMapping = new MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.AssetMapping
            //    {
            //        ActiveDepositAddessVaultAccountId = "16",
            //        AssetId = "TestEth",
            //        DepositType = MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Intermediate,
            //        FireblocksAssetId = "ETH_TEST",
            //        NetworkId = "TestEth", 
            //        WithdrawalVaultAccountId = "11"
            //    }
            //});

            //await assetMappings.UpsertAssetMappingAsync(new Service.Blockchain.Wallets.Grpc.Models.AssetMappings.UpsertAssetMappingRequest()
            //{
            //    AssetMapping = new MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.AssetMapping
            //    {
            //        ActiveDepositAddessVaultAccountId = "16",
            //        AssetId = "TestBtc",
            //        DepositType = MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Broker,
            //        FireblocksAssetId = "BTC_TEST",
            //        NetworkId = "TestBtc",
            //        WithdrawalVaultAccountId = "11"
            //    }
            //});

            //await assetMappings.UpsertAssetMappingAsync(new Service.Blockchain.Wallets.Grpc.Models.AssetMappings.UpsertAssetMappingRequest()
            //{
            //    AssetMapping = new MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.AssetMapping
            //    {
            //        ActiveDepositAddessVaultAccountId = "16",
            //        AssetId = "TestXlm",
            //        DepositType = MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Broker,
            //        FireblocksAssetId = "XLM_TEST",
            //        NetworkId = "TestXlm",
            //        WithdrawalVaultAccountId = "11"
            //    }
            //});

            var respx = await client.GetUserByAddressAsync(new()
            {
                Addresses = new AddressAndTag[]
                {
                    new AddressAndTag{ Address = "0x44bc49a035f5c4806d7aa803cc6754e06cfee92b", Tag = null,},
                    //new AddressAndTag{ Address = "0x5a036b34148c438766195d3c61b2e0113804e714", Tag = "",},
                }
            });

            var resp = await client.GetUserWalletAsync(new ()
            {
                AssetSymbol = "ETH",
                AssetNetwork = "fireblocks-eth-test",
                WalletId = "Test",
                BrokerId = "jetwallet",
                ClientId = "Test"
            });

            var resp2 = await client.GetUserWalletAsync(new()
            {
                AssetSymbol = "ETH",
                AssetNetwork = "fireblocks-eth-test",
                WalletId = "Test",
                BrokerId = "jetwallet",
                ClientId = "Test"
            });

            //var resp2 = await client.GetUserWalletAsync(new()
            //{
            //    AssetSymbol = "TestEth",
            //    AssetNetwork = "TestEth",
            //    WalletId = "Test"
            //});

            //var resp3 = await client.GetUserWalletAsync(new()
            //{
            //    AssetSymbol = "TestBtc",
            //    AssetNetwork = "TestBtc",
            //    WalletId = "Test"
            //});

            //var resp4 = await client.GetUserWalletAsync(new()
            //{
            //    AssetSymbol = "TestBtc",
            //    AssetNetwork = "TestBtc",
            //    WalletId = "Test"
            //});

            //var resp5 = await client.GetUserWalletAsync(new()
            //{
            //    AssetSymbol = "TestXlm",
            //    AssetNetwork = "TestXlm",
            //    WalletId = "Test"
            //});

            //var resp6 = await client.GetUserWalletAsync(new()
            //{
            //    AssetSymbol = "TestXlm",
            //    AssetNetwork = "TestXlm",
            //    WalletId = "Test"
            //});

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
