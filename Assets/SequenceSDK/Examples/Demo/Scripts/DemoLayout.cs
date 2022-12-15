using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DemoLayout : LayoutGroup
{
    public enum Fit
    {
        Default,
        FixedRows,
        FixedColumns
    }
    public Fit fit;
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;

    public override void CalculateLayoutInputVertical()
    {
        base.CalculateLayoutInputHorizontal();

        if (fit == Fit.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }
        else if (fit == Fit.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }
        else
        {
            float sqt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqt);
            columns = Mathf.CeilToInt(sqt);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / (float)columns - (spacing.x / (float)columns * 2) - padding.left / (float)columns - padding.right / (float)columns;
        float cellHeight = parentHeight / (float)rows - (spacing.y / (float)rows * 2) - padding.top / (float)rows - padding.bottom / (float)rows;

        cellSize.x = cellWidth;
        cellSize.y = cellHeight;

        int columnIdx;
        int rowIdx;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowIdx = i / columns;
            columnIdx = i % columns;

            var curr = rectChildren[i];

            var xPos = (cellSize.x * columnIdx) + (spacing.x * columnIdx) + padding.left;
            var yPos = (cellSize.y * rowIdx) + (spacing.y * rowIdx) + padding.top;

            SetChildAlongAxis(curr, 0, xPos, cellSize.x);
            SetChildAlongAxis(curr, 1, yPos, cellSize.y);

        }

    }

    public override void SetLayoutHorizontal()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        //throw new System.NotImplementedException();
    }
}
