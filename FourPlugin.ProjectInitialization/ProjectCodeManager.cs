using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourPlugin.ProjectInitialization
{
    public class ProjectCodeManager
    {
        public static Guid KeyParameterGuid
        {
            get { return new Guid("B9A98716-517A-493E-9E53-71545A15B0DE"); }
        }

        public static readonly Dictionary<string, string> SystemCodeDictionary = new Dictionary<string, string>
        {
            {"01","本则" },
            {"02","龙湖" }
        };

        public static readonly Dictionary<string, List<string>> PlanCode = new Dictionary<string, List<string>>
        {
            {"02",LongForPlanCode }
        };

        private static readonly List<string> LongForPlanCode = new List<string>
        {
            "D1-800",
            "D2-1050",
            "G1",
            "G2"
        };
    }
}
