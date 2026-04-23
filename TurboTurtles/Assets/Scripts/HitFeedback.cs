using System.ComponentModel;
using TMPro;
using UnityEngine;

public class HitFeedback : MonoBehaviour
{
    public static HitFeedback instance;

    [SerializeField] TMP_Text hitText;
    [SerializeField] float displayTime = 0.2f;

    private void Awake()
    {
        instance = this;
        Debug.Log("HitFeedback is ready");
    }

    public void ShowHit()
    {
        hitText.gameObject.SetActive(true);
        CancelInvoke("HideHit");
        Invoke("HideHit", displayTime);
    }

    void HideHit()
    {
        hitText.gameObject.SetActive(false);
    }
}
