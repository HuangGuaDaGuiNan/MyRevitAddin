using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace FourPlugin.RPackaging
{
    [Transaction(TransactionMode.Manual)]
    class ProjectPackaging : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //载入FourPlugin.dll


            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            string copyFilePath;//储存文件夹路径
            string temFilePath;//临时文件路径
            bool isWorkshared;//当前文档是否开启工作共享
            bool enoughRAM;//内存是否足够
            bool enoughDiskSpace;//储存空间是否足够
            bool purgeUnused;//是否清除未使用项
            bool packageLinkingModels;//是否打包链接模型
            bool clearLinkingModels;//是否清除链接

            string fileFullName;

            #region 环境判断及初始化

            copyFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + doc.Title + ".rvt";

            temFilePath = Constants.TemFolder + @"\\temFile.rvt";
            isWorkshared = doc.IsWorkshared;
            enoughRAM = true;
            enoughDiskSpace = GetHardDiskFreeSpace("C") > 1.5;

            #endregion

            #region 警告
            if (!enoughRAM) message += "内存不足，请关闭一些软件释放运行内存\r\n";
            if (!enoughDiskSpace) message += "C盘临时空间不足，请关闭一些程序释放空间\r\n";

            if (string.IsNullOrEmpty(message)) return Result.Cancelled;
            #endregion

            #region 模型打包

            //分离中心文件
            if (isWorkshared)
            {
                //同步
                doc.SynchronizeWithCentral(new TransactWithCentralOptions(), new SynchronizeWithCentralOptions());
                //创建临时文件
                Document temDoc = uiapp.Application.NewProjectDocument(@"C:\ProgramData\Autodesk\RVT 2019\Templates\China\Construction-DefaultCHSCHS.rte");
                temDoc.SaveAs(temFilePath);
                temDoc.Close(false);
                uiapp.OpenAndActivateDocument(temFilePath);
                temDoc = uiapp.ActiveUIDocument.Document;
                //关闭本地文件
                string docPathName = doc.PathName;
                ModelPath modelPath = doc.GetWorksharingCentralModelPath();
                doc.Close(false);
                //分离模型
                OpenOptions openOptions = new OpenOptions
                {
                    DetachFromCentralOption = DetachFromCentralOption.DetachAndDiscardWorksets
                };
                Document detachDoc = uiapp.Application.OpenDocumentFile(modelPath, openOptions);
                detachDoc.SaveAs(copyFilePath);
                detachDoc.Close(true);
                //重新打开本地文件
                uiapp.OpenAndActivateDocument(docPathName);
                //清理临时文件
                temDoc.Close(false);
                File.Delete(temFilePath);
            }
            else
            {
                doc.Save();
                File.Copy(doc.PathName, copyFilePath);
            }

            #endregion

            return Result.Succeeded;
        }

        public static long GetHardDiskFreeSpace(string hardDiskName)
        {
            long freeSpcae = new long();
            if (!hardDiskName.EndsWith(@":\\"))
                hardDiskName += @":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == hardDiskName)
                {
                    freeSpcae = drive.TotalFreeSpace / (1024 * 1024 * 1024);
                    return freeSpcae;
                }
            }
            return freeSpcae;
        }
    }
}
