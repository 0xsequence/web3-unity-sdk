using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public struct Point
{
    public int x;
    public int y;
}
public class ClickInWalletTest : StandaloneInputModule
{

    public Button testButton;
    public Canvas canvas;

    private IEnumerator coroutine;

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pos);

    // Start is called before the first frame update
    void Start()
    {
   
        Camera cam = FindObjectOfType<Camera>();
        Vector3 screenPos = cam.WorldToScreenPoint(testButton.transform.position);
        Debug.Log("target is at " + screenPos);
        testButton.onClick.AddListener(TestButtonClicked);

        StartCoroutine(MouseClicks());
        
    }
    private IEnumerator MouseClicks()
    {
        yield return new WaitForSeconds(1);
        Vector2 testPos = new Vector2(800, 500);//middle point of screen
        MouseOperations.SetCursorPosition((int)testPos.x, (int)testPos.y);
        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed primary button.");
    }


    private void TestButtonClicked()
    {
        Debug.Log("clicked" + Input.mousePosition);
    }

    /// <summary>
    /// Reference:
    /// https://stackoverflow.com/questions/2416748/how-do-you-simulate-mouse-click-in-c
    /// </summary>

    public class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            Debug.Log("cursor position" + currentMousePoint.X+","+ currentMousePoint.Y);
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();
            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }


}
