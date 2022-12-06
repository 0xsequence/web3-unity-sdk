using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

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

    //Connect Panel
    [Header("Connect")]
    public GridLayoutGroup connectPanelLayout;
    private List<Button> connectButtons;


    //Welcome Panel
    [Header("Welcome Panel")]
    public GridLayoutGroup welcomePanelLayout;
    public DemoLayout abiExamplePanelLayout;
    private List<Button> welcomeButtons;
    private List<Button> abiExampleButtons;
    private Image welcomeBackgroundImage;
    public TMP_Dropdown networkDropdown;

    //Address Panel
    [Header("Address")]
    public TMP_Text addressText;
    public Button addressBackButton;

    


    //Collection Scroll
    [Header("Collection")]
    public RectTransform collectionRect;
    public RectTransform collectionCatRect;
    public RectTransform collectionScrollRect;
    public GridLayoutGroup collectionCatLayout;
    public GridLayoutGroup collectionScrollLayout;

    //Sprites
    [Header("Sprites")]
    public Sprite buttonSprite;

    //Colors
    [Header("Colors")]
    public Color backgroundColor;
    public Color buttonBackgroundColor;
    public Color buttonHighlightColor;
    public Color buttonTextColor;
    public Color outlineColor; //Todo: make it gradient
    public Color addressTextColor;

    private ScreenRatio screenRatio= ScreenRatio.Default;
    private void Start()
    {
        screenRatio = GetScreenRatio();
        GetAllElements();
        SetStyle();
    }


    private void GetAllElements()
    {
        //Connect
        connectButtons = new List<Button>(connectPanelLayout.GetComponentsInChildren<Button>());
        //welcome panel        
        welcomeButtons = new List<Button>(welcomePanelLayout.GetComponentsInChildren<Button>());
        welcomeBackgroundImage = welcomePanelLayout.GetComponent<Image>();
        abiExampleButtons = new List<Button>(abiExamplePanelLayout.GetComponentsInChildren<Button>());
        //Address

        //Collections

        //History

        //Metamask

    }
    private void ClearAllElements()
    {
        //Connect
        connectButtons.Clear();
        //welcome panel
        welcomeButtons.Clear();
        abiExampleButtons.Clear();
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
                SetConnectPanelStyle(35, 4f);
                SetWelcomePanelStyle(3, new Vector2(200, 50), new Vector2(20, 25), 14, 4f);
                SetABIExampleButtonStyle(10,6f);
                SetAddressPanelStyle(35, 6f);
                break;
            case ScreenRatio.OneHalf_One:
                SetConnectPanelStyle(35, 4f);
                SetWelcomePanelStyle(2, new Vector2(300, 50), new Vector2(40, 25), 14, 4f);
                SetABIExampleButtonStyle(10, 6f);
                SetAddressPanelStyle(35, 6f);
                break;
            case ScreenRatio.One_One:
                SetConnectPanelStyle(28, 4f);
                SetWelcomePanelStyle(2, new Vector2(300, 50), new Vector2(40, 25), 14, 4f);
                SetABIExampleButtonStyle(10,6f);
                SetAddressPanelStyle(28, 6f);
                break;
            case ScreenRatio.One_OneHalf:
                SetConnectPanelStyle(28, 2f);
                SetWelcomePanelStyle(1, new Vector2(450, 90), new Vector2(0, 30), 28, 2f);
                SetABIExampleButtonStyle(25,4f);
                SetAddressPanelStyle(28, 6f);
                break;
            case ScreenRatio.One_Two:
                SetConnectPanelStyle(28, 2f);
                SetWelcomePanelStyle(1, new Vector2(450, 90), new Vector2(0, 30), 28, 2f);
                SetABIExampleButtonStyle(25,4f);
                SetAddressPanelStyle(28, 6f);
                break;
        }
        //Connect 

        //Welcome Panel (check)

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
            var txts = button.gameObject.GetComponentsInChildren<TMP_Text>();
            foreach (var txt in txts)
            {
                txt.color = buttonTextColor;
                txt.fontSize = fontSize;
            }
            //outlines
            EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();

            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { AddOutLine(button); });
            eventTrigger.triggers.Add(hoverEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { ClearOutline(button); });
            eventTrigger.triggers.Add(exitEntry);

        }

        //Chain Selector Dropdown
        var parentWidth = welcomePanelLayout.GetComponent<RectTransform>().rect.width;
        networkDropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(parentWidth / 3f, buttonSize.y/2f);
        
        //font and colors
        networkDropdown.GetComponent<Image>().color = buttonBackgroundColor;
        networkDropdown.captionText.fontSize = fontSize;
        networkDropdown.captionText.color = buttonTextColor;
        networkDropdown.itemText.fontSize = fontSize;
        networkDropdown.itemText.color = buttonTextColor;
        Toggle networkToggle = networkDropdown.template.GetComponentInChildren<Toggle>();
        networkToggle.GetComponentInChildren<TMP_Text>().fontSize = fontSize;
        networkToggle.GetComponentInChildren<TMP_Text>().color = buttonTextColor;
        ColorBlock toggleCB = networkToggle.colors;
        toggleCB.normalColor= buttonBackgroundColor;
        toggleCB.selectedColor= buttonHighlightColor;
        networkToggle.colors = toggleCB;

    }

    private void SetConnectPanelStyle(int fontSize, float roundCorner)
    {
        //Skip Sequence Connect Button
        foreach(Button button in connectButtons.Skip(1))
        {
            var btnImage = button.gameObject.GetComponent<Image>();
            btnImage.sprite = buttonSprite;
            btnImage.color = buttonBackgroundColor;
            btnImage.pixelsPerUnitMultiplier = roundCorner;
            var txts = button.gameObject.GetComponentsInChildren<TMP_Text>();
            foreach (var txt in txts)
            {
                txt.color = buttonTextColor;
                txt.fontSize = fontSize;
            }
        }
    }

    private void SetAddressPanelStyle(int fontSize, float roundCorner)
    {
        //Text:
        addressText.color = addressTextColor;
        //Button:
        var btnImage = addressBackButton.gameObject.GetComponent<Image>();
        btnImage.sprite = buttonSprite;
        btnImage.color = buttonBackgroundColor;
        btnImage.pixelsPerUnitMultiplier = roundCorner;
        var txt = addressBackButton.gameObject.GetComponentInChildren<TMP_Text>();
        txt.fontSize = fontSize;
        txt.color = buttonTextColor;
    }
    private void SetABIExampleButtonStyle(int fontSize, float roundCorner)
    {
        //ABI Examples
        foreach (var button in abiExampleButtons)
        {
            var btnImage = button.gameObject.GetComponent<Image>();
            btnImage.sprite = buttonSprite;
            btnImage.color = buttonBackgroundColor;
            btnImage.pixelsPerUnitMultiplier = roundCorner;
            var txt = button.gameObject.GetComponentInChildren<TMP_Text>();
            txt.fontSize = fontSize;
            txt.color = buttonTextColor;
        }
    }


    private void AddOutLine(Button button)
    {
        var outline = button.gameObject.GetComponent<Outline>();
        if (!outline)
        {
            outline = button.gameObject.AddComponent<Outline>();
        }
        outline.enabled = true;
        outline.effectColor = outlineColor;
    }

    private void ClearOutline(Button button)
    {
        var outline = button.gameObject.GetComponent<Outline>();
        outline.enabled = false;
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
