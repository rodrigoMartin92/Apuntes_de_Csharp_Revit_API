#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

// CARPETAS DONDE ESTA INSTALADO EL PLUG IN
// C:\Users\ASUS\AppData\Roaming\Autodesk\Revit\Addins\2023
//

// Plantilla: "Revit 2023 Addin


namespace Apuntes_de_Csharp_Revit_API
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        // Este metodo se ejecuta cuando la aplicacion se inicia 
        {
            TaskDialog.Show("TaskDialog.Show", "TaskDialog.Show");
            MessageBox.Show("MessageBox.Show");
            // Linea de texto en pantalla simple

            string CadenaDeTexto_1 = "CadenaDeTexto_1";
            string CadenaDeTexto_2 = "CadenaDeTexto_2";
            string CadenaDeTexto_3 = "CadenaDeTexto_3";

            TaskDialog.Show("Formato prolijo", string.Format("String 1 {0}, String 2 {1}, String 3 {2}",
                CadenaDeTexto_1, CadenaDeTexto_2, CadenaDeTexto_3));
            // Este es un formato de escritura mas "profesional" y prolijo.

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        // Este metodo se ejecuta cuando la aplicacion se cierra 
        {
            return Result.Succeeded;
        }


        // En revit podemos encontrar 4 tipos de objetos: las categorias, las familias, los simbolos y las instancias.

        /* Al crear un proyecto nuevo con la plantilla, se deben revisar las referencias:
        RevitAPI
        RevitAPIUI
        System
        System.Windows.Forms

        -> Estas referencias deben tener su propiedad "copia local" en "Flase", sino va a copiar muchos megas innecesariamente.


         */

    }

}
