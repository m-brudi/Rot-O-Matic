using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseToBeat : MonoBehaviour
{
    [SerializeField] float pulseSizeMultiplier = 1.15f;
    [SerializeField] float returnSpeed = 5;
    [SerializeField] bool pulseToEveryBeat = false;
    Vector3 startSize;
    int noteDivision;
    int numOfNotes = 0;
    void Start()
    {
        startSize = transform.localScale;
        Controller.Instance.Beat += Pulse;
        Controller.Instance.NoteDivisionChanged += SetupNoteDivision;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * returnSpeed);
    }

    void SetupNoteDivision(int division) {
        noteDivision = division;
        numOfNotes = 0;
    }

    void Pulse() {
        if (pulseToEveryBeat) transform.localScale = startSize * pulseSizeMultiplier;
        else {
            numOfNotes++;
            if (numOfNotes >= noteDivision) {
                transform.localScale = startSize * pulseSizeMultiplier;
                numOfNotes = 0;
            }
        }
    }
}
