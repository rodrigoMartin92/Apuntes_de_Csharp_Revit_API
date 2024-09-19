#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

#endregion

// CARPETAS DONDE ESTA INSTALADO EL PLUG IN
// C:\Users\ASUS\AppData\Roaming\Autodesk\Revit\Addins\2023
// 

namespace Apuntes_de_Csharp_Revit_API
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
