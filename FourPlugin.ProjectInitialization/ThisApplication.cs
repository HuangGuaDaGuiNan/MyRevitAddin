using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;

namespace FourPlugin.ProjectInitialization
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
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(m_TabName, "项目初始化");

            //设置
            PushButtonData pbd_Settings = new PushButtonData("FourPlugin_ProjectInitialization_Settings", "设置", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "定义一些信息，用于作为初始化的依据"
            };
            ribbonPanel.AddItem(pbd_Settings);
            //资源初始化
            PushButtonData pbd_BasicInitialization = new PushButtonData("FourPlugin_ProjectInitialization_BasicInitialization", "资源初始化", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "初始化项目模板和族"
            };
            ribbonPanel.AddItem(pbd_BasicInitialization);
            //图纸初始化
            PushButtonData pbd_DrawingInitialization = new PushButtonData("FourPlugin_ProjectInitialization_DrawingInitialization", "图纸初始化", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "初始化图纸"
            };
            ribbonPanel.AddItem(pbd_DrawingInitialization);

            return Result.Succeeded;
        }
    }
}
