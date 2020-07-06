using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Exceptions;

namespace DeckPropertiesTest
{
    [Transaction(TransactionMode.Manual)]
    public class ThisApplication : IExternalApplication
    {
        private static readonly DockablePaneId userDockablePaneId = new DockablePaneId(new Guid("A782B040-98DD-4383-B114-F5717C682CBF"));

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            thisApp = this;

            CreateUserDockablePane();


            return Result.Succeeded;
        }

        #region 注册可停靠页面的方法

        public void RegisterDockableWindow(UIApplication uiApplication)
        {
            uiApplication.RegisterDockablePane(userDockablePaneId, "资源浏览器", ThisApplication.thisApp.GetUserDockablePane() as IDockablePaneProvider);
        }

        public void RegisterDockableWindow(UIControlledApplication uiControlledApplication)
        {            
            uiControlledApplication.RegisterDockablePane(userDockablePaneId, "资源浏览器", ThisApplication.thisApp.GetUserDockablePane() as IDockablePaneProvider);
        }

        #endregion

        /// <summary>
        /// 创建可停靠页面
        /// </summary>
        public void CreateUserDockablePane()
        {
            m_userDockablePane = new UserDockablePane();
        }

        /// <summary>
        /// 设置可停靠页面的可见性
        /// </summary>
        /// <param name="uiApplication"></param>
        /// <param name="state"></param>
        public void SetUserDockablePaneVisibility(UIApplication uiApplication, bool state)
        {
            DockablePane pane = uiApplication.GetDockablePane(userDockablePaneId);
            if (pane != null)
            {
                if (state)
                    pane.Show();
                else
                    pane.Hide();
            }
        }

        /// <summary>
        /// 获得可停靠页面
        /// </summary>
        /// <returns></returns>
        public UserDockablePane GetUserDockablePane()
        {
            if (!IsUserDdockablePaneAvailable())
            {
                throw new System.InvalidOperationException("悬浮页面未被创建");
            }
            return m_userDockablePane;
        }

        /// <summary>
        /// 检测可停靠页面是否已生成
        /// </summary>
        /// <returns></returns>
        public bool IsUserDdockablePaneAvailable()
        {
            if (m_userDockablePane == null)
                return false;

            //可见或不可见，返回true，否则返回false
            bool isAvailable = true;
            try { bool isVisible = m_userDockablePane.IsVisible; }
            catch (Exception)
            {
                isAvailable = false;
            }
            return isAvailable;
        }

        public APIUtility GetDockableAPIUitility() { return m_APIUtility; }

        UserDockablePane m_userDockablePane;
        internal static ThisApplication thisApp = null;
        private APIUtility m_APIUtility;
    }
}
