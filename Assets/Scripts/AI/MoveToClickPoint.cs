using UnityEngine;

public class MoveToClickPoint : MonoBehaviour
{
    GameObject car;

    void Start()
    {
        car = GameObject.Find("Target");
    }

    void Update()
    {
    }
    void OnGUI()
    {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2
        {
            // Get the mouse position from Event.
            // Note that the y position from Event is inverted.
            x = e.mousePosition.x,
            y = c.pixelHeight - e.mousePosition.y
        };

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane));

        RaycastHit hit;
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane)), out hit))
            {
                transform.position = hit.point;
            }
        }
    }
}