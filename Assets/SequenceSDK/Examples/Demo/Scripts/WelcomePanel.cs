using UnityEngine;
using UnityEngine.UI;

public class WelcomePanel : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public Camera camera;

    private void Start()
    {
        AdjustLayoutGroup();
    }
    private void AdjustLayoutGroup()
    {
        Debug.Log(camera.aspect);
        if (camera.aspect > 1)
        {
            //more landscape
            gridLayoutGroup.constraintCount = 2;
            gridLayoutGroup.cellSize = new Vector2(300, 50);
            gridLayoutGroup.spacing = new Vector2(50, 25);


        }
        else
        {
            //more portrait, stay current setup
        }

    }
}
