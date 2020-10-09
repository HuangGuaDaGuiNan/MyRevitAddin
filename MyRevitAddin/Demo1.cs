using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Threading;

namespace MyRevitAddin
{
    [Transaction(TransactionMode.Manual)]
    class Demo1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            Demo1Updater closeUpdater = new Demo1Updater(uiapp.ActiveAddInId);
            if (UpdaterRegistry.IsUpdaterRegistered(closeUpdater.GetUpdaterId())) UpdaterRegistry.UnregisterUpdater(closeUpdater.GetUpdaterId());

            //注册更新
            Demo1Updater demo1Updater = new Demo1Updater(uiapp.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(demo1Updater);
            ElementCategoryFilter roomFilter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);
            UpdaterRegistry.AddTrigger(demo1Updater.GetUpdaterId(), roomFilter, Element.GetChangeTypeElementAddition());


            RevitCommandId room_CommandId = RevitCommandId.LookupPostableCommandId(PostableCommand.Room);
            if (uiapp.CanPostCommand(room_CommandId))
            {
                uiapp.PostCommand(room_CommandId);
            }


            ////注销更新
            //UpdaterRegistry.UnregisterUpdater(demo1Updater.GetUpdaterId());

            return Result.Succeeded;
        }
    }

    class Demo1Updater : IUpdater
    {
        static AddInId m_appId;
        static UpdaterId m_updaterId;

        public Demo1Updater(AddInId addInId)
        {
            m_appId = addInId;
            m_updaterId = new UpdaterId(m_appId, new Guid("075EA611-BC06-481D-9D56-D6C385E8977F"));
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();

            //TaskDialog.Show("title", "working");

            FilteredElementCollector elements1 = new FilteredElementCollector(doc);
            foreach (Element elem in elements1.OfCategory(BuiltInCategory.OST_Rooms))
            {
                if (elem.get_Parameter(BuiltInParameter.ROOM_NAME).AsString().StartsWith("房间"))
                    elem.get_Parameter(BuiltInParameter.ROOM_NAME).Set("次卧1");
            }

            Thread.Sleep(3000);

            View view1 = doc.GetElement(new ElementId(7853839)) as View;
            View view3d = doc.GetElement(new ElementId(7853559)) as View;

            view1.SetFilterVisibility(new ElementId(7853820), true);
            view3d.SetFilterVisibility(new ElementId(7853820), true);


        }

        public string GetAdditionalInformation()
        {
            return "this is a demo1";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "demo1Update";
        }
    }
}
