using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_Text message;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private GameObject viewLayout;

    public void Subscribe(UnityAction createGame, UnityAction joinGame)
    {
        hostButton.onClick.AddListener(createGame);
        clientButton.onClick.AddListener(joinGame);
    }

    public void SetTypeText(string text)
    {
        typeText.text = text;
    }

    public void SetMessageText(string text)
    {
        message.text = text;
    }

    public void SetActiveView(bool value)
    {
        if (viewLayout != null)
            viewLayout.SetActive(value);
    }

    private void OnDestroy()
    {
        hostButton.onClick.RemoveAllListeners();
        clientButton.onClick.RemoveAllListeners();
    }
}
