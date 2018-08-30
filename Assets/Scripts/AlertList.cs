using System;
using UnityEngine;
using UnityEngine.UI;

public class AlertList
{
    public static AlertList Instance
    {
        get
        {
            if (_Instance == null)
            {
                throw new Exception("AlertList is not instantiated.");
            }

            return _Instance;
        }
    }
    private static AlertList _Instance;

    public GameObject alertPrefab;

    // alertList points to the object with (probably) the VerticalLayoutGroup component
    public GameObject alertContainer;

    public AlertList(GameObject gameObject)
    {
        alertPrefab = Resources.Load("Prefabs/Alert", typeof(GameObject)) as GameObject;
        GameObject alertContainer = new GameObject("alertContainer");
        RectTransform rectTransform = alertContainer.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 0);
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 0.5f);
        rectTransform.localPosition = new Vector2(0, 0);
        VerticalLayoutGroup verticalLayoutGroup = alertContainer.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.childControlHeight = false;
        verticalLayoutGroup.childControlWidth = false;
        verticalLayoutGroup.childForceExpandHeight = false;
        verticalLayoutGroup.childForceExpandWidth = false;
        verticalLayoutGroup.childAlignment = TextAnchor.LowerRight;
        alertContainer.transform.SetParent(gameObject.transform, false);
        this.alertContainer = alertContainer;
        _Instance = this;
    }

    public static void NewAlert(string alertTextId, string alertIconId = "", GameObject focusGameObject = null)
    {
        // Create the new alert
        UIAlert uiAlert = new UIAlert(null, Instance.alertPrefab, Localization.GetString(alertTextId), alertIconId);

        // Add alert to the list
        GameObject alert = UnityEngine.Object.Instantiate(uiAlert.AlertGO, Instance.alertContainer.transform, false);

        // Add event AFTER instantiation because it won't register if done beforehand.
        alert.GetComponent<Button>().onClick.AddListener(() => { Debug.Log("LLL"); });
    }

    public static void NewAlert(string alertTextId, GameObject focusGameObject)
    {
        NewAlert(alertTextId, "", focusGameObject);
    }
}
