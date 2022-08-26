using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenCover : MonoBehaviour
{
    public static LoadingScreenCover Instance;

    public bool ContentLoading { get { return screenCoverRoot.activeSelf; } }

    [SerializeField] private GameObject screenCoverRoot = null;
    [SerializeField] private RectTransform loadingSymbol = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            // If an Instance of LoadingScreenCover already exists, destroy any newly created versions
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ContentLoading)
        {
            loadingSymbol.Rotate(Vector3.forward * -180f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Enables visuals that block interaction
    /// </summary>
    public void EnableLoadingCover()
    {
        screenCoverRoot.SetActive(true);
        loadingSymbol.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Disables visuals that block interaction
    /// </summary>
    public void DisableLoadingCover()
    {
        screenCoverRoot.SetActive(false);
    }
}
