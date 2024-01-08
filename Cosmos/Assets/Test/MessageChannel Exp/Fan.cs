using Cosmos.Infrastructure;
using System;
using UnityEngine;
using VContainer;

namespace Cosmos.Test
{
    public class Fan : IItem
    {
        private DisposableGroup _disposableGroup;

        [Inject]
        public Fan(ISubscriber<SwitchMessage> messageSubscriber)
        {
            Debug.Log("Fan Registered");

            IDisposable messageSubscriberDisposable = messageSubscriber.Subscribe(ToggleFan);
            _disposableGroup = new();
            _disposableGroup.Add(messageSubscriberDisposable);
        }

        private void ToggleFan(SwitchMessage message)
        {
            Debug.Log("Fan: " + message.message);
        }
    }
}