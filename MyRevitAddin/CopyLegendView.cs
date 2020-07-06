using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;


namespace MyRevitAddin
{
    [Transaction(TransactionMode.Manual)]
    class CopyLegendView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<ElementId> elemIdList = new List<ElementId>();
            Document templateDoc = null;
            ViewSheet tagVS = null;

            #region 打开模板文档,获得将要被复制的元素

            //获得文档
            templateDoc = uidoc.Application.Application.OpenDocumentFile(@"C:\Users\MyComputer\Desktop\TemplateRes.rvt");
            //获得视图
            FilteredElementCollector tagView = new FilteredElementCollector(doc);
            foreach(ViewSheet vs in tagView.OfClass(typeof(ViewSheet)))
            {
                if (vs.Name.StartsWith("设计说明（一）"))
                {
                    tagVS = vs;
                    break;
                }
            }
            //获得元素
            ElementMulticategoryFilter categoryFilter = new ElementMulticategoryFilter(new List<BuiltInCategory> {
                BuiltInCategory.OST_LegendComponents,
                BuiltInCategory.OST_TextNotes,
                BuiltInCategory.OST_Lines,
                BuiltInCategory.OST_GenericAnnotation,
                BuiltInCategory.OST_InsulationLines,
                BuiltInCategory.OST_Dimensions,
                BuiltInCategory.OST_IOSDetailGroups,
                BuiltInCategory.OST_RevisionClouds,
                BuiltInCategory.OST_RevisionCloudTags,
                BuiltInCategory.OST_DetailComponents,
                BuiltInCategory.OST_DetailComponentTags
            });
            FilteredElementCollector elems = new FilteredElementCollector(doc, tagVS.Id);
            foreach (Element elem in elems.WherePasses(categoryFilter))
            {
                if (elem.GroupId.IntegerValue == -1)
                {
                    elemIdList.Add(elem.Id);
                }
            }

            #endregion

            #region 新建图纸



            #endregion

            #region 复制元素
            #endregion

            #region 关闭模板文档
            #endregion



            CopyPasteOptions options = new CopyPasteOptions();
            options.SetDuplicateTypeNamesHandler(new CopyHandler());


            View v = templateDoc.GetElement(new ElementId(333550)) as View;

            using (Transaction tran = new Transaction(templateDoc, "CopyLegendView"))
            {
                tran.Start();

                //创建图纸
                ViewSheet viewSheet = ViewSheet.Create(doc, new ElementId(157755));


                ViewDrafting.Create(doc, new ElementId(54529));
                
                //View.copy
                //var s = doc.ActiveView.Scale;
                //ElementTransformUtils.CopyElements(doc.ActiveView, elemIdList, v, Transform.CreateTranslation(new XYZ(0, 0, 0)), options);
                //v.Scale = s;

                tran.Commit();
            }

            return Result.Succeeded;
        }

        public class CommandEnable : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
            {
                if (applicationData.ActiveUIDocument.ActiveView.ViewType == ViewType.Legend)
                    return true;
                return false;
            }
        }

        private class CopyHandler: IDuplicateTypeNamesHandler
        {
            public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
            {
                return DuplicateTypeAction.UseDestinationTypes;
            }
        }
    }
}
