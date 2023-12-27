using Cosmos.Gameplay.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class IPJoiningUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private TMP_InputField _ipInputField;

    [SerializeField] private TMP_InputField _portInputField;

    [SerializeField] private Button _joinButton;

    [Inject] private IPUIMediator _ipUIMediator;

    private void Awake()
    {
        _ipInputField.text = IPUIMediator.DEFAULT_IP;
        _portInputField.text = IPUIMediator.DEFAULT_PORT.ToString();
    }

    public void OnJoinButtonClicked()
    {
        _ipUIMediator.JoinWithIP(_ipInputField.text, _portInputField.text);
    }

    /// <summary>
    /// Added to the InputField component's OnValueChanged callback for the Room/IP UI text.
    /// </summary>
    public void SanitizeIPInputFieldText()
    {
        _ipInputField.text = IPUIMediator.SanitizeIP(_ipInputField.text);
        _joinButton.interactable = IPUIMediator.AreIpAddressAndPortValid(_ipInputField.text, _portInputField.text);
    }

    /// <summary>
    /// Added to the InputField component's OnValueChanged callback for the Port UI text.
    /// </summary>
    public void SanitizePortText()
    {
        _portInputField.text = IPUIMediator.SanitizePort(_portInputField.text);
        _joinButton.interactable = IPUIMediator.AreIpAddressAndPortValid(_ipInputField.text, _portInputField.text);
    }

    public void Show()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
}
