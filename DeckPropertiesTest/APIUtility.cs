using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DeckPropertiesTest
{
    public partial class APIUtility
    {
        public void Initialize(UIApplication uiApplication)
        {
            m_uiApplication = uiApplication;
        }

        private UIApplication m_uiApplication;
    }
}
