using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace MyRevitAddin
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            var documentSet = uiapp.Application.Documents;

            Document doc12 = null;
            Document doc13 = null;

            foreach(Document doc in documentSet)
            {
                if (doc.Title.Contains("12"))
                {
                    doc12 = doc;
                }
                else
                {
                    doc13 = doc;
                }
            }

            List<ElementId> ids = new List<ElementId>();
            ids.Add(new ElementId(334084));

            View sourView = doc12.GetElement(new ElementId(333728)) as View;

            using(Transaction tran=new Transaction(doc13))
            {
                tran.Start("copy");

                ElementTransformUtils.CopyElements(sourView, ids, doc13.ActiveView, Transform.CreateTranslation(new XYZ(0, 0, 0)), new CopyPasteOptions());

                tran.Commit();
            }

            return Result.Succeeded;
        }
    }
}
