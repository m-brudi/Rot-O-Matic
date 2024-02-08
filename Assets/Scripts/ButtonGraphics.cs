using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGraphics : MonoBehaviour
{
    [SerializeField] Image[] graphics;
    float alphaPercents = .3f;

    public void Setup(bool active) {
        if (active) {
            foreach (var item in graphics) {
                item.color = new(item.color.r, item.color.g, item.color.b, 1);
            }
        } else {
            foreach (var item in graphics) {
                item.color = new(item.color.r, item.color.g, item.color.b, alphaPercents);
            }
        }
    }
}
