var sequence = {
    TestFromJS: function()
    {
        var _test = window.sequenceLib.test();
        console.log("test in sequence.jslib:", _test);
    },

    //Connection:
    ConnectFromJS: async function()
    {
        var _connect = window.sequenceLib.connect();
        console.log("connect in sequence.jslib:", _connect);
    },

    DisconnectFromJS: function()
    {
        window.sequenceLib.disconnect();
    },

    OpenWalletFromJS: function()
    {
        window.sequenceLib.openWallet();
    },

    OpenWalletWithSettingsFromJS: function()
    {
        window.sequenceLib.openWalletWithSettings();
    },

    CloseWalletFromJS: function()
    {
        window.sequenceLib.closeWallet();
    },

    IsConnectedFromJS: async function()
    {
        window.sequenceLib.isConnected();
    },

    IsOpenedFromJS: async function()
    {
        window.sequenceLib.isOpened();
    },

    GetDefaultChainIDFromJS: async function()
    {
        window.sequenceLib.getDefaultChainID();
    },

    GetAuthChainIDFromJS: async function()
    {
        window.sequenceLib.getAuthChainID();
    },
    
    //State
    GetChainIDFromJS: async function()
    {
        window.sequenceLib.getChainID();
    },

    GetNetworksFromJS: async function()
    {
        window.sequenceLib.getNetworks();
    },

    GetAccountsFromJS: async function()
    {
        window.sequenceLib.getAccounts();
    },

    GetBalanceFromJS: async function()
    {
        window.sequenceLib.getBalance();
    },
    GetWalletStateFromJS: async function()
    {
        window.sequenceLib.getWalletState();
    },

    //Signing
    SignMessageFromJS: async function()
    {
        window.sequenceLib.signMessage();
    },
    SignTypedDataFromJS: async function()
    {
        window.sequenceLib.signTypedData();
    },
    SignAuthMessageFromJS: async function()
    {
        window.sequenceLib.signAuthMessage();
    },
    SignETHAuthFromJS: async function()
    {
        window.sequenceLib.signETHAuth();
    },

    //Simulation
    EstimateUnwrapGasFromJS: async function()
    {
        window.sequenceLib.estimateUnwrapGas();
    },
    
    //Transactions
    SendETHFromJS: async function()
    {
        window.sequenceLib.sendETH();
    },
    SendETHSidechainFromJS: async function()
    {
        window.sequenceLib.sendETHSidechain();
    },
    SendDAIFromJS: async function()
    {
        window.sequenceLib.sendDAI();
    },
    Send1155TokensFromJS: async function()
    {
        window.sequenceLib.send1155Tokens();
    },
    SendRinkebyUSDCFromJS: async function()
    {
        window.sequenceLib.sendRinkebyUSDC();
    },

    //Various
    ContractExampleFromJS: async function()
    {
        window.sequenceLib.contractExample();
    },
    FetchTokenBalancesFromJS: async function()
    {
        window.sequenceLib.fetchTokenBalances();
    }
};

mergeInto(LibraryManager.library, sequence);