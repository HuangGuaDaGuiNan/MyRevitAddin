using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace MyRevitAddin
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            using(Transaction tran=new Transaction(doc))
            {
                tran.Start("create parameter");

                Guid paraGuid = new Guid("B9A98716-517A-493E-9E53-71545A15B0DE");
                TaskDialog.Show("result", CreateKeyParameter(doc.Settings.Categories.get_Item(BuiltInCategory.OST_ProjectInformation), doc, "标准化编码", ParameterType.Text, BuiltInParameterGroup.PG_DATA, paraGuid).ToString());

                tran.Commit();
            }

            return Result.Succeeded;
        }

        bool CreateKeyParameter(Category category, Document document, string parameterName, ParameterType parameterType, BuiltInParameterGroup parameterGroup, Guid guid)
        {
            Application application = document.Application;
            string temShareDefinitionFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/temSharpParam.txt";
            string originalShareDefinitionFilePath = application.SharedParametersFilename;

            if (File.Exists(temShareDefinitionFilePath)) File.Delete(temShareDefinitionFilePath);
            FileStream fileStream = File.Create(temShareDefinitionFilePath);
            fileStream.Close();

            application.SharedParametersFilename = temShareDefinitionFilePath;
            DefinitionFile definitionFile = application.OpenSharedParameterFile();
            DefinitionGroup shareParaGroup = definitionFile.Groups.Create("mySharePara");
            ExternalDefinitionCreationOptions definitionOpt = new ExternalDefinitionCreationOptions(parameterName, parameterType);
            if (guid != null) definitionOpt.GUID = guid;
            Definition definition = shareParaGroup.Definitions.Create(definitionOpt);

            CategorySet categorySet = new CategorySet();
            categorySet.Insert(category);
            bool result = document.ParameterBindings.Insert(definition, new InstanceBinding(categorySet), parameterGroup);

            if (!String.IsNullOrEmpty(originalShareDefinitionFilePath)) application.SharedParametersFilename = originalShareDefinitionFilePath;
            File.Delete(temShareDefinitionFilePath);

            return result;
        }
    }
}
