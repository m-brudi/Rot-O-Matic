using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workaround {

    public class Oscillator : SingletonMonoBehaviour<Oscillator> {
        private double frequency;
        private double increment;
        private double phase;
        private double sampleRate;

        [SerializeField] float gain;
        [SerializeField] float volume;
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

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.layer == 6) {
                Controller.Instance.RemoveSingleMarble(gameObject);
            } else Play();
        }

        void Play() {
            int scaleInd = Controller.Instance.ActiveScale;
            gain = 0;
            int[] scale = scales[scaleInd];
            int note = scale[Random.Range(0, scale.Length)];

            //3 octaves range
            frequency = frequencies[note] * (Random.value>.5f? (Random.value>.5f?.5f:2):1);
            play = true;
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
}

