using System.Numerics;
using System.Threading.Tasks;
using System;
namespace SequenceSharp
{
    public class ERC721
    {
        private static string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"approved\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"bool\", \"name\": \"approved\", \"type\": \"bool\" } ], \"name\": \"ApprovalForAll\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"getApproved\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" } ], \"name\": \"isApprovedForAll\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"ownerOf\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"safeTransferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" }, { \"internalType\": \"bytes\", \"name\": \"_data\", \"type\": \"bytes\" } ], \"name\": \"safeTransferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"internalType\": \"bool\", \"name\": \"approved\", \"type\": \"bool\" } ], \"name\": \"setApprovalForAll\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes4\", \"name\": \"interfaceId\", \"type\": \"bytes4\" } ], \"name\": \"supportsInterface\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"tokenURI\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";

        public readonly static Wallet _wallet;
        public static async Task<string> Name(string address, string abi)
        {
            //throw new NotImplementedException();
            string name = await _wallet.ExecuteSequenceJS(@"
                const provider = wallet.getProvider()!
                const erc721 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var name = await erc721.name();
                return name;

            ");

            return name;

        }

        public static async Task<string> Symbol(string address, string abi)
        {
            //throw new NotImplementedException();
            string symbol = await _wallet.ExecuteSequenceJS(@"
                const provider = wallet.getProvider()!
                const erc721 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var symbol = await erc721.symbol();
                return symbol;

            ");
            return symbol;
        }

        public static async Task<string> TokenURI(BigInteger tokenId, string address, string abi)
        {
            //throw new NotImplementedException();
            string tokenURI = await _wallet.ExecuteSequenceJS(@"
                const provider = wallet.getProvider()!
                const erc721 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var tokenURI = await erc721.tokenURI("+tokenId+@");
                return tokenURI;

            ");

            return tokenURI;
        }
        public static async Task<BigInteger> BalanceOf(string owner, string address, string abi)
        {
            //throw new NotImplementedException();
            var balanceOf = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const provider = wallet.getProvider()!
                const signer = wallet.getSigner();
                const erc721 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var balanceOf = await erc721.balanceOf(signer.getAddress()||" + owner + @");
            "));
            return balanceOf;
        }

        public static async Task OwnerOf(BigInteger tokenId, string address, string abi)
        {
            //throw new NotImplementedException();
            string ownerOf = await _wallet.ExecuteSequenceJS(@"
                const provider = wallet.getProvider()!
                const erc721 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var ownerOf = await erc721.ownerOf(" + tokenId + @");
                return ownerOf;

            ");
        }

        public static async Task SafeTransferFrom(string from, string to, BigInteger tokenId)
        {
            throw new NotImplementedException();
        }

        public static async Task TransferFrom(string from, string to, BigInteger tokenId)
        {
            throw new NotImplementedException();
        }
        public static async Task Approve(string to, BigInteger tokenId)
        {
            throw new NotImplementedException();
        }
        public static async Task GetApproved(BigInteger tokenId)
        {
            throw new NotImplementedException();
        }
        public static async Task SetApprovalForAll(string operatorAddress, bool _approved)
        {
            throw new NotImplementedException();
        }

        public static async Task IsApprovedForAll(string owner, string operatorAddress)
        {
            throw new NotImplementedException();
        }

        public static async Task SafeTransferFrom(string from, string to, BigInteger tokenId, string data)
        {
            throw new NotImplementedException();
        }
    }
}