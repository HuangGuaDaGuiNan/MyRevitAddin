using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace FourPlugin.RPackaging
{
    [Transaction(TransactionMode.Manual)]
    class ProjectPackaging : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //TaskDialog.Show("hi", "hello Revit");
            Document doc = commandData.Application.ActiveUIDocument.Document;

            #region 检查当前文档，判断打包方案
            //test


            #endregion
            #region 模型打包
            #endregion

            return Result.Succeeded;
        }
    }
}
