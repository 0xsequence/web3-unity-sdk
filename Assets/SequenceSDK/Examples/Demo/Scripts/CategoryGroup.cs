using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class CategoryGroup : MonoBehaviour
{
    public UnityAction<float> OnScaleChange; // Returns scale diff

    [SerializeField] private TextMeshProUGUI groupLabel = null;
    [SerializeField] private Button btn = null;
    [SerializeField] private RectTransform arrowTransform = null;
    [SerializeField] private RectTransform contentRoot = null;

    private const float SELECTED_ARROW_ROTATION = 0f;
    private const float HIDDEN_ARROW_ROTATION = -90f;
    private const float EXPAND_COLLAPSE_DURATION = 0.2f;

    private float ButtonHeight { get { return btn.GetComponent<RectTransform>().sizeDelta.y; } }

    private List<Category> _categories = new List<Category>();

    private RectTransform _rectTransform;
    private bool _visible;
    private float _categorySpacing;
    private Vector2 _maxContentScale;
    private ContractType _contractType = ContractType.UNKNOWN;

    private IEnumerator _scalingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        AddEventListener(OnButtonClick);
    }

    /// <summary>
    /// Sets Group label name based on <paramref name="contractType"/>
    /// </summary>
    /// <param name="contractType"></param>
    public void InitGroup(ContractType contractType, float spacing)
    {
        _contractType = contractType;
        _categorySpacing = spacing;
        groupLabel.text = _contractType.ToString();
        GetRectTransform().sizeDelta = new Vector2(GetRectTransform().sizeDelta.x, ButtonHeight);
    }

    public void ShowCategories()
    {
        for (int i = 0; i < _categories.Count; i++)
        {
            _categories[i].gameObject.SetActive(true);
        }

        //SetContentRootScale(_categorySpacing);
        SetContentRootScale();
        StartCoroutine(SetArrowContentShown());

        if(_scalingCoroutine != null)
        {
            StopCoroutine(_scalingCoroutine);
        }

        _scalingCoroutine = ExpandContentView();
        StartCoroutine(_scalingCoroutine);
    }

    public void HideCategories()
    {
        // Shouldn't disable them right away
        //for (int i = 0; i < _categories.Count; i++)
        //{
        //    _categories[i].gameObject.SetActive(false);
        //}

        SetContentRootScale();
        StartCoroutine(SetArrowContentHidden());

        if (_scalingCoroutine != null)
        {
            StopCoroutine(_scalingCoroutine);
        }

        _scalingCoroutine = CollapseContentView();
        StartCoroutine(_scalingCoroutine);
    }

    /// <summary>
    /// Adds category to group, disables category gameobject
    /// </summary>
    /// <param name="cat"></param>
    public void AddToCategories(Category cat)
    {
        _categories.Add(cat);
        cat.transform.SetParent(contentRoot);
        cat.GetRectTransform().anchoredPosition = new Vector2(0f, (_categories.Count - 1) * -cat.GetRectTransform().sizeDelta.y);

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
    /// Returns height of Category Group
    /// </summary>
    /// <returns></returns>
    public float GetHeight()
    {
        return GetRectTransform().sizeDelta.y;
    }

    public RectTransform GetRectTransform()
    {
        if(_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        return _rectTransform;
    }

    /// <summary>
    /// Adds event to the CategoryGroup CollapseableButton onClick
    /// </summary>
    /// <param name="onClick"></param>
    public void AddEventListener(UnityAction onClick)
    {
        btn.onClick.AddListener(onClick);
    }

    private void SetContentRootScale(float catSpacing = 0f)
    {
        _maxContentScale = new Vector2(GetRectTransform().sizeDelta.x, ButtonHeight);

        if (_categories.Count > 0)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].gameObject.activeSelf)
                {
                    _maxContentScale.y += _categories[i].GetRectTransform().sizeDelta.y;
                }
            }

            _maxContentScale.y += catSpacing * (_categories.Count - 1);
        }
    }

    private void OnButtonClick()
    {
        if(_visible)
        {
            _visible = false;
            HideCategories();
        }
        else
        {
            _visible = true;
            ShowCategories();
        }
    }

    private IEnumerator SetArrowContentShown()
    {
        float startTime = Time.time;
        Quaternion startRotation = arrowTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, SELECTED_ARROW_ROTATION);
        float transition = 0f;

        while (transition < 1f)
        {
            transition = (Time.time - startTime) / EXPAND_COLLAPSE_DURATION;
            arrowTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, EaseIn(transition));

            yield return null;
        }

        arrowTransform.rotation = targetRotation;

        yield return null;
    }

    private IEnumerator SetArrowContentHidden()
    {
        float startTime = Time.time;
        Quaternion startRotation = arrowTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, HIDDEN_ARROW_ROTATION);
        float transition = 0f;

        while (transition < 1f)
        {
            transition = (Time.time - startTime) / EXPAND_COLLAPSE_DURATION;
            arrowTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, EaseIn(transition));

            yield return null;
        }

        arrowTransform.rotation = targetRotation;

        yield return null;
    }

    private IEnumerator ExpandContentView()
    {
        float startTime = Time.time;
        Vector2 startScale = GetRectTransform().sizeDelta;
        Vector2 targetScale = _maxContentScale;
        float transition = 0f;
        float percent = (_maxContentScale.y - startScale.y) / (_maxContentScale.y - ButtonHeight);
        float duration = EXPAND_COLLAPSE_DURATION * percent;
        float prevScale = startScale.y;

        while(transition < 1f)
        {
            transition = (Time.time - startTime) / duration;
            GetRectTransform().sizeDelta = Vector2.Lerp(startScale, targetScale, EaseIn(transition));
            OnScaleChange?.Invoke(GetRectTransform().sizeDelta.y - prevScale);
            prevScale = GetRectTransform().sizeDelta.y;
            yield return null;
        }

        GetRectTransform().sizeDelta = targetScale;
        OnScaleChange?.Invoke(GetRectTransform().sizeDelta.y - prevScale);

        yield return null;
    }

    private IEnumerator CollapseContentView()
    {
        float startTime = Time.time;
        Vector2 startScale = GetRectTransform().sizeDelta;
        Vector2 targetScale = new Vector2(startScale.x, ButtonHeight);
        float transition = 0f;
        float percent = Mathf.Clamp(startScale.y - ButtonHeight, 0f, _maxContentScale.y) / (_maxContentScale.y - ButtonHeight);
        float duration = EXPAND_COLLAPSE_DURATION * percent;
        float prevScale = startScale.y;

        while (transition < 1f)
        {
            transition = (Time.time - startTime) / duration;
            GetRectTransform().sizeDelta = Vector2.Lerp(startScale, targetScale, EaseIn(transition));
            OnScaleChange?.Invoke(GetRectTransform().sizeDelta.y - prevScale);
            prevScale = GetRectTransform().sizeDelta.y;

            yield return null;
        }

        GetRectTransform().sizeDelta = targetScale;
        OnScaleChange?.Invoke(GetRectTransform().sizeDelta.y - prevScale);

        yield return null;
    }

    private float EaseIn(float t)
    {
        return Flip(Square(Flip(t)));
    }

    private float Flip(float x)
    {
        return 1 - x;
    }

    private float Square(float x)
    {
        return x * x;
    }
}
