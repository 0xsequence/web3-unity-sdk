using System.Collections;
using UnityEngine;
using Nethereum.Unity.Rpc;
using Debug = UnityEngine.Debug;
#if UNITY_WEBGL
using Nethereum.Unity.Metamask;
#endif
using Nethereum.Unity.Contracts;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using UnityEngine.Events;

namespace SequenceSharp
{
    public class Metamask : MonoBehaviour
    {

        public BigInteger ChainId = 0;

        private string _selectedAccountAddress;
        private bool _isMetamaskInitialised = false;
        public decimal Amount = 1.1m;

        public decimal BalanceAddressTo = 0m;

        public UnityEvent MetamaskConnectedEvent;

        public bool IsWebGL()
        {
#if UNITY_WEBGL
            return true;
#else
      return false;
#endif
        }


        public bool IsMetamaskInitialised()
        {
            return _isMetamaskInitialised;
        }


        public void DisplayError(string errorMessage)
        {
            Debug.LogError(errorMessage);
        }

        public IEnumerator MetamaskConnect()
        {
#if UNITY_WEBGL
            if (IsWebGL())
            {
                if (MetamaskInterop.IsMetamaskAvailable())
                {
                    
                    MetamaskInterop.EnableEthereum(gameObject.name, nameof(EthereumEnabled), nameof(DisplayError));
                }
                else
                {
                    DisplayError("Metamask is not available, please install it");
                }
            }
#endif
            yield return null;
        }

        public void EthereumEnabled(string addressSelected)
        {
#if UNITY_WEBGL
            if (IsWebGL())
            {
                if (!_isMetamaskInitialised)
                {
                    MetamaskInterop.EthereumInit(gameObject.name, nameof(NewAccountSelected), nameof(ChainChanged));
                    MetamaskInterop.GetChainId(gameObject.name, nameof(ChainChanged), nameof(DisplayError));
                    _isMetamaskInitialised = true;
                    Debug.Log("is Metamask Initialized: " + _isMetamaskInitialised);
                    if(_isMetamaskInitialised)
                    {
                        MetamaskConnectedEvent.Invoke();
                    }
                }
                NewAccountSelected(addressSelected);
            }
#endif
        }

        public void ChainChanged(string chainId)
        {
            print(chainId);
            ChainId = new HexBigInteger(chainId).Value;

        }

        public void NewAccountSelected(string accountAddress)
        {
            _selectedAccountAddress = accountAddress;
        }

        public void SignMessageRequest(string message)
        {
            StartCoroutine(SignMessage(message));
        }

        public IEnumerator SignMessage(string message)
        {
#if UNITY_WEBGL
            var personalSignRequest = new EthPersonalSignUnityRequest(GetUnityRpcRequestClientFactory());
            yield return personalSignRequest.SendRequest(new HexUTF8String(message));
            if (personalSignRequest.Exception != null)
            {
                Debug.Log("Error signing message");
                DisplayError(personalSignRequest.Exception.Message);
                yield break;
            }
            Debug.Log("[Sequence] Metamask:" + personalSignRequest.Result);
#else
       
        PrivateKey = InputPrivateKey.text;
        var signer = new EthereumMessageSigner();
        LblSignedMessage.text = signer.EncodeUTF8AndSign(InputSignMessage.text, new EthECKey(PrivateKey));
        yield break;

#endif
        }


        public IUnityRpcRequestClientFactory GetUnityRpcRequestClientFactory()
        {
#if UNITY_WEBGL
            if (IsWebGL())
            {
                if (MetamaskInterop.IsMetamaskAvailable())
                {
                    return new MetamaskRequestRpcClientFactory(_selectedAccountAddress, null, 60000);
                }
                else
                {
                    // DisplayError("Metamask is not available, please install it");
                    return null;
                }
            }
            return null;
#endif
        }


    }
}

