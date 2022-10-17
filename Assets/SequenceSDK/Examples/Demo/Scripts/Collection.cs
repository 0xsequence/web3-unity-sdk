using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// Handle receiving an Account Address and pulling all contract info data to generate Category options for a user to load content from
/// </summary>
public class Collection : MonoBehaviour
{


    [Header("Categories")]
    [SerializeField] private RectTransform contentRoot = null;
    [SerializeField] private GameObject categoryGroupTemplate = null;
    [SerializeField] private GameObject categoryTemplate = null;
    [SerializeField, Min(0f)] private float categorySpacing = 5f;

    private string _currentAccountAddress;
    private BlockChainType _blockChainType;

    private Dictionary<ContractType, CategoryGroup> _categoryGroups = new Dictionary<ContractType, CategoryGroup>();

    private void Start()
    {
        RetriveContractInfoData("0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9");
    }

    public void RetriveContractInfoData(string accountAddress)
    {
        if (accountAddress.Length > 0)
        {
            //LoadingScreenCover.Instance.EnableLoadingCover();

            GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, "", true);
            Indexer.GetTokenBalances(_blockChainType, tokenBalancesArgs, (tokenBalances) =>
            {
                ClearCategories();
                _currentAccountAddress = accountAddress;

                if (tokenBalances != null && tokenBalances.balances.Length > 0)
                {
                    StartCoroutine(GenerateCategories(tokenBalances));
                }
                else
                {
                   // LoadingScreenCover.Instance.DisableLoadingCover();
                }
            });
        }
    }

    private IEnumerator GenerateCategories(GetTokenBalancesReturn tokenBalances)
    {
        GameObject newCatGo;
        Category newCategory;
        ContractInfo contractInfo;
        Texture logoTex = null;
        UnityWebRequest imgRequest;
        float totalHeight = 0f;

        for (int i = 0; i < tokenBalances.balances.Length; i++)
        {
            contractInfo = tokenBalances.balances[i].contractInfo;

            if (contractInfo != null)
            {
                newCatGo = Instantiate(categoryTemplate);
                newCategory = newCatGo.GetComponent<Category>();

                if (_categoryGroups.ContainsKey(tokenBalances.balances[i].contractType) == false)
                {
                    CategoryGroup newCatGroup = Instantiate(categoryGroupTemplate, contentRoot).GetComponent<CategoryGroup>();
                    _categoryGroups.Add(tokenBalances.balances[i].contractType, newCatGroup);
                    newCatGroup.InitGroup(tokenBalances.balances[i].contractType, categorySpacing);

                    totalHeight += newCatGroup.GetHeight();
                    totalHeight += categorySpacing;
                    contentRoot.sizeDelta = new Vector2(contentRoot.sizeDelta.x, totalHeight);

                    newCatGroup.OnScaleChange += OnGroupContentResize;
                }

                // Add new Category option to their relevant ContractType Group
                _categoryGroups[tokenBalances.balances[i].contractType].AddToCategories(newCategory);

                if (logoTex != null)
                {
                    Destroy(logoTex);
                }

                if (contractInfo.logoURI != null && contractInfo.logoURI.Length > 0)
                {
                    imgRequest = UnityWebRequestTexture.GetTexture(contractInfo.logoURI);

                    yield return imgRequest.SendWebRequest();

                    if (imgRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(imgRequest.error);
                    }
                    else
                    {
                        // Create new card and initiate it
                        logoTex = ((DownloadHandlerTexture)imgRequest.downloadHandler).texture;
                    }
                }

                newCategory.Init(contractInfo.name, logoTex, Enum.Parse<ContractType>(contractInfo.type));
            }
        }

        // Remove excess height 
        totalHeight -= categorySpacing;
        contentRoot.sizeDelta = new Vector2(contentRoot.sizeDelta.x, totalHeight);

        // Position Category Groups
        CategoryGroup currentGroup;
        float catPosY = 0f;
        foreach (ContractType cType in Enum.GetValues(typeof(ContractType)))
        {
            if (_categoryGroups.TryGetValue(cType, out currentGroup))
            {
                currentGroup.GetRectTransform().anchoredPosition = new Vector2(0f, catPosY);
                catPosY -= currentGroup.GetHeight() + categorySpacing;
            }
        }

        // Finished loading categories
        LoadingScreenCover.Instance.DisableLoadingCover();

        yield return null;
    }



    private void OnGroupContentResize(float scaleDiff)
    {
        Vector2 newScale = new Vector2(contentRoot.sizeDelta.x, contentRoot.sizeDelta.y + scaleDiff);
        contentRoot.sizeDelta = newScale;

        CategoryGroup currentGroup;
        float catPosY = 0f;
        foreach (ContractType cType in Enum.GetValues(typeof(ContractType)))
        {
            if (_categoryGroups.TryGetValue(cType, out currentGroup))
            {
                currentGroup.GetRectTransform().anchoredPosition = new Vector2(0f, catPosY);
                catPosY -= currentGroup.GetHeight() + categorySpacing;
            }
        }
    }

    /// <summary>
    /// Destroys all category gameobjects under <see cref="contentRoot"/>
    /// </summary>
    private void ClearCategories()
    {
        if (_categoryGroups.Count > 0)
        {
            CategoryGroup currentGroup;
            foreach (ContractType cType in Enum.GetValues(typeof(ContractType)))
            {
                if (_categoryGroups.TryGetValue(cType, out currentGroup))
                {
                    currentGroup.ClearCategoryGroup();
                    Destroy(currentGroup.gameObject);
                }
            }

            _categoryGroups.Clear();
        }
    }
}
