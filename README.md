# Web3 Unity SDK
Sequence Unity SDK provides full wallet integration for your Unity Games.
----------------
- Checkout the WebGL demo: https://0xsequence.github.io/demo-unity-webgl-game
- The demo is written similar to https://github.com/0xsequence/demo-dapp but with Unity.

## How To Use
### - Download the Sequence Unity SDK UnityPackage in Releases:
### - Unity Setup
1. Drag the Sequence Unity SDK UnityPackage into your Unity game project.
2. Sequence Unity SDK depends on Vuplex webview package, please import Vuplex for each platform you are building your game for. https://vuplex.com
3. Try out our Sample Scenes:
   -  Wallet Connection Demo
      [Insert demo pic]
   -  Indexer Demo
      [Insert demo pic]
4. Try out our Prefabs:
5. Main features for now:
   - Connect to Sequence Wallet:
    [Insert demo pic]
   - Get the wallet address:
    [Insert demo pic]
   - Open the wallet from your game:
    [Insert demo pic]
   - Get the blockchain network ID
    [Insert demo pic]

6. Build your game:
   - WebGL Build
      - You don't need to choose extra WebGLTemplate for sequence sdk, stay on your own template and sequenceSDK does the rest.
      - If you want to avoid buying Vuplex WebGL, since it's not technically used, then every time you do a WebGL build, _first move the Vuplex folder out of your project_, then move it back in after your build is done.
   - Other Builds (Standalone(Win/Mac)/IOS/Android:
      - Install vuplex webviw accordingly and build!
### Future Work (Soon release)
1. Connect to Metamask, WalletConnect etc..

