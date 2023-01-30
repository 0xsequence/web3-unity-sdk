using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestInputModule : StandaloneInputModule
{

    public Button testButton;
    Vector2 buttonPos;

    protected override void Start()
    {
        testButton.onClick.AddListener(TestButtonClicked);
        Camera cam = FindObjectOfType<Camera>();
        Vector3 screenPos = cam.WorldToScreenPoint(testButton.transform.position);
        buttonPos = screenPos;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ClickAt(buttonPos.x, buttonPos.y);
        }
    }
    void TestButtonClicked()
    {
        Debug.Log("Test Button Clicked");
    }
    public void ClickAt(float x, float y)
    {
        Input.simulateMouseWithTouches = true;
        var pointerData = GetTouchPointerEventData(new Touch()
        {
            position = new Vector2(x, y),
        }, out bool b, out bool bb);

        ProcessTouchPress(pointerData, true, true);
    }

    
}