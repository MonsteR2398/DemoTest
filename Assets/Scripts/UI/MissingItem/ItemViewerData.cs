using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemViewerData : MonoBehaviour
{
    public Image Image;


    void OnEnable()
    {
        if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
            transform.SetAsLastSibling();
    }

}
