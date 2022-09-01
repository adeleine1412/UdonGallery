
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Metadata : UdonSharpBehaviour {

    private UdonGallery udongallery;
    public GameObject[] people;
    public GameObject world;
    public string note;
    public int month;
    public int year;

    void Start() {
        udongallery = GameObject.Find("UdonGallery").GetComponent<UdonGallery>();
    }

    public override void Interact() {
        udongallery.SetProgramVariable("index", transform.GetSiblingIndex());
        udongallery.SendCustomEvent("SetIndex");
    }
}
