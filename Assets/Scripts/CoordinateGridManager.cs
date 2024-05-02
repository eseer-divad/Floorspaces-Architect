using UnityEngine;
using UnityEngine.Tilemaps;

public class CoordinateGridManager : MonoBehaviour
{
    public int gridSizeX = 100;
    public int gridSizeY = 100;
    public float cellSizeX = 1f; // set cell width
    public float cellSizeY = 1f; // set cell height

    public Vector2[,] gridPositions;

    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // populate grid with coordinates
        gridPositions = new Vector2[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                gridPositions[x, y] = new Vector2(x * cellSizeX, y * cellSizeY);
            }
        }
    }
}
