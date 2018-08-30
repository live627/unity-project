using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public class Tooltip
    {
        private string data;
        private GameObject tooltip;

        public Tooltip(GameObject tooltip)
        {
            this.tooltip = tooltip;
            tooltip.SetActive(false);

            // Add callback for our custom MonoBehavior.Update() event.
            Main.Updating += OnUpdate;
        }

        private void OnUpdate()
        {
            if (tooltip.activeSelf)
            {
                tooltip.transform.position = Input.mousePosition;
            }
        }

        public void Activate(Item item)
        {
            ConstructDataString(item);
            tooltip.SetActive(true);
        }

        public void Deactivate()
        {
            tooltip.SetActive(false);
        }

        public void ConstructDataString(Item item)
        {
            data = "<color=#FFEC58FF><b>" + item.Title + "</b></color>\n\n" + item.Description
                + "\nPower: " + item.Power;
            tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
        }

    }
}