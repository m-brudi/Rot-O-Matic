using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace Workaround {

public class Oscillator : SingletonMonoBehaviour<Oscillator> {
    private double frequency;
    private double increment;
    private double phase;
    private double sampleRate;

    [SerializeField] float gain;
    [SerializeField] float maxVolume;
    [SerializeField] float decay;

    float[] frequencies = { 261.63f, 277.18f, 293.66f, 311.13f, 329.63f, 349.23f, 369.99f, 392.00f, 415.30f, 440.00f, 466.16f, 493.88f };
    static int[] major = {0,2,4,5,7,9,11};
    static int[] minor = {0,2,3,5,7,8,10};
    static int[] pentatonic = {0,2,4,7,9};
    static int[] phrygian = {0,1,3,5,7,8,10};
    static int[] lydian = {0,2,4,6,7,9,11};

    List<int[]> scales = new List<int[]>() { major, minor, pentatonic, phrygian, lydian };

    /*
        1. C4: 261.63f
        2. C#4/Db4: 277.18f
        3. D4: 293.66f 
        4. D#4/Eb4: 311.13f
        5. E4: 329.63f 
        6. F4: 349.23f
        7. F#4/Gb4: 369.99f
        8. G4: 392.00f 
        9. G#4/Ab4: 415.30f
        10. A4: 440.00f 
        11. A#4/Bb4: 466.16f
        12. B4: 493.88f
    */

    bool play;
    float timer;
    float volume;
    bool moreThanOneOctave;

    [SerializeField] bool octaveDown;
    [SerializeField] bool octaveZero;
    [SerializeField] bool octaveUp;

    private void Awake() {
        sampleRate = AudioSettings.outputSampleRate;
    }

    private void Update() {
        if (play) {
            while (gain < volume) gain += 0.1f * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer > decay) {
                timer = 0;
                while (gain > 0) {
                    gain -= Time.deltaTime; 
                }
                play = false;
            }
        }
    }

    public void SetOctaves(bool[] octaves) {
        octaveDown = octaves[0];
        octaveZero = octaves[1];
        octaveUp = octaves[2];
        moreThanOneOctave = Controller.Instance.moreThanOneOctave;
    }


    public void Play(float force) {
        int scaleInd = Controller.Instance.ActiveScale;
        gain = 0;
        int[] scale = scales[scaleInd];
        int note = scale[Random.Range(0, scale.Length)];

        if (force == -1) volume = maxVolume;
        else volume = Map(force, 1, 4, 0.05f, maxVolume);

        frequency = frequencies[note];
        float octaveMultiplier = OctaveZeroCheck(OctaveDownCheck(OctaveUpCheck(1)));
        frequency *= octaveMultiplier;
        play = true;

    }

    float OctaveZeroCheck(float value) {
        if (octaveZero) return Random.value > .5f ? value : 1;
        else return value;
    }
    float OctaveDownCheck(float value) {
        if (octaveDown) {
            if (moreThanOneOctave)
                return Random.value > .5f ? value : .5f;
            else
                return .5f;
        } else return value;
    }
    float OctaveUpCheck(float value) {
        if (octaveUp) {
            if (moreThanOneOctave)
                return Random.value > .5f ? value : 2;
            else return 2;
        } else return value;
    }


    private void OnAudioFilterRead(float[] data, int channels) {
        increment = frequency * 2.0 * Mathf.PI / sampleRate;
        for (int i = 0; i < data.Length; i += channels) {
            phase += increment;

            data[i] = SquareWave(phase) - SineWave(phase);

            if (channels == 2) {
                data[i + 1] = data[i];
            }
            if (phase > (Mathf.PI * 2)) phase -= Mathf.PI * 2;
        }

    }

    float Map(float value, float oldMin, float oldMax, float newMin, float newMax) {
        return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
    }

    float SineWave(double phase) {
        return (float)(gain * Mathf.Sin((float)phase));
    }
    float SawToothWave(double phase) {
        return (float)(gain * phase * 2.0f - 1.0f);
    }

    float TriangleWave(double phase) {
        return (float)(gain * Mathf.PingPong((float)phase, 1.0f));
    }

    float SquareWave(double phase) {
        if (gain * Mathf.Sin((float)phase) >= 0 * gain) {
            return (float)gain * .6f;
        } else {
            return (-(float)gain) * .6f;
        }
    }

}
//}

