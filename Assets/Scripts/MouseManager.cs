using Main.AI.Pathfindidng;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public GameObject selectedObject;
    private Vector2 orgBoxPos = Vector2.zero;
    private Vector2 endBoxPos = Vector2.zero;
    public GUIStyle selectTexture;

    public delegate void RaycastHitEventHandler(RaycastHit hitInfo);
    public static event RaycastHitEventHandler RaycastHitEvent;

    void Update()
    {
        // Check if the mouse is not over a UI element.
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
                {
                    // Debug.Log("Mouse is over: " + hitInfo.transform.gameObject.name );

                    // The collider we hit may not be the "root" of the object
                    // You can grab the most "root-est" gameobject using
                    // transform.root, though if your objects are nested within
                    // a larger parent GameObject (like "All Units") then this might
                    // not work.  An alternative is to move up the transform.parent
                    // hierarchy until you find something with a particular component.

                    if (RaycastHitEvent != null)
                    {
                        RaycastHitEvent(hitInfo);
                    }

                    GameObject hitObject = hitInfo.transform.gameObject;

                  //  SelectObject(hitObject);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
            //    ClearSelection();
            }

            //BUG Box select doesn't select the correct objects with BoxCastAll, even though a box is correctly drawn with the mouse. May have to do with the inversion of screen space and GUI space.
            // SetBoxSelection();
        }
    }

    /// <summary>
    /// Handles the case where the user draws a rectangle to select some units.
    /// </summary>
    private void SetBoxSelection()
    {
        if (Input.GetMouseButton(0))
        {
            if (orgBoxPos == Vector2.zero)
            {
                orgBoxPos = Input.mousePosition;
            }
            endBoxPos = Input.mousePosition;
        }
        else
        {
            ResetBoxSelection();
        }
    }

    /// <summary>
    /// Reset the box coords.
    /// </summary>
    private void ResetBoxSelection()
    {
        orgBoxPos = Vector2.zero;
        endBoxPos = Vector2.zero;
    }

    /// <summary>
    /// Draws the selection rectangle if the user is holding the mouse down.
    /// </summary>
    private void DrawBoxSelection()
    {
        GUI.Box(GetRectangle(), "", selectTexture);
    }

    private void CastBoxSelection()
    {
        Vector3 wp1 = Camera.main.ScreenToWorldPoint(orgBoxPos);
        Vector3 wp2 = Camera.main.ScreenToWorldPoint(endBoxPos);
        Vector3 wp3 = Camera.main.ScreenToWorldPoint(new Vector2(orgBoxPos.x, endBoxPos.y));
        Vector3 wp4 = Camera.main.ScreenToWorldPoint(new Vector2(endBoxPos.x, orgBoxPos.y));

        //  Using these 4 points I can compute the center, the extents, etc.
        Vector3 wpcenter = Camera.main.ScreenToWorldPoint(
            new Vector2(orgBoxPos.x + (endBoxPos.x - orgBoxPos.x) / 2f,
                    orgBoxPos.y + (endBoxPos.y - orgBoxPos.y) / 2f)
        );
        Vector3 halfExtents = new Vector3(wpcenter.x - wp1.x, wp1.y - wpcenter.y, 25);

        // I then do a box cast into the world from the camera to see what I'm selecting:
        Vector3 direction = Camera.main.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Rect rect = GetRectangle();
        //RaycastHit[] hits = Physics.BoxCastAll(wpcenter, halfExtents, direction, rotation, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));
        RaycastHit[] hits = Physics.BoxCastAll(
            Camera.main.ScreenToWorldPoint(rect.center),
            Camera.main.ScreenToWorldPoint(new Vector2(rect.center.x - rect.x, rect.y - rect.center.y)), direction, rotation);

        List<string> hitNames = new List<string>();
        foreach (RaycastHit hit in hits)
        {
            SelectObject(hit.transform.gameObject);
            //if (!hitNames.Contains(hit.transform.name))
            //{
            //    Debug.Log(hit.transform.name);
            //    hitNames.Add(hit.transform.name);
            //}
        }
    }

    private Rect GetRectangle()
    {
        return new Rect(
            orgBoxPos.x,
            Screen.height - orgBoxPos.y,
            endBoxPos.x - orgBoxPos.x,
            -((Screen.height - orgBoxPos.y) - (Screen.height - endBoxPos.y))
        );
    }

    void OnGUI()
    {
        DrawBoxSelection();

        if (orgBoxPos != Vector2.zero && endBoxPos != Vector2.zero)
        {
            CastBoxSelection();
        }
    }

    private void SelectObject(GameObject obj)
    {
        if (obj != selectedObject)
        {
            if (selectedObject != null)
            {
                ClearSelection();
            }

            selectedObject = obj;
            if (obj.GetComponent<Unit>() != null)
            {
                PathRequestManager.instance.SelectUnit(obj);
            }
            else
            {
                Renderer[] rs = selectedObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rs)
                {
                    Material m = r.material;
                    m.color = Color.green;
                    r.material = m;
                }
            }
        }
    }

    private void ClearSelection()
    {
        if (selectedObject != null)
        {
            Renderer[] rs = selectedObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rs)
            {
                Material m = r.material;
                m.color = Color.white;
                r.material = m;
            }


            selectedObject = null;
        }
    }
}
