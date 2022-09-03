
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class Navigation : UdonSharpBehaviour {

    private Color normalColor;
    public GameObject target;
    public Button button;

    void Start() {
        button = transform.GetComponent<Button>();
        normalColor = button.colors.normalColor;
        UpdateColor();
    }

    public void UpdateColor() {
        ColorBlock colors = button.colors;
        colors.normalColor = (target.activeSelf ? colors.selectedColor : normalColor);
        button.colors = colors;

        SendCustomEventDelayedFrames("UpdateColor", 20);
    }

    public void SetActive() {
        target.SetActive(true);
        UpdateColor();
    }

    public void SetInactive() {
        target.SetActive(false);
        UpdateColor();
    }
}
