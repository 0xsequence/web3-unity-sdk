using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class DemoUIManager : MonoBehaviour
{
    enum ScreenRatio
    {
        Default,
        Two_One,
        OneHalf_One,
        One_One,
        One_OneHalf,
        One_Two
    }

    enum CollectionLayout
    {
        Vertical,
        Horizontal
    }
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
    public GameObject addressPanel;
    public TMP_Text addressText;
    public Button addressBackButton;


    //Collection Scroll
    [Header("Collection")]
    public Collection collection;
    public RectTransform collectionRect;
    public RectTransform collectionCatRect;
    public RectTransform collectionScrollRect;
    public GridLayoutGroup collectionCatLayoutGroup;
    public GridLayoutGroup collectionScrollLayoutGroup;
    public Button collectionBackButton;
    public Scrollbar collectionVerticalScrollBar;

    private int collectionCategoryGroupFontSize = 15;
    private int collectionCategoryFontSize = 10;
    private CollectionLayout collectionLayout;

    //History
    [Header("History")]
    public RectTransform historyRect;
    public GridLayoutGroup historyLayoutGroup;
    public Button historyBackButton;
    public Scrollbar historyVerticalScrollBar;

    private int historyTextFontSize = 25;
    private Vector2 historyUnitGridLayoutCellSize = new Vector2(150, 0);


    [Header("loading panel")]
    public GameObject loadingPanel;

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

    private ScreenRatio screenRatio = ScreenRatio.Default;
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
    public void SetStyle()
    {

        switch (screenRatio)
        {
            case ScreenRatio.Two_One:
                SetConnectPanelStyle(35, 4f);
                SetWelcomePanelStyle(3, new Vector2(200, 50), new Vector2(20, 25), 14, 4f);
                SetABIExampleButtonStyle(10, 6f);
                SetAddressPanelStyle(30);
                SetCollectionsStyle(CollectionLayout.Vertical);
                SetHistoryStyle(3, 12);

                break;
            case ScreenRatio.OneHalf_One:
                SetConnectPanelStyle(35, 4f);
                SetWelcomePanelStyle(2, new Vector2(300, 50), new Vector2(40, 25), 14, 4f);
                SetABIExampleButtonStyle(10, 6f);
                SetAddressPanelStyle(30);
                SetCollectionsStyle(CollectionLayout.Vertical);
                SetHistoryStyle(2, 18);

                break;
            case ScreenRatio.One_One:
                SetConnectPanelStyle(28, 4f);
                SetWelcomePanelStyle(2, new Vector2(300, 50), new Vector2(40, 25), 14, 4f);
                SetABIExampleButtonStyle(10, 6f);
                SetAddressPanelStyle(28);
                SetCollectionsStyle(CollectionLayout.Vertical);
                SetHistoryStyle(2, 15);
                break;
            case ScreenRatio.One_OneHalf:
                SetConnectPanelStyle(28, 2f);
                SetWelcomePanelStyle(1, new Vector2(450, 90), new Vector2(0, 30), 28, 2f);
                SetABIExampleButtonStyle(25, 4f);
                SetAddressPanelStyle(28);
                SetCollectionsStyle(CollectionLayout.Horizontal);
                SetHistoryStyle(1, 18);
                break;
            case ScreenRatio.One_Two:
                SetConnectPanelStyle(28, 2f);
                SetWelcomePanelStyle(1, new Vector2(450, 90), new Vector2(0, 30), 28, 2f);
                SetABIExampleButtonStyle(25, 4f);
                SetAddressPanelStyle(28);
                SetCollectionsStyle(CollectionLayout.Horizontal);
                SetHistoryStyle(1, 18);
                break;
        }

    }

    /// <summary>
    /// Show loading panel Gameobject
    /// </summary>
    public void ShowLoadingPanel()
    {
        loadingPanel.SetActive(true);
    }

    /// <summary>
    /// Hide loading panel Gameobject
    /// </summary>
    public void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    /// <summary>
    /// Set styles for Collection tokens.
    /// </summary>
    /// <param name="cat"></param>
    public void SetCollectionCategoryStyle(Category cat)
    {
        Button btn = cat.GetButton();
        btn.image.color = buttonBackgroundColor;

        TextMeshProUGUI label = cat.GetLabel();
        var labelRect = label.GetComponent<RectTransform>();
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        label.color = buttonTextColor;
        label.fontSize = collectionCategoryFontSize;

        //logo
        Image img = cat.GetImage();
        img.preserveAspect = true;
        var imgRect = img.GetComponent<RectTransform>();
        imgRect.pivot = new Vector2(0.5f, 0.5f);

        Vector2 parentSize = collectionScrollRect.rect.size;
        //TODO: Calculate widht or height base on collection style
        if (collectionLayout == CollectionLayout.Horizontal)
        {
            //portrait mode
            var side = parentSize.x / 4f;
            collectionScrollLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            collectionScrollLayoutGroup.constraintCount = 3;
            collectionScrollLayoutGroup.cellSize = new Vector2(side, side);

            labelRect.localPosition = new Vector2(0, -side / 4f);
            
            //imgRect.sizeDelta = new Vector2(side / 2f, side / 2f);
            imgRect.localPosition = new Vector2(0, side/5f);
        }
        else
        {
            //landscape mode
            var side = parentSize.x / 5f;
            collectionScrollLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            collectionScrollLayoutGroup.constraintCount = 3;
            collectionScrollLayoutGroup.cellSize = new Vector2(side, side);

            labelRect.localPosition = new Vector2(0, -side / 4f);

            imgRect.sizeDelta = new Vector2(side / 2f, side / 2f);
            imgRect.localPosition = new Vector2(0, side/5f);
        }

        
        


    }

    /// <summary>
    /// Set styles for Collection Group Buttons.
    /// </summary>
    /// <param name="group"></param>
    public void SetCollectionCategoryGroupStyle(CategoryGroup group)
    {

        Button btn = group.GetButton();
        btn.image.sprite = buttonSprite;
        btn.image.color = buttonBackgroundColor;
        SetOutlineEventForButton(btn);
        TextMeshProUGUI groupLabel = group.GetGroupLabel();
        groupLabel.color = buttonTextColor;
        groupLabel.fontSize = collectionCategoryGroupFontSize;

        Vector2 parentSize = collectionCatRect.rect.size;
        if (collectionLayout == CollectionLayout.Horizontal)
        {
            //portrait mode
            var side = parentSize.x / 5f;
            collectionCatLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            collectionCatLayoutGroup.constraintCount = 1;
            collectionCatLayoutGroup.cellSize = new Vector2(side, side);
        }
        else
        {
            //landscape mode
            var side = parentSize.y / 5f;
            collectionCatLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            collectionCatLayoutGroup.constraintCount = 1;
            collectionCatLayoutGroup.cellSize = new Vector2(side, side);

        }


    }

    /// <summary>
    /// Set styles for ViewHistory panel
    /// </summary>
    /// <param name="historyUnit"></param>
    public void SetHistoryUnitStyle(HistoryUnit historyUnit)
    {
        

        var img = historyUnit.GetComponent<Image>();
        img.color = buttonBackgroundColor;
        img.sprite = buttonSprite;
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = 1;

        var nameText = historyUnit.GetNameText();
        nameText.color = buttonTextColor;
        nameText.fontSize = historyTextFontSize;

        var timeText = historyUnit.GetTimeStampText();
        timeText.color = buttonTextColor;
        timeText.fontSize = historyTextFontSize;

        var countText = historyUnit.GetTokenCountText();
        countText.color = buttonTextColor;
        countText.fontSize = historyTextFontSize;

        var gridLayout = historyUnit.GetGridLayout();
        gridLayout.cellSize = historyUnitGridLayoutCellSize;

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
            SetOutlineEventForButton(button);

        }

        //Chain Selector Dropdown
        var parentWidth = welcomePanelLayout.GetComponent<RectTransform>().rect.width;
        networkDropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(parentWidth / 3f, buttonSize.y / 2f);

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
        toggleCB.normalColor = buttonBackgroundColor;
        toggleCB.selectedColor = buttonHighlightColor;
        networkToggle.colors = toggleCB;

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

    private void SetConnectPanelStyle(int fontSize, float roundCorner)
    {
        //Skip Sequence Connect Button
        foreach (Button button in connectButtons.Skip(1))
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

    private void SetAddressPanelStyle(int addressFontSize)
    {
        //Address Text:
        addressText.color = addressTextColor;
        addressText.fontSize = addressFontSize;
        //Back Button:
        Vector2 parentSize = addressPanel.GetComponent<RectTransform>().rect.size;
        SetBackButtonStyle(addressBackButton, parentSize);

        //outline
        SetOutlineEventForButton(addressBackButton);

    }


    private void SetCollectionsStyle(CollectionLayout layout)
    {
        
        collectionLayout = layout; //For button styles 
        Vector2 collectionPanelSize = collectionRect.sizeDelta;
        if (layout == CollectionLayout.Horizontal)
        {
            //Horizontal Layout
            HorizontalCollection(collectionPanelSize);
        }
        else
        {
            //Vertical Layout
            VerticalCollection(collectionPanelSize);
        }

    }

    private void HorizontalCollection(Vector2 parentSize)
    {
        var width = parentSize.x;
        var height = parentSize.y / 8f;
        //suitable for portrait layout
        //Group Layout

        collectionCatRect.sizeDelta = new Vector2(width, height);
        collectionCatRect.pivot = new Vector2(0.5f, 0.5f);
        collectionCatRect.localPosition = new Vector2(0, -height / 2f + parentSize.y / 2f);
        //Token Layout

        collectionScrollRect.sizeDelta = new Vector2(width, parentSize.y - height);
        collectionScrollRect.pivot = new Vector2(0.5f, 0.5f);
        collectionScrollRect.localPosition = new Vector2(0, -height / 2f);

        //Back Btn
        SetBackButtonStyle(collectionBackButton, parentSize);
        SetScrollBarStyle(collectionVerticalScrollBar,parentSize);

    }
    private void VerticalCollection(Vector2 parentSize)
    {
        var width = parentSize.x / 5f;
        var height = parentSize.y;
        //suitable for landscape layout

        collectionCatRect.sizeDelta = new Vector2(width, height);
        collectionCatRect.pivot = new Vector2(0.5f, 0.5f);
        collectionCatRect.localPosition = new Vector2(width / 2f - parentSize.x / 2f, 0);

        //Token Layout

        collectionScrollRect.sizeDelta = new Vector2(parentSize.x - width, height);
        collectionScrollRect.pivot = new Vector2(0.5f, 0.5f);
        collectionScrollRect.localPosition = new Vector2(width / 2, 0);

        //Back Btn

        SetBackButtonStyle(collectionBackButton, parentSize);
        SetScrollBarStyle(collectionVerticalScrollBar, parentSize);

    }

    private void SetHistoryStyle(int col, int fontSize)
    {
        historyTextFontSize = fontSize;
        historyLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        historyLayoutGroup.constraintCount = col;
        
        if(col == 2)
        {
            historyLayoutGroup.cellSize = new Vector2(300, 100);
            historyLayoutGroup.spacing = new Vector2(40, 20);
            historyUnitGridLayoutCellSize = new Vector2(80,0);

        }
        else if(col == 3)
        {
            
            historyLayoutGroup.cellSize = new Vector2(200, 100);
            historyLayoutGroup.spacing = new Vector2(20, 20);
            historyUnitGridLayoutCellSize = new Vector2(50, 0);
        }
        else
        {
            historyLayoutGroup.cellSize = new Vector2(600, 120);
            historyLayoutGroup.spacing = new Vector2(0, 20);
            historyUnitGridLayoutCellSize = new Vector2(150, 0);
        }
        SetBackButtonStyle(historyBackButton, historyRect.sizeDelta);
        SetScrollBarStyle(historyVerticalScrollBar,historyRect.sizeDelta);

    }




    private void SetOutlineEventForButton(Button button)
    {
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();

        if (!eventTrigger)
        {
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { AddOutLine(button); });
            eventTrigger.triggers.Add(hoverEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { ClearOutline(button); });
            eventTrigger.triggers.Add(exitEntry);

            EventTrigger.Entry pointEntry = new EventTrigger.Entry();
            pointEntry.eventID = EventTriggerType.PointerClick;
            pointEntry.callback.AddListener((data) => { ClearOutline(button); });
            eventTrigger.triggers.Add(pointEntry);
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


    private void SetBackButtonStyle(Button backButton,Vector2 panelSize)
    {
        RectTransform btnRect = backButton.GetComponent<RectTransform>();
        Vector2 btnSize = new Vector2(panelSize.x / 8f, panelSize.x / 16f);
        btnRect.sizeDelta = btnSize;
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.localPosition = new Vector2(panelSize.x / 2f - btnSize.x / 2f, panelSize.y / 2f - btnSize.y / 2f);
        backButton.image.color = buttonBackgroundColor;
        var btnText = backButton.GetComponentInChildren<TextMeshProUGUI>();
        btnText.fontSize = collectionCategoryGroupFontSize;
        btnText.color = buttonTextColor;
    }

    private void SetScrollBarStyle(Scrollbar scrollbar, Vector2 parentSize)
    {

        scrollbar.image.color = buttonHighlightColor;
        scrollbar.gameObject.GetComponent<Image>().color = buttonBackgroundColor;
        scrollbar.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(parentSize.x / 20, parentSize.y);
    }


    private ScreenRatio GetScreenRatio()
    {

        if (camera.aspect > 1.7)
        {
            return ScreenRatio.Two_One;
        }
        else if (camera.aspect > 1.2)
        {
            return ScreenRatio.OneHalf_One;
        }
        else if (camera.aspect > 0.9)
        {
            return ScreenRatio.One_One;
        }
        else if (camera.aspect > 0.5)
        {
            return ScreenRatio.One_OneHalf;
        }
        else
        {
            return ScreenRatio.One_Two;
        }
    }



}
