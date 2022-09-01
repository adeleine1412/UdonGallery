
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class StatusbarButton : UdonSharpBehaviour {

    private Color normalColor;
    public GameObject target;
    public Button button;
    private bool active;

    void Start() {
        button = transform.GetComponent<Button>();
        normalColor = button.colors.normalColor;
    }

    void Update() {
        ColorBlock colors = button.colors;

        if (target != null) {
            // check if target view is active, if yes, keep button active
            if (target.activeSelf && !active) active = true;
            if (!target.activeSelf && active) active = false;
        }

        if (active) colors.normalColor = colors.selectedColor;
        if (!active) colors.normalColor = normalColor;

        button.colors = colors;
    }

    public void SetActive() {
        target.SetActive(true);
    }

    public void SetInactive() {
        target.SetActive(false);
    }
}
