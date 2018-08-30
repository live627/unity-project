using Main.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Item item;
	public int amount;
	public int slotId;

	private Inventory inv;
    private Tooltip tooltip;
	private Vector2 offset;

    public void Init(Inventory inv, Tooltip tooltip)
    {
        this.inv = inv;
        this.tooltip = tooltip;
    }

    public void OnBeginDrag(PointerEventData eventData)
	{
		if (item != null)
		{
            transform.SetParent(transform.parent.parent, false);
            transform.position = eventData.position - offset;
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (item != null)
		{
            transform.position = eventData.position - offset;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
        transform.SetParent(inv.slots[slotId].transform);
        transform.position = inv.slots[slotId].transform.position;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		tooltip.Activate(item);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		tooltip.Deactivate();
	}
}
