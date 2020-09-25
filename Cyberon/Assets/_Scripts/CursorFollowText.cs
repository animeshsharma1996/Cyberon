using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CursorFollowText : MonoBehaviour
{

   // Vector3 offset = new Vector3(50.0f, 5.0f, 0.0f);
    TMP_Text tmptext = null;

    void Start()
    {
        tmptext = gameObject.GetComponent<TMP_Text>();
        tmptext.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;// + offset;

        if (!Cursor.visible)
        {
            tmptext.enabled = false;
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            tmptext.enabled = false;
            return;
        }
        tmptext.enabled = true;
    }

    public void SetText(string text)
    {
        tmptext.text = text;
    }

    public void SetColor(Color color)
    {
        tmptext.color = color;
    }
}
