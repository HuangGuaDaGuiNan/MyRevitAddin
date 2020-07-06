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
    class ModifyValue : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            ModifyValueForm uiForm = new ModifyValueForm(document);
            System.Windows.Interop.WindowInteropHelper thisForm = new System.Windows.Interop.WindowInteropHelper(uiForm)
            {
                Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle
            };
            if (uiForm.ShowDialog() == true)
            {
                using (Transaction tran = new Transaction(document, "修改 参数值"))
                {
                    tran.Start();

                    //0——文本替换
                    //1——加前缀
                    //2——加后缀
                    switch (uiForm.ModifyType)
                    {
                        case 0:
                            foreach (Element element in uiForm.GetElements)
                            {
                                Parameter parameter = element.get_Parameter(uiForm.ParameterDefinition);
                                if (parameter != null)
                                {
                                    parameter.Set(parameter.AsString().Replace(uiForm.Text1, uiForm.Text2));
                                }

                            }
                            break;
                        case 1:
                            foreach (Element element in uiForm.GetElements)
                            {
                                Parameter parameter = element.get_Parameter(uiForm.ParameterDefinition);
                                if (parameter != null)
                                {
                                    if (parameter.AsString() != "")
                                    {
                                        parameter.Set(uiForm.Text1);
                                    }
                                    else
                                    {
                                        parameter.Set(parameter.AsString().Insert(0, uiForm.Text1));
                                    }

                                }
                            }
                            break;
                        case 2:
                            foreach (Element element in uiForm.GetElements)
                            {
                                Parameter parameter = element.get_Parameter(uiForm.ParameterDefinition);
                                if (parameter != null)
                                {
                                    parameter.Set(parameter.AsString() + uiForm.Text1);
                                }
                            }
                            break;
                    }

                    tran.Commit();
                }
            }

            return Result.Succeeded;
        }

    }
}
