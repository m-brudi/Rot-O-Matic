using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField] Slider sidesSlider;
    [Space]
    [SerializeField] Slider speedSlider;
    [Space]
    [SerializeField] Button spawnBtn;
    [SerializeField] Button resetBtn;

    [Space]
    [SerializeField] string[] scalesNames;
    [SerializeField] Button leftBtn;
    [SerializeField] Button rightBtn;
    [SerializeField] TMP_Text scaleTxt;

    [Space]
    [SerializeField] Image[] marblesDisplay;
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;

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
        SidesChanged();
        SpeedChanged();
        DisplayMarbles(0);
        spawnBtn.onClick.AddListener(() => Controller.Instance.SpawnMarble());
        resetBtn.onClick.AddListener(() => Controller.Instance.RemoveMarbles());
        leftBtn.onClick.AddListener(() => ScaleIndex--);
        rightBtn.onClick.AddListener(() => ScaleIndex++);
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
