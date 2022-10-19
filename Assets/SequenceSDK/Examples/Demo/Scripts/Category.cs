using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SequenceSharp;

/*

Clicking on a Category will slide menu over to left and show content in a panel from the right
Categories are sorted into sub menus based on Contract Type

*/
[System.Serializable]
public class Category : MonoBehaviour
{
    [SerializeField] private Button catBtn = null;
    [SerializeField] private Image iconImg = null;
    [SerializeField] private TextMeshProUGUI btnLbl = null;

    public string _catName { get; private set; }
    public string _contactAddress { get; private set; }
    public Texture _icon { get; private set; }
    public ContractType _contractType { get; private set; }

    private RectTransform _rectTransform;

    private void OnDisable()
    {
        catBtn.onClick.RemoveListener(OnButtonClick);
    }

    private void Start()
    {
        if (catBtn == null)
        {
            catBtn = GetComponent<Button>();
        }

        catBtn.onClick.AddListener(OnButtonClick);
    }

    public void Init(string name)
    {
        _catName = name;

        btnLbl.text = name;

        iconImg.gameObject.SetActive(false);

    }

    public void Init(string name, Texture icon)
    {
        _catName = name;
        this._icon = icon;

        btnLbl.text = name;

        if (icon != null)
        {
            ApplyIcon(icon);
        }
        else
        {

        }
    }

    public void Init(string name, Texture icon, ContractType contractType)
    {
        _catName = name;
        this._icon = icon;
        this._contractType = contractType;

        btnLbl.text = name;

        if (icon != null)
        {
            ApplyIcon(icon);
        }
        else
        {
            iconImg.gameObject.SetActive(false);
            //StretchLabel();
        }
    }

    /// <summary>
    /// Get Rect Transform of Category
    /// </summary>
    /// <returns></returns>
    public RectTransform GetRectTransform()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        return _rectTransform;
    }

    private void OnButtonClick()
    {
        // TODO: Load metadata content

    }

    private void ApplyIcon(Texture tex)
    {
        if (tex != null)
        {
            float width = iconImg.rectTransform.rect.width;
            float height = iconImg.rectTransform.rect.height;
            iconImg.sprite = Sprite.Create(TextureHelper.ConvertToTexture2D(tex, (int)width, (int)height), new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
