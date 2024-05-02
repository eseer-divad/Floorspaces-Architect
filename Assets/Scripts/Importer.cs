using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class Importer : MonoBehaviour
{
    public List<List<Vector2>> Points = new List<List<Vector2>>();
    public string ShapeData = "";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetLines());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetLines()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://floorspaces.azurewebsites.net/api/Floorspaces/DownloadPoints/" + "4/");
        www.certificateHandler = new CertOverwrite();
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("Network Error Getting Points");
        }
        else
        {
            var pointmaker = GameObject.Find("Drawing Layer").GetComponent<CoordinateGridPen>();
            ShapeData += www.downloadHandler.text;
            string[] SplitPoints = ShapeData.Split('|');
            Points.Add(new List<Vector2>());

            foreach (var Line in SplitPoints)
            {
                if (Line == "}")
                {
                    pointmaker.FinishLine();
                    continue;
                }
                else if (Line == "{")
                {
                    pointmaker.lineRenderer = null;
                }
                else
                {
                    int yIndex = Line.IndexOf("Y");
                    int zIndex = Line.IndexOf("Z");

                    if (yIndex != -1 && zIndex != -1 && yIndex < zIndex)
                    {
                        var X = float.Parse(Line.Substring(1, yIndex - 1));
                        var Y = float.Parse(Line.Substring(yIndex + 1, zIndex - yIndex - 1));
                        var point = new Vector2(X, Y);
                        pointmaker.AddLine(point);
                    }
                    else
                    {
                        Debug.LogWarning("Invalid line format: " + Line);
                    }
                }
            }
        }
    }

    public class CertOverwrite : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
