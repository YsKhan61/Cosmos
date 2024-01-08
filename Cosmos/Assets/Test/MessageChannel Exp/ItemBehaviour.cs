using Cosmos.Infrastructure;
using System;
using UnityEngine;
using VContainer;

namespace Cosmos.Test
{
    public interface IItem { }
    public class ItemBehaviour : MonoBehaviour
    {
        private DisposableGroup _disposableGroup;

        // [Inject] private Fan _fan;
        // [Inject] private Light _light;

        [Inject]
        public void InjectAndInitialize(ISubscriber<SwitchMessage> messageSubscriber)
        {
            Debug.Log("Item Registered");

            IDisposable messageSubscriberDisposable = messageSubscriber.Subscribe(ToggleItem);
            _disposableGroup = new();
            _disposableGroup.Add(messageSubscriberDisposable);
        }

        private void ToggleItem(SwitchMessage message)
        {
            Debug.Log("Item: " + message.message);

            // if (_fan != null) Debug.Log("Fan is present in Item");
            // if (_light != null) Debug.Log("Light is present in Item");
        }
    }
}