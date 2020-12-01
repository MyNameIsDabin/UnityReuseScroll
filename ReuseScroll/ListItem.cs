using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListItem : MonoBehaviour
{
    public int Id { get; set; } = -1;

    [SerializeField]
    private RectTransform rectTransform = null;

    void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public virtual void InitWithId(int id)
    {
        SetIdAndUpdateData(id);
    }

    public virtual void OnUpdateData()
    {
        
    }

    public void SetIdAndUpdateData(int id)
    {
        Id = id;
        OnUpdateData();
    }


    public void SetLocalPosition(Vector2 position)
    {
        rectTransform.localPosition = position;
    }

    public Vector2 GetLocalPosition()
    {
        return rectTransform.localPosition;
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }
}
