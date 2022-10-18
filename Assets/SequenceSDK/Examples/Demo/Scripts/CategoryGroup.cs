using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class CategoryGroup : MonoBehaviour
{
    // public UnityAction<float> OnScaleChange; // Returns scale diff
    public Collection collection;
    [SerializeField] private TextMeshProUGUI groupLabel = null;
    [SerializeField] private Button btn = null;
    [SerializeField] private RectTransform contentRoot = null;


    private List<Category> _categories = new List<Category>();

    private bool _visible = false;

    private ContractType _contractType = ContractType.UNKNOWN;


    // Start is called before the first frame update
    void Start()
    {
        AddEventListener(OnButtonClick);
        collection = DemoManager.Instance.GetComponent<Collection>();
    }

    /// <summary>
    /// Sets Group label name based on <paramref name="contractType"/>
    /// </summary>
    /// <param name="contractType"></param>
    public void InitGroup(ContractType contractType, float spacing)
    {
        _contractType = contractType;
        groupLabel.text = _contractType.ToString();

    }

    public void ShowCategories()
    {
        for (int i = 0; i < _categories.Count; i++)
        {
            _categories[i].gameObject.SetActive(true);
        }
        _visible = true;
    }

    public void HideCategories()
    {

        // Shouldn't disable them right away
        for (int i = 0; i < _categories.Count; i++)
        {
            _categories[i].gameObject.SetActive(false);
        }
        _visible = false;

    }


    /// <summary>
    /// Adds category to group, disables category gameobject
    /// </summary>
    /// <param name="cat"></param>
    public void AddToCategories(Category cat)
    {
        _categories.Add(cat);

        if(cat.gameObject.activeSelf)
        {
            cat.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Destroys all categories in Group
    /// </summary>
    public void ClearCategoryGroup()
    {
        foreach (var cat in _categories)
        {
            Destroy(cat.gameObject);
        }

        _categories.Clear();
    }

    /// <summary>
    /// Adds event to the CategoryGroup CollapseableButton onClick
    /// </summary>
    /// <param name="onClick"></param>
    public void AddEventListener(UnityAction onClick)
    {
        btn.onClick.AddListener(onClick);
    }


    private void OnButtonClick()
    {
        collection.HideCategories();
        if (!_visible)
        {
            ShowCategories();
        }
    }


}
