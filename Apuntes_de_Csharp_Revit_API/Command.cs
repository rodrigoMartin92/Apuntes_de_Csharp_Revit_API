#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace Apuntes_de_Csharp_Revit_API
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]   // Esta linea configura que los cambios se realizan a travez de transacciones
                                                // Esta la opcion de solo lectura tambien, pero seria rarisimo que se usara.
    [Journaling(JournalingMode.UsingCommandData)] // Esta linea sirve para generar un archivo que va a recolectar
                                                  // los errores que cometa el usuario
    public class Command : IExternalCommand // "IExternalCommand" es una interfaz de Revit, exige que este el metodo Excecute.
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,   // "message" es una variable que podemos modificar para ponerla en el mensaje de error
                                // Se utiliza cunado retornamos "return Result.Failed" o "return Result.Cancelled"
          ElementSet elements // Aca estan los elementos que han participado de un error
            )
        {

            // -------------------------------------------- VARIABLES INICIALES ----------------------------------------------------

            UIApplication uiapp = commandData.Application;
            /* UIApplication es una clase que proporciona acceso a la aplicación de Revit en ejecución.
            A través de ella puedes interactuar con la interfaz gráfica de usuario y los documentos abiertos.
            commandData.Application contiene la instancia de la aplicación actual de Revit que se pasó al comando.
            Este objeto encapsula toda la sesión de Revit, permitiendo que el comando interactúe con el entorno de Revit. */

            UIDocument uidoc = uiapp.ActiveUIDocument;
            /* UIDocument es una clase que proporciona acceso al documento activo que el usuario tiene abierto en Revit.
            Esto incluye elementos como la selección actual del usuario y permite modificar el documento.
            uiapp.ActiveUIDocument devuelve el documento que está activo en la ventana actual de Revit.
            A través de este, puedes acceder a los elementos del proyecto que se encuentra abierto. */

            Application app = uiapp.Application;
            /* Application es una clase más baja en la jerarquía que UIApplication y proporciona acceso a funcionalidades del sistema,
            como la configuración general de la aplicación o las versiones de la API.
            Aquí se está almacenando en la variable app la aplicación subyacente de Revit desde uiapp.Application. */

            Document doc = uidoc.Document;
            /* Document es una clase que representa el modelo de Revit (el archivo de proyecto que el usuario está editando).
            Esta clase contiene toda la información del proyecto, incluyendo elementos del modelo, parámetros, vistas, etc.
            uidoc.Document devuelve el documento activo que está siendo editado por el usuario en la interfaz gráfica de Revit.
            A través de doc, puedes acceder y modificar los elementos dentro del archivo de proyecto. */

            // -------------------------------------- TRABAJO CON VARIABLES "elements" y "message" -----------------------------------

            // "message" y "elements"

            message = "Podemos cambiar el valor de 'message'";

            try
            {
                FilteredElementCollector Colector_Muros = new FilteredElementCollector(doc).
                    WhereElementIsNotElementType().OfCategory(BuiltInCategory.INVALID).OfClass(typeof(Wall));
                // Colector de instancias de muros

                foreach (Element elemento in elements)
                {
                    elements.Insert(elemento);
                    // Inserta a la variable "elements" el objeto que estamos trabajando
                    Debug.Print(elemento.Name);
                    // Imprime el nombre del elemento en la ventana de Salida de Visual Studio
                }
            }
            catch (Exception ex)
            {
                
                message = ex.ToString(); // Aca se modificamos el mensaje de error "message".
                return Result.Failed; // Esta linea ejecuta el mensaje de error, osea lo muestra.
                                      // sirve para arrojar que la clase fallo su ejecucion.
            }

            // ---------------------------------------------- TRABAJANDO CON TRANSACCION --------------------------------------------------

            try
            {
                // Modify document within a transaction

                using (Transaction Transaccion_1 = new Transaction(doc)) // Esta linea crea una nueva transaccion
                {
                    Transaccion_1.Start("Transaction Name"); // Esta linea "abre" la transaccion,
                    // todo lo que se ejecuta a partir de aca requiere de ser "guardado" al cerrar Revit

                    Transaccion_1.Commit(); // Cierra la transaccion, de no estar cerrada se producen errores.
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // ----------------------------------- TRABAJANDO "JournalingMode.UsingCommandData" ------------------------------------

            try
            {
                // Modify document within a transaction

                using (Transaction Transaccion_2 = new Transaction(doc)) // Esta linea crea una nueva transaccion
                {
                    Transaccion_2.Start("JournalingMode.UsingCommandData"); // Esta linea "abre" la transaccion,

                    doc.ProjectInformation.Author = "Autor 1"; // Cambia el nombre del autor del proyecto
                    // Este cambio se puede ver en -> Menu: "Gestionar" -> "Informacion del proyecto"
                    commandData.JournalData.Add("Dato 1", "Dato por autor 1"); // Agrega una linea al final del "JournalData"
                    // RUTA -> C:\Users\ASUS\AppData\Local\Autodesk\Revit\Autodesk Revit 2023\Journals


                    Transaccion_2.Commit(); // Cierra la transaccion, de no estar cerrada se producen errores.
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // ----------------------------------- OBTENCIOND DE ELEMENTOS Y SUS Ids ------------------------------------

            Selection seleccion_1 = uidoc.Selection; // agrega a "seleccion_1" las instancias seleccionadas.
            ElementId Elemento_1Id = seleccion_1.GetElementIds().FirstOrDefault(); // Obtiene el Id del primer elemento seleccionado.
            IList<ElementId> Lista_ElementosId = seleccion_1.GetElementIds().ToList(); // Obtiene todos los Ids de los elementos seleccionados.
            TaskDialog.Show("Elemento_1", Elemento_1Id.ToString()); // Muestra el valor de "Elemento_1".

            Element Elemento_1 = doc.GetElement(Elemento_1Id); // Obtenemos el elemento a partir del Id.
            ElementId Tipo_de_Id = Elemento_1.GetTypeId(); // Obtenemos el Id del elemento
            ElementType Tipo_del_elemento = doc.GetElement(Elemento_1Id) as ElementType; // Intenta obtener el "ElementType" del elemento
            ElementId Elemento_1Id_redundante = Elemento_1.Id; // Tambien obtiene el Id del elemento, redundante con la primera manera de obtenerlo.

            string Id_Unico = Elemento_1.UniqueId; // Es el Id unico para cada instancia de elemento creado.
                                                   // Combina el Guid y el Id convertido en Sexagesimal.
            Guid elementGuid = Elemento_1.VersionGuid;

            ICollection<ElementId> ICollection_Id_deElementos = seleccion_1.GetElementIds();
            //En la API de Revit, una ICollection<T> es una interfaz que representa una colección de objetos del tipo T.
            //Es similar a una lista o un conjunto, pero con una interfaz más básica,
            //sin algunas de las funcionalidades avanzadas que tienen otras estructuras como List<T> o HashSet<T>.


            // ----------------------------------- SELECCION EN EL MODELO DE REVIT ------------------------------------

            // SELECCION LIBRE DEL PUNTO
            try // Para los metodos de seleccion es NECESARIO usar el Try Catch para cuando se apriete "escape" o no se seleccion por algun motivo.
            {
                XYZ Punto_1 = uidoc.Selection.PickPoint(); // Selecciona un punto en la pantalla y almacena las coordenadas en "Punto_1"
                TaskDialog.Show(Punto_1.ToString(), Punto_1.X.ToString() + "\n" + Punto_1.Y.ToString() + "\n" + Punto_1.Z.ToString());
                // Muestra los datos de "Punto_1" usando saltos de linea \n para mas proligidad.
                // Entrega los numeros en "unidades internas", osea pulgadas y pies.

            }
            catch (Exception ex)
            {
                message = ex.ToString(); // Se modifica "message" para cubrir el error
                return Result.Failed; // Se cancela la operacion, se genera el cartel de error con el "message"
            }


            // SELECCION CONDICIONADA DEL PUNTO
            try
            {
                ObjectSnapTypes SnapTypes_Objeto = ObjectSnapTypes.Endpoints | ObjectSnapTypes.Centers;
                // Se configura este elemento de Revit que da varias opciones de intrucciones.
                // "Endpoints" Permite seleccionar puntos finales, "Centers" permite seleccionar centros. Solamente se pueden seleccionar estos parametros.
                XYZ Punto_2 = uidoc.Selection.PickPoint(SnapTypes_Objeto, "Selecciona un punto");
                // Se pide la seleccion del punto, pero condicionado por lo que le pusimos en "SnapTypes_Objeto".
                TaskDialog.Show(Punto_2.ToString(), Punto_2.X.ToString() + "\n" + Punto_2.Y.ToString() + "\n" + Punto_2.Z.ToString());
                // Mostramos las coordenadas del punto.
            } 
            catch (Exception ex)
            {
                message= ex.ToString();
                return Result.Failed;
            }

            // SELECCION POR RECTANGULO "TODO INCLUIDO" y "TODO LO INCLUIDO Y CORTADO"
            try
            {
                PickedBox SeleccionPorRectangulo_1 = uidoc.Selection.PickBox(PickBoxStyle.Directional, "Seleccion esquinas");
                // Esta seleccion es a traves de rectangulo, igual que las selecciones comunes en Revit.

                XYZ PuntoPB_1 = SeleccionPorRectangulo_1.Min;
                XYZ PuntoPB_3 = SeleccionPorRectangulo_1.Max;

                XYZ PuntoPB_2 = new XYZ(PuntoPB_3.X, PuntoPB_1.Y, 0);
                XYZ PuntoPB_4 = new XYZ(PuntoPB_1.X, PuntoPB_3.Y, 0);
                // Con esa logica creamos los 4 puntos que son los obtenidos desde la seleccion por rectangulo
            } 
            catch (Exception ex)
            {
                message= ex.ToString();
            }

            // CREACION DE UN CurveArray A TRAVES DE UN PickBox
            try
            {
                PickedBox SeleccionPorRectangulo_2 = uidoc.Selection.PickBox(PickBoxStyle.Directional, "Seleccion esquinas");

                XYZ PuntoPB_1 = SeleccionPorRectangulo_2.Min;
                XYZ PuntoPB_3 = SeleccionPorRectangulo_2.Max;

                XYZ PuntoPB_2 = new XYZ(PuntoPB_3.X, PuntoPB_1.Y, 0);
                XYZ PuntoPB_4 = new XYZ(PuntoPB_1.X, PuntoPB_3.Y, 0);

                CurveArray CurveArray_1 = new CurveArray();
                // Este elemento es una sucecion de lineas ordenadas que forman una forma geometrica cerrada. En este caso un Rectangulo.

                CurveLoop CurveLoop_1 = new CurveLoop();
                // Este elemento tambien es una sucecion de lineas ordenadas, necesario para la creacion en versiones mas recientes de Revit.

                Line Linea_1 = Line.CreateBound(PuntoPB_1, PuntoPB_2);
                Line Linea_2 = Line.CreateBound(PuntoPB_2, PuntoPB_3);
                Line Linea_3 = Line.CreateBound(PuntoPB_3, PuntoPB_4);
                Line Linea_4 = Line.CreateBound(PuntoPB_4, PuntoPB_1);
                // Se crean las lineas siguiendo los puntos de manera ordenada

                CurveArray_1.Append(Linea_1);
                CurveArray_1.Append(Linea_2);
                CurveArray_1.Append(Linea_3);
                CurveArray_1.Append(Linea_4);
                // Se agregan ordenadamente todas las lineas al CurveArray.

                CurveLoop_1.Append(Linea_1);
                CurveLoop_1.Append(Linea_2);
                CurveLoop_1.Append(Linea_3);
                CurveLoop_1.Append(Linea_4);
                // Se agregan las líneas al CurveLoop

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }


            // ----------------------------------- CREACION DE ELEMENTOS ------------------------------------

            // CREACION DE UN SUELO A PARTIR DE UN PickBox
            try
            {
                PickedBox SeleccionPorRectangulo_2 = uidoc.Selection.PickBox(PickBoxStyle.Directional, "Seleccion esquinas");

                XYZ PuntoPB_1 = SeleccionPorRectangulo_2.Min;
                XYZ PuntoPB_3 = SeleccionPorRectangulo_2.Max;

                XYZ PuntoPB_2 = new XYZ(PuntoPB_3.X, PuntoPB_1.Y, 0);
                XYZ PuntoPB_4 = new XYZ(PuntoPB_1.X, PuntoPB_3.Y, 0);

                // Crear el CurveLoop
                CurveLoop CurveLoop_1 = new CurveLoop();

                // Crear las líneas del rectángulo
                Line Linea_1 = Line.CreateBound(PuntoPB_1, PuntoPB_2);
                Line Linea_2 = Line.CreateBound(PuntoPB_2, PuntoPB_3);
                Line Linea_3 = Line.CreateBound(PuntoPB_3, PuntoPB_4);
                Line Linea_4 = Line.CreateBound(PuntoPB_4, PuntoPB_1);

                // Agregar las líneas al CurveLoop
                CurveLoop_1.Append(Linea_1);
                CurveLoop_1.Append(Linea_2);
                CurveLoop_1.Append(Linea_3);
                CurveLoop_1.Append(Linea_4);

                // Verificar si la vista actual es un plano
                if (uidoc.ActiveView.ViewType != ViewType.FloorPlan)
                {
                    message = "Usa una vista de plano, no vistas 3D";
                    return Result.Failed;
                }

                // Obtener el nivel de la vista actual
                Level Nivel_1 = doc.GetElement(doc.ActiveView.LevelId) as Level;

                // Obtener el tipo de suelo por defecto
                FloorType Suelo_PorDefecto_1 = doc.GetElement(doc.GetDefaultElementTypeId(ElementTypeGroup.FloorType)) as FloorType;

                // Crear una lista de CurveLoop
                List<CurveLoop> Lista_CurveLoop_1 = new List<CurveLoop> { CurveLoop_1 };

                // Iniciar la transacción para crear el suelo
                using (Transaction Transaccion_Suelos_1 = new Transaction(doc, "Crear suelos desde PickBox"))
                {
                    Transaccion_Suelos_1.Start();

                    // Crear el suelo
                    Floor.Create(doc, Lista_CurveLoop_1, Suelo_PorDefecto_1.Id, Nivel_1.Id);

                    // Confirmar la transacción
                    Transaccion_Suelos_1.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }



            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------



            return Result.Succeeded; // Muestra que la clase tuvo exito en su ejecucion
        }

    }
}
