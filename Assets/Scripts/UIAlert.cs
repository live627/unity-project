using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIAlert  
{
    private GameObject focusGameObject;
    private GameObject alertGO;
    private string alertText;
    private string alertIconId;

    public GameObject AlertGO
    {
        get
        {
            return alertGO;
        }

        private set
        {
            alertGO = value;
            alertGO.GetComponentInChildren<Text>().text = alertText;
            Sprite iconTexture = Resources.Load<Sprite>(Path.Combine("Icons", alertIconId));

            Transform iconTransform = alertGO.transform.Find("Icon Container/Icon");

            if (iconTexture == null)
            {
                // If we just use a null texture, we get an ugly white square.
                // Instead, just turn off the icon (we'll still have the circle
                // from the container).
                iconTransform.gameObject.SetActive(false);
            }
            else
            {
                iconTransform.GetComponent<Image>().sprite = iconTexture;
            }
        }
    }

    public UIAlert(GameObject focusGameObject, GameObject alertBtn, string alertText, string alertIconId)
    {
        this.focusGameObject = focusGameObject;
        this.alertText = alertText;
        this.alertIconId = alertIconId;
        AlertGO = alertBtn;
    }

    public void OnClick()
    {
        if(focusGameObject != null)
        {
            Debug.Log("UNIMPLEMENTED: Move camera to look at FocusGameObject gameobject.");
        }
        else
        {
            Debug.Log("Alert was clicked, but has nothing to zoom to.");
        }
    }

    public void Close()
    {
        Object.Destroy(alertGO);
    }
}
