using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FourPlugin.ProjectInitialization
{
    class PathHelper
    {
        public static class ResourcePath
        {
            //项目资源文件
            public static string TemplateResPath
            {
                get
                {
                    string templateResPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\\DebugTemplateRes.rvt";
                    if (!File.Exists(templateResPath))
                    {
                        templateResPath = @"K:\03-本则BIM\03_BIM项目资源\07_Revit通用插件\TemplateRes.rvt";
                    }
                    if (!File.Exists(templateResPath))
                    {
                        return null;
                    }
                    return templateResPath;
                }
            }

            public static string FamilyLibraryPath
            {
                get
                {
                    string familyLibraryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\\Family";
                    if (!Directory.Exists(familyLibraryPath))
                    {
                        familyLibraryPath = @"K:\03-本则BIM\03_BIM项目资源\07_Revit通用插件\Family";
                    }
                    if (!Directory.Exists(familyLibraryPath))
                    {
                        return null;
                    }
                    return familyLibraryPath;
                }
            }
        }
    }
}
