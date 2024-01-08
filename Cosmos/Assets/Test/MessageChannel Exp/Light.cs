using Cosmos.Infrastructure;
using System;
using UnityEngine;
using VContainer;

namespace Cosmos.Test
{
    public class Light : IItem
    {
        private DisposableGroup _disposableGroup;

        [Inject]
        public Light(ISubscriber<SwitchMessage> messageSubscriber)
        {
            Debug.Log("Light Registered");

            IDisposable messageSubscriberDisposable = messageSubscriber.Subscribe(ToggleLight);
            _disposableGroup = new();
            _disposableGroup.Add(messageSubscriberDisposable);
        }

        private void ToggleLight(SwitchMessage message)
        {
            Debug.Log("Light: " + message.message);
        }
    }
}