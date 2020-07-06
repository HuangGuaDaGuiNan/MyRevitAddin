using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace FourPlugin.ProjectInitialization
{
    class Settings : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //检查是否有固定数据

            return Result.Succeeded;
        }
    }
}
