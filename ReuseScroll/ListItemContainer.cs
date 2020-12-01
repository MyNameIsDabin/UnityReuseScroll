using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListItemContainer : ListItem
{
    public List<ListItem> Items { get; set; } = new List<ListItem>();
    
}
