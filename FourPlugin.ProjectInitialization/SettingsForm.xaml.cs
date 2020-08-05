using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using ARA = Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace FourPlugin.ProjectInitialization
{
    /// <summary>
    /// SettingsForm.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsForm : Window
    {
        string getKey;
        UIDocument uidoc;

        SetProjectCode setProjectCode;
        ExternalEvent clickOkHander;
        public ProjectFeatureCode projectFeatureCode;

        //List<DataSystem> systemList = new List<DataSystem>()
        //{
        //    new DataSystem("龙湖","02")
        //};
        List<string> planDList = new List<string>();


        public SettingsForm(UIDocument uiDocument,string key)
        {
            InitializeComponent();

            uidoc = uiDocument;
            getKey = key;

            setProjectCode = new SetProjectCode();
            clickOkHander = ExternalEvent.Create(setProjectCode);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_System.ItemsSource = ProjectCodeManager.SystemCodeDictionary;
            ComboBox_System.SelectedValuePath = "Key";
            ComboBox_System.DisplayMemberPath = "Value";

            ComboBox_Plan.ItemsSource = planDList;

            projectFeatureCode = new ProjectFeatureCode(uidoc.Document);
        }

        private void Btn_DefineBox_Click(object sender, RoutedEventArgs e)
        {          
            //选择范围框
            Reference reference = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new VBoxSelectionFilter(), "选择一个范围框");
            if (reference != null)
            {
                Lab_BoxId.Content = "id:" + reference.ElementId.IntegerValue.ToString();
            }
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            //判断是否修改



            //如修改
            setProjectCode.FeatureCode = "123321";
            clickOkHander.Raise();

            this.Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBox_System_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool haveValue = ProjectCodeManager.PlanCode.TryGetValue(ComboBox_System.SelectedValue.ToString(), out planDList);


            MessageBox.Show(ProjectCodeManager.PlanCode[ProjectCodeManager.PlanCode.Keys.First()].ToString());
            //ComboBox_Plan.IsEnabled = true;

            MessageBox.Show("hi");

            if (ComboBox_System.SelectedItem != null && haveValue)
            {
                ComboBox_Plan.IsEnabled = false;
                //ComboBox_Plan.ItemsSource = planDList;
            }
        }
    }

    class SetProjectCode : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (String.IsNullOrEmpty(FeatureCode))
            {
                return;
            }

            //to do，缺验证，待补充
            if (!HaveKeyParameter(doc,ProjectCodeManager.KeyParameterGuid))
            {
                using (Transaction tran = new Transaction(doc))
                {
                    tran.Start("创建 参数");
                    CreateKeyParameter(doc.Settings.Categories.get_Item(BuiltInCategory.OST_ProjectInformation), doc, "项目特征码", ParameterType.Text, BuiltInParameterGroup.PG_DATA, ProjectCodeManager.KeyParameterGuid);
                    tran.Commit();
                }
            }

            try
            {
                using (Transaction tran1 = new Transaction(doc))
                {
                    tran1.Start("写入 数据");
                    doc.ProjectInformation.get_Parameter(ProjectCodeManager.KeyParameterGuid).Set(FeatureCode);
                    tran1.Commit();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("写入数据失败" + "\n/\t" + e.Message);
            }

        }

        public string GetName()
        {
            return "CreaterDefineBox";
        }

        public string FeatureCode { get; set; }

        bool HaveKeyParameter(Document document, Guid guid)
        {
            foreach (Parameter para in document.ProjectInformation.Parameters)
            {
                if (para.IsShared && para.GUID.Equals(guid))
                {
                    return true;
                }
            }
            return false;
        }

        bool CreateKeyParameter(Category category, Document document, string parameterName, ParameterType parameterType, BuiltInParameterGroup parameterGroup, Guid guid)
        {
            ARA.Application application = document.Application;
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

    class VBoxSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if ((BuiltInCategory)elem.Category.Id.IntegerValue == BuiltInCategory.OST_VolumeOfInterest)
                return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class ProjectFeatureCode
    {
        private static readonly Dictionary<string, string> SystemCodeDictionary = new Dictionary<string, string>
        {
            {"unknow","unknow" },
            {"01","本则" },
            {"02","龙湖" }
        };
        private static readonly Dictionary<string, string> SpaceDictionary = new Dictionary<string, string>
        {
            {"u","unknow" },
            {"G","公区" },
            {"D","户型" }
        };
        public ProjectFeatureCode(Document document)
        {
            string code = GetProcjecCodeFromDocument(document, ProjectCodeManager.KeyParameterGuid);
            if (String.IsNullOrEmpty(code))
            {
                code = "02.unknow.unkonw";
            }

            code.Trim();
            //检查合法性

            string[] array = code.Split('.');
            string systemCodeStr = array[0];
            string tagCodeStr = array[1];
            string boxIdStr = array[2];

            dataSystem = new DataSystem(SystemCodeDictionary[systemCodeStr], systemCodeStr);
            dataSpace = new DataSpace(SpaceDictionary[tagCodeStr.Substring(0, 1)], tagCodeStr.Substring(0, 1));
            //dataType=new DataType
        }
        
        public DataSystem dataSystem { get; set; }
        DataSpace dataSpace { get; set; }
        DataType dataType { get; set; }

        string GetProcjecCodeFromDocument(Document document, Guid guid)
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
    }

    public class DataSystem
    {
        public DataSystem(string name,string code)
        {
            Name = name;
            Code = code;
        }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    class DataSpace
    {
        public DataSpace(string name,string code)
        {
            Name = name;
            Code = code;
        }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    class DataType
    {
        public DataType(string name,string code)
        {
            Name = name;
            Code = code;
        }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
