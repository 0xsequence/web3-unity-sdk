using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using SequenceSharp;
using System.Numerics;

/// <summary>
/// Handle receiving an Account Address and pulling all contract info data to generate Category options for a user to load content from
/// </summary>
public class Collection : MonoBehaviour
{
    [Header("Categories")]
    [SerializeField]
    private RectTransform tokensRoot = null;

    [SerializeField]
    private RectTransform catogryGroupRoot = null;

    [SerializeField]
    private GameObject categoryGroupTemplate = null;

    [SerializeField]
    private GameObject categoryTemplate = null;

    [SerializeField, Min(0f)]
    private float categorySpacing = 5f;

    public Button m_backButton;

    private Dictionary<ContractType, CategoryGroup> _categoryGroups =
        new Dictionary<ContractType, CategoryGroup>();

    private void OnEnable()
    {
        m_backButton.onClick.AddListener(BackToWelcomePanel);
    }

    private void OnDisable()
    {
        m_backButton.onClick.RemoveListener(BackToWelcomePanel);
    }

    public void BackToWelcomePanel()
    {
        DemoManager.Instance.HideCollectionPanel();
        DemoManager.Instance.DisplayWelcomePanel();
    }

    public void RetriveContractInfoData(TokenBalance[] tokenBalances)
    {
        ClearCategories();

        if (tokenBalances != null && tokenBalances.Length > 0)
        {
            StartCoroutine(GenerateCategories(tokenBalances));
        }
    }

    private IEnumerator GenerateCategories(TokenBalance[] tokenBalances)
    {
        GameObject newCatGo;
        Category newCategory;
        ContractInfo contractInfo = null;
        Texture logoTex = null;
        UnityWebRequest imgRequest;
        TokenMetadata tokenMetadata = null;
        string contractAddress;
        for (int i = 0; i < tokenBalances.Length; i++)
        {
            //check for metadata
            tokenMetadata = tokenBalances[i].tokenMetadata;
            contractInfo = tokenBalances[i].contractInfo;
            contractAddress = tokenBalances[i].contractAddress;

            newCatGo = Instantiate(categoryTemplate, tokensRoot);
            newCategory = newCatGo.GetComponent<Category>();
            if (_categoryGroups.ContainsKey(tokenBalances[i].contractType) == false)
            {
                CategoryGroup newCatGroup = Instantiate(categoryGroupTemplate, catogryGroupRoot)
                    .GetComponent<CategoryGroup>();
                _categoryGroups.Add(tokenBalances[i].contractType, newCatGroup);
                newCatGroup.InitGroup(tokenBalances[i].contractType, categorySpacing);
            }

            // Add new Category option to their relevant ContractType Group
            _categoryGroups[tokenBalances[i].contractType].AddToCategories(newCategory);

            if (logoTex != null)
            {
                Destroy(logoTex);
                logoTex = null;
            }
            if (tokenMetadata != null)
            {
                if (
                    tokenMetadata.image != null
                    && tokenMetadata.image.Length > 0
                    && !tokenMetadata.image.EndsWith("gif")
                )
                {
                    imgRequest = UnityWebRequestTexture.GetTexture(tokenMetadata.image);

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
                    imgRequest.Dispose();
                }
            }
            else
            {
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
                    imgRequest.Dispose();
                }
            }

            var type = ContractType.UNKNOWN;
            try
            {
                type = Enum.Parse<ContractType>(contractInfo.type);
            }
            catch
            {
                // ok!
            }
            newCategory.Init(
                tokenMetadata != null
                    ? ($"{tokenMetadata.name} ({contractInfo.name})")
                    : contractInfo.name,
                logoTex,
                type,
                contractAddress,
                tokenMetadata.tokenId != null ? BigInteger.Parse(tokenMetadata.tokenId) : null
            );
        }
        yield return null;
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

    public void HideCategories()
    {
        if (_categoryGroups.Count > 0)
        {
            CategoryGroup currentGroup;
            foreach (ContractType cType in Enum.GetValues(typeof(ContractType)))
            {
                if (_categoryGroups.TryGetValue(cType, out currentGroup))
                {
                    currentGroup.HideCategories();
                }
            }
        }
    }
}
