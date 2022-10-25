using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;

namespace SequenceSharp
{
    public interface IEthereumHostProvider
    {
        string Name { get; }

        bool Available { get; }
        string SelectedAccount { get; }
        long SelectedNetworkChainId { get; }
        bool Enabled { get; }

        event Func<string, Task> SelectedAccountChanged;
        event Func<long, Task> NetworkChanged;
        event Func<bool, Task> AvailabilityChanged;
        event Func<bool, Task> EnabledChanged;

        Task<bool> CheckProviderAvailabilityAsync();
        Task<Web3> GetWeb3Async();
        Task<string> EnableProviderAsync();
        Task<string> GetProviderSelectedAccountAsync();
        Task<string> SignMessageAsync(string message);
    }
    public class SequenceHostProvider : MonoBehaviour, IEthereumHostProvider
    {
        //private readonly ISequenceInterop _sequenceInterop;
        private Wallet _sequenceWallet;

        public static SequenceHostProvider Current { get; private set; }
        public string Name { get; } = "Sequence";
        public bool Available { get; private set; }
        public string SelectedAccount { get; private set; }
        public long SelectedNetworkChainId { get; private set; }
        public bool Enabled { get; private set; }

        private SequenceInterceptor _sequenceInterceptor;

        public event Func<string, Task> SelectedAccountChanged;
        public event Func<long, Task> NetworkChanged;
        public event Func<bool, Task> AvailabilityChanged;
        public event Func<bool, Task> EnabledChanged;
        public async Task<bool> CheckProviderAvailabilityAsync()
        {
            //var result = await _sequenceInterop.CheckSequenceAvailability().ConfigureAwait(false);
            await ChangeSequenceAvailableAsync(result).ConfigureAwait(false);
            return result;
        }


        public Task<Web3> GetWeb3Async()
        {
            var web3 = new Nethereum.Web3.Web3 { Client = { OverridingRequestInterceptor = _sequenceInterceptor } };
            return Task.FromResult((Web3.IWeb3)web3);
        }

        public async Task<string> EnableProviderAsync()
        {
            var selectedAccount = await _sequenceInterop.EnableEthereumAsync().ConfigureAwait(false);
            Enabled = !string.IsNullOrEmpty(selectedAccount);

            if (Enabled)
            {
                await ChangeSequenceEnabledAsync(true).ConfigureAwait(false);
                SelectedAccount = selectedAccount;
                if (SelectedAccountChanged != null)
                {
                    await SelectedAccountChanged.Invoke(selectedAccount).ConfigureAwait(false);
                }
                return selectedAccount;
            }

            return null;
        }
        
        public async Task<string> GetProviderSelectedAccountAsync()
        {
            var result = await _sequenceInterop.GetSelectedAddress().ConfigureAwait(false);
            await ChangeSelectedAccountAsync(result).ConfigureAwait(false);
            return result;
        }

        public async Task<string> SignMessageAsync(string message)
        {
            return await _sequenceInterop.SignAsync(message.ToHexUTF8()).ConfigureAwait(false);
        }

        public SequenceHostProvider(ISequenceInterop sequenceInterop)
        {
            _sequenceInterop = sequenceInterop;
            _sequenceInterceptor = new SequenceInterceptor(_sequenceInterop, this);
            Current = this;
        }

        public async Task ChangeSelectedAccountAsync(string selectedAccount)
        {
            if (SelectedAccount != selectedAccount)
            {
                SelectedAccount = selectedAccount;
                if (SelectedAccountChanged != null)
                {
                    await SelectedAccountChanged.Invoke(selectedAccount).ConfigureAwait(false);
                }
            }
        }

        public async Task ChangeSelectedNetworkAsync(long chainId)
        {
            if (SelectedNetworkChainId != chainId)
            {
                SelectedNetworkChainId = chainId;
                if (NetworkChanged != null)
                {
                    await NetworkChanged.Invoke(SelectedNetworkChainId).ConfigureAwait(false);
                }
            }
        }

        public async Task ChangeSequenceAvailableAsync(bool available)
        {
            if (Available != available)
            {
                Available = available;
                if (AvailabilityChanged != null)
                {
                    await AvailabilityChanged.Invoke(available).ConfigureAwait(false);
                }
            }
        }

        public async Task ChangeSequenceEnabledAsync(bool enabled)
        {
            if (Enabled != enabled)
            {
                Enabled = enabled;
                if (EnabledChanged != null)
                {
                    await EnabledChanged.Invoke(enabled).ConfigureAwait(false);
                }
            }
        }

    }
}
