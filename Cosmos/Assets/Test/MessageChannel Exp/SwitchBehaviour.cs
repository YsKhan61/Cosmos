using Cosmos.Infrastructure;
using UnityEngine;
using VContainer;

namespace Cosmos.Test
{
    public class SwitchBehaviour : MonoBehaviour
    {
        [Inject]
        private IPublisher<SwitchMessage> _jumpMessagePublisher;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                _jumpMessagePublisher.Publish(new SwitchMessage()
                {
                    message = "ON"
                });
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.S))
            {
                _jumpMessagePublisher.Publish(new SwitchMessage()
                {
                    message = "OFF"
                });
            }
        }
    }

}