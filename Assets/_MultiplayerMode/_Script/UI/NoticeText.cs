using TMPro;
using UnityEngine;
public class NoticeText : MonoBehaviour
{
    public GameObject textVFX;
    public TextMeshProUGUI noticeText;
    public void DisplayText(string content)
    {
        CancelInvoke("OffText");
        textVFX.gameObject.SetActive(false);
        textVFX.gameObject.SetActive(true);
        noticeText.gameObject.SetActive(true);
        noticeText.text = content;
        Invoke("OffText", 1.5f);
    }
    void OffText()
    {
        textVFX.gameObject.SetActive(false);
        noticeText.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        OffText();
    }
}
