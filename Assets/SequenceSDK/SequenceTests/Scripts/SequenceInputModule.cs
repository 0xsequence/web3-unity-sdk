using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public struct MouseInput
{
    public float X;
    public float Y;
}
public class SequenceInputModule : StandaloneInputModule
{
    MouseInput walletRejectButtonPosition;
    MouseInput walletConfirmButtonPosition;
    
    protected override void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            RecordWalletRejectButtonPosition(Input.mousePosition);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            RecordWalletConfirmButtonPosition(Input.mousePosition);
        }

    }
    
    public void ClickWalletRejectButton()
    {
        ClickAt(walletRejectButtonPosition.X, walletRejectButtonPosition.Y);
    }
    public void ClickWalletConfirmButton()
    {
        ClickAt(walletConfirmButtonPosition.X, walletConfirmButtonPosition.Y);
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

    public void RecordWalletRejectButtonPosition(Vector3 record)
    {
        walletRejectButtonPosition.X = record.x;
        walletRejectButtonPosition.Y = record.y;
    }

    public void RecordWalletConfirmButtonPosition(Vector3 record)
    {
        walletConfirmButtonPosition.X = record.x;
        walletConfirmButtonPosition.Y = record.y;
    }

}