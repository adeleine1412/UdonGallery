
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
    /* public GameObject desktop_warning; */
    public GameObject images;

    // info
    public GameObject info_timestamp;
    public GameObject info_people;
    public GameObject info_world;
    public GameObject info_note;

    // filters
    public GameObject people_filter_view;
    public GameObject worlds_filter_view;
    public GameObject timestamps_filter_view;

    // misc
    private GameObject[] filters;
    private GameObject[] views;
    /* public GameObject total; */
    public int index = 0;

    // filter values
    public string timestamp;
    public string person;
    public string world;

    void Start() {
        // define filter and main views
        filters = new GameObject[] { people_filter_view, worlds_filter_view, timestamps_filter_view };
        views = new GameObject[] { main_view, grid_view, info_view, filter_view };

        // load all images and show the first image
        ShowAllImages();
        ShowImage(index);

        // show the proper views
        /* ShowFilter(people_filter_view); */
        ShowView(main_view);

        // show warning if user is on desktop
        /* if (Networking.LocalPlayer != null && !Networking.LocalPlayer.IsUserInVR()) desktop_warning.SetActive(true); */
    }

    /* void Update() { if (desktop_warning.activeSelf && Input.GetKeyDown(KeyCode.Tab)) desktop_warning.SetActive(false); } */
    public void ShowView(GameObject view) { foreach (GameObject x in views) x.SetActive((x == view ? true : false)); }
    public void ShowFilter(GameObject filter) { foreach (GameObject x in filters) x.SetActive((x == filter ? true : false)); }
    public void Next() { ShowImage(((index + 1) == grid_album.transform.childCount) ? 0 : (index + 1)); }
    public void Previouse() { ShowImage((index == 0) ? (grid_album.transform.childCount - 1) : (index - 1)); }
    /* public void UpdateTotalCount() { total.GetComponent<Text>().text = (index + 1) + " / " + grid_album.transform.childCount; }
    public void SetTotalCount(int current, int max) { total.GetComponent<Text>().text = current + " / " + max; } */

    public void ShowImage(int i) {
        main_image.GetComponent<Image>().sprite = grid_album.transform.GetChild(i).GetComponent<Image>().sprite;
        index = i;

        SendCustomEventDelayedFrames("UpdateHighlighted", 10);
        /* SendCustomEventDelayedFrames("UpdateTotalCount", 10); */
        SendCustomEventDelayedFrames("UpdateMetadata", 10);
    }

    public void SetIndex() {
        ShowView(main_view);
        ShowImage(index);
    }

    public void UpdateHighlighted() {
        for (int i = 0; i < grid_album.transform.childCount; ++i) {
            var color = grid_album.transform.GetChild(i).GetComponent<Image>().color;
            color.a = (i == index) ? 0.25f : 1f;
            grid_album.transform.GetChild(i).GetComponent<Image>().color = color;
        }
    }

    public void AppendImage(GameObject image) {
        image.transform.position = grid_album.transform.position;
        image.transform.SetParent(grid_album.transform);
        image.SetActive(true);
        image.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);
    }

    public void UpdateMetadata() {
        ImageMeta metadata = grid_album.transform.GetChild(index).GetComponent<ImageMeta>();
        GameObject[] people = (GameObject[]) metadata.GetProgramVariable("people");
        GameObject world = (GameObject) metadata.GetProgramVariable("world");
        string note = (string) metadata.GetProgramVariable("note");
        int month = (int) metadata.GetProgramVariable("month");
        int year = (int) metadata.GetProgramVariable("year");
        
        string people_temp = ""; foreach (GameObject p in people) people_temp = (people_temp == "") ? p.name : people_temp + ", " + p.name;
        string timestamp_str = (month < 1 && year < 1) ? "Either not set yet or I could not remember! Sorry!" : $"{month} / {year}";
        string people_str = people == null ? "Either not set yet or I could not remember! Sorry!" : people_temp;
        string world_str = world == null ? "Either not set yet or I could not remember! Sorry!" : world.name;
        string note_str = note == "" ? "No notes available for this image" : note;

        info_timestamp.GetComponent<Text>().text = $"<b>Timestamp</b>\n\n<size=40>{timestamp_str}</size>";
        info_people.GetComponent<Text>().text = $"<b>On the picture</b>\n\n<size=40>{people_str}</size>";
        info_world.GetComponent<Text>().text = $"<b>World</b>\n\n<size=40>{world_str}</size>";
        info_note.GetComponent<Text>().text = $"<b>Note</b>\n\n<size=40>{note_str}</size>";
    }

    public void ShowAllImages() {
        // remove all current images
        for (int i = 0; i < grid_album.transform.childCount; ++i) Destroy(grid_album.transform.GetChild(i).gameObject, 0f);

        // make a copy of all images listed
        for (int i = 0; i < images.transform.childCount; i++) {
            var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
            AppendImage(grid_clone);
        }
    }

    public void FilterPerson() { Filter(0); }
    public void FilterWorld() { Filter(1); }
    public void FilterTimestamp() { Filter(2); }

    public void Filter(int type) {
        int result_count = 0;

        // remove all current images
        for (int i = 0; i < grid_album.transform.childCount; ++i) Destroy(grid_album.transform.GetChild(i).gameObject, 0f);

        for (int i = 0; i < images.transform.childCount; i++) {
            ImageMeta metadata = images.transform.GetChild(i).GetComponent<ImageMeta>();

            // by person
            if (type == 0) foreach (GameObject p in metadata.people) if (p.name == person) {
                var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
                AppendImage(grid_clone);
                result_count++;
                break;
            }

            // by world
            if (type == 1) if (metadata.world.name == world) {
                var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
                AppendImage(grid_clone);
                result_count++;
            }

            // by timestamp
            if (type == 2) if ((metadata.year + " / " + metadata.month) == timestamp) {
                var grid_clone = VRCInstantiate(images.transform.GetChild(i).transform.gameObject);
                AppendImage(grid_clone);
                result_count++;
            }

        }

        // update everything
        ShowImage(0);
        ShowView(grid_view);
    }
    
}
