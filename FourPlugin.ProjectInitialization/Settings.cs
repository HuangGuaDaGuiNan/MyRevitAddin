using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace FourPlugin.ProjectInitialization
{
    [Transaction(TransactionMode.Manual)]
    class Settings : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Guid paraGuid = new Guid("B9A98716-517A-493E-9E53-71545A15B0DE");
            string key = GetKeyValue(doc, paraGuid);

            //界面
            SettingsForm form = new SettingsForm(commandData.Application.ActiveUIDocument, key);
            form.Show();
            ////如无则需要创建
            //if (String.IsNullOrEmpty(key))
            //{
            //    //
            //}

            //if (key == null)
            //{
            //    CreateKeyParameter(doc.Settings.Categories.get_Item(BuiltInCategory.OST_ProjectInformation), doc, "标准化编码", ParameterType.Text, BuiltInParameterGroup.PG_DATA, paraGuid);
            //}

            return Result.Succeeded;
        }

        string GetKeyValue(Document document,Guid guid)
        {
            foreach (Parameter para in document.ProjectInformation.Parameters)
            {
                if (para.IsShared && para.GUID.Equals(guid))
                {
                    return para.AsString();
                }
            }
            return null;
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
            bool result = document.ParameterBindings.Insert(definition, new TypeBinding(categorySet), parameterGroup);

            if (!String.IsNullOrEmpty(originalShareDefinitionFilePath)) application.SharedParametersFilename = originalShareDefinitionFilePath;
            File.Delete(temShareDefinitionFilePath);

            return result;
        }
    }
}
