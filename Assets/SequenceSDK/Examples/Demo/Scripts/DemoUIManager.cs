using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoUIManager : MonoBehaviour
{
    //Welcome Panel
    [Header("Welcome Panel")]
    public GridLayoutGroup welcomePanelLayout;
    public Camera camera;

    //Collection Scroll
    [Header("Collection")]
    public RectTransform collectionRect;
    public RectTransform collectionCatRect;
    public RectTransform collectionScrollRect;
    public GridLayoutGroup collectionCatLayout;
    public GridLayoutGroup collectionScrollLayout;

    private void Start()
    {
        AdjustWelcomePanelLayoutGroup();
        float height = collectionRect.sizeDelta.y;
        AdjustCollectionScrollRect(height);
    }

    public void EnableCollectionPanel()
    {
        
    }
    private void AdjustWelcomePanelLayoutGroup()
    {
        Debug.Log(camera.aspect);
        if(camera.aspect > 1)
        {
            //more landscape
            welcomePanelLayout.constraintCount = 2;
            welcomePanelLayout.cellSize = new Vector2(300, 50);
            welcomePanelLayout.spacing = new Vector2(50, 25);


        }
        else
        {
            //more portrait, stay current setup
        }
        
    }

    private void AdjustCollectionScrollRect(float height)
    {
        float categoryHeight = height / 5.0f;
        float scrollHeight = height * 4.0f / 5.0f;
        collectionCatRect.sizeDelta = new Vector2(collectionCatRect.sizeDelta.x,categoryHeight);

        collectionScrollRect.anchoredPosition = new Vector2(collectionScrollRect.transform.position.x, -categoryHeight);
        collectionScrollRect.sizeDelta = new Vector2(collectionScrollRect.sizeDelta.x,scrollHeight);

        collectionCatLayout.cellSize = new Vector2(categoryHeight , categoryHeight );
        //collectionScrollLayout.cellSize = new Vector2(categoryHeight, categoryHeight);
    }
}
