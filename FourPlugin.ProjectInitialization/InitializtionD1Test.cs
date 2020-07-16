using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Macros;

namespace FourPlugin.ProjectInitialization
{
    [Transaction(TransactionMode.Manual)]
    class InitializtionD1Test : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
          
            if (PathHelper.ResourcePath.TemplateResPath == null)
            {
                message = "未检测到标准样板";
                return Result.Failed;
            }

            if (PathHelper.ResourcePath.FamilyLibraryPath == null)
            {
                message = "未检测到标准族库";
                return Result.Failed;
            }

            //打开样板文档
            Document templateResDoc = uidoc.Application.Application.OpenDocumentFile(PathHelper.ResourcePath.TemplateResPath);

            using(Transaction tran=new Transaction(doc))
            {
                tran.Start("初始化资源");

                //线样式
                CopyLineElement(templateResDoc, doc);
                //视图样板
                CopyViewTemplate(templateResDoc, doc);
                //载入族
                DirectoryInfo directoryInfo = new DirectoryInfo(PathHelper.ResourcePath.FamilyLibraryPath);
                foreach(FileInfo fileInfo in directoryInfo.GetFiles("*.rfa"))
                {
                    LoadFamily(doc, fileInfo.FullName);
                }
                //初始化设计说明
                FilteredElementCollector elems = new FilteredElementCollector(doc);
                foreach (FamilySymbol titleBlackType in elems.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks))
                {
                    if (titleBlackType.Name == "LH-A3图框" && titleBlackType.FamilyName == "LH-A3图框")
                    {
                        FilteredElementCollector views = new FilteredElementCollector(templateResDoc);
                        foreach (ViewSheet vs in views.OfClass(typeof(ViewSheet)))
                        {
                            if (vs.Name.StartsWith("D-02") || vs.Name.StartsWith("D1_"))
                            {
                                CreateGeneralDrawing(templateResDoc, doc, titleBlackType.Id, vs);
                            }
                        }

                        break;
                    }
                }

                tran.Commit();
            }

            //关闭样板文档
            templateResDoc.Close(false);

            return Result.Succeeded;
        }

        bool CopyLineElement(Document sourceDocument, Document destinationDocument)
        {
            string tagViewName = "Viewr_LineStyle";

            View view = GetViewByName(sourceDocument, tagViewName);
            if (view == null) return false;

            IList<ElementId> copyElemIdList = new List<ElementId>();
            FilteredElementCollector elems = new FilteredElementCollector(sourceDocument, view.Id);
            foreach (Element elem in elems.OfCategory(BuiltInCategory.OST_Lines))
            {
                DetailLine detailLine = elem as DetailLine;
                if (detailLine != null)
                    copyElemIdList.Add(detailLine.LineStyle.Id);
            }

            //copyElemIdList.GroupBy(a => a.IntegerValue).Select(b => b.First());
            copyElemIdList.Distinct(new ElementIdComparer());

            CopyPasteOptions options = new CopyPasteOptions();
            options.SetDuplicateTypeNamesHandler(new ElementCopyCoverHandler());
            ElementTransformUtils.CopyElements(sourceDocument, copyElemIdList, destinationDocument, null, options);

            return true;
        }

        bool CopyViewTemplate(Document sourceDocument,Document destinationDocument)
        {
            IList<ElementId> copyElemIdList = new List<ElementId>();
            FilteredElementCollector elems = new FilteredElementCollector(sourceDocument);
            foreach(View view in elems.OfClass(typeof(View)))
            {
                if (view.IsTemplate)
                    copyElemIdList.Add(view.Id);
            }

            CopyPasteOptions options = new CopyPasteOptions();
            options.SetDuplicateTypeNamesHandler(new ElementCopyCoverHandler());
            ElementTransformUtils.CopyElements(sourceDocument, copyElemIdList, destinationDocument,null, options);

            return true;
        }

        bool LoadFamily(Document document,string familyPath)
        {
            Family family = null;
            try
            {
                document.LoadFamily(familyPath, out family);
            }
            catch
            {
                return false;
            }
            return true;
        }

        bool CreateGeneralDrawing(Document sourceDocument,Document destinationDocument, ElementId titleBlockTypeId, ViewSheet viewSheet)
        {
            try
            {
                ViewSheet destiationView = ViewSheet.Create(destinationDocument, titleBlockTypeId);
                destiationView.SheetNumber = viewSheet.SheetNumber.Replace("D1_", "");
                destiationView.Name = viewSheet.Name;

                IList<ElementId> copyElemIdList = new List<ElementId>();
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
                FilteredElementCollector elems = new FilteredElementCollector(sourceDocument, viewSheet.Id);
                foreach (Element elem in elems.WherePasses(categoryFilter))
                {
                    if (elem.GroupId.IntegerValue == -1)
                    {
                        copyElemIdList.Add(elem.Id);
                    }
                }

                CopyPasteOptions options = new CopyPasteOptions();
                options.SetDuplicateTypeNamesHandler(new ElementCopyCoverHandler());
                ElementTransformUtils.CopyElements(viewSheet, copyElemIdList, destiationView, Transform.CreateTranslation(new XYZ(0, 0, 0)), options);
            }
            catch
            {
                return false;
            }
            return true;
        }

        View GetViewByName(Document document,string name)
        {
            FilteredElementCollector elements = new FilteredElementCollector(document);
            foreach (View view in elements.OfClass(typeof(View)))
            {
                if (view.Name == name)
                    return view;
            }
            return null;
        }

        private class ElementCopyCoverHandler : IDuplicateTypeNamesHandler
        {
            public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
            {
                return DuplicateTypeAction.UseDestinationTypes;
            }
        }

        private class ElementIdComparer : IEqualityComparer<ElementId>
        {
            public bool Equals(ElementId x, ElementId y)
            {
                return x.IntegerValue.Equals(y.IntegerValue);
            }

            public int GetHashCode(ElementId obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
