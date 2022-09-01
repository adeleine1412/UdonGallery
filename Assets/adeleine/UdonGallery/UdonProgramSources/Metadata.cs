
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Metadata : UdonSharpBehaviour {

    public ViewManager viewmanager;
    public GameObject[] people;
    public GameObject world;
    public string note;
    public int month;
    public int year;

    void Start() {
        viewmanager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
    }

    public override void Interact() {
        viewmanager.SetProgramVariable("index", transform.GetSiblingIndex());
        viewmanager.SendCustomEvent("SetIndex");
    }
}
