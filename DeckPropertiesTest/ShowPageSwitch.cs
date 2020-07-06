using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DeckPropertiesTest
{
    [Transaction(TransactionMode.Manual)]
    class ShowPageSwitch : IExternalCommand, IExternalCommandAvailability
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                ThisApplication.thisApp.GetDockableAPIUitility().Initialize(commandData.Application);

                bool isVisible = ThisApplication.thisApp.GetUserDockablePane().IsVisible;
                ThisApplication.thisApp.SetUserDockablePaneVisibility(commandData.Application, !isVisible);
            }
            catch (Exception)
            {
                TaskDialog.Show("test", "因为一些原因，可停靠窗口无法启动");
            }
            return Result.Succeeded;
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            if (applicationData.ActiveUIDocument == null)
                return false;
            else
                return true;
        }
    }
}
