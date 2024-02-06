using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : SingletonMonoBehaviour<Controller> {
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] PolygonCollider2D lineCollider;
    [SerializeField] float radius = 2;
    [SerializeField] GameObject marblePrefab;

    List<GameObject> marbles = new();

    int sides;
    float rotateSpeed;
    public float RotateSpeed {
        get { return rotateSpeed; }
        set { rotateSpeed = value; }
    }
    int activeScale;
    public int ActiveScale {
        get { return activeScale; }
        set {
            activeScale = value;
        }
    }
    public System.Action<int> MarblesCountChanged;
    int Marbles {
        get { return marbles.Count; }
        set {
            MarblesCountChanged.Invoke(marbles.Count);
        }
    }
    Vector3[] vertices;

    void Start()
    {
        Time.timeScale = .8f;
        activeScale = 0;
    }

    private void Update() {
        lineRenderer.transform.Rotate(new Vector3(0, 0, RotateSpeed) * Time.deltaTime);
    }

    public void SpawnMarble() {
        if(marbles.Count < 5) {
            GameObject m = Instantiate(marblePrefab, Vector3.zero, Quaternion.identity);
            marbles.Add(m);
            Marbles = marbles.Count;
        }
    }

    public void SidesChanged(int value) {
        sides = value;
        DrawPolygon();
    }

    public void RemoveMarbles() {
        foreach (var item in marbles) {
            Destroy(item);
        }
        marbles.Clear();
        Marbles = marbles.Count;
    }
    public void RemoveSingleMarble(GameObject marble) {
        marbles.Remove(marble);
        Marbles = marbles.Count;
        Destroy(marble);
    }

    void DrawPolygon() {
        lineCollider.transform.rotation = Quaternion.Euler(Vector3.zero);
        lineRenderer.positionCount = sides;
        float tau = 2 * Mathf.PI;

        for (int i = 0; i < sides; i++) {
            float currentRadian = ((float)i / sides) * tau;
            float x = Mathf.Sin(currentRadian) * radius;
            float y = Mathf.Cos(currentRadian) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
        SetupCollider();
    }
    

    void SetupCollider() {
        Mesh mesh = new ();
        lineRenderer.BakeMesh(mesh, true);
        vertices = mesh.vertices;
        List<Vector2> points = new();
        for (int i = 0; i < vertices.Length; i+=2) {
            points.Add(new(vertices[i].x, vertices[i].y));
        }
        for (int i = 1; i < vertices.Length; i += 2) {
            points.Add(new(vertices[i].x, vertices[i].y));
        }
        lineCollider.SetPath(0, points);
    }
}
