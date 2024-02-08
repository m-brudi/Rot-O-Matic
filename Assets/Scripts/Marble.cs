
using UnityEngine;

public class Marble : MonoBehaviour
{
    string collisionLayer ="CollisionLayer";
    string noCollisionLayer="NoCollisionLayer";
    Oscillator oscillator;
    TrailRenderer trail;
    bool impactForce;

    bool noteWaiting;
    float waitingVelocity = 1;
    void Start()
    {
        oscillator = GetComponent<Oscillator>();
        trail = GetComponent<TrailRenderer>();
        Controller.Instance.CollisionsChanged += SetCollisions;
        Controller.Instance.ImpactForceChanged += (value) => impactForce = value;
        Controller.Instance.TrailsChanged += SetTrail;
        Controller.Instance.OctavesChanged += SetupOctaves;
        Controller.Instance.Beat += Pulse;

        SetTrail(Controller.Instance.Trails);
        SetCollisions(Controller.Instance.Collisions);
        impactForce = Controller.Instance.ImpactForce;
        SetupOctaves(Controller.Instance.Octaves);
    }

    void Pulse() {
        if (noteWaiting) {
            oscillator.Play(waitingVelocity);
            noteWaiting = false;
        }
    }

    void SetupOctaves(bool[] octaves) {
        oscillator.SetOctaves(octaves);
    }

    void SetCollisions(bool c) {
        if (c) {
            gameObject.layer = LayerMask.NameToLayer(collisionLayer);
        } else {
            gameObject.layer = LayerMask.NameToLayer(noCollisionLayer);
        }
    }

    void SetTrail(bool t) {
        trail.enabled = t;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == 6) {
            Controller.Instance.RemoveSingleMarble(gameObject);
        } else {
            if (Controller.Instance.Synchronize) {
                noteWaiting = true;
                waitingVelocity = impactForce ? collision.relativeVelocity.magnitude : -1;
            } else {
                oscillator.Play(impactForce ? collision.relativeVelocity.magnitude : -1);
            }
        }
    }

    private void OnDisable() {
        if (Controller.Instance) {
            Controller.Instance.CollisionsChanged -= SetCollisions;
            Controller.Instance.ImpactForceChanged -= (value) => impactForce = value;
            Controller.Instance.TrailsChanged -= SetTrail;
            Controller.Instance.OctavesChanged -= SetupOctaves;
            Controller.Instance.Beat -= Pulse;
        }
    }
}
