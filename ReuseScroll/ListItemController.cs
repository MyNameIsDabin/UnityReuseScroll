using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItemController : MonoBehaviour
{
    enum ChildAlignment {
        Vertical,
        /*Horizontal*/ //Todo : 아직 구현 안됨..
    }
    public int itemCount = 0;

    [SerializeField] private bool InitOnStart = true;
    [SerializeField] private ChildAlignment alignment = ChildAlignment.Vertical;    
    [SerializeField] private int splitCount = 3;
    [SerializeField] private bool autoMargin = true;
    [SerializeField] private Vector2 margin = Vector2.zero;
    [SerializeField] private float spacing = 0.0f;

    [SerializeField] private RectTransform content;
    [SerializeField] private ListItemContainer containerPrefab;
    [SerializeField] private ListItem listItemPrefab;


    public LinkedList<ListItemContainer> containers { get; } = new LinkedList<ListItemContainer>();
    public LinkedList<ListItem> listItems { get; } = new LinkedList<ListItem>();
    public List<ListItem> items { get; } = new List<ListItem>();

    private ListItemContainer container = null;
    private bool useContainer = false;
    private Vector2 itemSize;
    private int useItemCount = 0;
    private int accIndex = 0;
    private int groupCount = 0;

    public void Start()
    {
        if (InitOnStart)
            Init();
    }

    public void Init(int? _itemCount = null)
    {
        itemCount = _itemCount ?? itemCount;

        if (containerPrefab != null)
        {
            useContainer = true;
            itemSize = containerPrefab.GetComponent<RectTransform>().rect.size;
            groupCount = itemCount / splitCount;
        }
        else
        {
            itemSize = listItemPrefab.GetComponent<RectTransform>().rect.size;
            groupCount = itemCount;
        }

        if (autoMargin)
        {
            margin = itemSize * 0.5f;
        }

        if (alignment == ChildAlignment.Vertical)
        {
            useItemCount = Mathf.CeilToInt(content.rect.height / (itemSize.y + spacing)) + 1;
            content.sizeDelta = new Vector2((itemSize.x + spacing), (itemSize.y + spacing) * groupCount);
        }


        content.pivot = new Vector2(0, 1);
        content.anchorMin = new Vector2(0.5f, 1);
        content.anchorMax = new Vector2(0.5f, 1);
        content.anchoredPosition = new Vector2(-content.rect.width * 0.5f, 0);

        if (useContainer)
            InitListItems(containers);
        else
            InitListItems(listItems);
    }

    private void InitListItems<T>(LinkedList<T> list) where T : ListItem
    {
        int generateCount = useContainer ? (useItemCount * splitCount) : useItemCount;

        for (int i = 0; i < generateCount; i++)
            AddItem().InitWithId(i);

        int order = 0;
        foreach (T container in list)
        {
            if (alignment == ChildAlignment.Vertical)
            {
                container.GetRectTransform().localPosition = new Vector3(
                    margin.x,
                    -(margin.y + (itemSize.y + spacing) * order),
                    container.GetRectTransform().localPosition.z);
            }
            order++;
        }
    }

    public ListItem AddItem()
    {
        if (useContainer && (items.Count % splitCount == 0))
        {
            container = Instantiate(containerPrefab).GetComponent<ListItemContainer>();
            container.InitWithId(containers.Count);
            container.transform.SetParent(content.transform);
            container.transform.localPosition = Vector3.zero;
            container.transform.localScale = Vector3.one;
            containers.AddLast(container);
        }

        ListItem item = Instantiate(listItemPrefab);

        if (useContainer)
            item.transform.SetParent(container.transform);
        else
            item.transform.SetParent(content.transform);

        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        items.Add(item);

        if (container != null)
            container.Items.Add(item);
        else
            listItems.AddLast(item);

        return item;
    }

    public void Update()
    {
        if (useContainer && containers.Count > 0)
        {
            ListItemContainer firstContainer = containers.First.Value;
            ListItemContainer lastContainer = containers.Last.Value;

            float itemWidth = (itemSize.x + spacing);
            float itemHeight = (itemSize.y + spacing);

            bool checkTopToBottom =
                (alignment == ChildAlignment.Vertical && content.localPosition.y > (accIndex + 1) * itemHeight);
            bool checkBottomToTop =
                (alignment == ChildAlignment.Vertical && content.localPosition.y < (accIndex) * itemHeight);


            if (accIndex < (itemCount / splitCount)-useItemCount && checkTopToBottom)
            {
                for (int i = 0; i < firstContainer.Items.Count; i++)
                    firstContainer.Items[i].SetIdAndUpdateData(((containers.Last.Value.Id + 1) * splitCount) + i);

                firstContainer.SetIdAndUpdateData(containers.Last.Value.Id + 1);

                if (alignment == ChildAlignment.Vertical)
                    firstContainer.SetLocalPosition(containers.Last.Value.GetLocalPosition() + Vector2.down * itemHeight);

                containers.AddLast(firstContainer);
                containers.RemoveFirst();
                accIndex++;
            }

            if (accIndex > 0 && checkBottomToTop)
            {
                for (int i = 0; i < lastContainer.Items.Count; i++)
                    lastContainer.Items[i].SetIdAndUpdateData(((firstContainer.Id - 1) * splitCount)  + i);

                lastContainer.SetIdAndUpdateData(containers.First.Value.Id - 1);

                if (alignment == ChildAlignment.Vertical)
                    lastContainer.SetLocalPosition(containers.First.Value.GetLocalPosition() + Vector2.up * itemHeight);

                containers.AddFirst(lastContainer);
                containers.RemoveLast();
                accIndex--;
            }
        }
        else if (listItems.Count > 0)
        {
            ListItem firstItem = listItems.First.Value;
            ListItem lastItem = listItems.Last.Value;

            float itemHeight = (itemSize.y + spacing);
            bool checkTopToBottom =
                (alignment == ChildAlignment.Vertical && content.localPosition.y > (accIndex + 1) * itemHeight);
            bool checkBottomToTop =
                (alignment == ChildAlignment.Vertical && content.localPosition.y < (accIndex) * itemHeight);

            if (accIndex < groupCount - useItemCount && checkTopToBottom)
            {
                firstItem.SetIdAndUpdateData(listItems.Last.Value.Id + 1);
                firstItem.SetLocalPosition(listItems.Last.Value.GetLocalPosition() + Vector2.down * itemHeight);
                listItems.AddLast(firstItem);
                listItems.RemoveFirst();
                accIndex++;
            }

            if (accIndex > 0 && checkBottomToTop)
            {
                lastItem.SetIdAndUpdateData(listItems.First.Value.Id - 1);
                lastItem.SetLocalPosition(listItems.First.Value.GetLocalPosition() + Vector2.up * itemHeight);
                listItems.AddFirst(lastItem);
                listItems.RemoveLast();
                accIndex--;
            }
        }
    }
}
