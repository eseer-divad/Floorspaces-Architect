using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ShapeScriptzExporter : MonoBehaviour
{
    public GameObject Drawing_Layer;
    public string data = "";

    private void Start()
    {
        Drawing_Layer = GameObject.Find("Drawing Layer");
    }

    public void GenerateScripts()
    {
        Debug.Log("Saving Points");
        List<Vector3[]> PointLines = Drawing_Layer.GetComponent<CoordinateGridPen>().ShapePointList;
        for (int i = 0; i != PointLines.Count; ++i)
        {
            for (int j = 0; j != PointLines.Count; ++j)
            {
                if (i != j && PointLines[j][0] == PointLines[i][PointLines[i].Length - 1])
                {
                    PointLines[i] = PointLines[i].Concat(PointLines[j]).ToArray();
                    PointLines.RemoveAt(j);
                    i = 0;
                    j = 0;
                }
            }
        }

        for (int i = 0; i != PointLines.Count; ++i)
        {
            if (!PointLoops(PointLines[i], PointLines[i][0]) || PointLines[i].Count() == 1)
            {
                Debug.Log("Non Closing Shape Found and Removed");
                PointLines.RemoveAt(i);
            }
            else
            {
                Debug.Log("Shape found and saved sucessfully");
            }
        }
        StartCoroutine(SavePoints(PointLines));
        //StartCoroutine(SendPoints(PointLines));
    }

    // Will not work with new changes, update to use lineData/PointLines
    /*
    public IEnumerator SendPoints(List<Vector3[]> PointLines)
    {
        foreach (var shape in PointLines)
        {
            data += "{\n";
            foreach(var point in shape)
            {
                data += "X" + point.x + "Y" + point.y + "Z" + point.z + "\n";
            }
            data += "}\n";
        }
        UnityWebRequest www = UnityWebRequest.Get("https://floorspaces.azurewebsites.net/api/Floorspaces/SendPoints/" + "4/" + data);
        www.certificateHandler = new CertOverwrite();
        yield return www.SendWebRequest();
    }
    */

    public IEnumerator SavePoints(List<Vector3[]> PointLines)
    {
        foreach (var shape in PointLines)
        {
            data += "{|";
            foreach (var point in shape)
            {
                data += "X" + point.x + "Y" + point.y + "Z" + point.z + "|";
            }
            data += "}|";
        }
        UnityWebRequest www = UnityWebRequest.Get("https://floorspaces.azurewebsites.net/api/Floorspaces/SavePoints/" + "4/" + data);
        www.certificateHandler = new CertOverwrite();
        yield return www.SendWebRequest();
    }

    public class CertOverwrite : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    public bool PointLoops(Vector3[] List, Vector3 value)
    {
        for (int i = 0; i != List.Length; i++)
        {
            if (List[i] == value && i == 0)
                continue;
            else if (List[i] == value && i != 0)
                return true;
        }
        return false;
    }

    public void ClearShapes()
    {
        StartCoroutine(DeletePoints());
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        data = "";
    }

    public IEnumerator DeletePoints()
    {
        Debug.Log("DeletePoints Coroutine Started");
        UnityWebRequest www = UnityWebRequest.Delete($"https://floorspaces.azurewebsites.net/api/Floorspaces/DeletePoints/" + "4");
        www.certificateHandler = new CertOverwrite();
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Points deleted successfully.");
        }
        else
        {
            Debug.LogError($"Error deleting points: {www.error}");
        }
    }
}
