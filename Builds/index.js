
window.sequenceLib = {
    //Test for js 
    test,

    //Connection:
    connect,
    disconnect,
    openWallet,
    openWalletWithSettings,
    closeWallet,
    isConnected,
    isOpened,
    getDefaultChainID,
    getAuthChainID,

    //State
    getChainID,
    getNetworks,
    getAccounts,
    getBalance,
    getWalletState,

    //Signing
    signMessage,
    signTypedData,
    signAuthMessage,
    signETHAuth,

    //Simulation
    estimateUnwrapGas,
    
    //Transactions
    sendETH,
    sendETHSidechain,
    sendDAI,
    send1155Tokens,
    sendRinkebyUSDC,

    //Various
    contractExample,
    fetchTokenBalances


}

const ERC_1155_ABI = [
  {
    inputs: [
      {
        internalType: 'address',
        name: '_from',
        type: 'address'
      },
      {
        internalType: 'address',
        name: '_to',
        type: 'address'
      },
      {
        internalType: 'uint256[]',
        name: '_ids',
        type: 'uint256[]'
      },
      {
        internalType: 'uint256[]',
        name: '_amounts',
        type: 'uint256[]'
      },
      {
        internalType: 'bytes',
        name: '_data',
        type: 'bytes'
      }
    ],
    name: 'safeBatchTransferFrom',
    outputs: [],
    stateMutability: 'nonpayable',
    type: 'function'
  }
]

const ERC_20_ABI = [
  {
    constant: false,
    inputs: [
      {
        internalType: 'address',
        name: 'recipient',
        type: 'address'
      },
      {
        internalType: 'uint256',
        name: 'amount',
        type: 'uint256'
      }
    ],
    name: 'transfer',
    outputs: [
      {
        internalType: 'bool',
        name: '',
        type: 'bool'
      }
    ],
    payable: false,
    stateMutability: 'nonpayable',
    type: 'function'
  }
]


const walletAppURL = 'https://sequence.app'
const network = 'polygon'
sequence.initWallet(network, { walletAppURL })
const wallet = sequence.getWallet()

function test()
{
    console.log("test index.js");
}

async function connect(authorize= false, withSettings= false)
 {
    const wallet = sequence.getWallet()

    const connectDetails = await wallet.connect({
      app: 'Demo Dapp',
      authorize,
      // keepWalletOpened: true,
      ...(withSettings && {
        settings: {
          theme: 'indigoDark',
          bannerUrl: `${window.location.origin}${skyweaverBannerUrl}`,
          includedPaymentProviders: ['moonpay'],
          defaultFundingCurrency: 'matic',
          defaultPurchaseAmount: 111
        }
      })
    })

    console.warn('connectDetails', { connectDetails })

    if (authorize) {
      const ethAuth = new ETHAuth()

      if (connectDetails.proof) {
        const decodedProof = await ethAuth.decodeProof(connectDetails.proof.proofString, true)

        console.warn({ decodedProof })

        const isValid = await wallet.utils.isValidTypedDataSignature(
          await wallet.getAddress(),
          connectDetails.proof.typedData,
          decodedProof.signature,
          await wallet.getAuthChainId()
        )
        console.log('isValid?', isValid)
        if (!isValid) throw new Error('sig invalid')
      }
    }
  }

  function disconnect()
  {
    const wallet = sequence.getWallet()
    wallet.disconnect()
  }

  function openWallet() {
    const wallet = sequence.getWallet()
    wallet.openWallet()
  }

  function openWalletWithSettings()
  {
    const wallet = sequence.getWallet()

    const settings = 
    {
      theme: 'goldDark',
      includedPaymentProviders: ['moonpay', 'ramp', 'wyre'],
      defaultFundingCurrency: 'eth',
      defaultPurchaseAmount: 400,
      lockFundingCurrencyToDefault: false
    }
    const intent =
    {
      type: 'openWithOptions',
      options: {
        settings
      }
    }
    const path = 'wallet/add-funds'
    wallet.openWallet(path, intent)
  }

  function closeWallet() 
  {
    const wallet = sequence.getWallet()
    wallet.closeWallet()
  }

  async function isConnected ()
  {
    const wallet = sequence.getWallet()
    console.log('isConnected?', wallet.isConnected())
  }

  async function isOpened()
  {
    const wallet = sequence.getWallet()
    console.log('isOpened?', wallet.isOpened())
  }

  async function getDefaultChainID()
  {
    console.log('TODO')
  }

  async function getAuthChainID()
{
    const wallet = sequence.getWallet()

    const authChainId = await wallet.getAuthChainId()
    console.log('auth chainId:', authChainId)
  }

  async function getChainID()
  {
    console.log('chainId:', await wallet.getChainId())

    const provider = wallet.getProvider()
    if(provider)
    {
        console.log('provider.getChainId()', await provider.getChainId())
    }
    const signer = wallet.getSigner()
    console.log('signer.getChainId()', await signer.getChainId())
  }

  async function getAccounts()
  {
    const wallet = sequence.getWallet()

    console.log('getAddress():', await wallet.getAddress())

    const provider = wallet.getProvider()
    if(provider)
    {
        console.log('accounts:', await provider.listAccounts())
    }
  }

  async function getBalance()
{
    const wallet = sequence.getWallet()

    const provider = wallet.getProvider()
    const account = await wallet.getAddress()
    if(provider)
    {
        const balanceChk1 = await provider.getBalance(account)
        console.log('balance check 1', balanceChk1.toString())
    }
    

    const signer = wallet.getSigner()
    const balanceChk2 = await signer.getBalance()
    console.log('balance check 2', balanceChk2.toString())
  }

  async function getWalletState()
  {
    console.log('wallet state:', await wallet.getSigner().getWalletState())
  }

  async function getNetworks()
  {
    const wallet = sequence.getWallet()

    console.log('networks:', await wallet.getNetworks())
  }

  async function signMessage()
  {
    const wallet = sequence.getWallet()

    console.log('signing message...')
    const signer = wallet.getSigner()

    const message = `1915 Robert Frost
The Road Not Taken

Two roads diverged in a yellow wood,
And sorry I could not travel both
And be one traveler, long I stood
And looked down one as far as I could
To where it bent in the undergrowth

Then took the other, as just as fair,
And having perhaps the better claim,
Because it was grassy and wanted wear
Though as for that the passing there
Had worn them really about the same,

And both that morning equally lay
In leaves no step had trodden black.
Oh, I kept the first for another day!
Yet knowing how way leads on to way,
I doubted if I should ever come back.

I shall be telling this with a sigh
Somewhere ages and ages hence:
Two roads diverged in a wood, and I—
I took the one less traveled by,
And that has made all the difference.

\u2601 \u2600 \u2602`

    // sign
    const sig = await signer.signMessage(message)
    console.log('signature:', sig)

    // validate
    const isValidHex = await wallet.utils.isValidMessageSignature(
      await wallet.getAddress(),
      ethers.utils.hexlify(ethers.utils.toUtf8Bytes(message)),
      sig,
      await signer.getChainId()
    )
    console.log('isValidHex?', isValidHex)

    const isValid = await wallet.utils.isValidMessageSignature(await wallet.getAddress(), message, sig, await signer.getChainId())
    console.log('isValid?', isValid)
    if (!isValid) throw new Error('sig invalid')

    // recover
    // const walletConfig = await wallet.utils.recoverWalletConfigFromMessage(
    //   await wallet.getAddress(),
    //   message,
    //   sig,
    //   await signer.getChainId(),
    //   sequenceContext
    // )
    // console.log('recovered walletConfig:', walletConfig)
    // const match = walletConfig.address.toLowerCase() === (await wallet.getAddress()).toLowerCase()
    // if (!match) throw new Error('recovery address does not match')
    // console.log('address match?', match)
  }

  async function signAuthMessage()
  {
    const wallet = sequence.getWallet()

    console.log('signing message on AuthChain...')
    const signer = await wallet.getAuthSigner()

    const message = 'Hi there! Please sign this message, 123456789, thanks.'

    // sign
    const sig = await signer.signMessage(message, await signer.getChainId()) //, false)
    console.log('signature:', sig)

    // here we have sig from above method, on defaultChain ..
    const notExpecting =
      '0x0002000134ab8771a3f2f7556dab62622ce62224d898175eddfdd50c14127c5a2bb0c8703b3b3aadc3fa6a63dd2dc66107520bc90031c015aaa4bf381f6d88d9797e9b9f1c02010144a0c1cbe7b29d97059dba8bbfcab2405dfb8420000145693d051135be70f588948aeaa043bd3ac92d98057e4a2c0fbd0f7289e028f828a31c62051f0d5fb96768c635a16eacc325d9e537ca5c8c5d2635b8de14ebce1c02'
    if (sig === notExpecting) {
      throw new Error('this sig is from the DefaultChain, not what we expected..')
    }

    // validate
    const isValidHex = await wallet.utils.isValidMessageSignature(
      await wallet.getAddress(),
      ethers.utils.hexlify(ethers.utils.toUtf8Bytes(message)),
      sig,
      await signer.getChainId()
    )
    console.log('isValidHex?', isValidHex)

    const isValid = await wallet.utils.isValidMessageSignature(await wallet.getAddress(), message, sig, await signer.getChainId())
    console.log('isValid?', isValid)
    if (!isValid) throw new Error('sig invalid')

    console.log('is wallet deployed on mainnet?', await wallet.isDeployed('mainnet'))
    console.log('is wallet deployed on matic?', await wallet.isDeployed('polygon'))

    // recover
    //
    // TODO: the recovery here will not work, because to use addressOf(), we must have
    // the init config for a wallet, wait for next index PR to come through then can fix this.
    //
    // TODO/NOTE: in order to recover this, the wallet needs to be updated on-chain,
    // or we need the init config.. check if its deployed and updated?
    // NOTE: this should work though, lets confirm it is deployed, and that the config is updated..
    // const walletConfig = await wallet.utils.recoverWalletConfigFromMessage(
    //   await wallet.getAddress(),
    //   message,
    //   sig,
    //   await signer.getChainId()
    // )

    // const match = walletConfig.address.toLowerCase() === (await wallet.getAddress()).toLowerCase()
    // // if (!match) throw new Error('recovery address does not match')
    // console.log('address match?', match)
  }

  async function signTypedData()
  {
    const wallet = sequence.getWallet()

    console.log('signing typedData...')
    //typedData : sequence.utils.TypedData 
    const typedData = {
      domain: {
        name: 'Ether Mail',
        version: '1',
        chainId: await wallet.getChainId(),
        verifyingContract: '0xCcCCccccCCCCcCCCCCCcCcCccCcCCCcCcccccccC'
      },
      types: {
        Person: [
          { name: 'name', type: 'string' },
          { name: 'wallet', type: 'address' }
        ]
      },
      message: {
        name: 'Bob',
        wallet: '0xbBbBBBBbbBBBbbbBbbBbbbbBBbBbbbbBbBbbBBbB'
      }
    }

    const signer = wallet.getSigner()

    const sig = await signer.signTypedData(typedData.domain, typedData.types, typedData.message)
    console.log('signature:', sig)

    // validate
    const isValid = await wallet.utils.isValidTypedDataSignature(
      await wallet.getAddress(),
      typedData,
      sig,
      await signer.getChainId()
    )
    console.log('isValid?', isValid)
    if (!isValid) throw new Error('sig invalid')

    // recover
    // const walletConfig = await wallet.utils.recoverWalletConfigFromTypedData(
    //   await wallet.getAddress(),
    //   typedData,
    //   sig,
    //   await signer.getChainId()
    // )
    // console.log('recovered walletConfig:', walletConfig)

    // const match = walletConfig.address.toLowerCase() === (await wallet.getAddress()).toLowerCase()
    // if (!match) throw new Error('recovery address does not match')
    // console.log('address match?', match)
  }

  async function signETHAuth()
  {
    const wallet = sequence.getWallet()

    const address = await wallet.getAddress()

    const authSigner = await wallet.getAuthSigner()
    console.log('AUTH CHAINID..', await authSigner.getChainId())
    const authChainId = await authSigner.getChainId()

    const proof = new Proof()
    proof.address = address
    proof.claims.app = 'wee'
    proof.claims.ogn = 'http://localhost:4000'
    proof.setIssuedAtNow()
    proof.setExpiryIn(1000000)

    const messageTypedData = proof.messageTypedData()

    const digest = sequence.utils.encodeTypedDataDigest(messageTypedData)
    console.log('we expect digest:', digest)

    const sig = await authSigner.signTypedData(messageTypedData.domain, messageTypedData.types, messageTypedData.message)
    console.log('signature:', sig)

    // validate
    const isValid = await wallet.utils.isValidTypedDataSignature(await wallet.getAddress(), messageTypedData, sig, authChainId)
    console.log('isValid?', isValid)
    if (!isValid) throw new Error('sig invalid')

    // recover
    // TODO/NOTE: in order to recover this, the wallet needs to be updated on-chain,
    // or we need the init config.. check if its deployed and updated
    // const walletConfig = await wallet.utils.recoverWalletConfigFromTypedData(
    //   await wallet.getAddress(),
    //   messageTypedData,
    //   sig,
    //   authChainId
    // )

    // console.log('recovered walletConfig:', walletConfig)
    // const match = walletConfig.address.toLowerCase() === (await wallet.getAddress()).toLowerCase()
    // // if (!match) throw new Error('recovery address does not match')
    // console.log('address match?', match)
  }

  async function estimateUnwrapGas(){
    const wallet = sequence.getWallet()

    const wmaticContractAddress = '0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270'
    const wmaticInterface = new ethers.utils.Interface(['function withdraw(uint256 amount)'])
    // tx :  sequence.transactions.Transaction 
    const tx= {
      to: wmaticContractAddress,
      data: wmaticInterface.encodeFunctionData('withdraw', ['1000000000000000000'])
    }

    const provider = wallet.getProvider()
    if(provider)
    {
        const estimate = await provider.estimateGas(tx)
        console.log('estimated gas needed for wmatic withdrawal:', estimate.toString())
    }

    
  }

  //signer : sequence.provider.Web3Signer
  async function sendETH(signer)
  {
    const wallet = sequence.getWallet()

    signer = signer || wallet.getSigner() // select DefaultChain signer by default

    console.log(`Transfer txn on ${signer.getChainId()} chainId`)

    // NOTE: on mainnet, the balance will be of ETH value
    // and on matic, the balance will be of MATIC value
    // const balance = await signer.getBalance()
    // if (balance.eq(ethers.constants.Zero)) {
    //   const address = await signer.getAddress()
    //   throw new Error(`wallet ${address} has 0 balance, so cannot transfer anything. Deposit and try again.`)
    // }

    const toAddress = ethers.Wallet.createRandom().address
    //tx1 : sequence.transactions.Transaction
    const tx1 = {
      delegateCall: false,
      revertOnError: false,
      gasLimit: '0x55555',
      to: toAddress,
      value: ethers.utils.parseEther('1.234'),
      data: '0x'
    }
    //tx2 : sequence.transactions.Transaction
    const tx2 = {
      delegateCall: false,
      revertOnError: false,
      gasLimit: '0x55555',
      to: toAddress,
      value: ethers.utils.parseEther('0.4242'),
      data: '0x'
    }

    const provider = signer.provider

    console.log(`balance of ${toAddress}, before:`, await provider.getBalance(toAddress))

    const txnResp = await signer.sendTransactionBatch([tx1, tx2])
    // await txnResp.wait() // optional as sendTransactionBatch already waits for the receipt
    console.log('txnResponse:', txnResp)

    console.log(`balance of ${toAddress}, after:`, await provider.getBalance(toAddress))
  }
  // signer : sequence.provider.Web3Signer
  async function sendRinkebyUSDC(signer){
    const wallet = sequence.getWallet()

    signer = signer || wallet.getSigner() // select DefaultChain signer by default

    const toAddress = ethers.Wallet.createRandom().address

    const amount = ethers.utils.parseUnits('1', 1)

    const daiContractAddress = '0x4DBCdF9B62e891a7cec5A2568C3F4FAF9E8Abe2b' // (USDC address on Rinkeby)
    //tx : sequence.transactions.Transaction
    const tx = {
      delegateCall: false,
      revertOnError: false,
      gasLimit: '0x55555',
      to: daiContractAddress,
      value: 0,
      data: new ethers.utils.Interface(ERC_20_ABI).encodeFunctionData('transfer', [toAddress, amount.toHexString()])
    }

    const txnResp = await signer.sendTransactionBatch([tx], 4)
    // await txnResp.wait() // optional as sendTransactionBatch already waits for the receipt
    console.log('txnResponse:', txnResp)
  }
  //signer : sequence.provider.Web3Signer
  async function sendDAI(signer)
  {
    const wallet = sequence.getWallet()

    signer = signer || wallet.getSigner() // select DefaultChain signer by default

    const toAddress = ethers.Wallet.createRandom().address

    const amount = ethers.utils.parseUnits('5', 18)

    const daiContractAddress = '0x8f3Cf7ad23Cd3CaDbD9735AFf958023239c6A063' // (DAI address on Polygon)
    //tx : sequence.transactions.Transaction
    const tx = {
      delegateCall: false,
      revertOnError: false,
      gasLimit: '0x55555',
      to: daiContractAddress,
      value: 0,
      data: new ethers.utils.Interface(ERC_20_ABI).encodeFunctionData('transfer', [toAddress, amount.toHexString()])
    }

    const txnResp = await signer.sendTransactionBatch([tx])
    // await txnResp.wait() // optional as sendTransactionBatch already waits for the receipt
    console.log('txnResponse:', txnResp)
  }

  async function sendETHSidechain()
  {
    const wallet = sequence.getWallet()

    // const signer = wallet.getSigner(137)
    // Select network that isn't the DefaultChain..
    const networks = await wallet.getNetworks()
    const n = networks.find(n => n.isAuthChain)
    sendETH(wallet.getSigner(n))
  }

  async function send1155Tokens()
  {
    console.log('TODO')
  }
  //signer : sequence.provider.Web3Signer
  async function contractExample(signer)
  {
    const wallet = sequence.getWallet()

    signer = signer || wallet.getSigner()

    const abi = [
      'function balanceOf(address owner) view returns (uint256)',
      'function decimals() view returns (uint8)',
      'function symbol() view returns (string)',

      'function transfer(address to, uint amount) returns (bool)',

      'event Transfer(address indexed from, address indexed to, uint amount)'
    ]

    // USD Coin (PoS) on Polygon
    const address = '0x2791bca1f2de4661ed88a30c99a7a9449aa84174'

    const usdc = new ethers.Contract(address, abi, signer)

    console.log('Token symbol:', await usdc.symbol())

    const balance = await usdc.balanceOf(await signer.getAddress())
    console.log('Token Balance', balance.toString())
  }

  async function fetchTokenBalances()
  {
    const wallet = sequence.getWallet()

    const signer = wallet.getSigner()
    const accountAddress = await signer.getAddress()

    const indexer = new sequence.SequenceIndexerClient(sequence.SequenceIndexerServices.POLYGON)

    const tokenBalances = await indexer.getTokenBalances({
      accountAddress: accountAddress,
      includeMetadata: true
    })
    console.log('tokens in your account:', tokenBalances)

    // NOTE: you can put any NFT/collectible address in the `contractAddress` field and it will return all of the balances + metadata.
    // We use the Skyweaver production contract address here for demo purposes, but try another one :)
    const skyweaverCollectibles = await indexer.getTokenBalances({
      accountAddress: accountAddress,
      includeMetadata: true,
      contractAddress: '0x631998e91476DA5B870D741192fc5Cbc55F5a52E'
    })
    console.log('skyweaver collectibles in your account:', skyweaverCollectibles)
  }
