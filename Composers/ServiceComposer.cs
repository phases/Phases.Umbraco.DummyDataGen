using DummyDataGen.Notification;
using DummyDataGen.Services.Interface;
using DummyDataGen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Microsoft.Extensions.DependencyInjection;

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
