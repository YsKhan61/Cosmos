using Cosmos.Infrastructure;
using VContainer;
using VContainer.Unity;

namespace Cosmos.Test
{
    public class Manager : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<ItemBehaviour>();

            MessageChannel<SwitchMessage> messageChannel = new();
            builder.RegisterInstance(messageChannel).AsImplementedInterfaces();

            builder.RegisterInstance(new Fan(messageChannel)).AsImplementedInterfaces();
            builder.RegisterInstance(new Light(messageChannel)).AsImplementedInterfaces();
        }
    }
}
