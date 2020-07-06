using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Autodesk.Revit.UI;

namespace FourPlugin.Demo
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
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(m_TabName, "Demo");

            //图纸排序
            PushButtonData pbd_SortingDrawings = new PushButtonData("FourPlugin_Deom_SortingDrawings", "图纸排序", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "图纸排序"
            };
            ribbonPanel.AddItem(pbd_SortingDrawings);
            //标注轴网
            PushButtonData pbd_DimensionGrid = new PushButtonData("FourPlugin_Deom_DimensionGrid", "标注轴网", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "标注轴网"
            };
            ribbonPanel.AddItem(pbd_DimensionGrid);
            //明细表导出
            PushButtonData pbd_ExportSchedule = new PushButtonData("FourPlugin_Deom_ExportSchedule", "明细表导出", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "明细表导出"
            };
            ribbonPanel.AddItem(pbd_ExportSchedule);
            //打开文件夹
            PushButtonData pbd_OpenFolder = new PushButtonData("FourPlugin_Deom_OpenFolder", "明细表导出", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "打开与当前项目文件相关的文件夹"
            };
            ribbonPanel.AddItem(pbd_OpenFolder);
            //数据修改器
            PushButtonData pbd_DataModifier = new PushButtonData("FourPlugin_Deom_DataModifier", "数据修改器", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "快速管理你的模型数据"
            };
            ribbonPanel.AddItem(pbd_DataModifier);
            //模型自检
            PushButtonData pbd_ModelSelfCheck = new PushButtonData("FourPlugin_Deom_ModelSelfCheck", "自动核查", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "通过自定义的检查方案进行模型自动核查"
            };
            ribbonPanel.AddItem(pbd_ModelSelfCheck);
            //资源浏览器Pro
            PushButtonData pbd_ResourceBrowserPro = new PushButtonData("FourPlugin_Deom_ResourceBrowserPro", "资源浏览器Pro", Assembly.GetExecutingAssembly().Location, "com1")
            {
                ToolTip = "显示或者关闭加强版的资源浏览器"
            };
            ribbonPanel.AddItem(pbd_ResourceBrowserPro);

            return Result.Succeeded;
        }
    }
}
