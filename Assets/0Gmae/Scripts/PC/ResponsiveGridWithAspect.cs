using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGridWithRatio : MonoBehaviour
{
    [Header("Setting")]
    public Vector2 aspectRatio = new Vector2(1920, 1080);
    public Vector2 minCellSize = new Vector2(100, 100);

    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    void OnEnable() => UpdateGrid();

#if UNITY_EDITOR
    void OnValidate()
    {
        UpdateGrid();
    }
#endif

    float currentChildCount;
    private void Update()
    {
        if (currentChildCount == transform.childCount) return;
        currentChildCount = transform.childCount;
        UpdateGrid();
    }

    void UpdateGrid()
    {
        if (grid == null) grid = GetComponent<GridLayoutGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (grid == null || rectTransform == null) return;

        int childCount = transform.childCount;
        if (childCount == 0)
            return;

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float ar = aspectRatio.x / aspectRatio.y;
        float spacingX = grid.spacing.x;
        float spacingY = grid.spacing.y;

        if (childCount == 1)
        {
            float widthFromWidth = parentWidth;
            float heightFromWidth = widthFromWidth / ar;

            float heightFromHeight = parentHeight;
            float widthFromHeight = heightFromHeight * ar;

            float finalWidth, finalHeight;

            if (heightFromWidth <= parentHeight)
            {
                finalWidth = widthFromWidth;
                finalHeight = heightFromWidth;
            }
            else
            {
                finalWidth = widthFromHeight;
                finalHeight = heightFromHeight;
            }

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 1;
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.cellSize = new Vector2(finalWidth, finalHeight);
            return;
        }

        int bestCol = 1;
        float bestCellSize = 0f;

        for (int col = 1; col <= childCount; col++)
        {
            int row = Mathf.CeilToInt((float)childCount / col);

            float totalSpacingX = spacingX * (col - 1);
            float cellWidth = (parentWidth - totalSpacingX) / col;
            float cellHeight = cellWidth / ar;

            float totalHeight = (cellHeight * row) + spacingY * (row - 1);

            if (totalHeight > parentHeight)
                continue;

            if (cellWidth < minCellSize.x || cellHeight < minCellSize.y)
                continue;

            if (cellWidth > bestCellSize)
            {
                bestCellSize = cellWidth;
                bestCol = col;
            }
        }

        if (bestCellSize == 0)
        {
            bestCol = 1;
            bestCellSize = minCellSize.x;
        }

        float finalCellWidth = bestCellSize;
        float finalCellHeight = Mathf.Max(bestCellSize / ar, minCellSize.y);

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = bestCol;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.cellSize = new Vector2(finalCellWidth, finalCellHeight);
    }

}
