using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Autodesk.Revit.UI;

namespace FourPlugin.RPackaging
{
    public class ThisApplication : IExternalApplication
    {
        private string m_TabName;

        public ThisApplication(string tabName)
        {
            m_TabName = tabName;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(m_TabName, "成果打包");

            //图纸打包
            PushButtonData pbd_DrawingPackaging = new PushButtonData("FourPlugin_RPackaging_DrawingPackaging", "图纸打包", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "通过一些简单设置进行图纸打包"
            };
            ribbonPanel.AddItem(pbd_DrawingPackaging);
            //模型打包
            PushButtonData pbd_ProjectPackaging = new PushButtonData("FourPlugin_ProjectInitialization_ProjectPackaging", "模型打包", Assembly.GetExecutingAssembly().Location, "FourPlugin.RPackaging.ProjectPackaging")
            {
                ToolTip = "通过一些简单设置进行模型打包"
            };
            ribbonPanel.AddItem(pbd_ProjectPackaging);
            //打包方案
            PushButtonData pbd_ManagerPackagingPlan = new PushButtonData("FourPlugin_ProjectInitialization_ManagerPackagingPlan", "打包方案", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "创建自定义的打包方案，方便日后重复使用"
            };
            ribbonPanel.AddItem(pbd_ManagerPackagingPlan);

            return Result.Succeeded;
        }
    }
}
