using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

using System.Reflection;
using System.Collections;

namespace FourPlugin
{
    public class FourApplication : IExternalApplication
    {
        ArrayList plugins = new ArrayList();

        public FourApplication()
        {
            //载入DLL
            string moduleLibraryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\module";
            foreach (string pluginFile in Directory.GetFiles(moduleLibraryPath, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(pluginFile);
                foreach(Type type in assembly.GetTypes())
                {
                    if (type.GetInterface("IExternalApplication") != null)
                    {
                        plugins.Add(assembly.CreateInstance(type.FullName, true, BindingFlags.Default, null, new object[] { Constants.TabName }, null, null));
                    }
                }
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {

            //关闭模块界面
            foreach (var plugin in plugins)
            {
                Type type = plugin.GetType();
                ; MethodInfo onShutdown = type.GetMethod("OnShutdown");
                onShutdown.Invoke(plugin, new object[] { application });
            }


            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //检查更新
            //to do

            //创建Tab
            application.CreateRibbonTab(Constants.TabName);
            //检查功能页

            //创建模块界面
            foreach(var plugin in plugins)
            {
                Type type = plugin.GetType();
;               MethodInfo onStartup = type.GetMethod("OnStartup");
                onStartup.Invoke(plugin, new object[] { application });
            }

            //创建默认面板


            return Result.Succeeded;
        }
    }
}
