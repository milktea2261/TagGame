using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//選項:提供功能，清除舊圖表產生的垃圾
//bug:子物件要生成在相對的位置上
public class RadarChart : MonoBehaviour
{
    public float chartSize = 10f;
    public float fontSize = 1f;
    public Color fontColor = Color.black;

    [Space()]
    public TextMeshPro text3D_Prefab;
    [SerializeField] MeshFilter meshFilter = null;
    [SerializeField] MeshFilter bgMeshFilter = null;
    [SerializeField] List<Transform> texts = new List<Transform>();
    [Space()]
    public ChartAttritube[] datas = new ChartAttritube[3];
    Vector3[] verteies = new Vector3[0];
    Vector3[] bgVerteies = new Vector3[0];
    int[] triangles = new int[0];

    private void Awake() {
        ClearChart();
    }

    public void CreateChart(ChartAttritube[] datas, bool showAttritubeName) {

        this.datas = datas;

        CreateBGChart(datas.Length);
        UpdateChart(datas);

        //顯示屬性名稱
        if(showAttritubeName) {
            ShowAttritubeName();
        }
    }

    void CreateBGChart(int attritibueCount) {
        Mesh bgMesh = new Mesh { name = "BG Chart" };

        bgVerteies = new Vector3[attritibueCount + 1];
        triangles = new int[attritibueCount * 3];

        bgVerteies[0] = Vector3.zero;
        for(int i = 0; i < attritibueCount; i++) {
            float angle = (360 / attritibueCount * i) * Mathf.Deg2Rad;
            bgVerteies[i + 1] = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * chartSize;
        }

        for(int i = 0; i < attritibueCount; i++) {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        triangles[triangles.Length - 1] = 1;//封閉圖形

        bgMesh.vertices = bgVerteies;
        bgMesh.triangles = triangles;
        bgMesh.RecalculateNormals();
        bgMeshFilter.mesh = bgMesh;
    }

    public void UpdateChart(ChartAttritube[] datas) {
        Mesh chartMesh = new Mesh {
            name = "Radar Chart"
        };

        verteies = new Vector3[datas.Length + 1];
        verteies[0] = Vector3.zero;
        for(int i = 0; i < datas.Length; i++) {
            float angle = (360 / datas.Length * i) * Mathf.Deg2Rad;
            verteies[i + 1] = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * datas[i].Percent * chartSize;
        }

        chartMesh.vertices = verteies;
        chartMesh.triangles = triangles;
        chartMesh.RecalculateNormals();
        meshFilter.mesh = chartMesh;
    }

    void ShowAttritubeName() {
        texts.Clear();
        for(int i = 0; i < datas.Length; i++) {
            TextMeshPro text3D = Instantiate(text3D_Prefab, transform);
            text3D.text = datas[i].attritubeName;
            text3D.fontSize = fontSize;
            text3D.color = fontColor;
            text3D.transform.localPosition = bgVerteies[i + 1] + new Vector3(0,0.5f,0);
            text3D.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            texts.Add(text3D.transform);
        }
    }

    [ContextMenu("Clear Chart")]
    public void ClearChart() {
        for(int i = texts.Count-1; i >= 0 ; i--) {
            DestroyImmediate(texts[i].gameObject);
        }
        texts.Clear();

        datas = new ChartAttritube[0];
        verteies = new Vector3[0];
        bgVerteies = new Vector3[0];
        triangles = new int[0];
    }
}

[System.Serializable]
public class ChartAttritube
{

    public string attritubeName = "Attritube";
    public float m_value = 0f;
    public Vector2 valueRange = new Vector2(0, 100f);

    public ChartAttritube(string attritubeName, float value, Vector2 valueRange) {
        this.attritubeName = attritubeName;
        m_value = value;
        this.valueRange = valueRange;
    }

    public float Percent { get { return (m_value - valueRange.x) / (valueRange.y - valueRange.x); } }


}