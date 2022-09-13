
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Navigation : UdonSharpBehaviour {

    private Color normalColor;
    public GameObject target;
    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        anim.SetBool("Active", (target.activeSelf ? true : false));
    }

    public void SetActive() {
        target.SetActive(true);
        anim.SetBool("Active", true);
    }

    public void SetInactive() {
        target.SetActive(false);
        anim.SetBool("Active", false);
    }
}
