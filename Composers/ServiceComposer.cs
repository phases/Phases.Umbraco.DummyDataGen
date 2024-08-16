using DummyDataGen.Notification;
using DummyDataGen.Services;
using DummyDataGen.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace DummyDataGen.Composers
{
    public class ServiceComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IuDummyNodeService, uDummyNodeService>();
            builder.AddNotificationHandler<MenuRenderingNotification, TreeNotificationHandler>();
        }
    }
}
