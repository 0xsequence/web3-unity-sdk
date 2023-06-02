using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using SequenceSharp;
using System.Numerics;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Linq;

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

    private DemoUIManager uiManager;

    private void OnEnable()
    {
        m_backButton.onClick.AddListener(BackToWelcomePanel);
        uiManager = DemoManager.Instance.uiManager;
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

    public async Task RetriveContractInfoData(TokenBalance[] tokenBalances)
    {
        ClearCategories();

        if (tokenBalances != null && tokenBalances.Length > 0)
        {
            await GenerateCategories(tokenBalances);
        }
    }

    private async Task GenerateCategories(TokenBalance[] tokenBalances)
    {

        await Task.WhenAll(tokenBalances.Select(async (tb) =>
        {
            //check for metadata
            var tokenMetadata = tb.tokenMetadata;
            var contractInfo = tb.contractInfo;
            var contractAddress = tb.contractAddress;
            Texture2D logoTex = null;

            var metaURL = tokenMetadata != null && tokenMetadata.image != null
                    && tokenMetadata.image.Length > 0
                    && !tokenMetadata.image.EndsWith("gif") ? tokenMetadata.image : ((contractInfo.logoURI != null && contractInfo.logoURI.Length > 0) ? contractInfo.logoURI : null);
            if (metaURL != null)
            {
                using (var imgRequest = UnityWebRequestTexture.GetTexture(metaURL))
                {
                    await imgRequest.SendWebRequest();

                    if (imgRequest.result != UnityWebRequest.Result.Success)
                    {

                        Debug.Log(metaURL + ", " + imgRequest.error);
                    }
                    else
                    {
                        // Create new card and initiate it
                        logoTex = ((DownloadHandlerTexture)imgRequest.downloadHandler).texture;
                    }
                }
            }

            var type = ContractType.UNKNOWN;
            try
            {
#if UNITY_2021_3_OR_NEWER
                type = Enum.Parse<ContractType>(contractInfo.type);
#else
                Enum.TryParse<ContractType>(contractInfo.type, out type);
#endif
            }
            catch
            {
                // ok!
            }
#if UNITY_2021_3_OR_NEWER
                BigInteger? tokenID =
                    (tokenMetadata != null)
                    ? tokenMetadata.tokenId
                    : null;
#else
            BigInteger? tokenID;

            if (tokenMetadata != null)
            {
                tokenID = tokenMetadata.tokenId;
            }
            else
            {
                tokenID = null;
            }
#endif
            var newCatGo = Instantiate(categoryTemplate, tokensRoot);
            var newCategory = newCatGo.GetComponent<Category>();

            uiManager.SetCollectionCategoryStyle(newCategory);

            if (_categoryGroups.ContainsKey(tb.contractType) == false)
            {
                CategoryGroup newCatGroup = Instantiate(categoryGroupTemplate, catogryGroupRoot)
                    .GetComponent<CategoryGroup>();
                Debug.Log(newCatGroup);
                _categoryGroups.Add(tb.contractType, newCatGroup);
                newCatGroup.InitGroup(tb.contractType, categorySpacing);

                uiManager.SetCollectionCategoryGroupStyle(newCatGroup);
            }

            // Add new Category option to their relevant ContractType Group
            _categoryGroups[tb.contractType].AddToCategories(newCategory);
            newCategory.Init(
                tokenMetadata != null
                    ? ($"{tokenMetadata.name} ({contractInfo.name})")
                    : contractInfo.name,
                logoTex,
                type,
                contractAddress,
                tokenID
            );
        }));

    }

    /// <summary>
    /// Destroys all category gameobjects.
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
