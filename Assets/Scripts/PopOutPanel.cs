using UnityEngine;
using UnityEngine.UI;

public class PopOutPanel : MonoBehaviour
{
    public GameObject Panel;
    public GameObject ClearButton;
    public GameObject SaveButton;
    private ShapeScriptzExporter ShapeExporter;

    public bool extended = false;

    private void Start()
    {
        // Assign the ShapeScriptzExporter component reference
        ShapeExporter = GetComponent<ShapeScriptzExporter>();

        // Add button click listeners
        if (SaveButton != null && SaveButton.GetComponent<Button>() != null)
        {
            SaveButton.GetComponent<Button>().onClick.AddListener(TogglePanel);
        }

        if (ClearButton != null && ClearButton.GetComponent<Button>() != null)
        {
            ClearButton.GetComponent<Button>().onClick.AddListener(() => ClearShapes());
        }
    }

    private void ClearShapes()
    {
        if (ShapeExporter != null)
        {
            // Call ClearShapes method
            ShapeExporter.ClearShapes();
        }
    }

    // translates the panel
    // buttons must be pre-translated to align with one another
    public void TogglePanel()
    {
        if (!extended)
        {
            Panel.transform.position = new Vector3(Panel.transform.position.x - 200, Panel.transform.position.y);
            SaveButton.transform.position = new Vector3(SaveButton.transform.position.x - 200, SaveButton.transform.position.y);
            ClearButton.transform.position = new Vector3(ClearButton.transform.position.x - 200, ClearButton.transform.position.y);
        }
        else
        {
            Panel.transform.position = new Vector3(Panel.transform.position.x + 200, Panel.transform.position.y);
            SaveButton.transform.position = new Vector3(SaveButton.transform.position.x + 200, SaveButton.transform.position.y);
            ClearButton.transform.position = new Vector3(ClearButton.transform.position.x + 200, ClearButton.transform.position.y);
        }

        extended = !extended;
    }
}