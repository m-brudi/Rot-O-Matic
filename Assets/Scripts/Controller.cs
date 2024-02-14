using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Controller : SingletonMonoBehaviour<Controller> {
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] PolygonCollider2D lineCollider;
    [SerializeField] float radius = 2;
    [SerializeField] GameObject marblePrefab;
    public AudioLowPassFilter filter;
    public AudioReverbFilter reverb;
    List<GameObject> marbles = new();

    public bool moreThanOneOctave;

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

    public System.Action<bool> CollisionsChanged;
    bool collisions;
    public bool Collisions {
        get { return collisions; }
        set {
            collisions = value;
            CollisionsChanged?.Invoke(collisions);
        }
    }
    public System.Action<bool> ImpactForceChanged;
    bool impactForce;
    public bool ImpactForce {
        get { return impactForce; }
        set {
            impactForce = value;
            ImpactForceChanged?.Invoke(impactForce);
        }
    }

    public System.Action<bool> TrailsChanged;
    bool trails;
    public bool Trails {
        get { return trails; }
        set {
            trails = value;
            reverb.enabled = value;
            TrailsChanged?.Invoke(trails);
        }
    }

    //octave zero by default
    [SerializeField]bool[] octaves = { false, true, false };
    public System.Action<bool[]> OctavesChanged;
    public bool[] Octaves {
        get {
            return octaves; }
        set {
            octaves = value;
        }
    }

    public System.Action Beat;
    [SerializeField] int bpm = 120;
    [SerializeField] int noteDivision = 16;

    public System.Action<int> NoteDivisionChanged;
    public int NoteDivision {
        get { return noteDivision; }
        set {
            noteDivision = value;
            secPerBeat = 60f / (BPM * noteDivision);
            NoteDivisionChanged?.Invoke(noteDivision);
        }
    }

    public int BPM {
        get { return bpm; }
        set {
            bpm = value;
            secPerBeat = 60f / (value * noteDivision);
        }
    }

    bool synchronize;
    public bool Synchronize {
        get { return synchronize; }
        set {
            synchronize = value;
        }
    }

    float beatTime;
    [SerializeField] float secPerBeat;
    Vector3[] vertices;

    void Start()
    {
        Time.timeScale = .8f;
        activeScale = 0;
        reverb.enabled = false;
        secPerBeat = 60f / (BPM * noteDivision);
        ZenMode(false);
#if UNITY_EDITOR
        //Cursor.SetCursor(PlayerSettings.defaultCursor, new(16,16), CursorMode.ForceSoftware);
#endif
    }

    private void Update() {
        lineRenderer.transform.Rotate(new Vector3(0, 0, RotateSpeed) * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        beatTime += Time.deltaTime;

        if(beatTime >= secPerBeat) {
            if(synchronize) Beat.Invoke();
            beatTime = 0;
        }

    }

    public void ZenMode(bool on) {
        if (on) {
            Camera.main.orthographicSize = 1.7f;
        } else {
            Camera.main.orthographicSize = 3f;
        }
    }

    public void ChangeOctave() {
        int i = 0;
        foreach (var item in octaves) {
            if (item) i++;
        }
        moreThanOneOctave = i > 1;

        OctavesChanged?.Invoke(octaves);
    }

    public void SpawnMarble() {
        if(marbles.Count < 5) {
            GameObject m = Instantiate(marblePrefab, Vector3.zero, Quaternion.identity);
            marbles.Add(m);
            Marbles = marbles.Count;
        }
    }

    public void FilterChanged(float value) {
        filter.cutoffFrequency = value;
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
