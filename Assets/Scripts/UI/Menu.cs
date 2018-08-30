using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Main.UI
{
    public class Menu
    {
        protected GameObject canvas;
        private GameObject inventoryPanel;
        protected GameObject map;
        protected Text topLabel;
        private Text label;
        protected bool isBuilding = false;
        protected KeyCode keyCode;
        private Tooltip tooltip;
        Stack<GameObject> layers = new Stack<GameObject>();
        private GameObject samp;
        private GameObject menu;

        public Menu(GameObject map, GameObject canvas)
        {
            this.canvas = canvas;
            inventoryPanel = canvas.transform.GetChild(1).gameObject;
            topLabel = canvas.transform.GetChild(0).GetComponent<Text>();
            label = canvas.transform.GetChild(0).GetComponent<Text>();
            keyCode = KeyCode.Escape;

            // Add callback for our custom MonoBehavior.Update() event.
            Main.Updating += OnUpdate;

            samp = Object.Instantiate(inventoryPanel);
            inventoryPanel.SetActive(false);
            Object.Destroy(samp.transform.GetChild(0).gameObject);
            Object.Destroy(samp.transform.GetChild(1).gameObject);
            menu = Object.Instantiate(samp, canvas.transform);
            menu.name = "menutype_" + GetType().Name.ToLower();
            RectTransform rt = menu.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(210, 320);

            Dictionary<string, UnityAction> btnMap = new Dictionary<string, UnityAction>();
            btnMap.Add("newgame", () => { BtnNew_Click(map); });
            btnMap.Add("loadgame", BtnLoad_Click);
            GameObject btnPrefab = Resources.Load("Prefabs/UI/Button") as GameObject;
            foreach (var btnInfo in btnMap)
            {
                GameObject btnObj = Object.Instantiate(btnPrefab, menu.transform);
                btnObj.GetComponentInChildren<Text>().text = Localization.GetString(btnInfo.Key);
                btnObj.GetComponent<Button>().onClick.AddListener(btnInfo.Value);
            }
        }

        private void OnUpdate()
        {
            if (Input.GetKeyDown(keyCode))
            {
                Cancel();
            }
        }

        private void Cancel()
        {
            string name = Application.productName;
            if (layers.Count == 0)
            {
                layers.Push(menu);
                menu.SetActive(true);
                name = Localization.GetString(menu.name);
            }
            else
            {
                GameObject gameObject = layers.Pop();
                gameObject.SetActive(false);
                
                if (layers.Count == 0)
                {
                    gameObject.SetActive(true);
                    name = Localization.GetString(gameObject.name);
                }
                else
                {
                    gameObject.transform.parent.gameObject.SetActive(true);
                    name = Localization.GetString(gameObject.transform.parent.name);
                }
            }
            topLabel.text = name;
        }

        private void BtnNew_Click(GameObject map)
        {
            Cancel();
            CameraControls cameraControls = new CameraControls(Camera.main);
            new Inventory();
            MapGenerator mapGenerator = new MapGenerator(map);
            new AI.Waypoint(map, canvas);
            new AI.Road(map, canvas);
            new Menu(map, canvas);
            AlertList alertList = new AlertList(canvas);
            AlertList.NewAlert("UI_ALERT_TEST", "mug", null);
        }

        public void BtnLoad_Click()
        {
            GameObject gameObject = Object.Instantiate(samp, canvas.transform);
            gameObject.name = "loadgame";
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(360, 420);
            ShowFileDialog(gameObject);
            layers.Push(gameObject);
            Dictionary<string, UnityAction> btnMap = new Dictionary<string, UnityAction>
            {
                { "load", () => { BtnNew_Click(map); } },
                { "cancel", Cancel }
            };
            GameObject btnGrp = new GameObject();
            btnGrp.transform.parent = gameObject.transform;
            HorizontalLayoutGroup horizontalLayoutGroup = btnGrp.AddComponent<HorizontalLayoutGroup>();
            GameObject btnPrefab = Resources.Load("Prefabs/UI/Button") as GameObject;
            foreach (var btnInfo in btnMap)
            {
                GameObject btnObj = Object.Instantiate(btnPrefab, btnGrp.transform);
                btnObj.GetComponentInChildren<Text>().text = Localization.GetString(btnInfo.Key);
                btnObj.GetComponent<Button>().onClick.AddListener(btnInfo.Value);
                rt = btnObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(60, 20);
            }
                menu.SetActive(false);
        }

        private void ShowFileDialog(GameObject gameObject)
        {
            int i = 0;
            GameObject fileList = new GameObject();
            fileList.transform.parent = gameObject.transform;
            RectTransform rectTransform = fileList.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.localPosition = new Vector2(5, 2.5f);
            rectTransform.localScale = Vector3.zero;

            foreach (var file in DataHandler.ListSaveDataFiles())
            { 
                string fileName = Path.GetFileNameWithoutExtension(file.FullName);
                GameObject button = new GameObject(fileName);
                button.transform.parent = fileList.transform;
                button.AddComponent<RectTransform>();
                button.AddComponent<Button>();

                
            GameObject go2 = new GameObject("Text");
            go2.transform.parent = button.transform;
                Image goImg = button.AddComponent<Image>();
                goImg.color = (i++) % 2 == 0 ? Color.green : Color.blue;
                goImg.sprite = Resources.Load("Sprites/btn") as Sprite;
                Text goText = go2.AddComponent<Text>();
                    goText.font = Resources.Load("Fonts/OpenSans-Regular") as Font;
                    goText.text = string.Format("{0}\n<size=11><i>{1}</i></size>", fileName, file.LastWriteTime);
                goText.color = Color.black;
             rectTransform = go2.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = Vector3.zero;
            }

            // Set scroll sensitivity based on the save-item count
            //fileList.GetComponentInParent<ScrollRect>().scrollSensitivity = fileList.childCount / 2;
        }
    }
}