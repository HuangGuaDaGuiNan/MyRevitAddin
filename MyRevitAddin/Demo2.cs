using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Threading;

namespace MyRevitAddin
{
    [Transaction(TransactionMode.Manual)]
    class Demo2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var ref_elem = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new RoomSelFilter(), "选择房间");

            Element elem_Room = doc.GetElement(ref_elem);
            View view1 = doc.GetElement(new ElementId(7855232)) as View;

            if (elem_Room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString().StartsWith("次卧"))
            {
                using(Transaction tran=new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855445), true);
                    doc.Regenerate();
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855466), true);
                    doc.Regenerate();
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855485), true);
                    doc.Regenerate();
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855502), true);
                    doc.Regenerate();
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855511), true);
                    doc.Regenerate();
                    tran.Commit();
                }

            }
            else
            {
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855530), true);
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855551), true);
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855570), true);
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855579), true);
                    tran.Commit();
                }
                using (Transaction tran = new Transaction(doc, "create"))
                {
                    tran.Start();
                    Thread.Sleep(800);
                    view1.SetFilterVisibility(new ElementId(7855588), true);
                    tran.Commit();
                }
            }

            return Result.Succeeded;
        }
    }

    class RoomSelFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Autodesk.Revit.DB.Architecture.Room)
                return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
