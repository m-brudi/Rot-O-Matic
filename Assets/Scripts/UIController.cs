using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [Header("SHAPE PANEL")]
    [SerializeField] Slider sidesSlider;
    [SerializeField] Slider speedSlider;
    [SerializeField] Button resetSpeedBtn;

    [Space]
    [Header("SOUND PANEL")]
    [SerializeField] string[] scalesNames;
    [SerializeField] Button leftBtn;
    [SerializeField] Button rightBtn;
    [SerializeField] TMP_Text scaleTxt;
    [SerializeField] Slider filterSlider;
    [SerializeField] Button octaveDownBtn;
    [SerializeField] Button octaveZeroBtn;
    [SerializeField] Button octaveUpBtn;
    [SerializeField] GameObject[] octavesIndicators;

    [Space]
    [Header("MARBLES PANEL")]
    [SerializeField] Image[] marblesDisplay;
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;
    [SerializeField] Button collisionBtn;
    [SerializeField] Button forceImpactBtn;
    [SerializeField] Button trailBtn;
    [SerializeField] ButtonGraphics collisionBtnGraphics;
    [SerializeField] ButtonGraphics forceImpactBtnGraphics;
    [SerializeField] ButtonGraphics trailBtnGraphics;

    [Space]
    [Header("RHYTM PANEL")]
    [SerializeField] Slider bpmSlider;
    [SerializeField] Button syncBtn;
    [SerializeField] GameObject syncBtnIndicator;
    [SerializeField] TMP_Text bpmTxt;

    [SerializeField] Button note2Btn;
    [SerializeField] Button note4Btn;
    [SerializeField] Button note8Btn;
    [SerializeField] Button note16Btn;
    [SerializeField] Button note32Btn;

    [SerializeField] GameObject note2Indicator;
    [SerializeField] GameObject note4Indicator;
    [SerializeField] GameObject note8Indicator;
    [SerializeField] GameObject note16Indicator;
    [SerializeField] GameObject note32Indicator;

    [Space]
    [SerializeField] Button zenModeOnBtn;
    [SerializeField] Button zenModeOffBtn;
    [SerializeField] Button spawnBtn;
    [SerializeField] Button resetBtn;
    [SerializeField] GameObject controls;
    int scaleIndex = 0;

    public int ScaleIndex {
        get { return scaleIndex; }
        set {
            if (value < 0) value = scalesNames.Length-1;
            else if (value > scalesNames.Length-1) value = 0;
            scaleIndex = value;
            Controller.Instance.ActiveScale = value;
            ChangeScale(value);
        }
    }

    void Start()
    {
        Controller.Instance.MarblesCountChanged += DisplayMarbles;
        sidesSlider.onValueChanged.AddListener(delegate { SidesChanged(); });
        speedSlider.onValueChanged.AddListener(delegate { SpeedChanged(); });
        filterSlider.onValueChanged.AddListener(delegate { FilterChanged(); });
        bpmSlider.onValueChanged.AddListener(delegate { BpmChanged(); });
        SidesChanged();
        SpeedChanged();
        DisplayMarbles(0);
        SetupButtons();
        filterSlider.value = Controller.Instance.filter.cutoffFrequency;
        bpmSlider.value = Controller.Instance.BPM;
        BpmChanged();
        zenModeOffBtn.gameObject.SetActive(false);
        zenModeOnBtn.onClick.AddListener(()=> {
            controls.SetActive(false);
            zenModeOffBtn.gameObject.SetActive(true);
            Controller.Instance.ZenMode(true);
        });

        zenModeOffBtn.onClick.AddListener(() => {
            controls.SetActive(true);
            zenModeOffBtn.gameObject.SetActive(false);
            Controller.Instance.ZenMode(false);
        });

        resetSpeedBtn.onClick.AddListener(() => speedSlider.value = 0);
    }



    void SetupButtons() {
        spawnBtn.onClick.AddListener(() => Controller.Instance.SpawnMarble());
        resetBtn.onClick.AddListener(() => Controller.Instance.RemoveMarbles());
        leftBtn.onClick.AddListener(() => ScaleIndex--);
        rightBtn.onClick.AddListener(() => ScaleIndex++);

        collisionBtnGraphics.Setup(false);
        forceImpactBtnGraphics.Setup(false);
        trailBtnGraphics.Setup(false);

        collisionBtn.onClick.AddListener(() => {
            Controller.Instance.Collisions = !Controller.Instance.Collisions;
            collisionBtnGraphics.Setup(Controller.Instance.Collisions);
        });
        forceImpactBtn.onClick.AddListener(() => {
            Controller.Instance.ImpactForce = !Controller.Instance.ImpactForce;
            forceImpactBtnGraphics.Setup(Controller.Instance.ImpactForce);
        });
        trailBtn.onClick.AddListener(() => {
            Controller.Instance.Trails = !Controller.Instance.Trails;
            trailBtnGraphics.Setup(Controller.Instance.Trails);
        });

        ChangeRhythmSection(false);

        ClearNoteDivisionsIndicators();

        note2Btn.onClick.AddListener(() => {
            if(Controller.Instance.Synchronize) {
                ClearNoteDivisionsIndicators();
                note2Indicator.SetActive(false);
                Controller.Instance.NoteDivision = 2;
            }
        });
        note4Btn.onClick.AddListener(() => {
            if (Controller.Instance.Synchronize) {
                ClearNoteDivisionsIndicators();
                note4Indicator.SetActive(false);
                Controller.Instance.NoteDivision = 4;
            }
        });
        note8Btn.onClick.AddListener(() => {
            if (Controller.Instance.Synchronize) {
                ClearNoteDivisionsIndicators();
                note8Indicator.SetActive(false);
                Controller.Instance.NoteDivision = 8;
            }
        });
        note16Btn.onClick.AddListener(() => {
            if (Controller.Instance.Synchronize) {
                ClearNoteDivisionsIndicators();
                note16Indicator.SetActive(false);
                Controller.Instance.NoteDivision = 16;
            }
        });
        note32Btn.onClick.AddListener(() => {
            if (Controller.Instance.Synchronize) {
                ClearNoteDivisionsIndicators();
                note32Indicator.SetActive(false);
                Controller.Instance.NoteDivision = 32;
            }
        });

        syncBtn.onClick.AddListener(() =>{
            Controller.Instance.Synchronize = !Controller.Instance.Synchronize;
            ChangeRhythmSection(Controller.Instance.Synchronize);
        });

        octaveDownBtn.onClick.AddListener(() => ChangeOctave(0));
        octaveZeroBtn.onClick.AddListener(() => ChangeOctave(1));
        octaveUpBtn.onClick.AddListener(() => ChangeOctave(2));
        UpdateOctaveBtns();
    }


    void ClearNoteDivisionsIndicators() {
        note2Indicator.SetActive(true);
        note4Indicator.SetActive(true);
        note8Indicator.SetActive(true);
        note16Indicator.SetActive(true);
        note32Indicator.SetActive(true);
    }

    void ChangeRhythmSection(bool active) {
        syncBtnIndicator.SetActive(!active);
        if (!active) bpmTxt.text = "BPM";
        else bpmTxt.text = bpmSlider.value.ToString();
        if (active) note8Btn.onClick.Invoke();
        else ClearNoteDivisionsIndicators();
    }

    void BpmChanged() {
        if(Controller.Instance.Synchronize)bpmTxt.text = bpmSlider.value.ToString();
        Controller.Instance.BPM = (int)bpmSlider.value;
    }

    void ChangeOctave(int ind) {
        if (Controller.Instance.Octaves[ind]) {
            if (CheckIfCanDisableOctave()) Controller.Instance.Octaves[ind] = !Controller.Instance.Octaves[ind];
        } else {
            Controller.Instance.Octaves[ind] = !Controller.Instance.Octaves[ind];
        }
        Controller.Instance.ChangeOctave();
        UpdateOctaveBtns();
    }

    void UpdateOctaveBtns() {
        for (int i = 0; i < Controller.Instance.Octaves.Length; i++) {
            octavesIndicators[i].SetActive(!Controller.Instance.Octaves[i]);
        }
    }

    bool CheckIfCanDisableOctave() {
        bool[] oct = Controller.Instance.Octaves;
        int active = 0;
        foreach (var item in oct) {
            if (item == true) active++;
        }
        return active > 1;
    }

    void FilterChanged() {
        Controller.Instance.FilterChanged(filterSlider.value);
    }
    void SidesChanged() {
        Controller.Instance.SidesChanged((int)sidesSlider.value);
    }


    void DisplayMarbles(int num) {
        for (int i = 0; i < marblesDisplay.Length; i++) {
            if (i < num) marblesDisplay[i].color = activeColor;
            else marblesDisplay[i].color = inactiveColor;
        }
    }

    void ChangeScale(int index) {
        scaleTxt.text = scalesNames[index];
    }

    void SpeedChanged() {
        Controller.Instance.RotateSpeed = speedSlider.value;
    }
}
