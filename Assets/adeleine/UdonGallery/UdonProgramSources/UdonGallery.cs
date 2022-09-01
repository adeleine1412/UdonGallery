﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class UdonGallery : UdonSharpBehaviour {

    // views
    public GameObject main_view;
    public GameObject grid_view;
    public GameObject info_view;
    public GameObject filter_view;
    public GameObject main_image;
    public GameObject grid_album;
    public GameObject images;

    // info
    public GameObject info_timestamp;
    public GameObject info_people;
    public GameObject info_world;
    public GameObject info_note;

    // misc
    private GameObject[] views;
    public GameObject total;
    public int index = 0;

    // filter values
    public string timestamp;
    public string person;
    public string world;

    void Start() {
        views = new GameObject[] { main_view, grid_view, info_view, filter_view };
        LoadAllImages();
    }

    public void ShowView(GameObject view) { foreach (GameObject x in views) x.SetActive((x == view ? true : false)); }
    public void Next() { ShowImage(((index + 1) == grid_album.transform.childCount) ? 0 : (index + 1)); }
    public void Previouse() { ShowImage((index == 0) ? (grid_album.transform.childCount - 1) : (index - 1)); }

    public void SetIndex() {
        ShowView(main_view);
        ShowImage(index);
    }

    public void ShowImage(int i) {
        index = i;
        main_image.GetComponent<Image>().sprite = grid_album.transform.GetChild(i).GetComponent<Image>().sprite;
        UpdateInfoAttributes();
    }

    public void UpdateInfoAttributes() {
        // update info view with metadata
        Metadata metadata = grid_album.transform.GetChild(index).GetComponent<Metadata>();

        GameObject[] people = (GameObject[]) metadata.GetProgramVariable("people");
        GameObject world = (GameObject) metadata.GetProgramVariable("world");
        string note = (string) metadata.GetProgramVariable("note");
        int month = (int) metadata.GetProgramVariable("month");
        int year = (int) metadata.GetProgramVariable("year");
        string people_temp = "";

        foreach (GameObject p in people) people_temp = (people_temp == "") ? p.name : people_temp + ", " + p.name;

        string timestamp_str = (month < 1 && year < 1) ? "Either not set yet or I could not remember! Sorry!" : $"{month} / {year}";
        string people_str = people == null ? "Either not set yet or I could not remember! Sorry!" : people_temp;
        string world_str = world == null ? "Either not set yet or I could not remember! Sorry!" : world.name;
        string note_str = note == "" ? "No notes available for this image" : note;

        info_timestamp.GetComponent<Text>().text = $"<b>Timestamp</b>\n\n<size=40>{timestamp_str}</size>";
        info_people.GetComponent<Text>().text = $"<b>On the picture</b>\n\n<size=40>{people_str}</size>";
        info_world.GetComponent<Text>().text = $"<b>World</b>\n\n<size=40>{world_str}</size>";
        info_note.GetComponent<Text>().text = $"<b>Note</b>\n\n<size=40>{note_str}</size>";

        // set new highlight in grid view
        for (int i = 0; i < grid_album.transform.childCount; ++i) {
            var color = grid_album.transform.GetChild(i).GetComponent<Image>().color;
            color.a = grid_album.transform.GetChild(i).GetSiblingIndex() == index ? 0.1f : 1f;
            grid_album.transform.GetChild(i).GetComponent<Image>().color = color;
        }

        // update total count on top statusbar
        total.GetComponent<Text>().text = (index + 1) + " / " + grid_album.transform.childCount;
    }

    public void LoadAllImages() {
        for (int i = 0; i < images.transform.childCount; i++) {
            // create a main view clone of the image database
            var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
            grid_clone.transform.position = grid_album.transform.position;
            grid_clone.transform.SetParent(grid_album.transform);
            grid_clone.SetActive(true);

            grid_clone.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);
        }

        index = 0;
        SetIndex();
    }

    public void FilterPerson() {
        for (int i = 0; i < grid_album.transform.childCount; ++i) Destroy(grid_album.transform.GetChild(i).gameObject, 0f);

        for (int i = 0; i < images.transform.childCount; i++) {
            Metadata metadata = images.transform.GetChild(i).GetComponent<Metadata>();
            foreach (GameObject p in metadata.people) if (p.name == person) {
                var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
                grid_clone.transform.position = grid_album.transform.position;
                grid_clone.transform.SetParent(grid_album.transform);
                grid_clone.SetActive(true);
                grid_clone.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);
                break;
            }
        }

        index = 0;
        SendCustomEventDelayedFrames("SetIndex", (grid_album.transform.childCount / 10));
    }

    public void FilterTimestamp() {
        for (int i = 0; i < grid_album.transform.childCount; ++i) Destroy(grid_album.transform.GetChild(i).gameObject, 0f);

        for (int i = 0; i < images.transform.childCount; i++) {
            Metadata metadata = images.transform.GetChild(i).GetComponent<Metadata>();
            if ((metadata.year + " / " + metadata.month) == timestamp) {
                var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
                grid_clone.transform.position = grid_album.transform.position;
                grid_clone.transform.SetParent(grid_album.transform);
                grid_clone.SetActive(true);
                grid_clone.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);
            }
        }

        index = 0;
        SendCustomEventDelayedFrames("SetIndex", (grid_album.transform.childCount / 10));
    }

    public void FilterWorld() {
        for (int i = 0; i < grid_album.transform.childCount; ++i) Destroy(grid_album.transform.GetChild(i).gameObject, 0f);

        for (int i = 0; i < images.transform.childCount; i++) {
            Metadata metadata = images.transform.GetChild(i).GetComponent<Metadata>();
            if (metadata.world.name == world) {
                var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
                grid_clone.transform.position = grid_album.transform.position;
                grid_clone.transform.SetParent(grid_album.transform);
                grid_clone.SetActive(true);
                grid_clone.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);
            }
        }

        index = 0;
        SendCustomEventDelayedFrames("SetIndex", (grid_album.transform.childCount / 10));
    }
    
}
