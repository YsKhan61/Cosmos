using System;
using System.Collections;
using TMPro;
using UnityEngine;


namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Display or Hide the notification ui
    /// </summary>
    public class NotificationUI : MonoBehaviour
    {
        const string DISPLAY = "Display";
        const string HIDE = "Hide";

        [SerializeField]
        Animator _animator;

        [SerializeField]
        TextMeshProUGUI _statusText;

        private void Start()
        {
            HidePanel();
        }

        public void DisplayStatus(string text, float secondsToWaitBeforeAutoClose)
        {
            _statusText.text = text;
            ShowPanel();

            if (secondsToWaitBeforeAutoClose > 0)
                StartCoroutine(ClosePanelAfterSeconds(secondsToWaitBeforeAutoClose));
        }

        IEnumerator ClosePanelAfterSeconds(float secondsToWaitBeforeAutoClose)
        {
            yield return new WaitForSeconds(secondsToWaitBeforeAutoClose);
            HidePanel();
        }

        private void ShowPanel()
        {
            _animator.SetTrigger(DISPLAY);
        }

        private void HidePanel()
        {
            _animator.SetTrigger(HIDE);
        }
    }
}