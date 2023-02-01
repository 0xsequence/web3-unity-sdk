using UnityEngine;
using UnityEngine.EventSystems;

public struct MouseInput
{
    public float X;
    public float Y;
}
public class SequenceInputModule : StandaloneInputModule
{
    public bool rejectBtnIsRecorded;
    public bool confirmBtnIsRecorded;
    MouseInput _walletRejectButtonPosition;
    MouseInput _walletConfirmButtonPosition;
    
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
        Debug.Log("Clicking At: " + _walletRejectButtonPosition.X + ", " + _walletRejectButtonPosition.Y);
        ClickAt(_walletRejectButtonPosition.X, _walletRejectButtonPosition.Y);
    }
    public void ClickWalletConfirmButton()
    {
        ClickAt(_walletConfirmButtonPosition.X, _walletConfirmButtonPosition.Y);
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
        _walletRejectButtonPosition.Y = record.y;
        _walletRejectButtonPosition.X = record.x;
    }

    public void RecordWalletConfirmButtonPosition(Vector3 record)
    {
        _walletConfirmButtonPosition.X = record.x;
        _walletConfirmButtonPosition.Y = record.y;
    }

}