using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//選項:提供功能，清除舊圖表產生的垃圾
//bug:子物件要生成在相對的位置上
public class RadarChart : MonoBehaviour
{
    public float chartSize = 10f;

    public bool showAttritubeName = false;
    public float fontSize = 1f;
    public Attritube[] datas = new Attritube[3];
    [Space()]
    public TextMesh text3D;
    [SerializeField] MeshFilter meshFilter = null;
    [SerializeField] MeshFilter bgmeshFilter = null;
    [SerializeField] List<Transform> texts = new List<Transform>();

    Vector3[] verteies = new Vector3[0];
    Vector3[] bgVerteies = new Vector3[0];
    int[] triangles = new int[0];

    private void Start() {
        CreateChart();
    }

    [ContextMenu("Create Chart")]
    public void CreateChart() {
        Mesh chartMesh = new Mesh();
        chartMesh.name = "Radar Chart";

        verteies = new Vector3[datas.Length + 1];
        triangles = new int[datas.Length * 3];

        verteies[0] = Vector3.zero;
        for(int i = 0; i < datas.Length; i++) {
            float angle = (360/ datas.Length * i) * Mathf.Deg2Rad;
            verteies[i + 1] = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * datas[i].Percent * chartSize;
        }

        for(int i = 0; i < datas.Length; i++) {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i+1;
            triangles[i * 3 + 2] = i+2;
        }
        triangles[triangles.Length -1] = 1;//封閉圖形

        chartMesh.vertices = verteies;
        chartMesh.triangles = triangles;
        chartMesh.RecalculateNormals();
        meshFilter.mesh = chartMesh;

        CreateBGChart();
        //顯示屬性名稱
        if(showAttritubeName) {
            ShowAttritubeName();
        }
    }

    void CreateBGChart() {
        Mesh bgMesh = new Mesh();
        bgMesh.name = "BG Chart";

        bgVerteies = new Vector3[verteies.Length];

        bgVerteies[0] = Vector3.zero;
        for(int i = 0; i < datas.Length; i++) {
            float angle = (360 / datas.Length * i) * Mathf.Deg2Rad;
            bgVerteies[i + 1] = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * chartSize;
        }

        bgMesh.vertices = bgVerteies;
        bgMesh.triangles = triangles;
        bgMesh.RecalculateNormals();
        bgmeshFilter.mesh = bgMesh;

    }

    void ShowAttritubeName() {
        texts.Clear();
        for(int i = 0; i < datas.Length; i++) {
            TextMesh textMesh = Instantiate(text3D, transform);
            textMesh.text = datas[i].attritubeName;
            textMesh.characterSize = fontSize;
            textMesh.transform.localPosition = bgVerteies[i + 1];
            texts.Add(textMesh.transform);
        }

    }

    [ContextMenu("Clear Chart")]
    public void ClearChart() {
        for(int i = texts.Count-1; i >= 0 ; i--) {
            DestroyImmediate(texts[i].gameObject);
        }
        texts.Clear();

    }
}

[System.Serializable]
public class Attritube {

    public string attritubeName = "Attritube";
    public float m_value = 0f;
    public Vector2 valueRange = new Vector2(0, 100f);

    public float Percent { get { return (m_value - valueRange.x) / (valueRange.y - valueRange.x); } }
}