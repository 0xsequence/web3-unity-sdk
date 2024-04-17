# Web3 Unity SDK

*Notice*: This SDK has been deprecated and is no longer being actively supported. Please see our new [Unity SDK](https://github.com/0xsequence/sequence-unity) and [documentation](https://docs.sequence.xyz/sdk/unity/overview)!

The Sequence Unity SDK provides full Sequence Wallet & Indexer integration for your Unity Games.

This SDK follows [Semantic Versioning](https://semver.org/) (`major.minor.patch`). While we're still in `0.x.y` builds, API breaking changes can be made at any time. After `1.0.0`, breaking changes will always cause a `major` version increment, non-breaking new features will cause a `minor` version increment, and bugfixes will cause a `patch` version increment.

## [Check out the Unity WebGL build of our demo dApp!](https://0xsequence.github.io/web3-unity-sdk/)

The demo offers the same functionality as [our sequence.js demo dApp](https://github.com/0xsequence/demo-dapp), but using Unity UI & the Unity SDK.

## Using the SDK

Read the docs over at [https://docs.sequence.xyz/unity-sdk](https://docs.sequence.xyz/unity-sdk)!

## Testing the Example locally

1. Download [the latest release of the Sequence Unity SDK UnityPackage](https://github.com/0xsequence/web3-unity-sdk/releases)
2. Drag the `.unitypackage` file into your Unity game project & import the entire thing.
3. The Sequence Unity SDK currently depends on the [Vuplex webview package](https://vuplex.com), so you'll need to purchase and import Vuplex to run the demos.
4. Open the scene in `SequenceSDK/Examples/Demo/Scenes`.

## Developing the SDK

### Formatting

Before committing, please run `dotnet format demo-unity-webgl-game.sln`
