
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

    // meta
    private string timestamp_tpl = "<b>Timestamp</b>\n\n<size=40>{timestamp}</size>";
    private string people_tpl = "<b>On the picture</b>\n\n<size=40>{people}</size>";
    private string world_tpl = "<b>World</b>\n\n<size=40>{world}</size>";
    private string note_tpl = "<b>Note</b>\n\n<size=40>{note}</size>";

    // statusbar
    public GameObject total;

    // actions
    private int increment;
    public int index;

    void Start() {
        LoadAllImages();
        InitializeUI();
    }

    public void UpdateTotalCount() {
        total.GetComponent<Text>().text = (index + 1) + " / " + grid_album.transform.childCount;
    }

    public void InitializeUI() {
        grid_view.SetActive(false);
        info_view.SetActive(false);
        filter_view.SetActive(false);
    }
    
    public void Next() {
        IncrementIndex(true);
    }

    public void Previouse() {
        IncrementIndex(false);
    }

    public void SetIndex() {
        ShowImage(index);

        grid_view.SetActive(false);
        main_view.SetActive(true);

        UpdateTotalCount();
        UpdateGridHighlight();
        UpdateInfoView();
    }

    public void IncrementIndex(bool forward) {
        int count = grid_album.transform.childCount;
        int imageIndex = index + (forward ? 1 : -1);

        // move any out of bounds index
        if (imageIndex < 0) imageIndex = (count - 1);
        if (imageIndex == count) imageIndex = 0;

        // set new index active
        ShowImage(imageIndex);
        
        UpdateGridHighlight();
        UpdateTotalCount();
        UpdateInfoView();
    }

    public void UpdateGridHighlight() {
        for (int i = 0; i < grid_album.transform.childCount; ++i) {
            Image tempimg = grid_album.transform.GetChild(i).GetComponent<Image>();
            var tempcolor = tempimg.color;

            tempcolor.a = grid_album.transform.GetChild(i).GetSiblingIndex() == index ? 0.1f : 1f;
            tempimg.color = tempcolor;
        }
    }

    public void UpdateInfoView() {
        Metadata metadata = grid_album.transform.GetChild(index).GetComponent<Metadata>();

        GameObject[] people = (GameObject[]) metadata.GetProgramVariable("people");
        GameObject world = (GameObject) metadata.GetProgramVariable("world");
        string note = (string) metadata.GetProgramVariable("note");
        int month = (int) metadata.GetProgramVariable("month");
        int year = (int) metadata.GetProgramVariable("year");

        string temp_people = "";
        if (people.Length > 0) {
            foreach (GameObject p in people) {
                if (temp_people == "") {
                    temp_people = p.name;
                } else {
                    temp_people = temp_people + ", " + p.name;
                }
            }
        } else {
            temp_people = "Either empty or I could not remember! Sorry!"; 
        }

        string timestamp_new = timestamp_tpl.Replace("{timestamp}", month + " / " + year);
        string people_new = people_tpl.Replace("{people}", temp_people);
        if (note == "") note = "No notes available for this image";
        string note_new = note_tpl.Replace("{note}", note);

        string world_new = world_tpl.Replace("{world}", "Either not set yet or I could not remember! Sorry!");
        if (world != null) world_new = world_tpl.Replace("{world}", world.name);

        info_timestamp.GetComponent<Text>().text = timestamp_new;
        info_people.GetComponent<Text>().text = people_new;
        info_world.GetComponent<Text>().text = world_new;
        info_note.GetComponent<Text>().text = note_new;
    }

    public void ShowImage(int i) {
        main_image.GetComponent<Image>().sprite = grid_album.transform.GetChild(i).GetComponent<Image>().sprite;
        index = i;
    }

    public void LoadAllImages() {
        for (int i = 0; i < images.transform.childCount; i++) {
            // create a main view clone of the image database
            var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
            grid_clone.transform.position = grid_album.transform.position;
            grid_clone.transform.SetParent(grid_album.transform);
            grid_clone.SetActive(true);

            // scale to fit
            grid_clone.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);

            index = 0;
            ShowImage(index);
        }

        UpdateGridHighlight();
        UpdateTotalCount();
        UpdateInfoView();
    }

}
