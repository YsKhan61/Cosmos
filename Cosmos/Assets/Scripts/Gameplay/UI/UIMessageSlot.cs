using System.Collections;
using UnityEngine;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// A single slot of message to be displayed in the message feed.
    /// </summary>
    public class UIMessageSlot : MonoBehaviour
    {
        /*private const string HIDE_TRIGGER = "Hide";
        private const string DISPLAY_TRIGGER = "Display";*/

        /*[SerializeField]
        Animator m_Animator;*/

        [SerializeField]
        TMPro.TextMeshProUGUI m_TextLabel;

        /*[SerializeField]
        float m_HideDelay = 10;*/
        public bool IsDisplaying { get; private set; }

        /// <summary>
        /// The sender's message will be displayed in the right side of the message feed.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isRightAlligned"></param>
        public void Display(string text, bool isRightAlligned = false)
        {
            m_TextLabel.text = text;

            if (isRightAlligned)
                m_TextLabel.alignment = TMPro.TextAlignmentOptions.Right;

            transform.parent.SetAsLastSibling();

            /*if (!IsDisplaying)
            {
                IsDisplaying = true;
                // m_Animator.SetTrigger(DISPLAY_TRIGGER);
                // StartCoroutine(HideCoroutine());
                m_TextLabel.text = text;

                if (isRightAlligned)
                    m_TextLabel.alignment = TMPro.TextAlignmentOptions.Right;

                transform.parent.SetAsLastSibling();
            }*/
        }

        /*IEnumerator HideCoroutine()
        {
            yield return new WaitForSeconds(m_HideDelay);
            m_Animator.SetTrigger(HIDE_TRIGGER);
        }

        public void Hide()
        {
            if (IsDisplaying)
            {
                IsDisplaying = false;
            }
        }*/
    }
}
