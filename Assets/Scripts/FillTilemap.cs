using UnityEngine;
using UnityEngine.Tilemaps;

public class FillTilemap : MonoBehaviour
{
    public Tilemap tilemap; // Assign in the inspector
    public Tile tile; // Assign your custom Tile in the inspector

    public CoordinateGridManager gridManager;
    
    void Start()
    {
        FillTilemapWithTile();
    }

    void FillTilemapWithTile()
    {
        // adjust to match the exact grid size from CoordinateGridManager
        for (int x = 0; x < gridManager.gridSizeX; x++)
        {
            for (int y = 0; y < gridManager.gridSizeY; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0); // Assuming (0,0) is the start of the Tilemap
                tilemap.SetTile(position, tile);
            }
        }
    }
}
