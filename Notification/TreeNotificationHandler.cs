using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace DummyDataGen.Notification
{
    public class TreeNotificationHandler : INotificationHandler<MenuRenderingNotification>
    {
        private readonly IBackOfficeSecurity _backOfficeSecurity;

        public TreeNotificationHandler(IBackOfficeSecurity backOfficeSecurity)
        {
            _backOfficeSecurity = backOfficeSecurity;
        }

        public void Handle(MenuRenderingNotification notification)
        {

            if (!string.IsNullOrWhiteSpace(notification?.NodeId))
            {
                if (!(notification.TreeAlias == "content"))
                    return;

                if (Convert.ToInt32(notification.NodeId) >= -1 && _backOfficeSecurity?.CurrentUser?.IsAdmin() == true)
                {
                    //string menuTitle = Convert.ToInt32(notification.NodeId) == -1 ? "Import Nodes Under Content" : "Import Nodes Under The Selected Node";
                    MenuItem menuItem2 = new MenuItem("uDummyNodeCreate", "Create Dummy Node");
                    menuItem2.LaunchDialogView("/App_Plugins/DummyDataGen/html/uCreateDummyNodeV2.html?v=3.2", "Create Dummy Node");
                    menuItem2.Icon = "page-add";
                    notification.Menu.Items.Add(menuItem2);
                }
            }
        }
    }
}
