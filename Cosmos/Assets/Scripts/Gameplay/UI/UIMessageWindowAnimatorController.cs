using UnityEngine;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Scale up or Scale down the entire message window using two buttons.
    /// </summary>
    public class UIMessageWindowAnimatorController : MonoBehaviour
    {
        const string SCALE_UP = "ScaleUp";
        const string SCALE_DOWN = "ScaleDown";

        [SerializeField]
        Animator m_Animator;

        private void Start()
        {
            m_Animator.gameObject.transform.localScale = Vector3.zero;
        }

        public void ScaleUp()
        {
            m_Animator.SetTrigger(SCALE_UP);
        }

        public void ScaleDown()
        {
            m_Animator.SetTrigger(SCALE_DOWN);
        }
    }
}