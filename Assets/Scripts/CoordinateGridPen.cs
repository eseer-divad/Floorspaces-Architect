using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoordinateGridPen : MonoBehaviour
{
    private bool isDrawing;
    private GameObject currentLine;
    public LineRenderer lineRenderer;
    public CoordinateGridManager gridManager;
    public Material lineMaterial; // Set this in the Unity Editor
    public List<Vector3[]> ShapePointList = new List<Vector3[]>();
    public EventSystem ButtonEvents;

    void Start()
    {
        ButtonEvents = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    void Update()
    {
        if (!ButtonEvents.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartNewLine();
            }
            else if (Input.GetMouseButton(0) && isDrawing)
            {
                // Rather than call every frame check if it is close enough to a point to snap to.
                // Taking this method ensures that we can draw diag lines because if we where to move the mouse to make one
                // if we get the sensativity right (AKA how close to a point you need in order to snap to it)
                // then we can draw diag lines slowly without triggering the other corners
                AddPointToLine();
            }
            else if (Input.GetMouseButtonUp(0) && isDrawing)
            {
                FinishLine();
            }
        }
    }

    void StartNewLine()
    {
        isDrawing = true;
        currentLine = new GameObject("Line");
        lineRenderer = currentLine.AddComponent<LineRenderer>();

        lineRenderer.material = lineMaterial; // Use the pre-assigned material
        lineRenderer.sortingLayerName = "Drawing Layer";
        lineRenderer.sortingOrder = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // Reset position count for a new line
    }

    public void AddLine(Vector3 position)
    {
        if (!lineRenderer)
        {
            StartNewLine();
        }
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
    }

    /*   code to erase lines
     *   requires lineData structure
    void EraseLine()
    {
        // get the mouse postion
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // for each line object in the ShapePointList
        // if the mouse is in some threshold of a line
        // remove the object from ShapePointList
        foreach (var lineData in ShapePointList)
        {
            for (int i = 0; i < lineData.points.Length; i++)
            {
                if (Vector3.Distance(lineData.points[i], mousePos) < someThreshold)
                {
                    List<Vector3> pointsList = lineData.points.ToList();
                    pointsList.RemoveAt(i);
                    lineData.points = pointsList.ToArray();

                    // Update the LineRenderer
                    lineData.renderer.positionCount = lineData.points.Length;
                    lineData.renderer.SetPositions(lineData.points);

                    break;
                }
            }
        }
    }*/

    void AddPointToLine()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Align with the 2D plane
        Vector2Int gridPoint = GetCellCoordinatesFromWorldPosition(mousePos);

        Vector3 gridWorldPosition = new Vector3(
            gridPoint.x * gridManager.cellSizeX + gridManager.cellSizeX / 2,
            gridPoint.y * gridManager.cellSizeY + gridManager.cellSizeY / 2,
            0);

        // Only add if different from the last position
        if (lineRenderer.positionCount == 0 || lineRenderer.GetPosition(lineRenderer.positionCount - 1) != gridWorldPosition)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, gridWorldPosition);

            // Logging the added point
            //Debug.Log("Added point: (" + gridWorldPosition.x + ", " + gridWorldPosition.y + ")");
        }
    }

    public void FinishLine()
    {
        var result = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(result);
        ShapePointList.Add(result);
        isDrawing = false;
    }

    Vector2Int GetCellCoordinatesFromWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridManager.cellSizeX);
        int y = Mathf.FloorToInt(worldPosition.y / gridManager.cellSizeY);
        return new Vector2Int(x, y);
    }
}
