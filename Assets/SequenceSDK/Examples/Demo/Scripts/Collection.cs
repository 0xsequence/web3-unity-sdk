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
    [SerializeField] private RectTransform tokensRoot = null;
    [SerializeField] private RectTransform catogryGroupRoot = null;
    [SerializeField] private GameObject categoryGroupTemplate = null;
    [SerializeField] private GameObject categoryTemplate = null;
    [SerializeField, Min(0f)] private float categorySpacing = 5f;

    private string _currentAccountAddress;
    private BlockChainType _blockChainType;

    private Dictionary<ContractType, CategoryGroup> _categoryGroups = new Dictionary<ContractType, CategoryGroup>();


    public void RetriveContractInfoData(string accountAddress)
    {
        if (accountAddress.Length > 0)
        {


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

        for (int i = 0; i < tokenBalances.balances.Length; i++)
        {
            contractInfo = tokenBalances.balances[i].contractInfo;

            if (contractInfo != null)
            {
                newCatGo = Instantiate(categoryTemplate, tokensRoot);

                newCategory = newCatGo.GetComponent<Category>();

                if (_categoryGroups.ContainsKey(tokenBalances.balances[i].contractType) == false)
                {
                    CategoryGroup newCatGroup = Instantiate(categoryGroupTemplate, catogryGroupRoot).GetComponent<CategoryGroup>();
                    _categoryGroups.Add(tokenBalances.balances[i].contractType, newCatGroup);
                    newCatGroup.InitGroup(tokenBalances.balances[i].contractType, categorySpacing);

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
}
