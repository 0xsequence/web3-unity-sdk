using System.Collections;
using UnityEngine.TestTools;
using NBitcoin;
using Nethereum.ABI.EIP712;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Newtonsoft.Json;
using SequenceSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

public class EthCallTest
{
    public Web3 web3;
    private SequenceWeb3Client _sequenceWeb3Client;

    //Scene

    private GameObject cameraObject;
    private GameObject canvasObject;

    [SetUp]
    public void Setup()
    {
 
        cameraObject = new GameObject();
        cameraObject.name = "camera";
        Camera cam = cameraObject.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.cullingMask = LayerMask.NameToLayer("Everything");
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 1000;
        cam.rect = new Rect(0, 0, 1, 1);
        cam.depth = -1;


        canvasObject = new GameObject();
        canvasObject.name = "Canvas - Wallet";
        Canvas walletCanvas = canvasObject.AddComponent<Canvas>();
        walletCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        walletCanvas.worldCamera = cam;
        walletCanvas.planeDistance = 100;
        walletCanvas.sortingLayerID = 1;

        CanvasScaler walletCanvasScaler = canvasObject.AddComponent<CanvasScaler>();
        walletCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        walletCanvasScaler.referenceResolution = new UnityEngine.Vector2(800, 600);
        walletCanvasScaler.matchWidthOrHeight = 0f;
        walletCanvasScaler.referencePixelsPerUnit = 100;

        GraphicRaycaster walletGraphicRaycaster = canvasObject.AddComponent<GraphicRaycaster>();
        walletGraphicRaycaster.ignoreReversedGraphics = true;
        walletGraphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        walletGraphicRaycaster.blockingMask = LayerMask.NameToLayer("Everything");




    }

    [UnityTest]
    public IEnumerator WalletInitialization()
    {

        GameObject sequenceWalletPrefab = GameObject.Instantiate(Resources.Load("Canvas-Wallet") as GameObject);
        Wallet wallet = sequenceWalletPrefab.GetComponent<Wallet>();
        _sequenceWeb3Client = new SequenceWeb3Client(wallet, Chain.Polygon);
        Assert.AreEqual(_sequenceWeb3Client.name, "Sequence Web3 Client");
        web3 = new Web3(_sequenceWeb3Client);
        Assert.NotNull(web3);

        yield return new WaitForSeconds(1);
    }

    [UnityTest]
    public IEnumerator WalletConnect()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    #region API_METHOD_methods tests

    [UnityTest]
    public IEnumerator API_METHOD_Eth_ChainId()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_ProtocolVersion()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_Syncing()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_Coinbase()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_Mining()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_Hashrate()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_GasPrice()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_FeeHistory()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_Accounts()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_BlockNumber()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetBalance()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetStorageAt()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetTransactionCount()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetBlockTransactionCountByHash()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetBlockTransactionCountByNumber()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetUncleCountByBlockHash()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetUncleCountByBlockNumber()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetCode()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_Sign()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_SendTransaction()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_SendRawTransaction()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_Call()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_EstimateGas()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetBlockByHash()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetBlockByNumber()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetTransactionByBlockHashAndIndex()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetTransactionByBlockNumberAndIndex()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }

    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetTransactionReceipt()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetUncleByBlockHashAndIndex()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetUncleByBlockNumberAndIndex()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetCompilers()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_CompileLLL()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_CompileSolidity()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_CompileSerpent()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_NewFilter()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_NewBlockFilter()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_NewPendingTransactionFilter()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_UninstallFilter()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetFilterChanges()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetFilterLogs()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetLogs()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_GetWork()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_SubmitWork()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_SubmitHashrate()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_Subscribe()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    [UnityTest]
    public IEnumerator API_METHOD_Eth_Unsubscribe()
    {
        Assert.Fail("TEST NOT IMPLEMENTED");
        yield return null;
    }
    #endregion

}
