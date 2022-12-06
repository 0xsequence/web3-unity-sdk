using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum ScreenRatio
{
    Default,
    Two_One,
    OneHalf_One,
    One_One,
    One_OneHalf,
    One_Two
}
public class DemoUIManager : MonoBehaviour
{

    
    public new Camera camera;

    //Welcome Panel
    [Header("Welcome Panel")]
    public GridLayoutGroup welcomePanelLayout;
    public TMP_Dropdown networkDropdown;

    //Collection Scroll
    [Header("Collection")]
    public RectTransform collectionRect;
    public RectTransform collectionCatRect;
    public RectTransform collectionScrollRect;
    public GridLayoutGroup collectionCatLayout;
    public GridLayoutGroup collectionScrollLayout;

    //Button styles
    [Header("Buttons")]
    public GameObject welcomeButtonsParent;
    private List<Button> welcomeButtons;
    private Image welcomeBackgroundImage;

    //Sprites
    [Header("Sprites")]
    public Sprite buttonSprite;

    //Colors
    [Header("Colors")]
    public Color backgroundColor;
    public Color buttonBackgroundColor;
    public Color buttonHighlightColor;
    public Color buttonTextColor;

    private ScreenRatio screenRatio= ScreenRatio.Default;
    private void Start()
    {
        screenRatio = GetScreenRatio();
        GetAllElements();
        SetStyle();

    }


    private void GetAllElements()
    {
        //welcome panel        
        welcomeButtons = new List<Button>(welcomeButtonsParent.GetComponentsInChildren<Button>());
        welcomeBackgroundImage = welcomeButtonsParent.GetComponent<Image>();

        //Address

        //Collections

        //History

        //Metamask

    }
    private void ClearAllElements()
    {
        //welcome panel
        welcomeButtons.Clear();
        //Address

        //Collections

        //History

        //Metamask
    }
    /// <summary>
    /// Hardcoded style sheet for demo
    /// </summary>
    private void SetStyle()
    {
        switch(screenRatio)
        {
            case ScreenRatio.Two_One:
                SetWelcomePanelStyle(3, new Vector2(200, 50), new Vector2(20, 25), 14, 4f);
                break;
            case ScreenRatio.OneHalf_One:
                SetWelcomePanelStyle(2, new Vector2(300, 50), new Vector2(40, 25), 14, 4f);
                break;
            case ScreenRatio.One_One:
                SetWelcomePanelStyle(2, new Vector2(300, 50), new Vector2(40, 25), 14, 4f);
                break;
            case ScreenRatio.One_OneHalf:
                SetWelcomePanelStyle(1, new Vector2(450, 90), new Vector2(0, 30), 28, 2f);
                break;
            case ScreenRatio.One_Two:
                SetWelcomePanelStyle(1, new Vector2(450, 90), new Vector2(0, 30), 28, 2f);
                break;
        }
        //Welcome Panel

        //Address

        //Collections

        //History

        //Metamask
    }
    /// <summary>
    /// Set Button styles in welcome panel
    /// </summary>
    /// <param name="fontSize"></param>
    /// <param name="roundCorner">Unit per pixel in image component </param>
    private void SetWelcomePanelStyle(int groupConstraint, Vector2 buttonSize, Vector2 buttonSpacing, int fontSize, float roundCorner)
    {
        //Panel Layout
        welcomePanelLayout.constraintCount = groupConstraint;
        welcomePanelLayout.cellSize = buttonSize;
        welcomePanelLayout.spacing = buttonSpacing;
        //Panel Color
        welcomeBackgroundImage.color = Color.black;
        //Buttons
        foreach (var button in welcomeButtons)
        {
            var btnImage = button.gameObject.GetComponent<Image>();
            btnImage.sprite = buttonSprite;
            btnImage.color = buttonBackgroundColor;
            btnImage.pixelsPerUnitMultiplier = roundCorner;
            var txt = button.gameObject.GetComponentInChildren<TMP_Text>();
            txt.color = buttonTextColor;
            txt.fontSize = fontSize;
        }
        //Chain Selector
        var parentWidth = welcomePanelLayout.GetComponent<RectTransform>().rect.width;
        networkDropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(parentWidth / 3f, buttonSize.y/2f);
        
        //font and colors
        networkDropdown.GetComponent<Image>().color = buttonBackgroundColor;
        networkDropdown.captionText.fontSize = fontSize;
        networkDropdown.captionText.color = buttonTextColor;
        networkDropdown.itemText.fontSize = fontSize;
        networkDropdown.itemText.color = buttonTextColor;
        Toggle networkToggle = networkDropdown.template.GetComponentInChildren<Toggle>();
        ColorBlock toggleCB = networkToggle.colors;
        toggleCB.normalColor= buttonBackgroundColor;
        toggleCB.selectedColor= buttonHighlightColor;
        networkToggle.colors = toggleCB;
    }

    private ScreenRatio GetScreenRatio()
    {

        if (camera.aspect > 1.7)
        {
            return ScreenRatio.Two_One;           
        }
        else if(camera.aspect > 1.2)
        {
            return ScreenRatio.OneHalf_One;
        }
        else if(camera.aspect > 0.9)
        {
            return ScreenRatio.One_One;
        }
        else if(camera.aspect> 0.5)
        {
            return ScreenRatio.One_OneHalf;
        }
        else
        {
            return ScreenRatio.One_Two;
        }
    }


    //========Todo: Find another script for the following =============
    public void EnableCollectionPanel()
    {
        float height = collectionRect.sizeDelta.y;
        AdjustCollectionScrollRect(height);
    }


    private void AdjustCollectionScrollRect(float height)
    {
        float categoryHeight = height / 5.0f;
        float scrollHeight = height * 4.0f / 5.0f;
        collectionCatRect.sizeDelta = new Vector2(collectionCatRect.sizeDelta.x, categoryHeight);

        collectionScrollRect.anchoredPosition = new Vector2(collectionScrollRect.transform.position.x, -categoryHeight);
        collectionScrollRect.sizeDelta = new Vector2(collectionScrollRect.sizeDelta.x, scrollHeight);

        collectionCatLayout.cellSize = new Vector2(categoryHeight * 0.8f, categoryHeight * 0.8f);
        //collectionScrollLayout.cellSize = new Vector2(categoryHeight, categoryHeight);
    }
}
