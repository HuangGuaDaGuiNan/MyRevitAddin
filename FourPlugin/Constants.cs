using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace FourPlugin
{
    public class Constants
    {
        public const string TabName = "FourPlugin";

        public static readonly string ModuleLibraryFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\module";
        public static readonly string ResourceFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\resource";

        public static readonly string TemFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\FourPlugin";

    }
}
