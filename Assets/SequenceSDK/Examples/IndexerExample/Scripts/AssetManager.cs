using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;

namespace demo
{
    public class AssetManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField = null;
        [SerializeField] private BlockChainDropdown blockChainDropDown = null;
        [SerializeField] private Button submitBtn = null;

        [Header("Assets")]
        [SerializeField] private RectTransform contentRoot = null;
        [SerializeField] private GameObject assetTemplate = null;
        [SerializeField] private GridLayoutGroup gridLayoutGroup = null;

        private CardProperties cardProperties;
        private string _currentAccountAddress;
        private List<CardAsset> cardAssets;
        private void OnDisable()
        {
            submitBtn.onClick.RemoveListener(OnSubmitPressed);
        }

        private void OnEnable()
        {
            submitBtn.onClick.AddListener(OnSubmitPressed);
        }

        private void Awake()
        {
            cardAssets = new List<CardAsset>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (submitBtn.interactable)
                {
                    OnSubmitPressed();
                }
            }
        }

        public async void RetriveAssetInfoData(string accountAddress)
        {
            if (accountAddress.Length > 0)
            {
                LoadingScreenCover.Instance.EnableLoadingCover();

                GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, "", true);
                BlockChainType blockChainType = blockChainDropDown.GetSelectedOption();
                var tokenBalances = await Indexer.GetTokenBalances(blockChainType, tokenBalancesArgs);
                _currentAccountAddress = accountAddress;

                if (tokenBalances != null && tokenBalances.balances.Length > 0)
                {
                    for (int i = 0; i < tokenBalances.balances.Length; i++)
                    {
                        ContractInfo contractInfo = tokenBalances.balances[i].contractInfo;
                        string contractAddress = contractInfo.address;
                        if (contractInfo.name.Contains("skyweaver") || contractInfo.name.Contains("Skyweaver"))
                        {
                            GetTokenBalancesArgs tokenBalancesArgsWithContract = new GetTokenBalancesArgs(accountAddress, contractAddress, true);
                            var tokenBalancesWithContract = await Indexer.GetTokenBalances(blockChainType, tokenBalancesArgsWithContract);

                            if (tokenBalancesWithContract != null && tokenBalancesWithContract.balances.Length > 0)
                            {
                                StartCoroutine(GenerateAssets(tokenBalancesWithContract));
                            }
                            else
                            {
                                LoadingScreenCover.Instance.DisableLoadingCover();
                            }
                        }
                    }

                }
                else
                {
                    LoadingScreenCover.Instance.DisableLoadingCover();
                }

            }
        }

        private IEnumerator GenerateAssets(GetTokenBalancesReturn tokenBalances)
        {

            UnityWebRequest imgRequest;
            for (int i = 0; i < tokenBalances.balances.Length; i++)
            {
                if (tokenBalances.balances[i].tokenMetadata == null || tokenBalances.balances[i].tokenMetadata.image == null)
                {
                    continue;
                }

                if (tokenBalances.balances[i].tokenMetadata.image.Length > 0)
                {
                    imgRequest = UnityWebRequestTexture.GetTexture(tokenBalances.balances[i].tokenMetadata.image);

                    yield return imgRequest.SendWebRequest();

                    if (imgRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(imgRequest.error);
                    }
                    else
                    {


                        foreach (var item in tokenBalances.balances[i].tokenMetadata.properties)
                        {

                            switch (item.Key)
                            {

                                case "artist":
                                    cardProperties.artist = (Artist)item.Value;
                                    break;
                                case "attachment":
                                    cardProperties.attachment = (string)item.Value;
                                    break;
                                case "baseCardId":
                                    cardProperties.baseCardId = Convert.ToInt32(item.Value);
                                    break;
                                case "cardType":
                                    cardProperties.cardType = (string)item.Value;
                                    break;
                                case "element":
                                    cardProperties.element = (string)item.Value;
                                    break;
                                case "health":
                                    cardProperties.health = Convert.ToInt32(item.Value);
                                    break;
                                case "mana":
                                    cardProperties.mana = Convert.ToInt32(item.Value);
                                    break;
                                case "power":
                                    cardProperties.power = Convert.ToInt32(item.Value);
                                    break;
                                case "prism":
                                    cardProperties.prism = (string)item.Value;
                                    break;
                                case "trait":
                                    cardProperties.trait = (Trait)item.Value;
                                    break;
                                case "type":
                                    cardProperties.type = (string)item.Value;
                                    break;
                                default:
                                    break;

                            }

                        }
                        // Create new card and initiate it
                        AddAssetToGallery(((DownloadHandlerTexture)imgRequest.downloadHandler).texture, tokenBalances.balances[i].tokenMetadata.name, cardProperties);
                    }

                    yield return null;
                }
            }
        }

        private void AddAssetToGallery(Texture texture, string name, CardProperties cardProperties)
        {
            CardAsset newCard = Instantiate(assetTemplate, contentRoot.transform).GetComponent<CardAsset>();

            int texW = Mathf.CeilToInt(texture.width);
            int texH = Mathf.CeilToInt(texture.height);


            newCard.InitCardAsset(Sprite.Create(TextureHelper.ConvertToTexture2D(texture, texW, texH), new Rect(0f, 0f, texW, texH), new Vector2(0.5f, 0.5f), 100f), cardProperties);

            //newCard.cardProperties = cardProperties;

            //m_currentDeck.Add(newCard);

            if (!cardAssets.Contains(newCard))
            {
                cardAssets.Add(newCard);
            }
        }

        private void OnSubmitPressed()
        {
            if (inputField.text.Length > 0)
            {
                _currentAccountAddress = inputField.text;
                RetriveAssetInfoData(_currentAccountAddress);
            }
        }

        public void ClearAssets()
        {
            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }
        }


    }
}