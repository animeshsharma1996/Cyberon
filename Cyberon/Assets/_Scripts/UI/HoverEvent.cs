using UnityEngine;

public class HoverEvent : MonoBehaviour
{
    [SerializeField]
    private string nameButton = "";

    public void Trigger()
    {
        UIManager.Instance.cursorFollowText.SetText(nameButton);
    }

}
