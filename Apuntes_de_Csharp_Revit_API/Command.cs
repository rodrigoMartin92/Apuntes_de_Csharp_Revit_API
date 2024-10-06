
/* -------------------------------------------- INDICE --------------------------------------------
    
    0.0 IMPORTACIONES

    1.1 VARIABLES INICIALES
    1.2 TRABAJO CON VARIABLES "elements" y "message"
    1.3 TRABAJANDO CON TRANSACCION
    1.4 OBTENCIOND DE ELEMENTOS Y SUS Ids

    2.0 SELECCION EN EL MODELO DE REVIT

    2.1 SELECCION LIBRE DEL PUNTO
    2.2 SELECCION CONDICIONADA DEL PUNTO
    2.3 SELECCION POR RECTANGULO "TODO INCLUIDO" y "TODO LO INCLUIDO Y CORTADO"
    2.4 CREACION DE UN CurveArray A TRAVES DE UN PickBox
    2.5 SELECCION PUNTUAL CON PickObject
    2.6 SELECCIONAMOS VARIOS ELEMENTOS - USO DE FUNCION FLECHA.
    2.7 SELECCION CON FILTROS - El filtro esta en una clase nueva llamada "FiltroDeSeleccionDeMuros_1"
    2.8 SELECCION CON FILTROS - Filtramos caras planas y mostramos su nombre y area.
    2.9 SELECCION POR RECTANGULO "PickElementsByRectangle"
    2.10 SELECCIONAR ELEMENTOS CON PickObjects Y FILTRO - Permite seleccionar por rectangulo
    2.11 AGREGAR UNA A UNA SELECCION PREVIA UNA NUEVA SELECCION

    3.0  FILTROS 

    3.1 FILTROS RAPIDOS
    3.2 FILTROS LENTOS

    4.0 CREACION DE ELEMENTOS
    
    4.1 CREACION DE UN MURO.
    4.2 CREACION DE UN SUELO.
    4.3 MODIFICACION DEL SUELO
    4.4 CREACION DE PLATAFORMAS DE NIVELACION.
    4.5 CREACION DE CRISTALERAS

    5.0 EDICION DE ELEMENTOS

    5.1 MOVER ELEMENTOS
    5.2 COPIAR ELEMENTOS
    5.3 ROTAR ELEMENTOS
    5.4 SIMETRIA
    5.5 TRANSFORMAR ELEMENTOS
    5.6 BORRADO DE ELEMENTOS

    6.0 UNIDADES

    6.1 CONVERSION DE UNIDADES

    7.0 SYMBOLS, VISTAS, REGIONES, REVISIONES
    7.1 SYMBOLS
    7.2 FASES
    7.3 VISTAS
    7.4 REGIONES
    7.5 REVISIONES

    8.0 FILTROS

    8.1 FILTROS

    9.0 TASKDIALOG

    10.0 TRANSACCIONES

    11.0 VALORES GEOMETRICOS

    12.0 ETIQUETAS, COTAS, TEXTOS

    13.0 MATERIALES

    14.0 ESQUEMAS

    15.0 ARCHIVOS VINCULADOS

    16.0 CORRECCION DE ERRORES

 */

// 0.0 -------------------------------------- IMPORTACIONES -----------------------------------

#region Namespaces - IMPORTACIONES
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Reflection.Emit;
using System.Security.Cryptography;

#endregion

namespace Apuntes_de_Csharp_Revit_API
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]   // Esta linea configura que los cambios se realizan a travez de transacciones
                                                // Esta la opcion de solo lectura, pero seria rarisimo que se usara.
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

            // 1.1 -------------------------------------------- VARIABLES INICIALES ----------------------------------------------------

            #region VALORES INICIALES
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
            #endregion

            #region ESTRUCTURA TRY CATCH SIMPLE
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 1.2 -------------------------------------- TRABAJO CON VARIABLES "elements" y "message" -----------------------------------

            #region 1.2.1 MODIFICACION DE message
            message = "Podemos cambiar el valor de 'message'";
            #endregion

            #region 1.2.2 COLECTOR DE MUROS BASICO
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
            #endregion

            // 1.3 ---------------------------------------------- TRABAJANDO CON TRANSACCION --------------------------------------------------

            #region 1.3.1 TRANSACCION BASICA - Es el menos "prolijo", ya que la estructura no es tan visual y nos puede hacer olvidar cerrar la transaccion
            try
            {
                // Modify document within a transaction
                Transaction Transaccion_1 = new Transaction(doc);
                Transaccion_1.Start("Transaction Name");
                    // Aca va el contenido de la transaccion
                Transaccion_1.Commit();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.3.2 TRANSACCION CON using - Mas prolijo, su estructura es mas visual y nos ayuda a recordar cerrar la transaccion.
            try
            {
                // Modify document within a transaction

                using (Transaction Transaccion_2 = new Transaction(doc)) // Esta linea crea una nueva transaccion
                {
                    Transaccion_2.Start("Transaction Name"); // Esta linea "abre" la transaccion,
                    // todo lo que se ejecuta a partir de aca requiere de ser "guardado" al cerrar Revit

                    Transaccion_2.Commit(); // Cierra la transaccion, de no estar cerrada se producen errores.
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 1.4 ----------------------------------- OBTENCIOND DE ELEMENTOS, SUS Ids ------------------------------------

            #region 1.4.1 OBTENCION DE LOS TIPOS DE Id QUE EXISTEN EN REVIT

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

            #endregion

            // 1.5 ----------------------------------- CARACTERISTICAS GENERALES DE LOS Element ------------------------------------

            #region 1.5.1 CARACTERISTICAS DE LOS Element
            try
            {
                IList<Element> ListaDeElement = new List<Element>();
                IList<string> CaracteristicasElement = new List<string>();

                foreach (Element element in ListaDeElement)
                {
                    // NOMBRE
                    CaracteristicasElement.Add(element.Name);

                    // ID
                    CaracteristicasElement.Add(element.Id.ToString());

                    // TITULO DEL DOCUMENTO DEL ELEMENTO
                    CaracteristicasElement.Add(element.Document.Title);

                    //
                    CaracteristicasElement.Add(element.DesignOption?.Name ?? "Sin opción de diseño");
                    CaracteristicasElement.Add(element.CreatedPhaseId.ToString());
                    CaracteristicasElement.Add(element.Document.ToString());

                    // CATEGORIAS
                    CaracteristicasElement.Add(element.Category?.Name ?? "Sin Category");
                    CaracteristicasElement.Add(element.Category?.Id.ToString() ?? "Sin Category");
                    CaracteristicasElement.Add(element.Category?.Material.ToString() ?? "Sin Category");
                    CaracteristicasElement.Add(element.Category?.BuiltInCategory.ToString() ?? "Sin Category");
                    CaracteristicasElement.Add(element.Category?.CategoryType.ToString() ?? "Sin Category");
                    CaracteristicasElement.Add(element.Category?.Parent.ToString() ?? "Sin Category");
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            #endregion

            #region 1.5.2 CARACTERISTICAS DE LOS Element.Parameters
            try
            {
                IList<Element> ListaDeElement = new List<Element>();
                IList<string> CaracteristicasElement = new List<string>();

                foreach (Element element in ListaDeElement)
                {
                    // PARAMETROS - IMPORTANTE
                    foreach (Parameter parametro in element.Parameters)
                    {
                        // Agregamos el nombre de la definición del parámetro a la lista
                        CaracteristicasElement.Add(parametro.Definition.Name);
                        CaracteristicasElement.Add(parametro.HasValue.ToString());
                        CaracteristicasElement.Add(parametro.Id.ToString());
                        CaracteristicasElement.Add(parametro.StorageType.ToString());
                        CaracteristicasElement.Add(parametro.UserModifiable.ToString());

                        Parameter param = element.LookupParameter(parametro.Definition.Name);
                        if (param != null && param.StorageType == StorageType.Double)  // Aseguramos que sea un parámetro de tipo Double
                        {
                            double valor = param.AsDouble();
                            Console.WriteLine(valor);

                            // Si quieres agregarlo a la lista, puedes hacerlo aquí:
                            CaracteristicasElement.Add(valor.ToString());
                        }
                    }

                    // PARAMETROS - TIPO DE ALMACENAMIENTO
                    foreach (Parameter parametro in element.Parameters)
                    {
                        // Verificamos el tipo de almacenamiento del parámetro
                        if (parametro.StorageType == StorageType.Double)
                        {
                            CaracteristicasElement.Add("StorageType = Número fraccionario: " + element.LookupParameter(parametro.Definition.Name).AsDouble());
                        }
                        else if (parametro.StorageType == StorageType.String)
                        {
                            CaracteristicasElement.Add("StorageType = Texto: " + element.LookupParameter(parametro.Definition.Name).AsString());
                        }
                        else if (parametro.StorageType == StorageType.Integer)
                        {
                            CaracteristicasElement.Add("StorageType = Número redondo " + element.LookupParameter(parametro.Definition.Name).AsInteger());
                        }
                        else
                        {
                            Console.WriteLine("Tipo de parámetro no admitido");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            #endregion

            #region 1.5.3 Element.LookupParameter
            try
            {
                IList<Element> ListaDeElement = new List<Element>();
                IList<string> CaracteristicasElement = new List<string>();

                foreach (Element element in ListaDeElement)
                {
                    // LOOKUPPARAMETERS - LookupParameter

                    // Inicializa la lista para almacenar los parámetros encontrados
                    List<Parameter> lookupParameters = new List<Parameter>();

                    // Itera sobre todos los parámetros del elemento
                    foreach (Parameter parametro in element.Parameters)
                    {
                        // Verifica si el parámetro tiene un nombre definido
                        if (!string.IsNullOrEmpty(parametro.Definition.Name))
                        {
                            // Agrega el parámetro a la lista si es un LookupParameter
                            Parameter lookupParam = element.LookupParameter(parametro.Definition.Name);
                            if (lookupParam != null)
                            {
                                lookupParameters.Add(lookupParam);
                            }
                        }
                    }

                    // Ahora puedes utilizar la lista 'lookupParameters' como desees
                    foreach (Parameter lookupParam in lookupParameters)
                    {
                        CaracteristicasElement.Add($"Nombre del LookupParameter: {lookupParam.Definition.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            #endregion

            #region 1.5.4 OTRAS CARACTERISTICAS
            try
            {
                IList<Element> ListaDeElement = new List<Element>();
                IList<string> CaracteristicasElement = new List<string>();

                foreach (Element element in ListaDeElement)
                {

                    // LOCALIZACION
                    Location location = element.Location;
                    if (location is LocationPoint locationPoint)
                    {
                        // LOCALIZACION BASADA EN UN PUNTO
                        CaracteristicasElement.Add(locationPoint.Point.ToString());
                    }
                    else if (location is LocationCurve locationCurve)
                    {
                        // LOCALIZACION BASADA EN UNA CURVA
                        CaracteristicasElement.Add(locationCurve.Curve.ToString());
                    }

                    // BOUNDINGBOX DEL ELEMENTO
                    BoundingBoxXYZ boundingBox = element.get_BoundingBox(null);
                    if (boundingBox != null)
                    {
                        CaracteristicasElement.Add($"Min: {boundingBox.Min.ToString()}, Max: {boundingBox.Max.ToString()}");
                    }

                    ElementType elementType = element.Document.GetElement(element.GetTypeId()) as ElementType;
                    if (elementType != null)
                    {
                        CaracteristicasElement.Add(elementType.Name);
                    }

                    // MATERIALES
                    if (element is FamilyInstance familyInstance)
                    {
                        Material material = familyInstance.Symbol.GetMaterialIds(false)
                            .Select(id => element.Document.GetElement(id) as Material).FirstOrDefault();
                        if (material != null)
                        {
                            CaracteristicasElement.Add(material.Name);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 1.6 ----------------------------------- CATEGORIAS ------------------------------------

            #region 1.6.1 CATEGORIAS
            try
            {
                // Inicializamos la lista para almacenar las categorías
                List<string> categorias = new List<string>();

                // Iteramos sobre las categorías en la configuración del documento
                foreach (Category Categoria in doc.Settings.Categories)
                {
                    // Agregamos el nombre de la categoría a la lista
                    categorias.Add(Categoria.Name);
                }

                // Ordenamos la lista de categorías
                categorias.Sort();

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 1.7 ----------------------------------- COLECTORES ------------------------------------

            #region 1.7.1 COLECTOR BASICO
            try
            {
                // Crear un colector de elementos filtrado a partir del documento
                FilteredElementCollector Instancia_del_colector = new FilteredElementCollector(doc);
                // El colector sin una categoría no permite ver elementos si no tiene una "BuiltInCategory"
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.7.2 COLECTOR POR BuiltInCategory (MUROS)
            try
            {
                // Crear un colector filtrado por la categoría BuiltInCategory.OST_Walls (muros)
                FilteredElementCollector Instancia_del_colector_muros = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls);
                // Colector que tiene una lista con todas las instancias de muros y todos los tipos de muros
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.7.3 COLECTOR POR BuiltInCategory (MUROS) por vistas
            try
            {
                // Crear un colector filtrado por la categoría BuiltInCategory.OST_Walls, solo en la vista activa
                FilteredElementCollector Instancia_del_colector_por_vistas = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfCategory(BuiltInCategory.OST_Walls);
                // Colector que tiene una lista con todas las instancias de muros y tipos de muros en la vista actual
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.7.4 OBTENER LOS Element DE UN COLECTOR
            try
            {
                // Obtener los elementos del colector
                FilteredElementCollector Instancia_del_colector_muros = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls);

                IList<Element> Elementos_del_colector = Instancia_del_colector_muros.ToElements();
                // Devuelve los elementos extraídos por el colector usado
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.7.5 OBTENER LOS ElementId DE UN COLECTOR
            try
            {
                // Obtener los Ids de los elementos del colector
                FilteredElementCollector Instancia_del_colector_muros = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls);

                ICollection<ElementId> Ids_de_elementos_del_colector = Instancia_del_colector_muros.ToElementIds();
                // Devuelve los IDs de los elementos extraídos por el colector usado
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.7.6 OBTENER LOS TIPOS ElementType DE UN COLECTOR
            try
            {
                // Obtener solo los tipos de elementos (familias)
                FilteredElementCollector Instancia_del_colector_muros = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls);

                IList<Element> Tipos_de_elementos = Instancia_del_colector_muros.WhereElementIsElementType().ToElements();
                // Devuelve las familias
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.7.7 OBTENER LAS INSTANCIAS DE UN COLECTOR
            try
            {
                // Obtener solo las instancias de elementos
                FilteredElementCollector Instancia_del_colector_muros = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls);
                IList<Element> Instancias_de_elementos = Instancia_del_colector_muros.WhereElementIsNotElementType().ToElements();
                // Devuelve las instancias
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 1.7.8 COLECTOR POR CLASE
            try
            {
                FilteredElementCollector instanciaDelColectorMuros = new FilteredElementCollector(doc)
                    .OfClass(typeof(Wall));
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            #endregion

            #region 1.7.9 EJEMPLOS DE ESTRUCTURAS COMPLETAS
            try
            {
                IList<ElementId> Ids_familias_muros = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToElementIds().ToList();

                IList<ElementId> Ids_instancias_muros = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElementIds().ToList();

                IList<ElementId> Ids_familias_muros_2 = new FilteredElementCollector(doc)
                    .OfClass(typeof(Wall)).WhereElementIsElementType().ToElementIds().ToList();

                IList<ElementId> Ids_instancias_muros_2 = new FilteredElementCollector(doc)
                    .OfClass(typeof(Wall)).WhereElementIsNotElementType().ToElementIds().ToList();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 2.0 ----------------------------------- SELECCION EN EL MODELO DE REVIT ------------------------------------

            // 2.1 ELEMENTOS PREVIAMENTE SELECCIONADOS EN EL MODELO

            #region 2.1.1 ELEMENTOS SELECCIONADOS - Selection
            try
            {
                Selection ElementoSeleccionado_1 = uidoc.Selection;
                // Agrega a la variable "Selection" el objeto actualmente seleccionado.
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.1.2 ELEMENTO SELECCIONADO Ids - ICollection<ElementId>
            try
            {
                ICollection<ElementId> ElementoSeleccionadoId_1 = uidoc.Selection.GetElementIds();

                // ICollection < T >
                // Es la interfaz más básica para trabajar con colecciones en.NET, y es más general que IList<T>.
                // Proporciona funcionalidades mínimas, como contar elementos, verificar si un elemento está contenido,
                // y agregar o eliminar elementos.

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.1.3 ELEMENTO SELECCIONADO Id - IList<ElementId>
            try
            {
                // Requiere usar el metodo "ToList()"
                IList<ElementId> ElementoSeleccionadoId_1 = uidoc.Selection.GetElementIds().ToList();

                // IList < T >
                // Es una interfaz más específica que hereda de ICollection<T> y, además de las funcionalidades que hereda,
                // proporciona capacidades adicionales para manipular colecciones indexadas, es decir,
                // colecciones en las que se pueden acceder a los elementos por su posición (índice).

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            #endregion

            #region 2.1.4 ELEMENTO SELECCIONADO - IList<Element>
            try
            {
                IList<Element> ListaElementos = new List<Element>();

                IList<ElementId> ElementoSeleccionadoId_1 = uidoc.Selection.GetElementIds().ToList();
                foreach (ElementId ElementoId in ElementoSeleccionadoId_1)
                {
                    Element Elemento = uidoc.Document.GetElement(ElementoId);
                    ListaElementos.Add(Elemento);
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }


            #endregion

            // 2.2 SELECCION DE PUNTOS - POR PUNTOS

            #region 2.2.1 SELECCION LIBRE DEL PUNTO
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
            #endregion

            #region 2.2.2 SELECCION CONDICIONADA DEL PUNTO
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
            #endregion

            // 2.3 SELECCION DE ELEMENTOS - POR PUNTOS

            #region 2.3.1 SELECCION PUNTUAL CON PickObject - Element
            try
            {
                // Seleccionamos un elemento completo en el modelo.
                // Lo que obtenemos aquí es una "Reference" que apunta a un "Element".
                Reference ReferenciaPuntual_1 = uidoc.Selection.PickObject(ObjectType.Element, "Selecciona un elemento");

                // Podemos obtener el elemento completo usando la "Reference" obtenida de la selección de un "Element".
                // Hay dos maneras de hacerlo: usando el ElementId o directamente la Reference.

                // Opción 1: Obtener el Element usando el ElementId de la referencia.
                Element ElementoDesdeLaReferencia_1 = doc.GetElement(ReferenciaPuntual_1.ElementId);

                // Opción 2: Obtener el Element directamente usando la Reference (internamente usa el ElementId).
                Element ElementoDesdeLaReferencia_2 = doc.GetElement(ReferenciaPuntual_1);

                // Ambas líneas anteriores devolverán el mismo Element, ya que Reference.ElementId apunta al mismo elemento.

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.3.2 SELECCION PUNTUAL CON PickObject - Edge
            try
            {
                // Seleccionamos una arista (Edge) de un elemento en el modelo.
                // Lo que obtenemos aquí también es una "Reference", pero esta vez la referencia apunta a una arista específica del elemento.
                Reference ReferenciaPuntual_2 = uidoc.Selection.PickObject(ObjectType.Edge, "Selecciona una arista");

                // Cuando seleccionamos una arista, la referencia apunta a una parte específica del elemento.
                // Podemos obtener el elemento completo desde la referencia de la arista.
                Element ElementoDesdeLaArista = doc.GetElement(ReferenciaPuntual_2.ElementId);

                // Ahora tenemos acceso a la arista específica del elemento, a través de la referencia.
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.3.3 SELECCIONAMOS VARIOS ELEMENTOS - SIN FUNCION FLECHA.
            try
            {
                IList<Reference> Referencias_1 = uidoc.Selection.PickObjects(ObjectType.Element, "Selecciona varios elementos");
                // Obtenemos una lista de referencias, pero estas referencias son a Elements.

                // Primera manera de agregar los Element a una lista, sin funcion flecha.
                List<Element> ListaElements_1 = new List<Element>();
                foreach (Reference Referencia in Referencias_1)
                {
                    // Obtenemos el Element a partir de la referencia y accedemos a su nombre
                    Element elemento = doc.GetElement(Referencia.ElementId);

                    // Agregamos el nombre del elemento a la lista
                    ListaElements_1.Add(elemento);
                }

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.3.4 SELECCIONAMOS VARIOS ELEMENTOS - CON FUNCION FLECHA.
            try
            {
                IList<Reference> Referencias_2 = uidoc.Selection.PickObjects(ObjectType.Element, "Selecciona varios elementos");
                // Obtenemos una lista de referencias, pero estas referencias son a Elements.

                // Segunda manera de agregar los Element a una lista, con funcion flecha.
                List<Element> ListaElements_2 = Referencias_2.Select(Referencia => doc.GetElement(Referencia.ElementId)).ToList();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 2.4 SELECCION DE PUNTOS - POR RECTANGULOS

            #region 2.4.1 CREACION DE 4 PUNTOS A TRAVES DE PickBox(), A TRAVES DE LA SELECCION POR RECTANGULO
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
            #endregion

            #region 2.4.2 CREACION DE 4 Line Y UN CurveArray O CurveLoop A TRAVES DE UN PickBox().
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
            #endregion

            // 2.5 SELECCION DE ELEMENTOS - POR RECTANGULOS ------------------- FALTA ORDENAR A PARTIR DE ACA

            #region 2.5.1 SELECCION POR RECTANGULO "PickElementsByRectangle()"
            try
            {
                // Seleccionamos varios muros usando la instancia del filtro creado, y los colocamos en una lista de Element.
                IList<Element> SeleccionDeMurosFiltrados_2 = uidoc.Selection.PickElementsByRectangle();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.5.2 SELECCION POR RECTANGULO "PickElementsByRectangle()" Y FILTRO
            try
            {
                // Creamos una instancia de filtro usando una clase de filtro ya creada
                ISelectionFilter Filtro_1 = new FiltroDeSeleccionDeMuros_1();

                // Seleccionamos varios muros usando la instancia del filtro creado, y los colocamos en una lista de Element.
                IList<Element> SeleccionDeMurosFiltrados_2 = uidoc.Selection.PickElementsByRectangle(Filtro_1, "Seleccione ...");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.5.3 SELECCIONAR ELEMENTOS CON PickObjects Y FILTRO - Permite seleccionar por rectangulo
            try
            {
                ISelectionFilter selectionFilter = new FiltroDeSeleccionDeMuros_1();
                IList<Reference> referencesPre = new List<Reference>();
                List<ElementId> elementIds = new List<ElementId>();  // Inicializamos como lista vacía

                // Nueva selección de objetos. Partimos de preselcción anterior
                IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element,
                    selectionFilter, "Seleccionar elementos");

                // Necesitamos iDs. MODO LARGO
                foreach (Reference referenceTotal in references)
                {
                    elementIds.Add(referenceTotal.ElementId);
                }

                // Mostramos en pantalla la selección actual
                uidoc.ShowElements(elementIds);

                // Incorporamos los IDs a selección actual
                uidoc.Selection.SetElementIds(elementIds);

            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;  // Aseguramos que se devuelva el resultado fallido en caso de excepción
            }
            #endregion

            #region 2.5.4 SELECCION CON FILTROS - El filtro esta en una clase nueva llamada "FiltroDeSeleccionDeMuros_1"
            try
            {
                // Creamos una instancia de filtro usando una clase de filtro ya creada
                ISelectionFilter FiltroDeSeleccionDeMuro_1 = new FiltroDeSeleccionDeMuros_1();

                // FILTRO USANDO "PickObject"
                // Seleccionamos un muro usando la instancia del filtro creada, y obtenemos la referencia del mismo.
                Reference SeleccionDeMuroFiltrado_1 = uidoc.Selection.
                    PickObject(ObjectType.Element, FiltroDeSeleccionDeMuro_1, "Seleccione un muro");

                // Obtenemos el Element desde la referencia
                Element ElementoSeleccionado_1 = doc.GetElement(SeleccionDeMuroFiltrado_1.ElementId);

                // FILTRO USANDO "PickObjects"
                // Seleccionamos varios muros usando la instancia del filtro creado, y los colocamos en una lista de referencias.
                IList<Reference> SeleccionDeMurosFiltrados_2 = uidoc.Selection.
                    PickObjects(ObjectType.Element, FiltroDeSeleccionDeMuro_1, "Seleccione muros");

                // Obtenemos una lista de Element desde la lista de Reference
                IList<Element> ListaDeElementos = new List<Element>();
                foreach (Reference Referencia in SeleccionDeMurosFiltrados_2)
                {
                    Element ElementoSeleccionado_2 = doc.GetElement(Referencia.ElementId);
                    ListaDeElementos.Add(ElementoSeleccionado_2);
                }

                // FILTRO USANDO "PickElementsByRectangle" - EN ESTE CASO OBTENEMOS DIRECTAMENTE: Element
                // Selecciona los elementos dentro del rectángulo dibujado por el usuario
                IList<Element> selectedElements = uidoc.Selection.
                    PickElementsByRectangle(FiltroDeSeleccionDeMuro_1, "Seleccione muros dentro del rectángulo");

                foreach (Element elem in selectedElements)
                {
                    TaskDialog.Show("Selección", "Has seleccionado el muro: " + elem.Name);
                }

                foreach (Element elem in selectedElements)
                {
                    TaskDialog.Show("Selección", "Has seleccionado el muro: " + elem.Name);
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion
            // FALTA SUBDIVIDIR LOS 3 METODOS, PICK OBJECT(), PICK OBJECTS() Y PICKELEMENTSBYRECTANGLE()

            #region 2.5.5 SELECCION CON FILTROS - Filtramos caras planas y mostramos su nombre y area.
            try
            {
                ISelectionFilter FiltroDeCarasPlanas_1 = new FiltroDeSeleccionDeCarasPlanas_1(uidoc.Document);
                // El filtro esta definido en una nueva clase llamada "FiltroDeSeleccionDeCarasPlanas_1"

                Reference CarasDeReferencia = uidoc.Selection.PickObject(ObjectType.Face, FiltroDeCarasPlanas_1, "Seleccione caras");
                if (CarasDeReferencia != null)
                {
                    // Obtenemos el elemento y la cara del elemento.
                    Element Elemento = doc.GetElement(CarasDeReferencia.ElementId);
                    GeometryObject GeometriaDelElemento = Elemento.GetGeometryObjectFromReference(CarasDeReferencia);
                    Face Cara = GeometriaDelElemento as Face;

                    // Mostramos el nombre y el area del elemento.
                    TaskDialog.Show("Superficie seleccionada", "Nombre del elemento: " + Elemento.Name + "\n"
                        + "Area del elemento" + Cara.Area.ToString());
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region 2.5.6 SELECCION CON FILTRO MULTI-CATEGORIA
            try
            {
                // Ejemplo de uso
                List<string> categoriasSeleccionadas = new List<string> { "Walls", "Floors", "Doors" };
                FiltroDeSeleccionDeCategoriaMultiple filtroMultiple = new FiltroDeSeleccionDeCategoriaMultiple(categoriasSeleccionadas.ToArray());

                IList<Element> seleccionPorRectangulo = uidoc.Selection.PickElementsByRectangle(filtroMultiple);
                IList<string> nombres = new List<string>();

                foreach (Element element in seleccionPorRectangulo)
                {
                    nombres.Add(element.Name);
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion
            // FALTA AGREGAR UN FILTRO QUE SOLO SELECCIONE UNA CATEGORIA O GRUPO DE CATEGORIAS, SIN MAS LIMITACIONES.
            // FALTA AGREGAR FILTROS CON MAS VARIEDAD DE LIMITACIONES, PARA EJEMPLIFICAR.

            #region 2.5.7 SELECCION CON FILTRO MULTI-CATEGORIA Y DISCRIMINACION ESPECIAL PARA MUROS
            try
            {
                // Ejemplo de uso
                List<string> categoriasSeleccionadas = new List<string> { "Walls", "Floors", "Doors" };
                FiltroDeSeleccionDeCategoriaMultiple_DiscriminacionDeMuros filtroMultiple =
                    new FiltroDeSeleccionDeCategoriaMultiple_DiscriminacionDeMuros(categoriasSeleccionadas.ToArray());

                IList<Element> seleccionPorRectangulo = uidoc.Selection.PickElementsByRectangle(filtroMultiple);
                IList<string> nombres = new List<string>();

                foreach (Element element in seleccionPorRectangulo)
                {
                    nombres.Add(element.Name);
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 2.6 AGREGAR UNA SELECCION NUEVA

            #region 2.6.1 AGREGAR UNA A UNA SELECCION PREVIA UNA NUEVA SELECCION
            try
            {
                ISelectionFilter selectionFilter = new FiltroDeSeleccionDeMuros_1();
                IList<Reference> referencesPre = new List<Reference>();
                IList<Reference> referencesPre_2 = new List<Reference>();
                ICollection<ElementId> elementIds = uidoc.Selection.GetElementIds();
                // Agregamos a la lista de "Reference" las "Reference" desde los "ElementIds"
                foreach (ElementId elementId in elementIds)
                {
                    Element element = uidoc.Document.GetElement(elementId);
                    Reference reference = Reference.ParseFromStableRepresentation(uidoc.Document, element.UniqueId);
                    referencesPre.Add(reference);
                }
                // Agregamos a la lista de "Reference" las "Reference" desde los "ElementIds" - Metodo abreviado con funcion Flecha
                // De los id anterioers a IList<Reference> MODO CORTO 1 LINEA
                referencesPre_2 = elementIds.Select(x => Reference.ParseFromStableRepresentation
                (uidoc.Document, uidoc.Document.GetElement(x).UniqueId)).ToList();

                //Nueva selección de objetos. Partimos de preselcción anterior
                IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element,
                    selectionFilter, "Seleccionar elementos", referencesPre);
                // Podemos eliminar la selacción actual
                elementIds.Clear();
                // Necesitamos iDs. MODO LARGO
                foreach (Reference referenceTotal in references)
                {
                    elementIds.Add(referenceTotal.ElementId);
                }

                // Mostramos en pantalla la selección actual
                uidoc.ShowElements(elementIds);

                // Incorporamos los IDs a selección actual
                uidoc.Selection.SetElementIds(elementIds);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 3.0 ----------------------------------- FILTROS ------------------------------------

            // 3.1 FILTROS RAPIDOS

            #region FILTRO POR PUNTO - SELECCIONA TODO LO QUE ESTA PASANDO POR UN PUNTO EN ESPECIFICO
            try
            {
                // Elementos que contienen un punto en especifico
                XYZ Punto_1 = new XYZ(0, 0, 0);
                BoundingBoxContainsPointFilter Filtro = new BoundingBoxContainsPointFilter(Punto_1);
                FilteredElementCollector ColectorFitlrado = new FilteredElementCollector(doc);
                IList<Element> ListaDeElement = ColectorFitlrado.WherePasses(Filtro).ToElements();

                // Caso inverso
                BoundingBoxContainsPointFilter Filtro_2 = new BoundingBoxContainsPointFilter(Punto_1, true);
                FilteredElementCollector ColectorFitlrado_2 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElement_2 = ColectorFitlrado_2.WherePasses(Filtro_2).ToElements();

                // Caso inverso, solamente muros, ya que selecciona toda clase de cosas si no se especifica una clase.
                FilteredElementCollector ColectorFitlrado_3 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElement_3 = ColectorFitlrado_3.OfClass(typeof(Wall)).WherePasses(Filtro_2).ToElements();
            }
            catch ( Exception ex )
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO POR BoundingBox - SELECCIONA TODO LO QUE ESTA CONTENIDO O CORTADO POR UNA BoundingBox
            try
            {
                // Elementos cortados o incluidos en una BuildingBox
                double ValorCreciente = 10;
                Outline Outline_1 = new Outline(new XYZ(0, 0, 0), new XYZ(ValorCreciente, ValorCreciente, ValorCreciente));

                BoundingBoxIntersectsFilter Filtro = new BoundingBoxIntersectsFilter(Outline_1);
                FilteredElementCollector ColectorFitlrado = new FilteredElementCollector(doc);
                IList<Element> ListaDeElement = ColectorFitlrado.WherePasses(Filtro).ToElements();

                // Caso inverso, filtrando por clase tambien
                BoundingBoxIntersectsFilter Filtro_2 = new BoundingBoxIntersectsFilter(Outline_1, true);
                FilteredElementCollector ColectorFitlrado_2 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElement_2 = ColectorFitlrado_2.WherePasses(Filtro).ToElements();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO POR BoundingBox - SELECCIONA TODO LO QUE ESTA COMPLETAMENTE CONTENIDO POR UNA BoundingBox
            try
            {
                // Elementos completamente incluidos en una BuildingBox
                double ValorCreciente = 10;
                Outline Outline_1 = new Outline(new XYZ(0, 0, 0), new XYZ(ValorCreciente, ValorCreciente, ValorCreciente));

                BoundingBoxIsInsideFilter Filtro = new BoundingBoxIsInsideFilter(Outline_1);
                FilteredElementCollector ColectorFitlrado = new FilteredElementCollector(doc);
                IList<Element> ListaDeElement = ColectorFitlrado.WherePasses(Filtro).ToElements();

                // Caso inverso, filtrando por clase tambien
                BoundingBoxIntersectsFilter Filtro_2 = new BoundingBoxIntersectsFilter(Outline_1, true);
                FilteredElementCollector ColectorFitlrado_2 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElement_2 = ColectorFitlrado_2.WherePasses(Filtro).ToElements();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO POR CATEGORIA (Ejemplo "OST_Walls")
            try
            {
                // Elementos Que son muros, tanto tipos como instancias
                ElementCategoryFilter Filtro = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
                FilteredElementCollector Colector_1 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElementos = Colector_1.WherePasses(Filtro).ToElements();

                // Metodo mas abreviado
                FilteredElementCollector Colector_2 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElementos_2 = Colector_2.OfCategory(BuiltInCategory.OST_Walls).ToElements();

                // Metodo mas abreviado - Quita tambien los elementos que son tipos y deja los que son instancias.
                FilteredElementCollector Colector_3 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElementos_3 = Colector_3.OfCategory(BuiltInCategory.OST_Walls).
                    WhereElementIsNotElementType().ToElements();

                // Metodo inverso - No es abreviado
                ElementCategoryFilter FiltroInverso = new ElementCategoryFilter(BuiltInCategory.OST_Walls, true);
                FilteredElementCollector Colector_4 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElementos_4 = Colector_4.WherePasses(FiltroInverso).ToElements();

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO POR CLASE (Ejemplo "FamilyInstance", "Wall", "View") ---- FALTA COMPLETAR MAS
            try
            {
                // Filtro muy usado - Abreviado - Filtra los elementos que son instancias existentes
                FilteredElementCollector Colector_1 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElementos_1 = Colector_1.OfClass(typeof(FamilyInstance)).ToElements();

                // Podemos obtener los nombres de los Element
                List<string> Nombres = ListaDeElementos_1.Select(Elemento => Elemento.Name).ToList();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO MULTICATEGORIA
            try
            {
                // CREACION DE UN FILTRO MULTI-CATEGORIA
                List<BuiltInCategory> categorias = new List<BuiltInCategory>();
                // List<> en C# se utiliza para crear una lista fuertemente tipada
                // Agregamos las "BuiltInCategory" que necesitemos

                categorias.Add(BuiltInCategory.OST_Walls);
                categorias.Add(BuiltInCategory.OST_Floors);
                categorias.Add(BuiltInCategory.OST_Ceilings);

                // Creación del filtro multicategoría
                ElementMulticategoryFilter filtroMultiple = new ElementMulticategoryFilter(categorias);

                // Colección de elementos filtrados en base al filtro
                FilteredElementCollector colectorMultiple = new FilteredElementCollector(doc, doc.ActiveView.Id);
                ICollection<Element> elementosDelFiltro = colectorMultiple.WherePasses(filtroMultiple).ToElements();

                // Obtener los Ids de los elementos filtrados
                ICollection<ElementId> idsDelFiltro = colectorMultiple.WherePasses(filtroMultiple).ToElementIds();

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO MULTICLASE
            try
            {
                // Filtro que toma varios tipos diferentes
                List<Type> ListaElementType = new List<Type>() { typeof(Wall),  typeof(FamilyInstance) };
                ElementMulticlassFilter FiltroMultiClase_1 = new ElementMulticlassFilter(ListaElementType);
                FilteredElementCollector Colector_1 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElementos_1 = Colector_1.WherePasses(FiltroMultiClase_1).ToElements();

                // Filtramos nuevamente, quitamos categorías nulas o no cortables, agregamos los nombres a una lista de nombres.
                List<string> ListaDeNombres_1 = new List<string>(); // Creamos una lista vacía para los nombres

                foreach (var Elemento in ListaDeElementos_1)
                {
                    // Verificamos si la categoría no es nula y si el elemento es cortable
                    if (Elemento.Category != null && Elemento.Category.IsCuttable)
                    {
                        // Agregamos el nombre del elemento a la lista
                        ListaDeNombres_1.Add(Elemento.Name);
                    }
                }
                // Convertimos a lista

                // Metodo abreviado con funcion flecha.
                List<string> ListaDeNombres_2 = ListaDeElementos_1.Where(
                    Elemento => Elemento.Category != null && Elemento.Category.IsCuttable).Select(Elemento => Elemento.Name).ToList();

                // Filtro inverso
                ElementMulticlassFilter FiltroMultiClase_2 = new ElementMulticlassFilter(ListaElementType, true);
                FilteredElementCollector Colector_2 = new FilteredElementCollector(doc);
                IList<Element> ListaDeElementos_2 = Colector_2.WherePasses(FiltroMultiClase_2).ToElements();
                // Video RAPI 7-7
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO POR ELEMENTOS EN UNA VISTA
            try
            {
                // Element que dependen de la vista en la que se han creado.
                ElementOwnerViewFilter ElementosDependientesDeLaVista_1 = new ElementOwnerViewFilter(doc.ActiveView.Id);
                FilteredElementCollector Colector_1 = new FilteredElementCollector(doc);
                ICollection<Element> ListaDeElementos_1 = Colector_1.WherePasses(ElementosDependientesDeLaVista_1).ToElements();

                FilteredElementCollector Colector_2 = new FilteredElementCollector(doc);
                ICollection<Element> ListaDeElementos_2 = Colector_2.WherePasses(ElementosDependientesDeLaVista_1).
                    OfClass(typeof(IndependentTag)).ToElements();
                // Se puede armar un inverso tambien.
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO DE ELEMENTOS ESTRUCTURALES
            try
            {
                // Filtro de elementos estructurales.
                ElementStructuralTypeFilter ElementosEstructurales = 
                    new ElementStructuralTypeFilter(Autodesk.Revit.DB.Structure.StructuralType.Column);
                FilteredElementCollector Colector_1 = new FilteredElementCollector(doc);
                ICollection <Element> ColumnasEstructurales = Colector_1.WherePasses(ElementosEstructurales).ToElements();

                // Filtro de elementos estructurales INVERSO
                ElementStructuralTypeFilter ElementosEstructurales_2 =
                    new ElementStructuralTypeFilter(Autodesk.Revit.DB.Structure.StructuralType.Column, true);
                FilteredElementCollector Colector_2 = new FilteredElementCollector(doc);
                ICollection<Element> ColumnasEstructurales_2 = Colector_2.WherePasses(ElementosEstructurales_2).ToElements();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO DE EXCLUCION
            // Filtro que excluye los elementos que no se seleccionan.
            try
            {
                // Seleccionamos elementos con una seleccion por rectangulo.
                IList<Element> ElementosSeleccionados = uidoc.Selection.PickElementsByRectangle("Selecciona elementos");
                ICollection<ElementId> ElementosSeleccionados_Ids = ElementosSeleccionados.Select(Elementos => Elementos.Id).ToList();
                ElementIdSetFilter FiltroDeExclucion = new ElementIdSetFilter(ElementosSeleccionados_Ids);

                // Creamos el colector y le aplicamos 2 filtros.
                FilteredElementCollector Colector_1 = new FilteredElementCollector(doc);
                Colector_1.WherePasses(FiltroDeExclucion).ToElements();
                // Aca usamos el filtro que quita los elementos seleccionados de los elementos del colector
                IList<Element> MurosNoIncluidos = Colector_1.OfClass(typeof(Wall)).ToElements();
                // Aca aplicamos otro filtro para solamente agregar muros a la lista de elementos.
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region FILTRO DE SIMBOLOS
            try
            {
                Selection Seleccion_1 = uidoc.Selection;
                ICollection<ElementId> Seleccion_Ids = Seleccion_1.GetElementIds();
                if (Seleccion_Ids.Count == 1 && doc.GetElement(Seleccion_Ids.FirstOrDefault()) is FamilyInstance InstanciaDeFamilia)
                {
                    //Obtenemos el Id de la Familiy
                    ElementId familyId = InstanciaDeFamilia.Symbol.Family.Id;

                    //Creamos el filtro con el Id de la Familia. Considere utilizar  .GetFamilySymbolIds()
                    FamilySymbolFilter filterFamilySymbol = new FamilySymbolFilter(familyId);

                    FilteredElementCollector collector = new FilteredElementCollector(doc);

                    // Aplicamos el filtro a los elementos del documento activo

                    IList<Element> elementsSet = collector.WherePasses(filterFamilySymbol).ToElements();

                    List<string> names = elementsSet.Select(x => x.Name).ToList();
                    names.Insert(0, "Tipos que SI pertenecen a la Familia. Uso de filtro");
                    TaskDialog.Show("Manual Revit API", string.Join("\n", names));

                    // ALTERNATIVA AL FILTRO
                    // Obtenemos lo ids de tipos desde Family
                    ISet<ElementId> elementIds = InstanciaDeFamilia.Symbol.Family.GetFamilySymbolIds();
                    elementsSet = elementIds.Select(x => doc.GetElement(x)).ToList();

                    names = elementsSet.Select(x => x.Name).ToList();
                    names.Insert(0, "Tipos que SI pertenecen a la Familia. Metodo GetFamilySymbolIds()");
                    TaskDialog.Show("Manual Revit API", string.Join("\n", names));
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREACION DE ESQUEMAS Y FILTRO QUE BUSCA DETERMINADOS ESQUEMAS
            try
            {
                //Creamos un GUID
                Guid guidSchema = new Guid("4d8a80d3-e1c3-4b83-ffff-ce975e420529");

                // Seleccionamos algunos objetos y creamos en ellos un Schema
                IList<Element> elementSelect = null;

                //Hacemos una selección rectangular
                elementSelect = uidoc.Selection.PickElementsByRectangle("Selecciona objetos por rectángulo");

                // Creamos el Schema en cada objeto seleccionado y modificamos el doc; debemos utilizar una Transacion

                using (Transaction transaction = new Transaction(doc, "Crear Schema"))
                {
                    transaction.Start();
                    elementSelect.ToList().ForEach(x => CrearSchema(x, guidSchema));
                    transaction.Commit();
                }
                // Hasta aqui es una fase previa para crear las condiciones en el modelo
                // Ya creamos los esquemas, que a partir de aca buscaremos esos esquemas con el filtro

                // Creamos el filtro con el GUID
                ExtensibleStorageFilter filterExtensibleStorage = new ExtensibleStorageFilter(guidSchema);

                FilteredElementCollector collector = new FilteredElementCollector(doc);

                // Aplicamos el filtro a los elementos del documento activo

                collector.WherePasses(filterExtensibleStorage).ToElements();

                IList<Element> elementsSet = collector.ToElements();

                List<string> names = elementsSet.Select(x => x.Name).ToList();
                names.Insert(0, "Elementos que SI tienen el Schema");
                TaskDialog.Show("Manual Revit API", string.Join("\n", names));

            }
            #endregion

            // 3.2 FILTROS LENTOS

            #region ------------------------------------- FALTA COMPLETAR FILTROS LENTOS  ZZZZZZZZZZ ---------------

            catch (Exception ex)
            {
                message = ex.ToString();
            }

            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            #endregion

            // 4.0 ----------------------------------- CREACION DE ELEMENTOS ------------------------------------

            // 4.1 CREACION DE UN MURO

            #region MURO DESDE UNA LINEA
            try
            {
                // cREAMOS UN MURO SEGUN UNA LINEA
                // Recogemos el primer nivel de todos los niveles existentes
                FilteredElementCollector Colector_NivelesExistentes = new FilteredElementCollector(doc).OfClass(typeof(Level));
                Level Nivel_1 = Colector_NivelesExistentes.First() as Level;

                XYZ P1 = XYZ.Zero;
                XYZ P2 = new XYZ(10,0,0);

                // Creamos un vector vertical
                XYZ VectorVertical = new XYZ(0,0,10);

                //Curva para crear muro por linea
                Curve c1 = Line.CreateBound(P1, P2);

                //Creamos transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos transaction
                    tx.Start("Transaction Name");
                    //Creamos muro por curve
                    Wall wallPorLinea = Wall.Create(doc, c1, Nivel_1.Id, true);
                    //Confirmamos transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region MURO CREADO DESDE UN PERIMETRO VERTICAL
            try
            {
                // CREAMOS UN MURO USANDO UN PERIMETRO VERTICAL CREADO "MANUALMENTE"
                // Recogemos el primer nivel de todos los niveles existentes
                FilteredElementCollector Colector_NivelesExistentes = new FilteredElementCollector(doc).OfClass(typeof(Level));
                Level Nivel_1 = Colector_NivelesExistentes.First() as Level;

                // Creamos 4 puntos en planta, formando una cuadricula de 10 x 10
                XYZ P1 = XYZ.Zero;
                XYZ P2 = new XYZ(10, 0, 0);
                XYZ P3 = new XYZ(10, 10, 0);
                XYZ P4 = new XYZ(0, 10, 0);

                // Creamos un vector vertical
                XYZ VectorVertical = new XYZ(0, 0, 10);

                //Construimos 4 curvas para perímero del muro.
                //Podemos crear cualquier perimetro
                Curve c2Planta = Line.CreateBound(P2, P3);
                Curve c2V1 = Line.CreateBound(P2 + VectorVertical, P2);
                Curve c2V2 = Line.CreateBound(P3, P3 + VectorVertical);
                Curve c2Techo = Line.CreateBound(P2 + VectorVertical, P3 + VectorVertical);

                //Construimos lista de curvas para perímetro
                List<Curve> perimetroVertical = new List<Curve>() { c2Planta, c2V1, c2Techo, c2V2 };

                //Creamos transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos transaction
                    tx.Start("Transaction Name");
                    //Creamos muro por perímetro VERTICAL
                    Wall wallPorPerimetro = Wall.Create(doc, perimetroVertical, true);
                    //Confirmamos transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 4.2 CREACION DE UN SUELO

            #region SUELO CREADO CON UN AREA HORIZONTAL
            try
            {
                // CREACION DE UN SUELO A PARTIR DE UN PickBox.
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
            #endregion

            #region SUELO CON PENDIENTE CREADO DESDE UN PICKBOX
            try
            {
                // CREACION DE UN SUELO CON PENDIENTE A PARTIR DE UN PickBox.
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

                    // Crear el suelo // REVISAR SI ESTO ESTA CORRECTO... TENGO MIS DUDAS
                    Floor.Create(doc, Lista_CurveLoop_1, Suelo_PorDefecto_1.Id, Nivel_1.Id, true, Linea_1 as Line, 1);

                    // Confirmar la transacción
                    Transaccion_Suelos_1.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 4.3 MODIFICACION DEL SUELO - Video RAPI 8.3

            #region MODIFICACION DEL SUELO ----------------------------------- REVISAR
            try
            {
                //Filtramos los niveles, metodo abreviado. Filtro de clase
                FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Level));
                //Seleccionamos el primer nivel de la colección
                Level level = col.First() as Level;

                //Creamos 4 puntos en planta. Cuadricula 10*10
                XYZ xYZ0 = XYZ.Zero;
                XYZ xYZ1 = new XYZ(10, 0, 0);
                XYZ xYZ2 = new XYZ(10, 10, 0);
                XYZ xYZ3 = new XYZ(0, 10, 0);

                //Creamos primer conjunto de Curves
                Curve c0 = Line.CreateBound(xYZ0, xYZ1);
                Curve c1 = Line.CreateBound(xYZ1, xYZ2);
                Curve c2 = Line.CreateBound(xYZ2, xYZ3);
                Curve c3 = Line.CreateBound(xYZ3, xYZ0);

                //Creamos primer CurveLoop
                CurveLoop profileSuelo = new CurveLoop();
                profileSuelo.Append(c0);
                profileSuelo.Append(c1);
                profileSuelo.Append(c2);
                profileSuelo.Append(c3);

                //Obtenemos tipo de suelo por defecto
                FloorType floorType = doc.GetElement(doc.GetDefaultElementTypeId(ElementTypeGroup.FloorType)) as FloorType;

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Creación Suelos");

                    //Creamos suelo arquitectónico
                    Floor floor = Floor.Create(doc, new List<CurveLoop> { profileSuelo }, floorType.Id, level.Id);

                    ////Opción 1.Regeneramos para forzar cálculo geometría. 
                    //   doc.Regenerate();

                    ////Opción 2. Desplazomo vector nulo para forzar cálculo geometría. 
                    ElementTransformUtils.MoveElement(doc, floor.Id, XYZ.Zero);

                    ////Obtenemos el SlabShapeEditor del suelo arquitectónico
                    SlabShapeEditor slabShapeEditor = floor.SlabShapeEditor;

                    ////Creamos dos vertices nuevos en slabShapeEditor
                    SlabShapeVertex slabShapeVertex0 = slabShapeEditor.DrawPoint(new XYZ(5, 0, 0.3));
                    SlabShapeVertex slabShapeVertex1 = slabShapeEditor.DrawPoint(new XYZ(5, 10, 0.3));

                    //Creamos linea divisoria en slabShapeEditor
                    slabShapeEditor.DrawSplitLine(slabShapeVertex0, slabShapeVertex1);

                    //Confirmamos transaction
                    tx.Commit();

                    TaskDialog.Show("Manual Revit API", "Suelos creados");
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 4.4 CREACION DE PLATAFORMAS DE NIVELACION.

            #region PLATAFORMA A PARTIR DE 2 PERIMETROS HORIZONTALES
            try
            {
                //Filtramos los niveles, metodo abreviado. Filtro de clase
                FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Level));
                //Seleccionamos el primer nivel de la colección
                Level level = col.First() as Level;

                //Creamos 4 puntos en planta. Cuadricula 20*20
                XYZ xYZ0 = new XYZ(0, 0, -1);
                XYZ xYZ1 = new XYZ(20, 0, -1);
                XYZ xYZ2 = new XYZ(20, 20, -1);
                XYZ xYZ3 = new XYZ(0, 20, -1);

                //Creamos 4 puntos en planta. Cuadricula 10*10
                XYZ xYZ5 = new XYZ(5, 5, -1);
                XYZ xYZ6 = new XYZ(15, 5, -1);
                XYZ xYZ7 = new XYZ(15, 15, -1);
                XYZ xYZ8 = new XYZ(5, 15, -1);

                //Creamos primer conjunto de Curves
                Curve c0 = Line.CreateBound(xYZ0, xYZ1);
                Curve c1 = Line.CreateBound(xYZ1, xYZ2);
                Curve c2 = Line.CreateBound(xYZ2, xYZ3);
                Curve c3 = Line.CreateBound(xYZ3, xYZ0);

                //Creamos segundo conjunto de Curves
                Curve c4 = Line.CreateBound(xYZ5, xYZ6);
                Curve c5 = Line.CreateBound(xYZ6, xYZ7);
                Curve c6 = Line.CreateBound(xYZ7, xYZ8);
                Curve c7 = Line.CreateBound(xYZ8, xYZ5);

                IList<CurveLoop> curveLoops = new List<CurveLoop>();

                CurveLoop curvesA = new CurveLoop();
                curvesA.Append(c0);
                curvesA.Append(c1);
                curvesA.Append(c2);
                curvesA.Append(c3);

                CurveLoop curvesB = new CurveLoop();

                curvesB.Append(c4);
                curvesB.Append(c5);
                curvesB.Append(c6);
                curvesB.Append(c7);
                //Añadimos las dos CurveLoop a  IList<CurveLoop>. La segunda es interior a la primera
                curveLoops.Add(curvesA);
                curveLoops.Add(curvesB);
                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transactión
                    tx.Start("Transaction Name");

                    ElementId buiddingId = doc.GetDefaultElementTypeId(ElementTypeGroup.BuildingPadType);
                    //Creamos la plataforma. Necesitamos tener creada una Topografia capaz de hospedar la plataforma
                    Autodesk.Revit.DB.Architecture.BuildingPad buildingPad = Autodesk.Revit.DB.Architecture.
                        BuildingPad.Create(doc, buiddingId, level.Id, curveLoops);
                    ElementTransformUtils.MoveElement(doc, buildingPad.Id, new XYZ(0, 0, -1));
                    //Obtenemos la Topografía generada
                    Autodesk.Revit.DB.Architecture.TopographySurface topo = doc.GetElement(buildingPad.AssociatedTopographySurfaceId)
                        as Autodesk.Revit.DB.Architecture.TopographySurface;
                    //Confirmamos Transaction
                    tx.Commit();

                    TaskDialog.Show("Manual Revit API", "Plataforma de nivelación creada");
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 4.5 CREACION DE CRISTALERAS

            #region CRISTALERAS ------------------------------------------- REVISAR
            try
            {
                // Selección actual. Debemos chequear que la selección es correcta
                Selection sel = uidoc.Selection;

                //Construimos un collector para niveles
                FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Level));
                Level level = col.LastOrDefault() as Level;
                //Construimos un colector para tipos de cubierta
                FilteredElementCollector colRoof = new FilteredElementCollector(doc).OfClass(typeof(RoofType));

                //Construimos un CurveArray, para almacenar el contorno
                CurveArray curveArray = new CurveArray();

                //Obtenemos los elemento seleccionados (Wall o ModelLine)
                ICollection<ElementId> elementIds = sel.GetElementIds();
                foreach (ElementId elementId in elementIds)
                {
                    //Obtenemos el Element
                    Element element = doc.GetElement(elementId);

                    if (element is Wall wall)
                    {
                        //Si es Wall LocationCurve 
                        LocationCurve locationCurve = wall.Location as LocationCurve;
                        curveArray.Append(locationCurve.Curve);
                    }
                    else if (element is ModelLine modelLine)
                    {
                        //Si es ModelLine GeometryCurve
                        curveArray.Append(modelLine.GeometryCurve);
                    }
                }

                // Obtenemos el tipo por defecto para cubierta
                //No podemos garantizar si es cristalera o no
                RoofType roofType = doc.GetElement(doc.GetDefaultElementTypeId(ElementTypeGroup.RoofType)) as RoofType;

                // Obtenemos el primer tipo para cristalera inclinada
                RoofType roofTypeCristalera = colRoof.ToElements().Where(x => x.get_Parameter(BuiltInParameter.CURTAINGRID_ADJUST_BORDER_1) != null).First() as RoofType;

                //Obtenemos el tipo para Cubierta básica
                roofType = colRoof.ToElements().Where(x => x.get_Parameter(BuiltInParameter.CURTAINGRID_ADJUST_BORDER_1) == null).First() as RoofType;

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transactión
                    tx.Start("Crear cubiertas");

                    //Mueva ModelCurveArray para la salida de la cubierta
                    ModelCurveArray modelCurveArray = new ModelCurveArray();
                    //Creamos cubierta y salida
                    FootPrintRoof footPrintRoof = doc.Create.NewFootPrintRoof(curveArray, level, roofType /*roofTypeCristalera*/, out modelCurveArray);

                    ModelCurveArrayIterator modelCurveArrayIterator = modelCurveArray.ForwardIterator();
                    modelCurveArrayIterator.Reset();

                    while (modelCurveArrayIterator.MoveNext())
                    {
                        //Obtenemos la ModelCurve
                        ModelCurve modelCurve = modelCurveArrayIterator.Current as ModelCurve;
                        //Cambiamos pendiente. Intentamos cambiar alero y offset
                        footPrintRoof.set_DefinesSlope(modelCurve, true);
                        footPrintRoof.set_SlopeAngle(modelCurve, Math.PI / 12);
                        footPrintRoof.set_Offset(modelCurve, 10);
                        //  footPrintRoof.set_Overhang(modelCurve, 10);
                    }

                    //Creamos un plano de refrencia.
                    //Cada vez que rodemos se creara. Deberiamos buscar antes si existe uno adecuado.
                    ReferencePlane referencePlane = doc.Create.NewReferencePlane2(XYZ.Zero, XYZ.BasisX, XYZ.BasisZ, uidoc.ActiveView);
                    //Creamos CurveArray para el perfil de extrusión
                    CurveArray curveArrayEx = new CurveArray();
                    //Incluimos dos Line
                    Line line1 = Line.CreateBound(new XYZ(0, 0, 10), new XYZ(10, 0, 20));
                    Line line2 = Line.CreateBound(new XYZ(10, 0, 20), new XYZ(30, 0, 10));
                    curveArrayEx.Append(line1);
                    curveArrayEx.Append(line2);
                    //Creamos ExtrusionRoof
                    ExtrusionRoof extrusionRoof = doc.Create.NewExtrusionRoof(curveArrayEx, referencePlane, level, roofType, 0, 50);

                    tx.Commit();

                    TaskDialog.Show("Manual Revit API", "Cubiertas creadas");
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region OBTENER NIVELES
            try
            {
                // Access current selection

                Selection sel = uidoc.Selection;

                ICollection<ElementId> elementIdsList = sel.GetElementIds();
                if (elementIdsList.Count != 1)
                {
                    message = "Debe selecionar solo un elemento Wall.";
                    return Result.Failed;
                }
                else if (doc.GetElement(elementIdsList.FirstOrDefault()) is Wall wall)
                {
                    Level level = doc.GetElement(wall.LevelId) as Level;
                    string msg = string.Empty;


                    //obtenenos el nombre
                    msg = msg + ("\nNombre del nivel: " + level.Name);

                    //obtenemos la elevación sobre el origen de coordenadas
                    msg = msg + ("\nElevación sobre el origen de coordenadas (unidades internas): " + level.Elevation.ToString("N2"));

                    // obtenemos la elevación sobre el proyecto
                    msg = msg + ("\nElevación sobre el proyecto (unidades internas): " + level.ProjectElevation.ToString("N2"));

                    // obtenemos la vista asociada, si existe
                    ElementId elementId = level.FindAssociatedPlanViewId();
                    if (elementId != ElementId.InvalidElementId)
                        msg = msg + ("\nVista asociada: " + doc.GetElement(elementId).Name);
                    else
                        msg = msg + ("\n\tNo existe vista asociada");

                    //Obtenemos el tipo y buscamos se Base de elevación
                    LevelType levelType = doc.GetElement(level.GetTypeId()) as LevelType;
                    msg = msg + ("\n\n\"Base de elevación\" del tipo: " +
                        levelType.get_Parameter(BuiltInParameter.LEVEL_RELATIVE_BASE_TYPE).AsValueString());

                    //Solo en versiones 2020 y posteriores
                    double cotaParaBuscarNivel = -0.55; //en unidades internas
                                                        //Obtenemos el nivel mas cercano a la cota cotaParaBuscarNivel
                    ElementId id = Level.GetNearestLevelId(doc, cotaParaBuscarNivel, out double diferencia);
                    Level nivelCercano = doc.GetElement(id) as Level;
                    //Obtenemos el nivel y tambien la diferencia de cotas en unidades internas
                    msg = msg + ("\n\nEl nivel mas cercano a la cota (uninades internas): " + cotaParaBuscarNivel +
                        " es: " + nivelCercano.Name +
                        "\nLa diferencia de cota es (uninades internas): " + diferencia.ToString("N2"));

                    //Mostramos toda la información
                    TaskDialog.Show("Manual Revit API", msg);
                }
                else
                {
                    message = "Debe selecionar un ejemplar de Wall.";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREAR NIVELES
            try
            {

                // Selección actual
                Selection sel = uidoc.Selection;

                ICollection<ElementId> elementIdsList = sel.GetElementIds();
                //Cpmprobamos que solo tenemos 1 objeto seleccionado
                if (elementIdsList.Count != 1)
                {
                    message = "Debe selecionar solo un elemento Wall.";
                    return Result.Failed;
                }
                //Chequeamos si es Wall y almacenamos en wall.
                //Si no es Wall continuamos
                else if (doc.GetElement(elementIdsList.FirstOrDefault()) is Wall wall)
                {
                    BoundingBoxXYZ boundingBoxXYZ = null;
                    BoundingBoxXYZ boundingBoxXYZView = null;

                    //Obtenemos del muro el boundingBoxXYZ
                    boundingBoxXYZ = wall.get_BoundingBox(null);

                    //Obtenemos el nivel del muro
                    Level level = doc.GetElement(wall.LevelId) as Level;
                    // obtenemos la vista asociada, si existe
                    ElementId elementId = level.FindAssociatedPlanViewId();
                    if (elementId != ElementId.InvalidElementId)
                    {
                        View viewLevel = doc.GetElement(elementId) as View;
                        boundingBoxXYZView = wall.get_BoundingBox(viewLevel);
                    }

                    // La elevación la ponemos en la semi suma de las Zs del boundingBoxXYZ
                    double elevation = (boundingBoxXYZ.Max.Z + boundingBoxXYZ.Min.Z) / 2;
                    double elevationView = (boundingBoxXYZView.Max.Z + boundingBoxXYZView.Min.Z) / 2;

                    // Creamos el nivel. Usamos transaction
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.SetName("Creación de Nivel");
                        tx.Start();
                        Level levelGeom = Level.Create(doc, elevation);
                        Level levelView = Level.Create(doc, elevationView);

                        // Cambiamos el nombre
                        levelGeom.Name = "Nivel medio (muro)";
                        levelView.Name = "Nivel medio (vista)";

                        tx.Commit();
                    }

                }
                else
                {
                    message = "Debe selecionar un ejemplar de Wall.";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREAR REGILLAS
            try
            {
                //Creamos una lista de curves
                List<Curve> curves = new List<Curve>();

                //Creamos 4 Line y las añadimos a la lista
                curves.Add(Line.CreateBound(new XYZ(-18, -20, 0), new XYZ(-18, 20, 0)));
                curves.Add(Line.CreateBound(new XYZ(18, -20, 0), new XYZ(18, 20, 0)));
                curves.Add(Line.CreateBound(new XYZ(20, -18, 0), new XYZ(-20, -18, 0)));
                curves.Add(Line.CreateBound(new XYZ(20, 18, 0), new XYZ(-20, 18, 0)));

                using (Transaction tx = new Transaction(doc, "Crear Grids"))
                {
                    tx.Start();
                    //Para cada Line creamos una Grid
                    foreach (Line line in curves)
                        Grid.Create(doc, line);
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region OBTENEMOS INFORMACION DE LA REGILLA DE UNA COLUMNA
            try
            {

                //Selección actual
                Selection sel = uidoc.Selection;

                ICollection<ElementId> elementIdsList = sel.GetElementIds();
                if (elementIdsList.Count != 1)
                {
                    message = "Debe selecionar solo un elemento Pilar structural.";
                    return Result.Failed;
                }
                else if (doc.GetElement(elementIdsList.FirstOrDefault()) is FamilyInstance pilar
                    //además de ser Familiinstance debe tener categoría OST_StructuralColumns
                    && pilar.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                {
                    string marca = pilar.get_Parameter(BuiltInParameter.COLUMN_LOCATION_MARK).AsString();
                    // Doble Split, primero seleccionamos rejilla, y despues el nombre sin el desfase
                    string nombreRejilla = marca.Split('-')[0].Split('(')[0];
                    if (String.IsNullOrEmpty(nombreRejilla))
                    {
                        message = "El Pilar estructural no está asociado a ninguna rejilla.";
                        return Result.Failed;
                    }
                    // Filtramos para obtener Grids del proyecto
                    FilteredElementCollector col = new FilteredElementCollector(doc);
                    ICollection<Element> grids = col.OfClass(typeof(Grid)).ToElements();

                    // Obtenemos la primera y unica rejilla que cumple
                    Grid grid = grids.Where(x => x.Name == nombreRejilla).FirstOrDefault() as Grid;

                    string msg = "Rejilla : " + grid.Name;

                    // Obtenenos si es arco
                    msg += "\nLa rejilla es Arc: " + grid.IsCurved;

                    // Obtenemos la Curve
                    Autodesk.Revit.DB.Curve curve = grid.Curve;
                    if (grid.IsCurved)
                    {
                        //Si es Arc, obtenemos centro y radio
                        Autodesk.Revit.DB.Arc arc = curve as Autodesk.Revit.DB.Arc;
                        msg += "\nRadio del Arc: " + arc.Radius;
                        msg += "\nCentro del Arc:  (" + XYZString(arc.Center);
                    }
                    else
                    {
                        // Si es Line, obtenemos longitud
                        Autodesk.Revit.DB.Line line = curve as Autodesk.Revit.DB.Line;
                        msg += "\nLongitud de la Line: " + line.Length;
                    }
                    // Punto inicial
                    msg += "\nPunto inicial: " + XYZString(curve.GetEndPoint(0));
                    // Punto final
                    msg += "\nPunto final: " + XYZString(curve.GetEndPoint(1));

                    // Punto inicial
                    XYZString(curve.Tessellate()[0]);
                    // Punto final
                    XYZString(curve.Tessellate()[1]);

                    TaskDialog.Show("Manual Revit API", msg);
                }
                else
                {
                    message = "Debe selecionar un ejemplar de Pilar estructural.";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 5.0 ----------------------------------- EDICION DE ELEMENTOS ------------------------------------

            // 5.1 MOVER ELEMENTOS

            #region CAMBIAR LA UBICACION DE UN ELEMENTO, EN ESTE CASO, UNA COLUMNA QUE ESTA BASADO EN UN LocationPoint - USAMOS MoveElement EN EL Id DE LA FAMILIA
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos un objeto seleccionado
                if (sel.GetElementIds().Count != 1)
                {
                    message = "Se debe seleccionar un solo elemento";
                    return Result.Failed;
                }

                // Chequeamos que el objeto seleccionado es FamilyInstance
                if (doc.GetElement(sel.GetElementIds().First()) is FamilyInstance familyInstance)
                {
                    // Chequeamos la categoría de la FamilyInstance
                    if (familyInstance.Category.Id.IntegerValue != (int)BuiltInCategory.OST_StructuralColumns &&
                        familyInstance.Category.Id.IntegerValue != (int)BuiltInCategory.OST_Columns)
                    {
                        message = "Se debe seleccionar Pilar";
                        return Result.Failed;
                    }

                    // Chequeamos que el pilar esta basado en punto
                    if (familyInstance.Location is LocationPoint locationPoint)
                    {
                        // Creamos transaction
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Transaction Desplazar");
                            // Creamos el vector de desplazamiento
                            XYZ xYZDesplazado = new XYZ(10, 10, 10);
                            // Desplazamos el elemento
                            ElementTransformUtils.MoveElement(doc, familyInstance.Id, xYZDesplazado);
                            //Confirmamos transaction
                            tx.Commit();
                        }
                    }
                    else
                    {
                        message = "Se debe seleccionar Pilar vertical";
                        return Result.Failed;
                    }

                }
                else
                {
                    message = "Se debe seleccionar instancia";
                    return Result.Failed;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Pilar desplazado");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region AHORA CAMBIAMOS SU UBICACION REDEFINIENDO LA locationPoint, ES OTRA MANERA DE LOGRAR LO MISMO.
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos un objeto seleccionado
                if (sel.GetElementIds().Count != 1)
                {
                    message = "Se debe seleccionar un solo elemento";
                    return Result.Failed;
                }

                // Chequeamos que el objeto seleccionado es FamilyInstance
                if (doc.GetElement(sel.GetElementIds().First()) is FamilyInstance familyInstance)
                {
                    // Chequeamos la categoría de la FamilyInstance
                    if (familyInstance.Category.Id.IntegerValue != (int)BuiltInCategory.OST_StructuralColumns &&
                        familyInstance.Category.Id.IntegerValue != (int)BuiltInCategory.OST_Columns)
                    {
                        message = "Se debe seleccionar Pilar";
                        return Result.Failed;
                    }

                    // Chequeamos que el pilar esta basado en punto
                    if (familyInstance.Location is LocationPoint locationPoint)
                    {
                        // Creamos transaction
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Transaction Desplazar");
                            // Creamos el vector de desplazamiento
                            XYZ xYZDesplazado = new XYZ(10, 10, 10);
                            //Obtenemos el origen del pilar
                            XYZ xYZOriginal = locationPoint.Point;
                            // Modificamos la locationPoint, sumando los vectores
                            locationPoint.Point = xYZOriginal + xYZDesplazado;
                            tx.Commit();
                        }
                    }
                    else
                    {
                        message = "Se debe seleccionar Pilar vertical";
                        return Result.Failed;
                    }

                }
                else
                {
                    message = "Se debe seleccionar instancia";
                    return Result.Failed;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Pilar desplazado");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CAMBIAMOS LA UBICACION MOVIENDO UNA LocationCurve DE UN MURO USANDO Move()
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos un objeto seleccionado
                if (sel.GetElementIds().Count != 1)
                {
                    message = "Se debe seleccionar un solo elemento";
                    return Result.Failed;
                }

                // Chequeamos que el objeto seleccionado es Muro
                if (doc.GetElement(sel.GetElementIds().First()) is Wall wall)
                {

                    // Chequeamos que el muro esta basado en linea. Puede ser un muro basado en masa, o "In situ"
                    if (wall.Location is LocationCurve locationCurve)
                    {
                        // Creamos transaction
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Transaction Desplazar");
                            // desplazo la location curve un vector
                            locationCurve.Move(new XYZ(10, 10, 10));

                            TaskDialog.Show("Manual Revit API", "Elemento desplazado, desplazando su LocationCurve");
                            //Confirmamos transaction
                            tx.Commit();
                        }
                    }
                    else
                    {
                        message = "Se debe seleccionar muro basado en linea";
                        return Result.Failed;
                    }

                }
                else
                {
                    message = "Se debe seleccionar muro";
                    return Result.Failed;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Muro desplazado");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region REDEFINIMOS LA LOCATION CURVE DE UN MURO USANDO UNA LINEA PREVIAMENTE CREADA
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos un objeto seleccionado
                if (sel.GetElementIds().Count != 1)
                {
                    message = "Se debe seleccionar un solo elemento";
                    return Result.Failed;
                }

                // Chequeamos que el objeto seleccionado es Muro
                if (doc.GetElement(sel.GetElementIds().First()) is Wall wall)
                {

                    // Chequeamos que el muro esta basado en linea. Puede ser un muro basado en masa, o "In situ"
                    if (wall.Location is LocationCurve locationCurve)
                    {
                        // Creamos transaction
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Transaction Desplazar");

                            //Creamos 2 puntos
                            //Obedeceran a alguna lógica y por norma general serán calculados
                            XYZ xYZ0 = new XYZ(-10, -10, 0);
                            XYZ xYZ1 = new XYZ(10, 10, 0);
                            //Creamos una nueva Line
                            Line line = Line.CreateBound(xYZ0, xYZ1);
                            //Asignamos la bueva Line a Location
                            locationCurve.Curve = line;
                            TaskDialog.Show("Manual Revit API", "Elemento desplazado, desplazando su LocationCurve");
                            //Confirmamos transaction
                            tx.Commit();
                        }
                    }
                    else
                    {
                        message = "Se debe seleccionar muro basado en linea";
                        return Result.Failed;
                    }

                }
                else
                {
                    message = "Se debe seleccionar muro";
                    return Result.Failed;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Muro desplazado");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 5.2 COPIAR ELEMENTOS

            #region COPIAR ELEMENTOS
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Creamos el vector de copia
                XYZ vector = new XYZ(10, 10, 10);

                // Creamos transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Copiar");
                    // Creamos una Colleccion para almacenar los nuevos Element
                    ICollection<ElementId> elementosCopiados = new List<ElementId>();

                    //Obtenemos el numero de objetos de la selección previa
                    int numeroObjetos = sel.GetElementIds().Count;

                    if (numeroObjetos == 0)
                    {
                        message = "Se debe seleccionar algún objeto";
                        return Result.Failed;
                    }
                    else if (numeroObjetos == 1)
                    {
                        //Si se ha seleccionado 1
                        elementosCopiados = ElementTransformUtils.CopyElement(doc, sel.GetElementIds().First(), vector);
                    }
                    else
                    {
                        //Si se han seleccionado varios
                        elementosCopiados = ElementTransformUtils.CopyElements(doc, sel.GetElementIds(), vector);
                    }

                    //Confirmamos Transaction
                    tx.Commit();

                    TaskDialog.Show("Manual Revit API", elementosCopiados.Count + " objetos copiados");

                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 5.3 ROTAR ELEMENTOS

            #region ROTAR ELEMENTOS SEGUN UN EJE. ESTE SE CREA CON UN "CreateUnbound", QUE PASA POR EL EJE DE LA VIGA.
            try
            {
                //Creamos una Reference para poder seleccionar la viga
                Reference reference = null;
                //Creamos ISelectionFilter solo admitimos BuiltInCategory.OST_StructuralFraming
                ISelectionFilter beamSeleccionFilter = new BeamSeleccionFilter();

                try
                {
                    //Siempre PickObject en estructura try{ç carch{}
                    reference = uidoc.Selection.PickObject(ObjectType.Element, beamSeleccionFilter, "Seleccionar viga");
                }
                catch (Exception ex)
                {
                    // Si falla la selección de viga
                    message = ex.Message;
                    return Result.Failed;

                }
                //Obtenemos el Elemen, dado que PickObject no ha fallado
                Element element = doc.GetElement(reference.ElementId);

                //Obtenemos Location. como es viga lo convertimos a LocationCurve
                LocationCurve locationCurve = element.Location as LocationCurve;
                //Obtenemos curve
                Curve curve = locationCurve.Curve;
                //Comprobamos que es segmento rectilineo
                Line line = null;
                if (curve is Line)
                {
                    line = curve as Line;
                }
                else
                {
                    message = "Solo vigas rectilineas";
                    return Result.Failed;
                }
                //Obtenemos los puntos inicial y final.
                //Considere utilizar Tessellate() en lugar de GetEndPoint()
                //IList<XYZ> xYZs  line.Tessellate();

                XYZ xYZe0 = line.GetEndPoint(0);//Punto inicial
                XYZ xYZe1 = line.GetEndPoint(1);//Punto final

                //Obtenemos punto medio
                XYZ xYZgiro = (xYZe0 + xYZe1) / 2;
                //Creamos eje de giro desde el punto medio
                Line ejeGiro = Line.CreateUnbound(xYZgiro, XYZ.BasisZ);

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name");

                    //Rotamos el elemento
                    ElementTransformUtils.RotateElement(doc, element.Id, ejeGiro, Math.PI / 4);

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region ROTAR ELEMENTO SEGUN UN PLANO INCLINADO
            try
            {

                //Creamos una Reference para poder seleccionar la viga
                Reference reference = null;
                //Creamos ISelectionFilter solo admitimos BuiltInCategory.OST_StructuralFraming
                ISelectionFilter beamSeleccionFilter = new BeamSeleccionFilter();

                try
                {
                    //Siempre PickObject en estructura try{ç carch{}
                    reference = uidoc.Selection.PickObject(ObjectType.Element, beamSeleccionFilter, "Seleccionar viga");
                }
                catch (Exception ex)
                {
                    // Si falla la selección de viga
                    message = ex.Message;
                    return Result.Failed;

                }
                //Obtenemos el Elemen, dado que PickObject no ha fallado
                Element element = doc.GetElement(reference.ElementId);
                //Obtenemos Location. como es viga lo convertimos a LocationCurve
                LocationCurve locationCurve = element.Location as LocationCurve;
                //Obtenemos curve
                Curve curve = locationCurve.Curve;
                //Comprobamos que es segmento rectilineo
                Line line = null;
                if (curve is Line)
                {
                    line = curve as Line;
                }
                else
                {
                    message = "Solo vigas rectilineos";
                    return Result.Failed;
                }
                //Obtenemos los puntos inicial y final.
                //Considere utilizar Tessellate() en lugar de GetEndPoint()
                //IList<XYZ> xYZs  line.Tessellate();

                XYZ xYZe0 = line.GetEndPoint(0);//Punto inicial
                XYZ xYZe1 = line.GetEndPoint(1);//Punto final

                //Obtenemos punto medio
                XYZ xYZgiro = (xYZe0 + xYZe1) / 2;

                #region Eje vertical - ESTE METODO NO OTORGA RESULTADOS
                //Creamo un eje verical. Es incorrecto
                Line ejeGiro = Line.CreateBound(xYZgiro, xYZgiro + XYZ.BasisZ);
                #endregion

                #region Eje desde GetTransform().BasisZ - ESTE METODO SI FUNCIONA
                //Obtenemos la FamilyInstance
                FamilyInstance familyInstance = (element as FamilyInstance);
                //Obtenemos la normal desde familyInstance.GetTransform()
                Line ejeGiro_1 = Line.CreateUnbound(xYZgiro, familyInstance.GetTransform().BasisZ);
                #endregion

                #region Eje desde FaceNormal.Normalize() - ESTE METODO SI FUNCIONA
                //Obtenemos la FamilyInstance
                FamilyInstance familyInstance_1 = (element as FamilyInstance);
                //Como la viga esta hospedad en plano de cara, podemos obtener la cara
                Reference referenceFace = familyInstance_1.HostFace;
                //Obtenemos la cubierta desde la Reference
                Element cubierta = doc.GetElement(referenceFace.ElementId);
                //El plano seleccionado del faldón seleccionado es una cara "plana" luego podemos obtener su PlanarFace
                PlanarFace face = cubierta.GetGeometryObjectFromReference(referenceFace) as PlanarFace;
                //Obtenemos la normal de la Face. La normalizamos
                XYZ normal = face.FaceNormal.Normalize();
                Line ejeGiro_2 = Line.CreateUnbound(xYZgiro, normal);
                #endregion

                #region Eje desde producto vectorial - SI FUNCIONA
                //Creamos un Plane horizontal por el xYZe0
                Plane planeH = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, xYZe0);
                //****Error no es válido
                //Creamos la proyección de xYZe1 sobre el plano
                //Este método no es valido. no da una distancia orientada
                // plane.Project(xYZe0, out UV uVEnPlano, out double distancia);
                //****Error no es valido

                //Recurrimos a trigonometría
                //Vector de un punto en viga a un punto en plano
                XYZ vector = xYZe1 - planeH.Origin;
                //Distancia con signo Producto escalar
                double distancia = planeH.Normal.DotProduct(vector);
                //XYZ proyeccion del punto en viga sobre el plano horizontal
                XYZ xYZplaneH = xYZe1 - distancia * planeH.Normal;
                //XYZ vector del "alero", Producto vectorial (linea sobre plano horizontal * linea viga)
                XYZ vectorAlero = (xYZplaneH - xYZe1).CrossProduct(xYZe1 - xYZe0);
                //XYZ normal plano faldon= Producto vectorial (vectorAlero* linea viga)
                XYZ vectorNormalFaldon = vectorAlero.CrossProduct(xYZe1 - xYZe0);
                //Eje de giro
                Line ejeGiro_3 = Line.CreateUnbound(xYZgiro, vectorNormalFaldon);
                #endregion

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name");

                    //Rotamos el elemento
                    ElementTransformUtils.RotateElement(doc, element.Id, ejeGiro, Math.PI / 2);

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            //  5.4 SIMETRIA

            #region SIMETRIA A UN PLANO
            try
            {
                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos un objeto seleccionado
                if (sel.GetElementIds().Count != 1)
                {
                    message = "Se debe seleccionar un solo elemento";
                    return Result.Failed;
                }

                // Chequeamos que el objeto seleccionado es Muro
                if (doc.GetElement(sel.GetElementIds().First()) is Wall wall)
                {

                    // Chequeamos que el muro esta basado en linea. Puede ser un muro basado en masa, o "In situ"
                    if (wall.Location is LocationCurve locationCurve)
                    {
                        // Creamos transaction
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Transaction simetria");
                            //Creamos plano que pasa po (0,0,0) y normal (0,1,0)
                            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero);
                            //Simetria del muro manteniendo el original
                            //ElementTransformUtils.MirrorElement(doc, wall.Id, plane);
                            //TaskDialog.Show("Manual Revit API", "Elemento reflejado respecto al eje X. \nManteniendo original");

                            //Simetria del muro borrando el original
                            IList<ElementId> idsCopiados = ElementTransformUtils.MirrorElements(doc, new List<ElementId>() { wall.Id }, plane, true);
                            TaskDialog.Show("Manual Revit API", "Elemento reflejado respecto al eje X. \nBorrando original");

                            //Confirmamos transaction
                            tx.Commit();
                        }
                    }
                    else
                    {
                        message = "Se debe seleccionar muro basado en linea";
                        return Result.Failed;
                    }
                }
                else
                {
                    message = "Se debe seleccionar muro";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREACION DE MATRICES
            try
            {
                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que tenemos algún objeto seleccionado
                if (sel.GetElementIds().Count == 0)
                {
                    message = "Se debe seleccionar algún elemento";
                    return Result.Failed;
                }


                // Creamos transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Matriz");
                    #region lineal sin asociar
                    //Creamos matriz sin asociar 5 miembros
                    ICollection<ElementId> elementIdsSinAsociar = LinearArray.ArrayElementWithoutAssociation(doc, uidoc.ActiveView, sel.GetElementIds().First(), 5, new XYZ(0, 3, 0), ArrayAnchorMember.Second);
                    #endregion

                    #region lineal con asociación
                    //Creamos matriz lineal con asociación 5 miembros
                    LinearArray linearArray = LinearArray.Create(doc, uidoc.ActiveView, sel.GetElementIds(), 5, new XYZ(30, 0, 0), ArrayAnchorMember.Last);
                    //Podemos obtener los ElementId de los grupos de la matriz
                    ICollection<ElementId> elementIds = linearArray.GetOriginalMemberIds();
                    //Redimensionamo la matriz a 10
                    linearArray.NumMembers = 10;
                    #endregion

                    #region radial con asociación
                    //Creamos eje vertical por el (0,0,0)
                    Line eje = Line.CreateBound(XYZ.Zero, XYZ.Zero + XYZ.BasisZ);
                    //Creamos matriz radial
                    RadialArray radialArray = RadialArray.Create(doc, uidoc.ActiveView, elementIds.First(), 5, eje, Math.PI, ArrayAnchorMember.Last);
                    #endregion
                    //Confirmamos transaction
                    tx.Commit();
                }

                TaskDialog.Show("Manual Revit API", "Matrices creadas");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 5.5 TRANSFORMAR ELEMENTOS

            #region TRANSFORMAR ELEMENTOS
            try
            {
                //Creamos cuatro puntos cuadricula 10*10
                XYZ xYZ1 = XYZ.Zero;
                XYZ xYZ2 = new XYZ(10, 0, 0);
                XYZ xYZ3 = new XYZ(10, 10, 0);
                XYZ xYZ4 = new XYZ(0, 10, 0);

                //Creamos dos Transform
                Transform transform1 = Transform.CreateTranslation(new XYZ(20, 20, 0));
                Transform transform2 = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI / 4, XYZ.Zero);

                //Creamos cuatro Lines
                Line line1 = Line.CreateBound(xYZ1, xYZ2);
                Line line2 = Line.CreateBound(xYZ2, xYZ3);
                Line line3 = Line.CreateBound(xYZ3, xYZ4);
                Line line4 = Line.CreateBound(xYZ4, xYZ1);

                //Creamos un nuevo conjunto de Lines acumulando las Transform
                line1 = line1.CreateTransformed(transform1.Multiply(transform2)) as Line;
                line2 = line2.CreateTransformed(transform1.Multiply(transform2)) as Line;
                line3 = line3.CreateTransformed(transform1.Multiply(transform2)) as Line;
                line4 = line4.CreateTransformed(transform1.Multiply(transform2)) as Line;

                //Creamos un nuevo plano. Coincidente con el horizontal del Nivel 1
                //   Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Inicioamos Transaction
                    tx.Start("Transaction transform");

                    //Creamos SketchPlane
                    //   SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                    //Construccion alternativa desde PlanView
                    //Necesario una ViewPlan
                    ViewPlan viewPlan = doc.ActiveView as ViewPlan;
                    SketchPlane sketchPlane = viewPlan.SketchPlane;

                    //Creamos las 4 ModelLine
                    ModelLine modelLine1 = doc.Create.NewModelCurve(line1, sketchPlane) as ModelLine;
                    ModelLine modelLine2 = doc.Create.NewModelCurve(line2, sketchPlane) as ModelLine;
                    ModelLine modelLine3 = doc.Create.NewModelCurve(line3, sketchPlane) as ModelLine;
                    ModelLine modelLine4 = doc.Create.NewModelCurve(line4, sketchPlane) as ModelLine;

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region TRANSFORMAR DESDE UNA VISTA 3D
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos un objeto seleccionado
                if (sel.GetElementIds().Count != 1)
                {
                    message = "Se debe seleccionar un solo elemento";
                    return Result.Failed;
                }

                // Chequeamos que el objeto seleccionado es Muro
                if (doc.GetElement(sel.GetElementIds().First()) is Wall wall)
                {

                    // Chequeamos que el muro esta basado en linea. Puede ser un muro basado en masa, o "In situ"
                    if (wall.Location is LocationCurve locationCurve)
                    {
                        Line line = locationCurve.Curve as Line;
                        double angle = XYZ.BasisX.AngleTo(line.Direction);
                        // Creamos transaction
                        using (Transaction tx = new Transaction(doc))
                        {
                            //Iniciamos transaction
                            tx.Start("Transaction Desplazar");

                            //Obtenemos la vista actual
                            View view = uidoc.ActiveView;
                            // Si no es 3D finalizamos
                            if (view.ViewType != ViewType.ThreeD)
                            {
                                message = "Situate en una vista 3D";
                                return Result.Failed;
                            }

                            //Obtenemos la Caja de sección
                            BoundingBoxXYZ box = (view as View3D).GetSectionBox();

                            //Giramos según el cuadrante
                            if (angle > Math.PI / 2 && angle < 3 * Math.PI / 2) angle = -angle;

                            //Creamos una Transform con el giro del wall
                            Transform transform = Transform.CreateRotation(XYZ.BasisZ, -angle);
                            //Aplicamos la Transform
                            box.Transform = box.Transform.Multiply(transform);

                            //Asignamos la transformada a la vista 3D
                            (view as View3D).SetSectionBox(box);
                            //Confirmamos transaction

                            tx.Commit();
                        }
                    }
                    else
                    {
                        message = "Se debe seleccionar muro basado en linea";
                        return Result.Failed;
                    }
                }
                else
                {
                    message = "Se debe seleccionar muro";
                    return Result.Failed;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Caja de sección girada");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region ALINEAR ELEMENTOS
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos dos muros seleccionado           
                List<Wall> walls = sel.GetElementIds().Select(x => doc.GetElement(x)).Cast<Wall>().ToList();
                if (walls.Count != 2)
                {
                    message = "Se debe seleccionar dos muros";
                    return Result.Failed;
                }

                //Creamos una cota
                Dimension dimension = null;
                //Obtenemos la vista actual
                View view = doc.ActiveView;
                //Obtenemos las lineas centrales de los dos muros
                //Llamamos al método GetCenterline
                Line baseLine = GetCenterline(walls[0]);
                Line line = GetCenterline(walls[1]);
                //Convertimo la linea en UnBound
                line.MakeUnbound();
                //Proyectamos una linea sobre la otra 
                IntersectionResult result = line.Project(baseLine.Origin);
                //Si la proyección tiene resultado, hay intersección
                if (result != null)
                {
                    //Obtenemos el punro proyectado
                    XYZ point = result.XYZPoint;
                    //Calcylamos el vector de traslación, desde el punto proyectado a su proyección
                    XYZ vector = baseLine.Origin - point;
                    //Creamos una transaction
                    using (Transaction tx = new Transaction(doc))
                    {
                        try
                        {
                            //Iniciamos la transaction
                            tx.Start("Transaction alinear");
                            //Desplazamos el muro. Para alinear deben coincidir 
                            ElementTransformUtils.MoveElement(doc, walls[1].Id, vector);
                            //Ceamos la alineación
                            dimension = doc.Create.NewAlignment(view, baseLine.Reference, line.Reference);
                            //Confirmamos la transaction
                            tx.Commit();
                        }
                        catch (Exception ex)
                        {
                            //S falla anulamos la transaction
                            tx.RollBack();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 5.6 BORRADO DE ELEMENTOS

            #region BORRAR LOS ELEMENTOS SELECCIONADOS
            try
            {
                //Accedemos con algún objetos seleccionado
                Selection sel = uidoc.Selection;
                //creamos una Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Borrar elementos");
                    //Borramos los objetos
                    doc.Delete(sel.GetElementIds());
                    //Confirmamos la Transaction
                    tx.Commit();
                }
                TaskDialog.Show("Revit API Manual", "Elementos borrados");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 6.0 ----------------------------------- UNIDADES ------------------------------------

            // 6.1 CONVERSION DE UNIDADES

            #region CONVERSION DE UNIDADES
            try
            {
                // Access current selection
                Selection sel = uidoc.Selection;

                ICollection<ElementId> elementIdsList = sel.GetElementIds();
                if (elementIdsList.Count != 1)
                {
                    message = "Debe selecionar solo un elemento Wall.";
                    return Result.Failed;
                }
                else if (doc.GetElement(elementIdsList.FirstOrDefault()) is Wall wall)
                {
                    //Obtenemos altura inicial del muro
                    double alturaInicialInterna =
                        wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                    double alturaInicialMetros = UnitUtils.ConvertFromInternalUnits(alturaInicialInterna,
                        UnitTypeId.Meters /*DisplayUnitType.DUT_METERS*/);
                    string msg = "El muro tiene una altura inicial de \"" + alturaInicialMetros + "\" metros, sin redondeos";
                    TaskDialog.Show("Manual Revit API", msg);

                    // --------------------------------------------
                    using (Transaction tx = new Transaction(doc))
                    {
                        //Como modificamos el documento debemos abrir Transaction
                        tx.Start("Modificar altura muro");
                        //multiplicamos altura * 2.15 
                        double nuevaAlturaMetros = alturaInicialMetros * 2.15;
                        //Convertir a unidades internas. 
                        double nuevaAlturaInterna = UnitUtils.ConvertToInternalUnits(nuevaAlturaMetros
                            , UnitTypeId.Meters /*DisplayUnitType.DUT_METERS*/);
                        //Actualizamos el parámetro del muro con el nuevo valor
                        wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(nuevaAlturaInterna);
                        tx.Commit();
                        msg = "El muro tiene ahora una altura de \"" + nuevaAlturaInterna + "\"" +
                            ", unidades internas, sin redondeos.";
                        TaskDialog.Show("Manual Revit API", msg);
                    }
                    
                    // ---------------------------------------------

                    //2 convertir string a numero
                    string alturaTxtMetros = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsValueString();
                    //Obtenemos la configuración actual de unidades del Document
                    Units units = doc.GetUnits();
                    ValueParsingOptions valueParsingOptions = new ValueParsingOptions();

                    bool parsed = UnitFormatUtils.TryParse(units, SpecTypeId.Length /*UnitType.UT_Length*/, alturaTxtMetros
                      /* "10 imposible"*/, valueParsingOptions, out double valorConvertidoDesdeString, out message);

                    if (parsed == false)
                    {
                        message = "Introducir texto correcto en metros";
                        return Result.Failed;
                    }
                    msg = string.Format("El string con formato: \"{0}\", se ha convertido al valor double: \"{1}\""
                        , alturaTxtMetros, valorConvertidoDesdeString);
                    TaskDialog.Show("Manual Revit API", msg);

                    // ---------------------------------------------

                    //Convertir numero a string con el formato de unidades, tomado desde las Units del document
                    //Debe tener en Revit "Simbolo de unidad" y "Usar agrupacion de cifras"

                    //Creamos un valor inventado grande, para poder generar agrupación de cifras
                    double valorInventado = 30000;
                    //Con agrupación de mumeros (punto de millar)
                    string stringConAgrupacion = UnitFormatUtils.Format(units,
                        SpecTypeId.Length /*UnitType.UT_Length*/, valorInventado, /*false,*/ false);

                    //Sin agrupación de mumeros (punto de millar)
                    string stringSinAgrupacion = UnitFormatUtils.Format(units,
                        SpecTypeId.Length /*UnitType.UT_Length*/, valorInventado, /*false,*/ true);

                    msg = string.Format("Numero inventado convertido a unidades actuales.\n\nString con agrupación: \"{0}\",\nString sin agrupación: \"{1}\""
                        , stringConAgrupacion, stringSinAgrupacion);

                    TaskDialog.Show("Manual Revit API", msg);

                    // ---------------------------------------------

                    FormatOptions formatoptions = new FormatOptions();
                    formatoptions = units.GetFormatOptions(SpecTypeId.Length /*UnitType.UT_Length*/);
                    //Asignamos unidades
                    //formatoptions.DisplayUnits = DisplayUnitType.DUT_CENTIMETERS;
                    formatoptions.SetUnitTypeId(UnitTypeId.Centimeters);
                    //Asignamos simbolo
                    //formatoptions.UnitSymbol = UnitSymbolType.UST_CM;
                    formatoptions.SetSymbolTypeId(SymbolTypeId.Cm);
                    FormatValueOptions formatValueOptions = new FormatValueOptions();

                    formatValueOptions.SetFormatOptions(formatoptions);

                    string stringConAgrupacionMod = UnitFormatUtils.Format(units, SpecTypeId.Length,
                        valorConvertidoDesdeString, false, formatValueOptions);

                    //string stringConAgrupacionMod = UnitFormatUtils.Format(units, UnitType.UT_Length,
                    //valorConvertidoDesdeString, false, false, formatValueOptions);

                    msg = string.Format("Valor double: \"{0}\" unidades internas \nModificadas a Centimetros: \"{1}\"",
                        valorConvertidoDesdeString, stringConAgrupacionMod);

                    TaskDialog.Show("Manual Revit API", msg);

                }
                else
                {
                    message = "Debe selecionar un ejemplar de Wall.";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 7.0 ----------------------------------- SYMBOLS, VISTAS, REGIONES, REVISIONES ------------------------------------

            // 7.1 SYMBOLS

            #region CARGAR UN SYMBOL EN ESPECIFICO
            //
            try
            {

                //Creamos un nombre para el fichero
                string nombreFichero = string.Empty;

                //Creamos un formulario de lectura de ficheros
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

                //Obtenemos todas las bibliotecas instaladas
                IDictionary<string, string> keyValuePair = doc.Application.GetLibraryPaths();
                //Obtenemos la que coincide con nuestro nombre
                //Lo pasamos como carpeta inicial al formulario
                openFileDialog.InitialDirectory = keyValuePair["API Revit Manual"];

                //Filtramos tipo de archivos para leer
                openFileDialog.Filter = "Familias de Revit (*.rfa)|*.rfa";

                //Mostramos el formulario
                System.Windows.Forms.DialogResult dialogResult = openFileDialog.ShowDialog();
                //Si cancelamos o cerramos sin seleccionar archivo válido
                if (dialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    message = "Seleccion cancelada";
                    return Result.Cancelled;

                }
                //asignamos el nombre del fichero
                nombreFichero = openFileDialog.FileName;
                if (!System.IO.File.Exists(nombreFichero))
                {
                    message = "El archivo seleccionado no existe";
                    return Result.Failed;
                }
                //Abrimos el fichero de familia.
                //No lo pasamos al UIDocument
                Document familyDoc = doc.Application.OpenDocumentFile(nombreFichero);

                //Leemos desde el FamilyDocument todos los tipos
                FamilyTypeSet familyTypeSet = familyDoc.FamilyManager.Types;
                FamilyTypeSetIterator familyTypeSetIterator = familyTypeSet.ForwardIterator();
                //Reseteamos el Iteraror
                familyTypeSetIterator.Reset();

                //Creamos string para
                string textoSalida = string.Empty;
                //Creamos string para nombre de tipo
                string nombreSimbol = string.Empty;
                while (familyTypeSetIterator.MoveNext())
                {
                    FamilyType familyType = familyTypeSetIterator.Current as FamilyType;
                    nombreSimbol = familyType.Name;
                    if (string.IsNullOrWhiteSpace(nombreSimbol))
                    {
                        //Almacenamos el tipo. Podríamos salir, pero recooremos todos los tipos
                        nombreSimbol = System.IO.Path.GetFileNameWithoutExtension(nombreFichero);
                    }
                    //Construimos string con todos los tipos
                    textoSalida = (textoSalida == string.Empty) ? nombreSimbol : textoSalida + "\n" + nombreSimbol;
                }
                //Mostramos todos los tipos
                TaskDialog.Show("API", textoSalida);
                //Creamos string con nombre completo de archivo.
                //Coincide con el seleccionado en el OpenFileDialog
                string nombrePath = familyDoc.PathName;
                //Cerramos la family. La hemos abierto solo para consultar el nombre
                familyDoc.Close(false);

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name");
                    //Cargamos solamente el primer tipo
                    doc.LoadFamilySymbol(nombrePath, nombreSimbol, new OpcionesCargaFamiliasMin(), out FamilySymbol familySymbol);
                    //Antes de crear una FamilyInstance hay que activar el tipo
                    familySymbol.Activate();
                    //Creamos una FanmilyInstance
                    doc.Create.NewFamilyInstance(XYZ.Zero, familySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    //Conformamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            #endregion

            // 7.2 FASES

            #region CAMBIAR UN OBJETO DE FASE
            try
            {

                //Selección actual
                Selection sel = uidoc.Selection;

                ICollection<ElementId> elementIdsList = sel.GetElementIds();
                if (elementIdsList.Count != 1)
                {
                    message = "Debe selecionar solo un elemento Pilar structural.";
                    return Result.Failed;
                }
                else if (doc.GetElement(elementIdsList.FirstOrDefault()) is FamilyInstance pilar
                    //además de ser Familiinstance debe tener categoría OST_StructuralColumns
                    && pilar.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                {
                    // Obtenemos el listado de Fases desde el Document obtenido del pilar
                    // Podriamos tambien haberlo obtenido del doc
                    PhaseArray phaseArray = pilar.Document.Phases;

                    // Comprobamos que el pilar puede ser cambiado de fase 
                    bool isModificable = pilar.ArePhasesModifiable();

                    if (!isModificable)
                    {
                        // Si no es modificable salimos con Failed
                        message = "No se pueden modificar las Fases del elemento";
                        elements.Insert(pilar);
                        return Result.Failed;
                    }
                    // Obtenemos la Fase de creación del pilar
                    ElementId idFase = pilar.get_Parameter(BuiltInParameter.PHASE_CREATED).AsElementId();

                    // Creamos un Fase para almacenar en su caso la nueva Fase
                    Phase newFase = null;

                    // Buscamos el indice de la fase actual en phaseArray
                    System.Collections.IEnumerator enumerator = phaseArray.GetEnumerator();
                    enumerator.Reset();
                    int n = -1; //Contador
                    while (enumerator.MoveNext())
                    {
                        n++;
                        Phase mPhase = enumerator.Current as Phase;
                        // No puede ser el ultimo de la lista. Dado que queremos el siguiente
                        if (mPhase.Id == idFase && n < phaseArray.Size - 1)
                        {
                            // Obtenemos la Fase siguiente
                            newFase = phaseArray.get_Item(n + 1);
                            break;
                        }
                    }
                    // Si tenemos newFase
                    if (newFase != null)
                    {
                        using (Transaction tx = new Transaction(doc, "Cambio de Fase"))
                        {
                            tx.Start();
                            pilar.get_Parameter(BuiltInParameter.PHASE_CREATED).Set(newFase.Id);
                            tx.Commit();

                            TaskDialog.Show("Manual Revit API", "Nueva Fase para el elemento: " + newFase.Name);

                        }
                    }
                    // Si no tenemos newFase
                    else
                    {
                        TaskDialog.Show("Manual Revit API", "Imposible cambiar de Fase");

                    }
                }
                else
                {
                    message = "Debe selecionar un ejemplar de Pilar estructural.";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 7.3 VISTAS

            #region OPCIONES DE DISEÑO
            try
            {

                //Selección actual
                Selection sel = uidoc.Selection;

                ICollection<ElementId> elementIdsList = sel.GetElementIds();
                if (elementIdsList.Count != 1)
                {
                    message = "Debe selecionar solo un elemento Pilar structural.";
                    return Result.Failed;
                }
                else if (doc.GetElement(elementIdsList.FirstOrDefault()) is FamilyInstance pilar
                    //además de ser Familiinstance debe tener categoría OST_StructuralColumns
                    && pilar.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                {
                    string msg = "Pilar : " + pilar.Name;
                    //Obtenemos la opción de diseño del pilar
                    DesignOption designOption = pilar.DesignOption;
                    msg = msg + "\nOpción de diseño del pilar: " + designOption.Name;

                    // Obtenemos si es opción primaria
                    msg = msg + "\n" + designOption.Name + ", es primaria: " + designOption.IsPrimary;

                    // Obtenemos el ElementID del Conjunto de opciones de designOption
                    ElementId idOptionSet = designOption.get_Parameter(BuiltInParameter.OPTION_SET_ID).AsElementId();

                    // Obtenemos el  Conjunto de opciones
                    Element optionSet = doc.GetElement(idOptionSet);
                    msg = msg + "\n" + designOption.Name + ", pertenece a: " + optionSet.Name;

                    TaskDialog.Show("Manual Revit API", msg);
                }
                else
                {
                    message = "Debe selecionar un ejemplar de Pilar estructural.";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region OBTENER DATOS DE LA VISTA ACTUAL
            try
            {
                // Obtenemos la vista actual
                View view = uidoc.ActiveView;

                //Obtenemos la clase
                TaskDialog.Show("API Revit Manual", "Clase de la vista actual: " + view.GetType().Name);

                //Obtenemos el ViewType de la enumeración
                TaskDialog.Show("API Revit Manual", "ViewType de la vista actual: " + view.ViewType);

                //Obtenemos su tipo
                ViewFamilyType viewFamilyType = doc.GetElement(view.GetTypeId()) as ViewFamilyType;
                TaskDialog.Show("API Revit Manual", "Tipo de la vista actual, ViewFamilyType: " + viewFamilyType.Name);

                //Obtenemos su familia
                TaskDialog.Show("API Revit Manual", "Tipo de la vista actual, ViewFamily: " + viewFamilyType.ViewFamily);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREAR VISTA 3D
            try
            {
                // Access current selection
                //Filtro de clase ViewFamilyType

                FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));
                //Refinamos busqueda a ViewFamily.ThreeDimensional
                ViewFamilyType viewFamilyType = col.Cast<ViewFamilyType>().
                    Where(x => x.ViewFamily == ViewFamily/*.FloorPlan*/.ThreeDimensional).FirstOrDefault();

                // Modify document within a transaction
                // Creamos la transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos la Transaction
                    tx.Start("Creación de vista");

                    View3D mView3D = View3D.CreateIsometric(doc, viewFamilyType.Id);
                    //No asignamos ninguna plantilla
                    mView3D.ViewTemplateId = ElementId.InvalidElementId;
                    //Establecemos nivel de detalle
                    mView3D.DetailLevel = ViewDetailLevel.Fine;
                    // Damos nombre a la vista
                    mView3D.Name = "NuevaVista3D";
                    TaskDialog.Show("API Revit Manual", "Nueva vista 3D creada.");

                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region DUPLICAR VISTA 3D - SE PUEDE DUPLICAR COMO DEPENDIENTE
            try
            {
                //Obtenemos la vista actual
                View view = doc.ActiveView;

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciar Transaction
                    tx.Start("Duplicar vista");
                    // Creamos ViewDuplicateOption por defecto
                    ViewDuplicateOption viewDuplicateOption = ViewDuplicateOption.Duplicate;
                    //Chequeamos que la vista pueda ser duplicada como dependiente
                    if (view.CanViewBeDuplicated(ViewDuplicateOption.AsDependent))
                    {
                        //Si es poible la duplicaremos como dependiente
                        viewDuplicateOption = ViewDuplicateOption.AsDependent;
                    }
                    //Duplicamos la vista actual
                    ElementId newViewId = view.Duplicate(viewDuplicateOption);
                    View viewDuplicada = view.Document.GetElement(newViewId) as View;
                    if (null != viewDuplicada && viewDuplicateOption == ViewDuplicateOption.AsDependent)
                    {
                        if (viewDuplicada.GetPrimaryViewId() == view.Id)
                        {
                            TaskDialog.Show("API Revit Manual", "Vista dependiente duplicada");
                        }
                    }
                    else if (null != viewDuplicada)
                    {
                        TaskDialog.Show("API Revit Manual", "Vista duplicada");

                    }
                    tx.Commit();
                }

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region MANIPULACION DE LA ATENUACION LEJANA DE UNA VISTA
            try
            {

                // Access current selection
                //Vista actual
                View view = uidoc.ActiveView;

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction atenuación lejana");


                    if (!view.CanUseDepthCueing())
                    {
                        message = "La vista no soporta atenuación lejana";
                        return Result.Cancelled;
                    }
                    else
                    {
                        // Obtenemos los indicadores de profundidad
                        ViewDisplayDepthCueing depthCueing = view.GetDepthCueing();
                        // Activamos los indicadores de profundidad
                        depthCueing.EnableDepthCueing = true;
                        //Inicio y fin de fundido
                        depthCueing.SetStartEndPercentages(0, 40);
                        //Establecemos el límite de fundido
                        depthCueing.FadeTo = 20;
                        view.SetDepthCueing(depthCueing);
                    };

                    //Confirmamos Transaction
                    tx.Commit();
                }
                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Atenuación lejana actualizada");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region AJUSTE DEL ZOOM EN VISTA ACTUAL
            try
            {
                //Obtenemos la UIView que coincide con la vista actual
                UIView uiView = uidoc?.GetOpenUIViews()?.FirstOrDefault(item => item.ViewId == uidoc.ActiveView.Id);
                //Ajustamos en pamtalla.
                uiView.ZoomToFit();

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region DESPLAZAR OBJETOS SOLAMENTE EN LA VISTA - UTIL PARA ARMAR POSIBLES ENSAMBLADOS VISUALES.
            try
            {

                //Accedemos a la selección
                Selection sel = uidoc.Selection;
                //Accedemos a la view actual
                View view = uidoc.ActiveView;

                Reference edgeRef = null;
                try
                {
                    // PickObject siempre entre try{ç catch {}
                    //Llamamos a ISelectionFilter solo Edge de RoofBase
                    edgeRef = sel.PickObject(ObjectType.Edge, new RoofSelectionFilter(doc), "Seleccionar cubierta");

                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Cancelled;

                }
                //Obtenemos elemento desde Reference
                Element roof = doc.GetElement(edgeRef.ElementId);

                //Construimos coleccion con el seleccionado
                ICollection<ElementId> els = new List<ElementId>() { roof.Id };

                //Comprobamos que pueda desplazarse, en caso contrario salimos
                bool isDesplazable = DisplacementElement.CanElementsBeDisplaced(view, els);
                if (!isDesplazable)
                {
                    message = "El objeto seleccionado no se puede desplazar";
                    return Result.Cancelled;
                }

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name");
                    // Create a new top level DisplacementElement

                    //Creamos desplazamiento 
                    DisplacementElement dispElem = DisplacementElement.Create(doc, els, new XYZ(10, 0, 20), view, null);

                    // Creamos el camino asociado al elemento
                    DisplacementPath.Create(doc, dispElem, edgeRef, 0);
                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region GRAFICOS TEMPORALES - INCORPORA GRAFICOS VMP
            try
            {
                // Requiere la imagen .bmp en el proyecto. Debo profundizar en esto para entenderlo bien
                //Generamos un Guid
                Guid nGuid = Guid.NewGuid();

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos un objeto seleccionado
                if (sel.GetElementIds().Count != 1)
                {
                    message = "Se debe seleccionar un solo elemento";
                    return Result.Failed;
                }

                // Chequeamos que el objeto seleccionado es Muro
                if (doc.GetElement(sel.GetElementIds().First()) is Wall wall)
                {
                    //Npmbre completo del actual fichero GraficosTemp.dll
                    string filename = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    //Carpeta del actual GraficosTemp.dll
                    string folder = (new System.IO.FileInfo(filename)).Directory.FullName;

                    MultiServerService externalService = ExternalServiceRegistry.GetService(
                        ExternalServices.BuiltInExternalServices.TemporaryGraphicsHandlerService) as MultiServerService;

                    MyGraphicsService myGraphicsService = new MyGraphicsService(nGuid, wall.Id);

                    externalService.AddServer(myGraphicsService);
                    externalService.SetActiveServers(new List<Guid> { myGraphicsService.GetServerId() });

                    TemporaryGraphicsManager mgr = TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc);

                    //Calculamos punto medio wall
                    XYZ controlPoint = ((LocationCurve)wall.Location).Curve.Evaluate(0.5, true);
                    //Asignamos imagen y punto
                    InCanvasControlData data = new InCanvasControlData(folder + "\\excel.bmp", controlPoint);
                    //Asignamos el control
                    mgr.AddControl(data, doc.ActiveView.Id);

                }
                else
                {
                    message = "Se debe seleccionar muro";
                    return Result.Failed;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Gráfico temporal creado");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 7.4 REGIONES

            #region CREAR REGIONES
            try
            {

                //Filtramos los niveles, metodo abreviado. Filtro de clase
                FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Level));
                //Seleccionamos el primer nivel de la colección
                Level level = col.First() as Level;

                //Creamos 4 puntos en planta. Cuadricula 10*10
                XYZ xYZ0 = XYZ.Zero;
                XYZ xYZ1 = new XYZ(10, 0, 0);
                XYZ xYZ2 = new XYZ(10, 10, 0);
                XYZ xYZ3 = new XYZ(0, 10, 0);

                //Creamos un vector de desplazamiento. a 45 º
                XYZ desfase = new XYZ(15, 15, 0);

                //Creamos 4 puntos en planta. Cuadricula 10*10
                XYZ xYZ5 = XYZ.Zero + desfase;
                XYZ xYZ6 = xYZ1 + desfase;
                XYZ xYZ7 = xYZ2 + desfase;
                XYZ xYZ8 = xYZ3 + desfase;

                //Creamos primer conjunto de Curves
                Curve c0 = Line.CreateBound(xYZ0, xYZ1);
                Curve c1 = Line.CreateBound(xYZ1, xYZ2);
                Curve c2 = Line.CreateBound(xYZ2, xYZ3);
                Curve c3 = Line.CreateBound(xYZ3, xYZ0);

                //Creamos segundo conjunto de Curves
                Curve c4 = Line.CreateBound(xYZ5, xYZ6);
                Curve c5 = Line.CreateBound(xYZ6, xYZ7);
                Curve c6 = Line.CreateBound(xYZ7, xYZ8);
                Curve c7 = Line.CreateBound(xYZ8, xYZ5);

                //Creamos primer CurveLoop 
                CurveLoop profileA = new CurveLoop();
                profileA.Append(c0);
                profileA.Append(c1);
                profileA.Append(c2);
                profileA.Append(c3);

                //Creamos segundo CurveLoop 
                CurveLoop profileB = new CurveLoop();
                profileB.Append(c4);
                profileB.Append(c5);
                profileB.Append(c6);
                profileB.Append(c7);

                //Creamos una lista de CurveLoop
                List<CurveLoop> curveLoops = new List<CurveLoop>() { profileA, profileB };
                //Obtenemos el ElementType por defecto
                //No es posible crear region de mascara
                ElementId id = doc.GetDefaultElementTypeId(ElementTypeGroup.FilledRegionType);
                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction región");
                    //Creamos la región
                    FilledRegion filledRegion = FilledRegion.Create(doc, id, uidoc.ActiveView.Id, curveLoops);
                    //Obtenemos las DetailCurve del contorno
                    IList<ElementId> idsCurves = filledRegion.GetDependentElements(new ElementClassFilter(typeof(CurveElement)));
                    List<Element> curves = idsCurves.Select(x => doc.GetElement(x)).ToList();

                    //Podriamos cambiar el Estilo de linea de eastas Lineas de detalle

                    TaskDialog.Show("Manual Revit API", "Region creada con " + curves.Count + " Lineas de detalle");
                    //Confirmamos transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region MANIPULACION DE REGIONES DE RECORTE
            try
            {

                //Solo admitimos vista en planta
                if (uidoc.ActiveView is ViewPlan view)
                {
                    //Creamos nuevo CurveLoop
                    CurveLoop loop = new CurveLoop();
                    //Creamos 6 XYZ
                    XYZ xYZ0 = new XYZ(30, 20, 0);
                    XYZ xYZ1 = new XYZ(30, -20, 0);
                    XYZ xYZ2 = new XYZ(-10, -20, 0);
                    XYZ xYZ3 = new XYZ(-10, -10, 0);
                    XYZ xYZ4 = new XYZ(-30, -10, 0);
                    XYZ xYZ5 = new XYZ(-30, 20, 0);

                    //Añadimos 6 Line ordenadasu orientadas al CurveLoop
                    loop.Append(Line.CreateBound(xYZ0, xYZ1));
                    loop.Append(Line.CreateBound(xYZ1, xYZ2));
                    loop.Append(Line.CreateBound(xYZ2, xYZ3));
                    loop.Append(Line.CreateBound(xYZ3, xYZ4));
                    loop.Append(Line.CreateBound(xYZ4, xYZ5));
                    loop.Append(Line.CreateBound(xYZ5, xYZ0));

                    //Creamos Transaction
                    using (Transaction tx = new Transaction(doc))
                    {
                        //Iniciamos Transaction
                        tx.Start("Transaction Region recorte");

                        //Accedemos a la region
                        ViewCropRegionShapeManager vcrShapeMgr = view.GetCropRegionShapeManager();
                        //Asignamos la region
                        vcrShapeMgr.SetCropShape(loop);

                        //Activamos recorte y visibilidad
                        view.CropBoxActive = true;
                        view.CropBoxVisible = true;

                        //Confirmamos Transaction
                        tx.Commit();
                    }
                }
                else
                {
                    message = "Solo vista en planta";
                    return Result.Cancelled;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Region de recorte creada");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region DIVIDIR UNA REGION DE RECORTE EN SECCIONES.
            try
            {
                // NECESITAMOS PRIMERO "RESTAURAR" LA REGION DE RECORTE, PARA LUEGO SUBDIVIDIRLA.
                // PODEMOS DIVIDIR UNA SUBDIVISION SUCESIVAMENTE.

                //Solo admitimos vista en planta
                if (uidoc.ActiveView is ViewPlan view)
                {
                    //Creamos Transaction
                    using (Transaction tx = new Transaction(doc))
                    {
                        //Iniciamos Transaction
                        tx.Start("Transaction Region recorte");

                        //Accedemos a la region
                        ViewCropRegionShapeManager vcrShapeMgr = view.GetCropRegionShapeManager();

                        //Eliminamos regiones poligonales
                        vcrShapeMgr.RemoveCropRegionShape();

                        //Dividimos la region
                        vcrShapeMgr.SplitRegionHorizontally(0, 0.4, 0.6);

                        //Activamos recorte y visibilidad
                        view.CropBoxActive = true;
                        view.CropBoxVisible = true;

                        //Confirmamos Transaction
                        tx.Commit();
                    }
                }
                else
                {
                    message = "Solo vista en planta";
                    return Result.Cancelled;
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Region de recorte dividida");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region MANIPULACION DE LOS MODOS TEMPORALES DE LAS VISTAS
            try
            {

                //Vista actual
                View view = uidoc.ActiveView;

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction modos temporales");

                    //Obtenemos los modos temporales
                    TemporaryViewModes viewModes = view.TemporaryViewModes;

                    if (viewModes == null)
                    {
                        message = "La vista no soporta modos temporales";
                        return Result.Cancelled;
                    }
                    else if (doc.IsFamilyDocument) //Si es FamilyDocument podemos acceder a PreviewFamilyVisibilityMode
                    {
                        // Los modos debe ser viables y estar habilitado
                        if (viewModes.IsModeAvailable(TemporaryViewMode.PreviewFamilyVisibility) && viewModes.IsModeEnabled(TemporaryViewMode.PreviewFamilyVisibility))
                        {
                            //El estado debe ser viable
                            if (viewModes.IsValidState(PreviewFamilyVisibilityMode.On))
                            {
                                viewModes.PreviewFamilyVisibility = PreviewFamilyVisibilityMode.On;
                            }
                        }
                    }
                    else
                    {
                        // Los modos debe ser viables y estar habilitado
                        if (viewModes.IsModeEnabled(TemporaryViewMode.RevealHiddenElements) && viewModes.IsModeAvailable(TemporaryViewMode.RevealHiddenElements))
                        {
                            viewModes.RevealHiddenElements = true;
                        }
                    };

                    //Confirmamos Transaction
                    tx.Commit();
                }
                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Modos cambiados");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 7.5 REVISIONES

            #region CREAR REVISIONES
            try
            {

                //Creamos 4 puntos 
                XYZ xYZ0 = XYZ.Zero;
                XYZ xYZ1 = new XYZ(0, 20, 0);
                XYZ xYZ2 = new XYZ(20, 20, 0);
                XYZ xYZ3 = new XYZ(20, 0, 0);

                //Creamos lista de curves para nube de revisión
                IList<Curve> curves = new List<Curve>();
                curves.Add(Line.CreateBound(xYZ0, xYZ1));
                curves.Add(Line.CreateBound(xYZ1, xYZ2));
                curves.Add(Line.CreateBound(xYZ2, xYZ3));
                curves.Add(Line.CreateBound(xYZ3, xYZ0));

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction revisiones");

                    //Creamos 3 Revision
                    AddNewRevision(doc, "Descripción de ejemplo 1", "Supervisor 1", "Arquitecto 1");
                    AddNewRevision(doc, "Descripción de ejemplo 2", "Supervisor 1", "Arquitecto 2");
                    Revision revision = AddNewRevision(doc, "Descripción de ejemplo 3", "Supervisor 1", "Arquitecto 3");

                    if (!(doc.ActiveView is View3D) && revision.Issued == false)
                    {
                        //Creamos nube de revisión
                        RevisionCloud.Create(doc, doc.ActiveView, revision.Id, curves);
                    }

                    //Confirmamos Transaction
                    tx.Commit();
                }

                //Mensaje final
                TaskDialog.Show("Manual Revit API", "Revisiones creadas");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region COMBINAR LAS NUVES DE REVISION
            try
            {

                //Obtenemos todas las Revisions
                List<Revision> revisions = Revision.GetAllRevisionIds(doc).Select(x => doc.GetElement(x)).Cast<Revision>().ToList();

                // Solo se pueden fusion las NO emitidas
                // Seleccionamos solamente NO emitidas
                revisions = revisions.Where(x => x.Issued == false).ToList();

                //Para combinar necesitas >1
                if (revisions.Count < 2)
                {
                    message = "Al menos necesitamos 2 Revisiones sin emitir";
                    return Result.Cancelled;
                }

                //Obtenemos la ultima
                Revision revision = revisions.LastOrDefault();
                string revisionId = revision.Id.IntegerValue.ToString();
                //Obtenemos revision previa
                Revision previousRevision = revisions.ElementAt(revisions.Count - 2);

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Combinar revisión");

                    //Combinamos ultima con previa. obtenemos RevisionCloud
                    ISet<ElementId> revisionCloudIds = Revision.CombineWithPrevious(doc, revision.Id);

                    //Número de RevisionClouds existentes en la última
                    int movedClouds = revisionCloudIds.Count;
                    //Si la ultima tenia RevisionCloud
                    if (movedClouds > 0)
                    {
                        //Obtenemios primer RevisionCloud
                        RevisionCloud cloud = doc.GetElement(revisionCloudIds.ElementAt(0)) as RevisionCloud;
                        if (cloud != null)
                        {
                            string msg = string.Format("La Revision {0} se ha borrado y {1} RevisionCloud añadidos a la Revision {2}",
                                revisionId, movedClouds, cloud.RevisionId.ToString());
                            TaskDialog.Show("Manual Revit API", msg);
                        }
                    }
                    //Confirmamos Transaction
                    tx.Commit();
                    TaskDialog.Show("Manual Revit API", "Combinacion terminada");

                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 8.0 ----------------------------------- FILTROS ------------------------------------

            // 8.1 FILTROS
            
            #region FILTROS DE VISUALIZACION PARA APLICAR EN UNA VISTA
            try
            {

                //Obtenemos vista actual
                View view = uidoc.ActiveView;

                // Creamos ub filtro de clase Floor
                FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Floor));
                ICollection<ElementId> elementIds = collector.ToElementIds();

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Filtro selección");

                    // Creamos filtro y asignamos los Floor
                    SelectionFilterElement filterElement = SelectionFilterElement.Create(doc, "Filtro para Floor");
                    filterElement.SetElementIds(elementIds);

                    ElementId filterId = filterElement.Id;

                    // Añadimos el filtro a la View
                    view.AddFilter(filterId);

                    //Por norma general debemos regenerar para visualizar el filtro
                    doc.Regenerate();

                    //Obtenemos configuración de gráficos
                    OverrideGraphicSettings overrideSettings = view.GetFilterOverrides(filterId);

                    //Cambiamos la configuración de gráficos existente a color azul
                    overrideSettings.SetProjectionLineColor(new Color(0x00, 0x00, 0xFF));

                    //Sobreescribimos en el filtro en la vista, la configuración de gráficos
                    view.SetFilterOverrides(filterId, overrideSettings);

                    //Confirmamos Transaction
                    tx.Commit();
                }

                TaskDialog.Show("API Revit Manual", "Filtro añadido");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region REGLAS PARA FILTROS - LOS ELEMENTOS ESTRUCTURALES DESAPARECEN DE LA VISTA
            try
            {
                //Obtenemos la vista actual
                View view = uidoc.ActiveView;

                //Creamos una coleccion con las categorias que deseamos filtrar. Walls
                ISet<ElementId> categories = new HashSet<ElementId>() { new ElementId(BuiltInCategory.OST_Walls) };

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Filtro reglas");

                    //Obtenemos el parametro Uso Estructura 0 = no es estructura, > 0 = si es estructura
                    ElementId exteriorParamId = new ElementId(BuiltInParameter.WALL_STRUCTURAL_USAGE_PARAM);
                    //Creamos el filtro de ElementParameterFilter
                    ElementParameterFilter filter = new ElementParameterFilter(ParameterFilterRuleFactory.CreateGreaterRule(exteriorParamId, 0));

                    // Creamos filtro asociado a las categorías de entrada (Wall)
                    if (ParameterFilterElement.ElementFilterIsAcceptableForParameterFilterElement(doc, categories, filter))
                    {
                        ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Filtro muros estructurales", categories);
                        parameterFilterElement.SetElementFilter(filter);
                        // Aplicamos filtro a la vista
                        view.AddFilter(parameterFilterElement.Id);
                        //Los objetos incluidos NO son visibles
                        view.SetFilterVisibility(parameterFilterElement.Id, false);
                    }
                    else
                    {
                        message = "El filtro no puede usarse";
                        return Result.Cancelled;
                    }
                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region MODIFICACION DE FILTROS - CAMBIAMOS EL COLOR DE LOS FILTROS
            try
            {

                //Obtenemos la vista actual
                View view = uidoc.ActiveView;
                // Find any filter with overrides setting cut color to Red

                //Obtenemos los filtros de la vista actual
                foreach (ElementId filterId in view.GetFilters())
                {
                    //Obtenemos configuración de gráficos
                    OverrideGraphicSettings overrideSettings = view.GetFilterOverrides(filterId);
                    //Obtenemos el color de las lineas de proyección
                    Color lineColor = overrideSettings.ProjectionLineColor;
                    //Si no tiene color 
                    if (!lineColor.IsValid || lineColor == Color.InvalidColorValue)
                        continue;

                    // Si el color es azul, lo cambiamos a verde
                    if (lineColor.Red == 0x00 && lineColor.Green == 0x00 && lineColor.Blue == 0xFF)
                    {
                        overrideSettings.SetProjectionLineColor(new Color(0x00, 0xFF, 0x00));

                        //Crteamos Transaction
                        using (Transaction tx = new Transaction(doc))
                        {
                            //Iniciamos Transaction
                            tx.Start("Transaction Name");

                            //Sobrescribimos en la vista, para el filtro seleccionado la configuración
                            view.SetFilterOverrides(filterId, overrideSettings);

                            //Confirmamos Transaction
                            tx.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 9.0 ----------------------------------- TASKDIALOG ------------------------------------

            #region TASKDIALOG FUNCIONALES
            try
            {

                // Creamos un TaskDialog de Revit para comunicar información al usuario.
                TaskDialog mainDialog = new TaskDialog("Título: TaskDialog");

                //Alternar valores de TitleAutoPrefix (Optativo)
                mainDialog.TitleAutoPrefix = false;

                mainDialog.MainInstruction = "Información para mostrar";
                mainDialog.MainContent =
                    "Este ejemplo muestra cómo utilizar un TaskDialog de Revit para comunicarse con el usuario."
                    + "\nLos siguientes enlaces de comandos abren TaskDialog adicionales con más información." +
                    "\n\nOK mostrara ambos mensajes";

                // Añadimos commmandLink. Podemos añadir hasta 4 Link (Optativo)
                mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,
                                          "Muestra información sobre la instalación de Revit.",
                                          "Linea opcional ampliando información.");
                mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,
                                          "Muestra información sobre el documento activo.",
                                          "Linea opcional ampliando información.");
                /* mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3,
                                          "No usado en este ejemplo");
                 mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink4,
                                          "No usado en este ejemplo");*/

                mainDialog.ExpandedContent = "Texto de ExpandedContent";

                // Añadimos un icono. (Optativo)
                mainDialog.MainIcon = TaskDialogIcon.TaskDialogIconInformation;

                // Botones comunes y botón por defecto.
                // Si no se añade ningún CommonButton o CommandLink, el TaskDialog mostrará el botón de Close por defecto
                mainDialog.CommonButtons = TaskDialogCommonButtons.Close | TaskDialogCommonButtons.Cancel | TaskDialogCommonButtons.Ok;
                mainDialog.DefaultButton = TaskDialogResult.Close;

                // El texto a pie de página se utiliza normalmente para enlazar con el documento de ayuda..
                mainDialog.FooterText =
                    "<a href=\"www.google.com \">"
                    + "Google, ejemplo de link a usar </a>";

                TaskDialogResult tResult = mainDialog.Show();

                // Si el usuario hace clic en el primer link, un TaskDialog con sólo un botón Close,
                // muestra información sobre la instalación de Revit. 
                if (TaskDialogResult.CommandLink1 == tResult)
                {
                    MostrarInfoRevit(app);
                }

                // Si el usuario hace clic en el primer link, un TaskDialog
                // creado mediante un método static muestra información sobre el documento activo
                else if (TaskDialogResult.CommandLink2 == tResult)
                {
                    MostrarInfoDoc(doc);
                }
                // Si el usuario hace clic en OK, se muestran los dos Taskdialog anteriores

                else if (TaskDialogResult.Ok == tResult)
                {
                    MostrarInfoRevit(app);
                    MostrarInfoDoc(doc);
                }
                else
                {
                    TaskDialog.Show("Resultado final", "No se ha pulsado ninguna opción controlada." +
                        "\nEl resultado final es: " + tResult.ToString());
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region TASKDIALOG CON UN CHECKBOX
            try
            {
                // TIENE CIERTA "INCOMPATIBILIDAD" CON LAS OPCIONES DE ANTES.
                // La imcompatibilidad es que ".VerificationText" y ".ExtraCheckBox" no pueden estar a la vez en un TaskDialog.

                // Cramos un TaskDialog de Revit para comunicar información al usuario.
                TaskDialog mainDialog = new TaskDialog("Título: TaskDialog");

                //Alternar valores de TitleAutoPrefix (Optativo)
                mainDialog.TitleAutoPrefix = false;

                mainDialog.MainInstruction = "Extra CheckBox";
                mainDialog.MainContent =
                    "Este ejemplo muestra como usar el ExtraCheckBox." +
                    "\nDebe seleccionar el CheckBox y pulsar Ok para mostrar siguiene acción";

                mainDialog.ExpandedContent = "Texto de ExpandedContent";

                mainDialog.ExtraCheckBoxText = "¿Realmente desea ejecutar la acción?";

                // Añadimos un icono. (Optativo)
                mainDialog.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                // Botones comunes y botón por defecto.
                // Si no se añade ningún CommonButton o CommandLink, el TaskDialog mostrará el botón de Close por defecto
                mainDialog.CommonButtons = TaskDialogCommonButtons.Cancel | TaskDialogCommonButtons.Ok;
                mainDialog.DefaultButton = TaskDialogResult.Cancel;

                // El texto a pie de página se utiliza normalmente para enlazar con el documento de ayuda..
                mainDialog.FooterText =
                    "<a href=\"www.linkedin.com/in/felipe-de-abajo-alonso-51794919 \">"
                    + "Click para ver perfil en Linkedin</a>";

                TaskDialogResult tResult = mainDialog.Show();

                if (TaskDialogResult.Ok == tResult && mainDialog.WasExtraCheckBoxChecked() == true)
                {
                    TaskDialog.Show("ExtraCheckBox", "La acción continúa");
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region OTRA OPCION DE TASKDIALOG CON UN CHECKBOX.
            try
            {

                // Cramos un TaskDialog de Revit para comunicar información al usuario.
                TaskDialog mainDialog = new TaskDialog("Título: TaskDialog");

                //Alternar valores de TitleAutoPrefix (Optativo)
                mainDialog.TitleAutoPrefix = false;

                mainDialog.MainInstruction = "Verification Text";
                mainDialog.MainContent =
                    "Este ejemplo muestra como usar el VerificationText." +
                    "\nDebe seleccionar el CheckBox y pulsar Aceptar para  mostrar siguiene acción";

                mainDialog.ExpandedContent = "Texto de ExpandedContent";

                mainDialog.VerificationText = "¿Realmente desea ejecutar la acción?";
                //ExtraCheckBoxText es incompatible con VerificationText
                //  mainDialog.ExtraCheckBoxText = "¿Realmente desea ejecutar la acción?";

                // Añadimos un icono. (Optativo)
                mainDialog.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                // Botones comunes y botón por defecto.
                // Si no se añade ningún CommonButton o CommandLink, el TaskDialog mostrará el botón de Close por defecto
                mainDialog.CommonButtons = TaskDialogCommonButtons.Cancel | TaskDialogCommonButtons.Ok;
                mainDialog.DefaultButton = TaskDialogResult.Cancel;

                // El texto a pie de página se utiliza normalmente para enlazar con el documento de ayuda..
                mainDialog.FooterText =
                    "<a href=\"www.linkedin.com/in/felipe-de-abajo-alonso-51794919 \">"
                    + "Click para ver perfil en Linkedin</a>";

                TaskDialogResult tResult = mainDialog.Show();

                if (TaskDialogResult.Ok == tResult && mainDialog.WasVerificationChecked() == true)
                {
                    TaskDialog.Show("ExtraCheckBox", "La acción continúa");
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region BARRA DE PROGRESO, DE MOMENTO PARECE QUE SU FUNCIONALIDAD ESTA INCOMPLETA
            try
            {
                // Cramos un TaskDialog de Revit para comunicar información al usuario.
                TaskDialog mainDialog = new TaskDialog("Título: TaskDialog");

                mainDialog.TitleAutoPrefix = false;

                mainDialog.MainInstruction = "Mostrar Barra de progreso";
                mainDialog.MainContent =
                    "Este ejemplo muestra cómo utilizar un TaskDialog de mostrando una \"Barra de Progreso\".";


                // mainDialog.ExtraCheckBoxText = "CheckBox extra. Incompatible con Barra de Progreso";

                mainDialog.ExpandedContent = "Texto de ExpandedContent";

                // Añadimos un icono. (Optativo)
                mainDialog.MainIcon = TaskDialogIcon.TaskDialogIconShield;
                // Botones comunes y botón por defecto.
                // Si no se añade ningún CommonButton o CommandLink, el TaskDialog mostrará el botón de Close por defecto
                mainDialog.CommonButtons = TaskDialogCommonButtons.Close | TaskDialogCommonButtons.Cancel | TaskDialogCommonButtons.Ok;
                mainDialog.DefaultButton = TaskDialogResult.Close;

                mainDialog.EnableMarqueeProgressBar = true;

                // El texto a pie de página se utiliza normalmente para enlazar con el documento de ayuda..
                mainDialog.FooterText =
                    "<a href=\"www.linkedin.com/in/felipe-de-abajo-alonso-51794919 \">"
                    + "Click para ver perfil en Linkedin</a>";

                TaskDialogResult tResult = mainDialog.Show();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 10.0 ----------------------------------- TRANSACCIONES ------------------------------------

            #region TRANSACCIONES - 2 ESTRUCTURAS DIFERENTES, USANDO "using" Y SIN USARLO.
            try
            {

                // Tambien hay Subtransacciones, sin nombre y que no se pueden acceder desde el menu "deshacer".
                // transactionGroup.Assimilate(); - Sirve para que el grupo de transacciones se vea en 1 solo paso en el boton deshacer.

                //Creamos TransactionGroup
                //   using (TransactionGroup transactionGroup = new TransactionGroup(doc, "Dibujar lineas"))
                //  {
                //Iniciamos TransactionGroup
                //   transactionGroup.Start();

                //Creamos Transaction
                Transaction txPlano = new Transaction(doc);
                //Iniciamos Transaction
                txPlano.Start("Transaction Plano de trabajo");

                //Creamos 4 puntos
                XYZ xYZ1 = XYZ.Zero;
                XYZ xYZ2 = new XYZ(10, 0, 0);
                XYZ xYZ3 = new XYZ(10, 10, 0);
                XYZ xYZ4 = new XYZ(0, 10, 0);
                //Creamos un Plane
                Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);
                //En la linea 35 iniciamos esta transacción. Nunca se ejecutara
                if (!txPlano.HasStarted()) txPlano.Start("Transaction Plano de trabajo");

                //Creamos un SketchPlane para poder crear ModelLines
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                //Damos nombre al plano
                sketchPlane.Name = "Revit API Manual";

                //Conformamos Transaction
                txPlano.Commit();
                //Anulamos Transaction. No existe el SketchPlane y no podemos crear ModelLines
                // tx.RollBack();

                //En lineas 53 o 55 cerramos la anterior Transaction podemos crear una nueva Transaction
                //encampusala entre llaves. Mejor opción
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction 3 lineas");
                    //Creamos una SubTransaction encapsulada
                    using (SubTransaction subTransaction1 = new SubTransaction(doc))
                    {
                        //Iniciamos SubTransaction
                        subTransaction1.Start();
                        ModelLine modelLine = CrearLineas(uidoc, sketchPlane, xYZ1, xYZ2);
                        //Confirmamos SubTransaction
                        subTransaction1.Commit();
                    }

                    //Creamos 2 ModelLine, dentro de la Transaction tx
                    CrearLineas(uidoc, sketchPlane, xYZ2, xYZ3);
                    CrearLineas(uidoc, sketchPlane, xYZ3, xYZ4);

                    //Confirmamos la transacción
                    //   tx.Commit();
                    //Anulamos la Transaction. Se anulan las 3 ModelLines
                    //tx.RollBack();
                }
                //Aunque no estan las lineas 78 ni 80, al salir del encapsulado la Transaction de linea 59 se cierra
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Transaction 1 linea");
                    CrearLineas(uidoc, sketchPlane, xYZ4, xYZ1);
                    //Confirmamos la transacción
                    tx.Commit();
                    //Anulamos la Transaction. Se anulan la ModelLine
                    //tx.RollBack();
                }
                //Confirmamos TransactionGroup. Tenemos en menú Deshacer 3 Transaction
                // transactionGroup.Commit();
                //Asimilamos TransactionGroup. Tenemos en menú Deshacer 1 sola Transaction
                //  transactionGroup.Assimilate();
                //  }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 11.0 ----------------------------------- VALORES GEOMETRICOS ------------------------------------

            #region VALORES DEL BoundingBox
            try
            {

                Selection sel = uidoc.Selection;
                //Obtenenos la selección
                ICollection<ElementId> elementIds = sel.GetElementIds();

                //Hay algún objeto seleccionado?
                if (elementIds.Count == 0)
                {
                    message = "Se debe seleccionar un objeto";
                    return Result.Cancelled;
                }

                //Recuperamos el primer objeto
                Element elm = doc.GetElement(elementIds.First());

                //Es de modelo?
                if (elm.Category.CategoryType != CategoryType.Model || elm.Category.IsCuttable == false)
                {
                    message = "Se debe seleccionar un objeto de 'modelo' y 'cortable'";
                    return Result.Cancelled;
                }

                View view = doc.ActiveView;

                //BoundingBoxXYZ de la vista
                BoundingBoxXYZ sectionBox = elm.get_BoundingBox(view);

                //Coordenadas de BoundingBoxXYZ absoluto
                XYZ max = sectionBox.Max; // Coordenadas máximas (esquina superior derecha frontal de la caja). 
                XYZ min = sectionBox.Min; // Coordenadas mínimas (esquina inferior izquierda trasera de la caja)

                TaskDialog.Show("Revit API Manual", "La 'Z' máxima es: " + max.Z.ToString("N2"));
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region OBTENER EL GeometryElement
            try
            {

                // ESTO REQUIERE DEL USO DE Options, DONDE VAMOS A ESPECIFICAR LAS CARACTERISTICAS A BUSCAR.
                // CON ESTE GeometryObject PODEMOS OBTENER EL RESTO DE LAS CARACTERISTICAS GEOMETRICAS DEL OBJETO.

                Selection sel = uidoc.Selection;
                //Obtenenos la selección
                ICollection<ElementId> elementIds = sel.GetElementIds();

                //Hay algún objeto seleccionado?
                if (elementIds.Count == 0)
                {
                    message = "Se debe seleccionar un objeto";
                    return Result.Cancelled;
                }

                //Recuperamos el primer objeto
                Element elm = doc.GetElement(elementIds.First());

                //Es de modelo?
                if (elm.Category.CategoryType != CategoryType.Model || elm.Category.IsCuttable == false)
                {
                    message = "Se debe seleccionar un objeto de 'modelo' y 'cortable'";
                    return Result.Cancelled;
                }

                //Creamos un Options
                Options options = new Options();
                //Asignamos Nivel de detalle Alto
                options.DetailLevel = ViewDetailLevel.Fine;
                //No incluimos objetos no visibles
                options.IncludeNonVisibleObjects = false;
                //No computar References
                options.ComputeReferences = false;

                //Creamos un solid vacio
                Solid solid = null;

                //Obtenemos el GeometryElement
                GeometryElement geometryElement = elm.get_Geometry(options);

                //Iteramos por GeometryObject cada en GeometryElement
                foreach (GeometryObject geometryObject in geometryElement)
                {
                    //Chequemos si es Solid y almacenamos en tempWall
                    if (geometryObject is Solid tempWall)
                    {
                        //Comprobamos si es Solid y su volumen > 0
                        if (tempWall != null && tempWall.Volume > 0)
                        {
                            solid = tempWall;
                            break;
                        }

                    }
                }
                #region BoundingBoxXYZ
                // Obtenemos un BoundingBoxXYZ desde el Solid
                BoundingBoxXYZ boundingBoxXYZ = solid.GetBoundingBox();

                //Obtenemos la Transform del BoundingBoxXYZ
                Transform transform = boundingBoxXYZ.Transform;

                //Obtenemos el max del BoundingBoxXYZ y le aplicamos la Transform
                XYZ max = boundingBoxXYZ.Max;
                max = transform.OfPoint(max); //Maximo XYZ
                #endregion

                #region Face
                //Creamos XYZ para almacenar maximo
                XYZ maxEdgeXYZ = null;

                //Obtenemos del solid un FaceArray y un FaceArrayIterator
                FaceArray faceArray = solid.Faces;
                FaceArrayIterator faceArrayIterator = faceArray.ForwardIterator();

                //Iteramos por FaceArrayIterator mientras podamos avanzar
                while (faceArrayIterator.MoveNext())
                {
                    //Obtenemos la Face actual
                    Face face = faceArrayIterator.Current as Face;
                    //es la Face PlanarFace? Es la Z de su Normal >0?
                    if (face is PlanarFace planarFace && planarFace.FaceNormal.Z > app.ShortCurveTolerance)
                    {
                        //Obtenemos EdgeArrayArray
                        EdgeArrayArray edgeArrayArray = planarFace.EdgeLoops;
                        //Obtenemos el primer bucle de EdgeArrayArray
                        EdgeArray edgeArray = edgeArrayArray.get_Item(0);
                        //Obtenemos el EdgeArrayIterator
                        EdgeArrayIterator edgeArrayIterator = edgeArray.ForwardIterator();

                        //Iteramos por EdgeArrayIterator mientras sea posible
                        while (edgeArrayIterator.MoveNext())
                        {
                            //Obtenemos el Edge actual
                            Edge edge = edgeArrayIterator.Current as Edge;
                            //Obtenemos su Line
                            Line line = edge.AsCurve() as Line;
                            //Obtenemos la lista de puntos ordenanos por Z
                            List<XYZ> xYZs = line.Tessellate().OrderBy(x => x.Z).ToList();
                            //Es maxEdgeXYZ null? La Z es mayor?
                            if (maxEdgeXYZ == null || xYZs.Last().Z > maxEdgeXYZ.Z)
                            {
                                //Actualizamos maxEdgeXYZ
                                maxEdgeXYZ = xYZs.Last();
                            }
                        }
                        //Ha sido la Face PlanarFace y Es la Z de su Normal >0.
                        //Interumpimos el bucle
                        break;
                    }
                }
                #endregion
                TaskDialog.Show("Revit API Manual", "La 'Z' máxima, desde el BoundingBoxXYZ es: " + max.Z.ToString("N2") +
                    "\rLa 'Z' máxima, desde el Edge es: " + maxEdgeXYZ.Z.ToString("N2"));
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region OBTENEMOS LA SUPERFICIE VIDRIADA DE UNA VENTANA EN ESPECIFICO
            try
            {
                // Requiere el ingreso a una "GeometryInstance", diferente al "GeometryElement" de antes.

                Selection sel = uidoc.Selection;
                //Obtenenos la selección
                ICollection<ElementId> elementIds = sel.GetElementIds();

                //Hay algún objeto seleccionado?
                if (elementIds.Count == 0)
                {
                    message = "Se debe seleccionar un objeto";
                    return Result.Cancelled;
                }

                //Recuperamos el primer objeto
                Element elm = doc.GetElement(elementIds.First());
                //Es de modelo?
                if (elm.Category.CategoryType != CategoryType.Model || elm.Category.IsCuttable == false)
                {
                    message = "Se debe seleccionar un objeto de 'modelo' y 'cortable'";
                    return Result.Cancelled;
                }

                FamilyInstance familyInstance = null;
                //Creamos un Options
                Options options = new Options();
                //Asignamos Nivel de detalle Alto
                options.DetailLevel = ViewDetailLevel.Fine;
                //No incluimos objetos no visibles
                options.IncludeNonVisibleObjects = false;
                //No computar References
                options.ComputeReferences = false;

                //Creamos un solid vacio
                Solid solidVidrio = null;

                //Obtenemos el GeometryElement
                GeometryElement geometryElement = elm.get_Geometry(options);
                //Iteramos por GeometryObject cada en GeometryElement
                foreach (GeometryObject geomObj in geometryElement)
                {
                    //Primero obtenemos GeometryInstance
                    GeometryInstance geoInstance = geomObj as GeometryInstance;
                    if (null != geoInstance)
                    {
                        //Como hemos obtenido GeometryInstance => elm es FamilyInstance
                        familyInstance = elm as FamilyInstance;

                        //Ahora la geometria de la instancia, cada ventana puede tener una altura, anchura etc
                        GeometryElement instanceGeometryElement = geoInstance.GetInstanceGeometry();
                        //Recorremos buscando Solid en cada GeometryObject
                        foreach (GeometryObject o in instanceGeometryElement)
                        {
                            solidVidrio = o as Solid;
                            //Chequemos que es solido (puede ser Face), y que el volumen sea  >0
                            if (solidVidrio != null && solidVidrio.Volume > 0)
                            {
                                //Comprobamos que sea vidrio la cara
                                GraphicsStyle style = doc.GetElement(solidVidrio.GraphicsStyleId) as GraphicsStyle;
                                if (style.Category != null && style.Category.Id.IntegerValue == (int)BuiltInCategory.OST_WindowsGlassProjection) break;
                            }
                        }
                    }

                }

                #region Face

                //Obtenemos del solid un FaceArray y un FaceArrayIterator
                FaceArray faceArray = solidVidrio.Faces;
                FaceArrayIterator faceArrayIterator = faceArray.ForwardIterator();
                //Creamos Face null
                Face faceVidrio = null;
                //Iteramos por FaceArrayIterator mientras podamos avanzar
                while (faceArrayIterator.MoveNext())
                {
                    //Obtenemos la Face actual
                    faceVidrio = faceArrayIterator.Current as Face;
                    //Es la Face PlanarFace? Es normal igual a la FacingOrientation. Perdicular las Host?
                    if (faceVidrio is PlanarFace planarFace && planarFace.FaceNormal.IsAlmostEqualTo(familyInstance.FacingOrientation))
                    {
                        break;
                    }
                }
                #endregion
                TaskDialog.Show("Revit API Manual", "El area del vidrio de la ventana es: " + faceVidrio.Area.ToString("N2"));

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREACION DE DirectShape - En este caso esta creado con la categoria "Walls", que es algo ilogico.
            try
            {
                // Definimos centro
                XYZ center = XYZ.Zero;
                double radius = 2.0;

                //Dos puntos del diámetro
                XYZ profilePlus = center + new XYZ(0, radius, 0);
                XYZ profileMinus = center - new XYZ(0, radius, 0);

                //Creamos un semicírculo para girarlo 360
                List<Curve> profile = new List<Curve>();
                profile.Add(Line.CreateBound(profilePlus, profileMinus));
                profile.Add(Arc.Create(profileMinus, profilePlus, center + new XYZ(radius, 0, 0)));

                //CurveLoop con semicírculo
                CurveLoop curveLoop = CurveLoop.Create(profile);

                //Creamos SolidOptions material y estilo = -1
                SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

                //Definimos Frame y comprobamos 
                Frame frame = new Frame(center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);
                if (Frame.CanDefineRevitGeometry(frame) == false)
                {
                    message = "Imposible crear DirectShape";
                    return Result.Failed;
                }

                //Creamos un sólido de revolución
                Solid sphere = GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI, options);
                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name Solid");

                    // Creamos una DirectShape en el doc categoría puertas
                    Autodesk.Revit.DB.DirectShape ds = Autodesk.Revit.DB.DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Doors));

                    //Completamos datos
                    ds.ApplicationId = "Revit API Manual";
                    ds.ApplicationDataId = "Revit API Manual. Creación esfera";
                    //Asignamos geometría
                    ds.SetShape(new GeometryObject[] { sphere });

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREACION TOTALMENTE MANUAL DEL DirectShape
            try
            {

                //Asumimos ejes en direccion estandard

                //Construimos BRepBuilder Solido
                BRepBuilder brepBuilder = new BRepBuilder(BRepType.Solid);

                //Definimos Frame y comprobamos 
                Frame frame = new Frame(new XYZ(50, -100, 0), new XYZ(0, 1, 0), new XYZ(-1, 0, 0), new XYZ(0, 0, 1));
                if (Frame.CanDefineRevitGeometry(frame) == false)
                {
                    message = "Imposible crear DirectShape";
                    return Result.Failed;
                }

                //Creamos geometria 4 semicirculos y dos generatrices de cilindro
                BRepBuilderEdgeGeometry frontEdgeBottom = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -100, 0), new XYZ(100, -100, 0), new XYZ(50, -50, 0)));
                BRepBuilderEdgeGeometry backEdgeBottom = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(100, -100, 0), new XYZ(0, -100, 0), new XYZ(50, -150, 0)));

                BRepBuilderEdgeGeometry frontEdgeTop = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -100, 100), new XYZ(100, -100, 100), new XYZ(50, -50, 100)));
                BRepBuilderEdgeGeometry backEdgeTop = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -100, 100), new XYZ(100, -100, 100), new XYZ(50, -150, 100)));

                BRepBuilderEdgeGeometry linearEdgeFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, -100, 0), new XYZ(100, -100, 100));
                BRepBuilderEdgeGeometry linearEdgeBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, -100, 0), new XYZ(0, -100, 100));

                //Añadimos los 6 Edges
                BRepBuilderGeometryId frontEdgeBottomId = brepBuilder.AddEdge(frontEdgeBottom);
                BRepBuilderGeometryId frontEdgeTopId = brepBuilder.AddEdge(frontEdgeTop);
                BRepBuilderGeometryId linearEdgeFrontId = brepBuilder.AddEdge(linearEdgeFront);
                BRepBuilderGeometryId linearEdgeBackId = brepBuilder.AddEdge(linearEdgeBack);
                BRepBuilderGeometryId backEdgeBottomId = brepBuilder.AddEdge(backEdgeBottom);
                BRepBuilderGeometryId backEdgeTopId = brepBuilder.AddEdge(backEdgeTop);

                // Las superficies de las cuatro Faces. 
                //Cilindrica
                CylindricalSurface cylSurf = CylindricalSurface.Create(frame, 50);
                //Planas superior e inferior
                Plane top = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 100));  //Normal hacia afuera
                Plane bottom = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 0)); //Normal hacia adentro

                //Añadimos las 4 Face
                BRepBuilderGeometryId frontCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(cylSurf, null), false);
                BRepBuilderGeometryId backCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(cylSurf, null), false);
                BRepBuilderGeometryId topFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
                BRepBuilderGeometryId bottomFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);

                //Añadimos Loops de las 4 Face
                BRepBuilderGeometryId loopId_Top = brepBuilder.AddLoop(topFaceId);
                BRepBuilderGeometryId loopId_Bottom = brepBuilder.AddLoop(bottomFaceId);
                BRepBuilderGeometryId loopId_Front = brepBuilder.AddLoop(frontCylFaceId);
                BRepBuilderGeometryId loopId_Back = brepBuilder.AddLoop(backCylFaceId);

                //Añadimos coEdge para el Loop de la Face frontal
                brepBuilder.AddCoEdge(loopId_Front, linearEdgeBackId, false);
                brepBuilder.AddCoEdge(loopId_Front, frontEdgeTopId, false);
                brepBuilder.AddCoEdge(loopId_Front, linearEdgeFrontId, true);
                brepBuilder.AddCoEdge(loopId_Front, frontEdgeBottomId, true);
                brepBuilder.FinishLoop(loopId_Front);
                brepBuilder.FinishFace(frontCylFaceId);

                //Añadimos coEdge para el Loop de la Face trasera
                brepBuilder.AddCoEdge(loopId_Back, linearEdgeBackId, true);
                brepBuilder.AddCoEdge(loopId_Back, backEdgeBottomId, true);
                brepBuilder.AddCoEdge(loopId_Back, linearEdgeFrontId, false);
                brepBuilder.AddCoEdge(loopId_Back, backEdgeTopId, true);
                brepBuilder.FinishLoop(loopId_Back);
                brepBuilder.FinishFace(backCylFaceId);

                //Añadimos coEdge para el Loop de la Face superior
                brepBuilder.AddCoEdge(loopId_Top, backEdgeTopId, false);
                brepBuilder.AddCoEdge(loopId_Top, frontEdgeTopId, true);
                brepBuilder.FinishLoop(loopId_Top);
                brepBuilder.FinishFace(topFaceId);

                //Añadimos coEdge para el Loop de la Face inferior
                brepBuilder.AddCoEdge(loopId_Bottom, frontEdgeBottomId, false);
                brepBuilder.AddCoEdge(loopId_Bottom, backEdgeBottomId, false);
                brepBuilder.FinishLoop(loopId_Bottom);
                brepBuilder.FinishFace(bottomFaceId);

                //Finalizamos construcción
                brepBuilder.Finish();

                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name BRepBuilder");

                    //Creamos una DirectShape en el doc categoría muros
                    Autodesk.Revit.DB.DirectShape ds = Autodesk.Revit.DB.DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Walls));

                    //Completamos datos
                    ds.ApplicationId = "Revit API Manual";
                    ds.ApplicationDataId = "Revit API Manual. Creación Tessellated";
                    //Asignamos geometría
                    ds.SetShape(brepBuilder);

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 12.0 ----------------------------------- ETIQUETAS, COTAS, TEXTOS ------------------------------------

            #region CREACION DE ETIQUETAS
            try
            {

                //Suponemos la Tag ya cargada
                Wall wall = null;
                Reference reference = null;

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;
                try
                {
                    //PickObject siemtre entre try{} catch {}
                    reference = sel.PickObject(ObjectType.Element, new WallSelectionFilterEtiquetas(), "Seleccione un muro");
                    wall = doc.GetElement(reference) as Wall;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Failed;
                }
                // O es 2D o 3D bloqueada
                View view = doc.ActiveView;
                if (view is View3D && (view as View3D).IsLocked == false)
                {
                    message = "No es posible en 3D, o 3D no bloqueadas";
                    return Result.Failed;
                }

                // Definimos modo de etiqueta por categoría y la orientación de la etiqueta
                TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagorn = TagOrientation.Horizontal;

                //Obtenemos LocationCurve y el punto medio
                LocationCurve wallLoc = wall.Location as LocationCurve;
                XYZ wallStart = wallLoc.Curve.GetEndPoint(0);
                XYZ wallEnd = wallLoc.Curve.GetEndPoint(1);
                XYZ wallMid = wallLoc.Curve.Evaluate(0.5, true);
                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Etiqueta");

                    IndependentTag newTag = IndependentTag.Create(doc, view.Id, reference, true, tagMode, tagorn, wallMid);
                    if (null == newTag)
                    {
                        throw new Exception("Creación de IndependentTag fallida.");
                    }

                    // newTag.TagText es de solo lectura, por lo que cambiamos el parámetro de tipo de Type Mark comotexto de la etiqueta.
                    // El parámetro de etiqueta para la familia de etiquetas determina qué parámetro de tipo se utiliza para el texto de la etiqueta.

                    //Obtenemos el tipo
                    WallType type = wall.WallType;
                    //Ontenemos el parámetro y le damos valor
                    type.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_MARK).Set("Hola API");

                    if (!(view is View3D))
                    {
                        //Si no ed 3D
                        // Establecemos el extremo libre, de lo contrario, el punto final se mueve con el codo
                        newTag.LeaderEndCondition = LeaderEndCondition.Free;
                        //Calculamos puntos para el codo y de posición
                        XYZ elbowPnt = wallMid + new XYZ(5.0, 5.0, 0.0);
                        newTag.SetLeaderElbow(reference, elbowPnt);
                        XYZ headerPnt = wallMid + new XYZ(10.0, 10.0, 0.0);
                        newTag.TagHeadPosition = headerPnt;
                    }
                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREACION DE COTAS
            try
            {

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;

                // Chequeamos que solo tenemos dos muros seleccionado           
                List<Wall> walls = sel.GetElementIds().Select(x => doc.GetElement(x)).Cast<Wall>().ToList();
                if (walls.Count != 2)
                {
                    message = "Se debe seleccionar dos muros";
                    return Result.Failed;
                }

                //Creamos una cota
                Dimension dimension = null;
                //Obtenemos la vista actual
                View view = doc.ActiveView;
                //Obtenemos las lineas centrales de los dos muros
                //Llamamos al método GetCenterline
                Line baseLine = GetCenterline(walls[0]);
                Line line = GetCenterline(walls[1]);
                #region Cota Centro muro
                //Construimos Line
                Line lineCota = Line.CreateBound(baseLine.GetEndPoint(0), line.GetEndPoint(0));

                //Construimos ReferenceArray
                ReferenceArray referenceArrayCenter = new ReferenceArray();

                //Agregamos Reference
                referenceArrayCenter.Append(baseLine.Reference);
                referenceArrayCenter.Append(line.Reference);
                #endregion

                #region referencias nucleo
                //Obtenemos UniqueId y Reference 0
                string uniqueIdWall0 = walls[0].UniqueId;
                string refStringWall0 = string.Format("{0}:{1}:{2}", uniqueIdWall0, -9999, 4);
                Reference core_centreWall0 = Reference.ParseFromStableRepresentation(doc, refStringWall0);

                //Obtenemos UniqueId y Reference 1
                string uniqueIdWall1 = walls[1].UniqueId;
                string refStringWall1 = string.Format("{0}:{1}:{2}", uniqueIdWall1, -9999, 3);
                Reference core_innerWall1 = Reference.ParseFromStableRepresentation(doc, refStringWall1);

                //Construimos ReferenceArray
                ReferenceArray referenceArrayCore = new ReferenceArray();

                //Agregamos Reference
                referenceArrayCore.Append(core_centreWall0);
                referenceArrayCore.Append(core_innerWall1);

                //Construimos Line
                Line lineCotaCore = Line.CreateBound(baseLine.GetEndPoint(1), line.GetEndPoint(1));
                #endregion

                #region Cota perpendicular 
                //Convertimo la linea en UnBound
                line.MakeUnbound();
                //Proyectamos una linea sobre la otra 
                IntersectionResult result = line.Project(baseLine.Evaluate(0.5, true));
                //Si la proyección tiene resultado, hay intersección
                Line lineAlineada = null;
                if (result != null)
                {
                    //Obtenemos el punro proyectado
                    XYZ point = result.XYZPoint;
                    //Calculamos el vector de traslación, desde el punto proyectado a su proyección
                    XYZ vector = baseLine.Origin - point;
                    //Construimos Line
                    lineAlineada = Line.CreateBound(point, baseLine.Evaluate(0.5, true));
                }
                #endregion

                //Creamos una transaction
                using (Transaction tx = new Transaction(doc))
                {
                    try
                    {
                        //Iniciamos la transaction
                        tx.Start("Transaction cotas");
                        //Ceamos las 3 Dimension
                        dimension = doc.Create.NewDimension(view, lineCota, referenceArrayCenter);
                        dimension = doc.Create.NewDimension(view, lineAlineada, referenceArrayCenter);
                        dimension = doc.Create.NewDimension(view, lineCotaCore, referenceArrayCore);

                        //Confirmamos la transaction
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        //Si falla anulamos la transaction
                        tx.RollBack();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region MODIFICACION DE COTAS Y OBTENCION DE INFORMACION DE LAS RESTRICCIONES.
            try
            {

                bool isCota = false;

                // Accedemos a la selección actual
                Selection sel = uidoc.Selection;
                //Creamos Reference
                Reference reference = null;
                // Chequeamos que solo tenemos dos muros seleccionado           
                List<Dimension> dimensions = sel.GetElementIds().Select(x => doc.GetElement(x)).Cast<Dimension>().ToList();
                if (dimensions.Count != 1)
                {
                    message = "Se debe seleccionar una sola Dimension";
                    return Result.Failed;
                }
                //Obtenemos Dimension
                Dimension dimension = dimensions.FirstOrDefault();

                //Creamos mensaje
                string mensaje = "Dimension : ";
                //Obtenemos el nombre 
                mensaje += "\nEl nombre es : " + dimension.Name;
                //Obtenemos la Curve
                Autodesk.Revit.DB.Curve curve = dimension.Curve;
                if (curve != null && curve.IsBound)
                {
                    //Obtenemos punto inicial y final
                    mensaje += "\nLínea punto inicial:(" + curve.GetEndPoint(0).X + ", " + curve.GetEndPoint(0).Y + ", " + curve.GetEndPoint(0).Z + ")";
                    mensaje += "; Línea punto final:(" + curve.GetEndPoint(1).X + ", " + curve.GetEndPoint(1).Y + ", " + curve.GetEndPoint(1).Z + ")";
                }
                //Obtenemos el nombre del typo
                mensaje += "\nNombre del tipo de Dimension : " + dimension.DimensionType.Name;

                //Obtenemos eumero de Reference
                mensaje += "\nNumero de Reference en la Dimension " + dimension.References.Size;
                //Obtenemos Dimensón/Restricción
                if ((int)BuiltInCategory.OST_Dimensions == dimension.Category.Id.IntegerValue)
                {
                    mensaje += "\nLa Dimension es cota.";
                    //Obtenemos el nombre de la vita. Solo si es cota
                    mensaje += "\nNombre de la vista : " + dimension.View.Name;
                    isCota = true;
                }
                else if ((int)BuiltInCategory.OST_Constraints == dimension.Category.Id.IntegerValue)
                {
                    mensaje += "\nla Dimension es restricción.";
                }

                TaskDialog.Show("Manual Revit API", mensaje);

                if (!isCota)
                {
                    TaskDialog.Show("Manual Revit API", "No se puede modificar");
                    return Result.Succeeded;

                }
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Transaction Cota");

                    // Check to see if we have a non-multisegment dimension and if text position is adjustable
                    if (dimension.NumberOfSegments == 0 && dimension.IsTextPositionAdjustable())
                    {
                        // Get the current text XYZ position
                        XYZ currentTextPosition = dimension.TextPosition;
                        // Calculate a new XYZ position by transforming the current text position
                        XYZ newTextPosition = Transform.CreateTranslation(new XYZ(0.0, 1.0, 0.0)).OfPoint(currentTextPosition);
                        // Set the new text position
                        dimension.TextPosition = newTextPosition;
                    }
                    else if (dimension.NumberOfSegments > 0)
                    {
                        foreach (DimensionSegment currentSegment in dimension.Segments)
                        {
                            if (currentSegment != null && currentSegment.IsTextPositionAdjustable())
                            {
                                // Get the current text XYZ position
                                XYZ currentTextPosition = currentSegment.TextPosition;
                                // Calculate a new XYZ position by transforming the current text position
                                XYZ newTextPosition = Transform.CreateTranslation(new XYZ(0, 1, 0)).OfPoint(currentTextPosition);
                                // Set the new text position for the segment's text
                                currentSegment.TextPosition = newTextPosition;
                            }
                        }
                    }
                    tx.Commit();
                    TaskDialog.Show("Manual Revit API", "Cota modificada");
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREAR UN ESTILO DE LINEA - PARA ModelLines POR EJEMPLO
            try
            {

                //Creamos patrçon de linea
                LinePattern linePattern = null;
                //Nombre para Subcategotia y Patron de lineas
                const string nombre = "Revit API Manual";
                //Creamos subcategoria
                Category subCategoriaLine = null;

                //Colector de clase LinePatternElement por LinePattern no podemos
                FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(LinePatternElement));
                //Buscamos el Patron de linea  "Revit API Manual"
                LinePatternElement linePatternElement = col.Cast<LinePatternElement>().Where(x => x.GetLinePattern().Name == nombre).FirstOrDefault();

                //Si no existe.
                if (linePatternElement == null)
                {
                    //Creamos lista de segmentos
                    List<LinePatternSegment> lstSegments = new List<LinePatternSegment>();
                    //Añadimos segmentos
                    lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dot, 0.0));
                    lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));
                    lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Dash, 0.03));
                    lstSegments.Add(new LinePatternSegment(LinePatternSegmentType.Space, 0.02));

                    //Crear patron de linea. con  "Revit API Manual"
                    linePattern = new LinePattern(nombre);
                    //Aplicamos segmentos
                    linePattern.SetSegments(lstSegments);
                }

                // El nuevo estilo de línea será una subcategoría de la categoría Líneas.
                // 
                //Obtenemos todas categorias
                Categories categories = doc.Settings.Categories;
                //Seleccionamos la categoría de Lines
                Category categoriaLine = categories.get_Item(BuiltInCategory.OST_Lines);
                //Buscamos si ya esta creada la subcategoria
                if (categoriaLine.SubCategories.Contains(nombre))
                    subCategoriaLine = categoriaLine.SubCategories.get_Item(nombre);
                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Create LineStyle");
                    //Creamos un LinePatternElement
                    if (linePatternElement == null)
                        linePatternElement = LinePatternElement.Create(doc, linePattern);
                    //Creamos suncategoria
                    if (subCategoriaLine == null)
                        subCategoriaLine = categories.NewSubcategory(categoriaLine, "Revit API Manual");

                    // doc.Regenerate();
                    //Configuramos subcategoria
                    //Espesor
                    subCategoriaLine.SetLineWeight(8, GraphicsStyleType.Projection);
                    //Color rojo
                    subCategoriaLine.LineColor = new Color(255, 0, 0);
                    //LinePatternElement
                    subCategoriaLine.SetLinePatternId(linePatternElement.Id, GraphicsStyleType.Projection);

                    //Conformamos Transaction
                    tx.Commit();
                }
                TaskDialog.Show("Manual Revit API", "Estilo de linea creado o actualizado");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREAR LINEAS DE DETALLE
            try
            {
                //Nombre para Subcategotia y Patron de lineas
                const string nombre = "Revit API Manual";

                //Creamos GraphicsStyle
                GraphicsStyle graphicsStyle = null;

                //Creamos 4 XYZ
                XYZ xYZ0 = XYZ.Zero;
                XYZ xYZ1 = new XYZ(10, 0, 0);
                XYZ xYZ2 = new XYZ(10, 10, 0);
                XYZ xYZ3 = new XYZ(0, 10, 0);

                //Creamos lista con 4 Lines
                List<Line> lines = new List<Line>();
                lines.Add(Line.CreateBound(xYZ0, xYZ1));
                lines.Add(Line.CreateBound(xYZ1, xYZ2));
                lines.Add(Line.CreateBound(xYZ2, xYZ3));
                lines.Add(Line.CreateBound(xYZ3, xYZ0));

                #region buscar Estilo grafico
                Categories categories = doc.Settings.Categories;
                Category categoriaLines = categories.get_Item(BuiltInCategory.OST_Lines);
                //Buscamos si  esta creada la subcategoria
                if (categoriaLines.SubCategories.Contains(nombre))
                {
                    Category subCategoriaLine = categoriaLines.SubCategories.get_Item(nombre);
                    //Asignamos el GraphicsStyle, en caso contrario permanece en null
                    graphicsStyle = subCategoriaLine.GetGraphicsStyle(GraphicsStyleType.Projection);
                }

                #endregion
                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamo Transaction
                    tx.Start("Transaction Name");
                    //Para cada miembro de la lista creamos Linea de detalle
                    foreach (Line line in lines)
                    {
                        DetailCurve detailCurve = doc.Create.NewDetailCurve(doc.ActiveView, line);
                        //Si hay GraphicsStyle lo cambiamos, si no se cra con el de defecto y no lo cambiamos
                        if (graphicsStyle != null) detailCurve.LineStyle = graphicsStyle;
                    }
                    //Confirmamos Transaction
                    tx.Commit();
                }

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region
            try
            {
                //Accedemos a la selección
                Selection sel = uidoc.Selection;

                //Obtenemos los Element seleccionados
                List<Element> curves = sel.GetElementIds().Select(x => doc.GetElement(x)).ToList();

                //Creamos nuevo DetailCurveArray y ModelCurveArray
                DetailCurveArray detailCurveArray = new DetailCurveArray();
                ModelCurveArray modelCurveArray = new ModelCurveArray();

                //Iteramos en toda la selección
                foreach (Element curve in curves)
                {
                    if (curve is DetailCurve) //Si es DetailCurve
                        detailCurveArray.Append(curve as DetailCurve);
                    else if (curve is ModelCurve) //Si es ModelCurve
                        modelCurveArray.Append(curve as ModelCurve);
                    //Pdemos haber seleccionado muros...
                }

                //Creamos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name");

                    //Convertimos
                    doc.ConvertDetailToModelCurves(doc.ActiveView, detailCurveArray);
                    doc.ConvertModelToDetailCurves(doc.ActiveView, modelCurveArray);

                    //Confirmamos Transaction
                    tx.Commit();
                }

                TaskDialog.Show("Manual Revit API", detailCurveArray.Size + " Lineas de detalle convertidas.\n" +
                     modelCurveArray.Size + " Lineas de modelo convertidas.");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 13.0 ----------------------------------- MATERIALES ------------------------------------

            #region OBTENER INFORMACION DE LOS MATERIALES - CAMBIO DEL COLOR DEL MATERIAL
            try
            {

                //Filtramos por la clase Material
                FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Material));
                //Seleccionamos el material Arena
                Material material = col.ToElements().Where(x => x.Name == "Arena").FirstOrDefault() as Material;

                //Si no existe salimos
                if (material == null)
                {
                    message = "Material no encontrado";
                    return Result.Failed;
                }

                //Preparamos salida
                string datos = "Caracteristicas del material: " + material.Name;

                //Obtenemos los tres (de los cinco) Asset accesibles
                ElementId strucAssetId = material.StructuralAssetId;
                ElementId apperanceAssetId = material.AppearanceAssetId;
                ElementId thermalAssetId = material.ThermalAssetId;

                //Obtenemos el color,
                int colorInt = material.get_Parameter(BuiltInParameter.MATERIAL_PARAM_COLOR).AsInteger();
                datos = datos + "\nColor int: " + colorInt;

                //Analizamos ThermalAsset
                if (thermalAssetId != ElementId.InvalidElementId)
                {
                    //Obtenemos PropertySetElement
                    PropertySetElement pse = doc.GetElement(thermalAssetId) as PropertySetElement;
                    if (pse != null)
                    {
                        //Obtenemos el ThermalAsset
                        ThermalAsset asset = pse.GetThermalAsset();

                        // Verificamos el material. Solo avanzamos si es sólido
                        if (asset.ThermalMaterialType == ThermalMaterialType.Solid)
                        {
                            //Obtenemos las propiedades que se admiten en tipo sólido 
                            bool isTransmitsLight = asset.TransmitsLight;
                            double permeability = asset.Permeability;
                            double porosity = asset.Porosity;
                            double reflectivity = asset.Reflectivity;
                            double resistivity = asset.ElectricalResistivity;
                            StructuralBehavior behavior = asset.Behavior;

                            // Obtenemos otras propiedades.
                            double heatOfVaporization = asset.SpecificHeatOfVaporization;
                            double emissivity = asset.Emissivity;
                            double conductivity = asset.ThermalConductivity;
                            double density = asset.Density;

                            //Mostramos p.e. la Conductividad
                            datos = datos + "\nConductividad: " + conductivity.ToString("N3");
                        }

                    }
                }

                //Analizamos StructuralAsset
                if (strucAssetId != ElementId.InvalidElementId)
                {
                    //Obtenemos PropertySetElement
                    PropertySetElement pse = doc.GetElement(strucAssetId) as PropertySetElement;
                    if (pse != null)
                    {
                        //Obtenemos StructuralAsset
                        StructuralAsset asset = pse.GetStructuralAsset();

                        // Verificamos el material. Solo avanzamos si es Isotropic
                        if (asset.Behavior == StructuralBehavior.Isotropic)
                        {
                            // Obtenemos la clase de material
                            StructuralAssetClass assetClass = asset.StructuralAssetClass;
                            datos = datos + "\nClase de material estructural: " + assetClass;

                            // Obtenemos otras propiedades.
                            double poisson = asset.PoissonRatio.X;
                            double youngMod = asset.YoungModulus.X;
                            double thermCoeff = asset.ThermalExpansionCoefficient.X;
                            double unitweight = asset.Density;
                            double shearMod = asset.ShearModulus.X;

                            if (assetClass == StructuralAssetClass.Metal)
                            {
                                //Propiedades especificas de metal
                                double dMinStress = asset.MinimumYieldStress;
                            }
                            else if (assetClass == StructuralAssetClass.Concrete)
                            {
                                //Propiedades especificas de hormigón
                                double dConcComp = asset.ConcreteCompression;
                            }

                            //Mostramos p.e. la Densidad
                            datos = datos + "\nDensidad: " + unitweight.ToString("N3");
                        }
                    }
                }


                // Definimos nueva Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Color");
                    //Obtenemos AppearanceAssetElement
                    AppearanceAssetElement assetElem = doc.GetElement(apperanceAssetId) as AppearanceAssetElement;
                    //Definimos AppearanceAssetEditScope- Similar a Transaction
                    using (AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc))
                    {
                        //Iniciamos AppearanceAssetEditScope
                        Asset editableAsset = editScope.Start(assetElem.Id);

                        //Buscamos los datos de "generic_diffuse"
                        AssetPropertyDoubleArray4d genericDiffuseProperty = editableAsset.FindByName("generic_diffuse") as AssetPropertyDoubleArray4d;
                        //Obtenemos el color en RGB

                        Color color = genericDiffuseProperty.GetValueAsColor();
                        datos = datos + "\nColor RGB actual: " + color.Red + " | " + color.Green + " | " + color.Blue;
                        //Definimos nuevo color
                        Color newColor = new Color(255, 0, 0);
                        genericDiffuseProperty.SetValueAsColor(newColor);
                        // Confirmamos AppearanceAssetEditScope
                        editScope.Commit(true);
                    }
                    //Confirmamos Transaction
                    tx.Commit();
                }

                TaskDialog.Show("Revit API Manual", datos);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREAR Y DUPLICAR MATERIALES
            try
            {
                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name");

                    //Creamos nuevo material
                    ElementId materialId = Material.Create(doc, "Nuevo Material");
                    Material material = doc.GetElement(materialId) as Material;

                    //Creamos un nuevo conjunto de propiedades. 
                    StructuralAsset strucAsset = new StructuralAsset("Nuevo Property Set", StructuralAssetClass.Concrete);
                    //Propiedades minimas
                    strucAsset.Behavior = StructuralBehavior.Isotropic;
                    strucAsset.Density = 232.0;

                    //Asignamos el conjunto de propiedades al material. Estructural
                    PropertySetElement pse = PropertySetElement.Create(doc, strucAsset);
                    material.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pse.Id);

                    //Nuevo nombre
                    string newName = material.Name + "_Duplicado";

                    //Filtramos materiales. Buscamos anteriores copias
                    FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(Material));
                    List<Material> materials = col.ToElements().Cast<Material>().ToList();

                    int nRepetidos = materials.Where(x => x.Name.StartsWith(newName)).Count();
                    if (nRepetidos > 0)
                    {
                        //Incrementamos con contador si existe el duplicado
                        newName = "New" + material.Name + "(" + nRepetidos + ")";
                    }

                    //Duplicamos material y asignamos nuevo nombre
                    Material myMaterial = material.Duplicate(newName);

                    if (null == myMaterial) TaskDialog.Show("Revit API Manual", "No se ha podido duplicar");
                    else TaskDialog.Show("Revit API Manual", "Material duplicado");

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region VER LOS DIFERENTES MATERIALES CON LOS QUE ESTA COMPUESTO UN MURO, VOLUMENES
            try
            {

                //Iniciamos con un Wall seleccionado
                Selection sel = uidoc.Selection;
                //Obtenemos el muro
                Wall wall = doc.GetElement(sel.GetElementIds().FirstOrDefault()) as Wall;
                if (wall is null)
                {
                    message = "Se debe iniciar con un muro seleccionado";
                    return Result.Cancelled;
                }

                //Obtenemos el tipo. Los muros cortina no tiene materiales ni capas
                WallType wallType = wall.WallType;
                if (wallType.Kind != WallKind.Basic)
                {
                    message = "Debe ser un muro básico";
                    return Result.Cancelled;
                }

                //Obtenemos el material por categoría
                Categories categories = doc.Settings.Categories;
                categories.get_Item(BuiltInCategory.OST_Walls);
                Material materialCategoria = wall.Category.Material;

                string mensaje = "Datos de composición del muro:";
                mensaje = mensaje + "\nEl material por categoría es: " + materialCategoria.Name;

                //Obtenemos la composición
                CompoundStructure compoundStructure = wallType.GetCompoundStructure();
                foreach (CompoundStructureLayer compoundStructureLayer in compoundStructure.GetLayers())
                {
                    //Iteramos entre cada material.Obtenemos material y función
                    Material layerMaterial = doc.GetElement(compoundStructureLayer.MaterialId) as Material;
                    MaterialFunctionAssignment materialFunctionAssignment = compoundStructureLayer.Function;

                    //Es funcion de aislamiento?
                    if (materialFunctionAssignment == MaterialFunctionAssignment.Insulation)
                    {
                        //Obtenemos propiedades del aislamiento
                        mensaje = mensaje + "\n\nAislamento. Material = " + layerMaterial.Name;
                        mensaje = mensaje + "\nEspesor= " + compoundStructureLayer.Width.ToString("N3");
                        double areaMate = wall.GetMaterialArea(layerMaterial.Id, false);
                        mensaje = mensaje + "\nArea= " + areaMate.ToString("N3");
                        double volumenMate = wall.GetMaterialVolume(layerMaterial.Id);
                        mensaje = mensaje + "\nVolumen= " + volumenMate.ToString("N3");
                    }

                }

                //Obtenemos los materiales de Pintura
                ICollection<ElementId> elementIdsMateriales = wall.GetMaterialIds(true);
                //Obtenemos area del material pintado. Y volumen
                double areaMatePintado = wall.GetMaterialArea(elementIdsMateriales.First(), true);
                double volumenMatePintado = wall.GetMaterialVolume(elementIdsMateriales.First());

                mensaje = mensaje + "\n\nMaterial Pintado. " + doc.GetElement(elementIdsMateriales.First()).Name +
                               "\nArea= " + areaMatePintado.ToString("N3") +
                               "\nVolumen= " + volumenMatePintado.ToString("N3");

                //Obtenemos indice de la primera capa del nucle
                mensaje = mensaje + "\n\nIndice capa nucleo: " + compoundStructure.GetCoreBoundaryLayerIndex(ShellLayerType.Exterior);

                #region Caras extremas
                //Obtenemos References Exterior. Tomamos 1ª. Suponemos es muro Plano
                IList<Reference> sideFacesE = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
                Reference referenceFaceE = sideFacesE[0];
                PlanarFace faceE = wall.Document.GetElement(referenceFaceE).GetGeometryObjectFromReference(referenceFaceE) as PlanarFace;

                //Obtenemos References Interior. Tomamos 1ª. Suponemos es muro Plano
                IList<Reference> sideFacesI = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);
                Reference referenceFaceI = sideFacesI[0];
                PlanarFace faceI = wall.Document.GetElement(referenceFaceI).GetGeometryObjectFromReference(referenceFaceI) as PlanarFace;

                //Obtenemos area de caras y por defecto
                mensaje = mensaje + "\n\nDatos de caras extremas:";
                mensaje = mensaje + "\nCara Exterior. Area: " + faceE.Area.ToString("N3");
                mensaje = mensaje + "\nCara Interior. Area: " + faceI.Area.ToString("N3");
                mensaje = mensaje + "\nPor defecto muro. Area: " + wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble().ToString("N3");

                #endregion
                TaskDialog.Show("Revit API Manual", mensaje);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 14.0 ----------------------------------- ESQUEMAS ------------------------------------

            #region CREACION DE ESQUEMAS - ¿PUEDE SERVIR PARA CREAR CAPAS?
            try
            {

                //Iniciamos con un Wall seleccionado
                Selection sel = uidoc.Selection;

                ElementId id = sel.GetElementIds().FirstOrDefault();
                if (id is null)
                {
                    message = "Se debe iniciar con un muro seleccionado";
                    return Result.Cancelled;
                }

                //Obtenemos el muro
                Wall wall = doc.GetElement(id) as Wall;
                if (wall is null)
                {
                    message = "Se debe iniciar con un muro seleccionado";
                    return Result.Cancelled;
                }

                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Schema");

                    Guid guid = Guid.Empty;
                    #region Simple_double
                    guid = new Guid("4d8a80d3-e1c3-4b83-ada1-ce975e420529");
                    //Recuperamos Schema
                    Schema schema = Schema.Lookup(guid);
                    //Si es null, le creamos. Si no es null genera error
                    if (schema == null)
                    {
                        //Construimos Schema
                        SchemaBuilder schemaBuilder = new SchemaBuilder(guid);
                        schemaBuilder.SetVendorId("FdAA");
                        schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                        schemaBuilder.SetSchemaName("EspesorMuro");
                        schemaBuilder.SetDocumentation("Schema un Field. Simple <double> = Espesor del muro");
                        /*
                        //Construimos Field
                        FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField("CampoEspesorMuro", typeof(double));
                        fieldBuilder.SetSpec(SpecTypeId.Length); //V2022
                                                                 // fieldBuilder.SetUnitType(UnitType.UT_Length);/V2021
                        */
                        //Finalizamos constructor Field
                        schema = schemaBuilder.Finish();
                    }
                    double espesor = wall.Document.GetElement(wall.GetTypeId()).get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();

                    //Construimos Entity.
                    Entity entity = new Entity(schema);
                    //Asignamos datos V2022 UnitTypeId.Feet | V2021 DisplayUnitType.DUT_DECIMAL_FEET
                    entity.Set<double>("CampoEspesorMuro", espesor, UnitTypeId.Feet /*DisplayUnitType.DUT_DECIMAL_FEET*/);
                    //Asociamos a wall
                    if (entity.IsValid()) wall.SetEntity(entity);
                    #endregion

                    #region Array_double
                    guid = new Guid("5d8a80d3-e1c3-4b83-ada1-ce975e420529");
                    //Recuperamos Schema
                    schema = Schema.Lookup(guid);
                    //Si es null, le creamos. Si no es null genera error
                    if (schema == null)
                    {
                        //Construimos Schema
                        SchemaBuilder schemaBuilder = new SchemaBuilder(guid);
                        schemaBuilder.SetVendorId("FdAA");
                        schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                        schemaBuilder.SetSchemaName("EspesorMuroLongitud");
                        schemaBuilder.SetDocumentation("Schema un Field. Array<double> = Espesor y longitud del muro");
                        /*
                        //Construimos Field
                        FieldBuilder fieldBuilder = schemaBuilder.AddArrayField("CampoEspesorLongitudMuro", typeof(double));
                        fieldBuilder.SetSpec(SpecTypeId.Length);//V2022
                                                                //fieldBuilder.SetUnitType(UnitType.UT_Length);//V2021
                        */
                        //Finalizamos constructor Field
                        schema = schemaBuilder.Finish();
                    }
                    double longitud = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                    IList<double> vs = new List<double>() { espesor, longitud };

                    //Construimos Entity. 
                    entity = new Entity(schema);
                    //Asignamos datos V2022 UnitTypeId.Feet | V2021 DisplayUnitType.DUT_DECIMAL_FEET
                    entity.Set<IList<double>>("CampoEspesorLongitudMuro", vs, UnitTypeId.Feet /*DisplayUnitType.DUT_DECIMAL_FEET*/);
                    //Asociamos a wall
                    if (entity.IsValid()) wall.SetEntity(entity);
                    #endregion

                    #region Simple_string+Array_double
                    guid = new Guid("6d8a80d3-e1c3-4b83-ada1-ce975e420529");
                    //Recuperamos Schema
                    schema = Schema.Lookup(guid);
                    //Si es null, le creamos. Si no es null genera error
                    if (schema == null)
                    {
                        //Construimos Schema
                        SchemaBuilder schemaBuilder = new SchemaBuilder(guid);
                        schemaBuilder.SetVendorId("FdAA");
                        schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                        schemaBuilder.SetSchemaName("EspesorMuroLongitudyNombre");
                        schemaBuilder.SetDocumentation("Schema dos Field. Simple <string> = nombre. Array<double> = Espesor y longitud del muro");
                        /*
                        //Construimos Fields
                        FieldBuilder fieldBuilder = schemaBuilder.AddArrayField("CampoEspesorLongitud", typeof(double));
                        fieldBuilder.SetSpec(SpecTypeId.Length);//.SetUnitType(UnitType.UT_Length);
                                                                //Segundo Field
                        */
                        schemaBuilder.AddSimpleField("CampoNombreMuro", typeof(string));

                        //Finalizamos constructor Fields
                        schema = schemaBuilder.Finish();
                    }
                    longitud = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                    vs = new List<double>() { espesor, longitud };

                    //Construimos Entity. 
                    entity = new Entity(schema);
                    //Asignamos datos V2022 UnitTypeId.Feet | V2021 DisplayUnitType.DUT_DECIMAL_FEET
                    entity.Set<IList<double>>("CampoEspesorLongitud", vs, UnitTypeId.Feet /*DisplayUnitType.DUT_DECIMAL_FEET*/);
                    entity.Set<string>("CampoNombreMuro", wall.Name);
                    //Asociamos a wall
                    if (entity.IsValid()) wall.SetEntity(entity);
                    #endregion

                    #region  Simple_ElementIdMapField_XYZ
                    guid = new Guid("7d8a80d3-e1c3-4b83-ada1-ce975e420529");
                    //Recuperamos Schema
                    schema = Schema.Lookup(guid);
                    //Si es null, le creamos. Si no es null genera error
                    if (schema == null)
                    {
                        //Construimos Schema
                        SchemaBuilder schemaBuilder = new SchemaBuilder(guid);
                        schemaBuilder.SetVendorId("FdAA");
                        schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                        schemaBuilder.SetSchemaName("DiccionarioXYZ");
                        schemaBuilder.SetDocumentation("Schema dos Field. Simple <ElementId> = Id.  IDictionary<int, XYZ> = Indide y XYZ");
                        /*
                        //Construimos Fields
                        FieldBuilder fieldBuilder = schemaBuilder.AddMapField("CampoDiccionario", typeof(int), typeof(XYZ));
                        fieldBuilder.SetSpec(SpecTypeId.Length);//V2022
                                                                //.SetUnitType(UnitType.UT_Length);//V2021
                                                                //Segundo Field
                        */
                        schemaBuilder.AddSimpleField("CampoID", typeof(ElementId));

                        //Finalizamos constructor Fields
                        schema = schemaBuilder.Finish();
                    }

                    IDictionary<int, XYZ> keyValuePairs = new Dictionary<int, XYZ>();
                    LocationCurve locationCurve = wall.Location as LocationCurve;
                    Curve line = locationCurve.Curve;
                    keyValuePairs.Add(0, line.GetEndPoint(0));
                    keyValuePairs.Add(1, line.GetEndPoint(1));

                    //Construimos Entity.
                    entity = new Entity(schema);
                    //Asignamos datos V2022 UnitTypeId.Feet | V2021 DisplayUnitType.DUT_DECIMAL_FEET
                    entity.Set<IDictionary<int, XYZ>>("CampoDiccionario", keyValuePairs, UnitTypeId.Feet /*DisplayUnitType.DUT_DECIMAL_FEET*/);
                    entity.Set<ElementId>("CampoID", wall.Id);
                    //Asociamos a wall
                    if (entity.IsValid()) wall.SetEntity(entity);
                    #endregion

                    //Confirmamos Transaction
                    tx.Commit();
                    TaskDialog.Show("Revit API Manual", "Schemas creados");

                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region "RECUPERAR" DATOS DE LA ENTITY
            try
            {

                //Iniciamos con un Wall seleccionado
                Selection sel = uidoc.Selection;
                ElementId id = sel.GetElementIds().FirstOrDefault();
                if (id is null)
                {
                    message = "Se debe iniciar con un muro seleccionado";
                    return Result.Cancelled;
                }

                //Obtenemos el muro
                Wall wall = doc.GetElement(id) as Wall;
                if (wall is null)
                {
                    message = "Se debe iniciar con un muro seleccionado";
                    return Result.Cancelled;
                }
                string txtSalida = "Datos recuperados: ";

                //GUID almacenado Debemos conocer todos los datos y estructura
                Guid guidSchema = new Guid("4d8a80d3-e1c3-4b83-ada1-ce975e420529");
                //Obtenemos el Schema
                Schema schema = Schema.Lookup(guidSchema);
                //Schema es nulo?
                if (schema != null)
                {
                    //Obtenemos Entity
                    Entity entity = wall.GetEntity(schema);
                    //Recuperamos el valor. V2022
                    //V2021 alternar 3º parametro
                    if (entity == null || entity.Schema == null || !entity.IsValidObject)
                    {
                        message = "El muro no tiene asignado el Entity";
                        return Result.Failed;
                    }
                    double espesorRecuperado = entity.Get<double>("CampoEspesorMuro", UnitTypeId.Meters /*DisplayUnitType.DUT_METERS*/);
                    txtSalida = txtSalida + "\n\tCampoEspesorMuro: " + espesorRecuperado.ToString("N3") + " metros";
                }

                txtSalida = txtSalida + "\n\nDatos recursivos:";

                IList<Guid> guids = wall.GetEntitySchemaGuids();

                foreach (Guid guid in guids)
                {
                    //Obtenemos el Schema
                    schema = Schema.Lookup(guid);
                    //Schema es nulo?
                    if (schema != null)
                    {
                        txtSalida = txtSalida + "\nSchema nombre: " + schema.SchemaName;
                        //Obtenemos Entity
                        Entity entity = wall.GetEntity(schema);
                        //Obtenemos lista de Fields en Schema
                        IList<Field> fields = schema.ListFields();
                        //Iteramos en cada Field del Schema
                        foreach (Field field in fields)
                        {
                            //Nombre del Field
                            string nameField = field.FieldName;
                            //Obtenemos tipo de dato almacenado
                            Type type = field.ValueType;

                            //Obtenemos unidades
                            ForgeTypeId forgeTypeId = field.GetSpecTypeId();
                            ForgeTypeId unit = UnitUtils.IsMeasurableSpec(forgeTypeId) ? UnitUtils.GetValidUnits(field.GetSpecTypeId()).First() : UnitTypeId.Custom;

                            //Nombre unidades metros, centimetros
                            string txtUnits = unit.TypeId.Split(':')[1].Split('-')[0];

                            //Es string=
                            if (type == typeof(string))
                            {
                                string valueString = entity.Get<string>(nameField, unit);
                                txtSalida = txtSalida + "\n      " + nameField + ": " + valueString;

                            }
                            //Es double y Simple?
                            else if (type == typeof(double) && field.ContainerType == ContainerType.Simple)
                            {
                                double valueDouble = entity.Get<double>(nameField, unit);
                                txtSalida = txtSalida + "\n      " + nameField + ": " + valueDouble.ToString("N3") + " " + txtUnits;

                            }
                            //Es double y Array?
                            else if (type == typeof(double) && field.ContainerType == ContainerType.Array)
                            {
                                IList<double> valueList = entity.Get<IList<double>>(nameField, unit);
                                txtSalida = txtSalida + "\n      " + $"{nameField} Espesor: {valueList.ElementAt(0) + " " + txtUnits}, Longitud: {valueList.ElementAt(1) + " " + txtUnits}";

                            }
                            //Es XYZ
                            else if (type == typeof(XYZ) && field.ContainerType == ContainerType.Map)
                            {
                                IDictionary<int, XYZ> valueDic = entity.Get<IDictionary<int, XYZ>>(nameField, unit);
                                txtSalida = txtSalida + "\n      " + $"{nameField} InicioX: {valueDic[0].X} {txtUnits} FinX: {valueDic[1].X} {txtUnits}";

                            }
                            //Es ElemenId?
                            else if (type == typeof(ElementId))
                            {
                                ElementId mId = entity.Get<ElementId>(nameField);
                                txtSalida = txtSalida + "\n      " + $"{nameField} Ejemplar: {mId.IntegerValue}.";

                            }
                        }
                    }
                }
                TaskDialog.Show("Revit API Manual", txtSalida);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region BORRAR ESQUEMAS, HAY 2 METODOS
            try
            {

                //Iniciamos con un Wall seleccionado
                Selection sel = uidoc.Selection;
                ElementId id = sel.GetElementIds().FirstOrDefault();
                if (id is null)
                {
                    message = "Se debe iniciar con un muro seleccionado";
                    return Result.Cancelled;
                }

                //Obtenemos el muro
                Wall wall = doc.GetElement(id) as Wall;
                if (wall is null)
                {
                    message = "Se debe iniciar con un muro seleccionado";
                    return Result.Cancelled;
                }
                //Obtenemos los Schemas en memoria. 
                IList<Schema> schemasPre = Schema.ListSchemas();

                //Obtenenos los GUID de Schemas en muro
                IList<Guid> guids = wall.GetEntitySchemaGuids();
                Schema schema = null;

                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamoa Transaction
                    tx.Start("Transaction Borrar Schema y Entity");
                    //Primer GUID
                    Guid guid = guids.FirstOrDefault();
                    //Obtenemos primer Schema en muro
                    schema = Schema.Lookup(guid);
                    //Es nulo?
                    if (schema != null)
                    {
                        //Borramos Entity en solo este muro
                        wall.DeleteEntity(schema);
                    }

                    //Ultimo GUID
                    guid = guids.LastOrDefault();
                    //Obtenemos ultimo Schema en muro
                    schema = Schema.Lookup(guid);
                    //Es nulo?
                    if (schema != null)
                    {
                        //Borramos Schema de todo el document
                        doc.EraseSchemaAndAllEntities(Schema.Lookup(guid));
                    }
                    //Confirmamos Transaction
                    tx.Commit();
                }

                //Obtenemos los Schemas en memoria. 
                IList<Schema> schemasPost = Schema.ListSchemas();

                TaskDialog.Show("Revit API Manual", "Schemas iniciales: " + schemasPre.Count + "\n" + String.Join("\n", schemasPre.Select(x => x.SchemaName).ToList()) +
                     "\n\nSchemas finales: " + schemasPost.Count + "\n" + String.Join("\n", schemasPost.Select(x => x.SchemaName).ToList()));

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 15.0 ----------------------------------- ARCHIVOS VINCULADOS ------------------------------------

            #region INFORMACION DEL SiteLocation. - Hay mas metodos en los videos - RAPI 27 - 1
            try
            {
                //Obtenemos la SiteLocation.
                SiteLocation site = doc.SiteLocation;

                //Constante para convertir radianes <=> grados
                const double angleRatio = Math.PI / 180;

                //Información actual. 
                string prompt = "SiteLocation del proyecto actual antes:";
                prompt += "\n\t" + "Latitud: " + site.Latitude / angleRatio + " grados";
                prompt += "\n\t" + "Longitud: " + site.Longitude / angleRatio + " grados";
                prompt += "\n\t" + "Nombre: " + site.PlaceName;
                prompt += "\n\t" + "Estación metereológica: " + site.WeatherStationName;

                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name SiteLocation");

                    //Cambiamos SiteLocation latitud, longitud y timeZone.
                    site.Latitude = 0.5;
                    site.Longitude = 0.5;
                    site.TimeZone = -5;
                    //Confirmamos Transaction
                    tx.Commit();
                }

                //Información actualizada. 
                prompt += "\n\t";
                prompt += "\n\t";
                prompt += "SiteLocation del proyecto actual después:";
                prompt += "\n\t" + "Latitud: " + site.Latitude / angleRatio + " grados";
                prompt += "\n\t" + "Longitud: " + site.Longitude / angleRatio + " grados";
                prompt += "\n\t" + "Nombre: " + site.PlaceName;
                prompt += "\n\t" + "Estación metereológica: " + site.WeatherStationName;

                TaskDialog.Show("Revit API Manual", prompt);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CREAR LINK
            try
            {

                //Obtenemos la carpeta actual del proyecto Master
                string folder = System.IO.Path.GetDirectoryName(doc.PathName);

                //Componemos path completo deLink
                string nameLink = folder + "\\Link.rvt";

                //Buscamos RevitLinkType coincidente
                FilteredElementCollector col = new FilteredElementCollector(doc);
                Element revitLinkType = col.OfClass(typeof(RevitLinkType)).Where(x => x.Name == "Link.rvt").FirstOrDefault();

                //Creamos ElementId para RevitLinkType
                ElementId revitLinkTypeId = ElementId.InvalidElementId;

                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction Name CrearLink");

                    //Si existe
                    if (revitLinkType != null)
                    {
                        TaskDialog.Show("Revit API Manual", "Ya está definido el RevitLinkType");
                        //Obtenemos ElementId
                        revitLinkTypeId = revitLinkType.Id;
                    }
                    else
                    {
                        #region Crear RevitLinkType
                        //Construimos FilePath
                        FilePath path = new FilePath(nameLink);
                        //Ruta asoluta
                        RevitLinkOptions options = new RevitLinkOptions(false);
                        //Creamos el RevitLinkType
                        LinkLoadResult result = RevitLinkType.Create(doc, path, options);
                        //Obtenemos ElementId
                        revitLinkTypeId = result.ElementId;
                        #endregion
                    }


                    #region 
                    //Creamos una RevitLinkInstance en el origen
                    RevitLinkInstance instance1 = RevitLinkInstance.Create(doc, revitLinkTypeId);

                    //Creamos otra RevitLinkInstance en el origen
                    RevitLinkInstance instance2 = RevitLinkInstance.Create(doc, revitLinkTypeId);

                    //Desplazamos la 2º RevitLinkInstance
                    Location location = instance2.Location;
                    location.Move(new XYZ(0, -100, 0));
                    #endregion

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region CARGAR Y DESCARGAR LINK
            try
            {

                //Buscanos RevitLinkType coincidente
                FilteredElementCollector col = new FilteredElementCollector(doc);
                RevitLinkType revitLinkType = col.OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().Where(x => x.Name == "Link.rvt").FirstOrDefault();

                //Creamos ElementId para RevitLinkType
                ElementId revitLinkTypeId = ElementId.InvalidElementId;

                //Si no existe
                if (revitLinkType == null)
                {
                    message = "No existe el RevitLinkType";
                    return Result.Failed;
                }
                else
                {
                    //Si está leido
                    if (revitLinkType.GetLinkedFileStatus() == LinkedFileStatus.Loaded)
                    {
                        revitLinkType.Unload(null);
                    }
                    //Si no está leido
                    else if (revitLinkType.GetLinkedFileStatus() == LinkedFileStatus.Unloaded)
                    {
                        revitLinkType.Load();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region DESPLAZAR LINK
            try
            {
                //Definimos Reference
                Reference reference;

                try
                {
                    //Seleccionamos un pilar estructural en Link
                    reference = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Seleccionar Pilar");
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Cancelled;
                }

                //Obtenemos RevitLinkInstance desde Reference
                RevitLinkInstance revitLinkInstance = doc.GetElement(reference.ElementId) as RevitLinkInstance;

                //Obtenemos Document de Link.rvt
                Document documentLink = revitLinkInstance.GetLinkDocument();

                //Obtenemos la Transforn de la RevitLinkInstance
                Transform transform = revitLinkInstance.GetTotalTransform();

                //Creamos XYZ para señalar
                XYZ xYZSeñalado = XYZ.Zero;

                try
                {
                    //Obteneos el XYZ, forzar a intersección. señalamos en Master.rvt
                    xYZSeñalado = uidoc.Selection.PickPoint(ObjectSnapTypes.Intersections, "Punto de inserción");
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Cancelled;
                }

                //Transformamos las coordenadas de Master a las de Link
                XYZ xYZSeñaladoTranformado = transform.Inverse.OfPoint(xYZSeñalado);

                //Obtenemos Path de Link
                string pathLink = documentLink.PathName;

                //Obtenemos el Pilar estructural
                Element pilarLink = documentLink.GetElement(reference.LinkedElementId);

                //LocationPoint del Pilar estructural
                XYZ locationPoint = (pilarLink.Location as LocationPoint).Point;

                //Obtenemos el RevitLinkType
                RevitLinkType revitLinkType = doc.GetElement(revitLinkInstance.GetTypeId()) as RevitLinkType;

                //Creamos un Document nul
                Document documentLinkAbierto = null;

                //Obtenemos el ElementId. Obtener antes de Unload
                ElementId elementIdPilar = pilarLink.Id;

                //Descargamos Link. 
                revitLinkType.Unload(null);

                //Leeos el Document del Link
                documentLinkAbierto = uiapp.Application.OpenDocumentFile(pathLink);

                //Definimos Transaction en documentLinkAbierto
                using (Transaction tx = new Transaction(documentLinkAbierto))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction DesplazarEnLink");

                    //Obtenemos vector
                    XYZ vectorDesplazamiento = xYZSeñaladoTranformado - locationPoint;

                    //Desplazamos pilar
                    ElementTransformUtils.MoveElement(documentLinkAbierto, elementIdPilar, vectorDesplazamiento);

                    //Confirmamos Transaction
                    tx.Commit();
                }

                //Cerramos Link y salvamos
                documentLinkAbierto.Close(true);

                //Releemos RevitLinkType
                revitLinkType.Load();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            // 16.0 ----------------------------------- CORRECCION DE ERRORES ------------------------------------

            #region SISTEMA DE CORRECCION DE ERRORES SEGUN REGLAS DADAS POR REVIT
            try
            {
                //Creamos string para salida
                string rules = string.Empty;

                //Iteramos por todas las reglas
                foreach (PerformanceAdviserRuleId id in PerformanceAdviser.GetPerformanceAdviser().GetAllRuleIds())
                {
                    //Obtenenos nombre de cada regla
                    string ruleName = PerformanceAdviser.GetPerformanceAdviser().GetRuleName(id);
                    rules = rules + ruleName + "\r";

                }

                //Obtenemos numero de reglas
                int numberRules = PerformanceAdviser.GetPerformanceAdviser().GetNumberOfRules();

                rules = rules + "\n\nNúmero total de reglas: " + numberRules;
                TaskDialog.Show("Revit API Manual", rules);

                //Ejecutamos todas las reglas
                IList<FailureMessage> failureMessages = PerformanceAdviser.GetPerformanceAdviser().ExecuteAllRules(doc);

                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction ReglasPredefinidas");

                    foreach (FailureMessage failureMessage in failureMessages)
                    {
                        //Obtenemos descripción
                        string temp = failureMessage.GetDescriptionText();

                        //Obtenemos ElementId involucrados
                        ICollection<ElementId> elementIds = failureMessage.GetFailingElements();

                        //Procesamos las fallas. Debe estar en una Transaction
                        doc.PostFailure(failureMessage);
                    }

                    //Confirmamos Transaction
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            #region SISTEMA DE CORRECCION DE ERRORES SEGUN REGLAS PERSONALIZADAS
            try
            {
                //Obtenemos numero de reglas
                int numeroReglas = PerformanceAdviser.GetPerformanceAdviser().GetNumberOfRules();

                //Ejecutamos solo la última
                IList<FailureMessage> failureMessages = PerformanceAdviser.GetPerformanceAdviser().ExecuteRules(doc, new List<int>() { numeroReglas - 1 });

                //Definimos Transaction
                using (Transaction tx = new Transaction(doc))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction ReglasPersonalizadas");

                    foreach (FailureMessage failureMessage in failureMessages)
                    {
                        //Procesamos las fallas. Debe estar en una Transaction
                        doc.PostFailure(failureMessage);
                    }
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            return Result.Succeeded; // Muestra que la clase tuvo exito en su ejecucion
        }

        // -------------------------------------------- FUNCIONES ADICIONALES ----------------------------------------------------

        #region CrearLineas()
        public ModelLine CrearLineas(UIDocument uidoc, SketchPlane sketchPlane, XYZ xYZ1, XYZ xYZ2)
        {
            Document doc = uidoc.Document;
            Line line1 = Line.CreateBound(xYZ1, xYZ2);
            return doc.Create.NewModelCurve(line1, sketchPlane) as ModelLine;
        }
        #endregion

        #region MostrarInfoRevit()
        private void MostrarInfoRevit(Application app)
        {
            TaskDialog dialog_CommandLink1 = new TaskDialog("Instalación de Revit");
            dialog_CommandLink1.MainInstruction =
                "Versión Revit. Nombre: " + app.VersionName + "\n"
                + "Versión Revit. Número: " + app.VersionNumber + "\n"
                + "Versión Revit. Compilación: " + app.VersionBuild;

            dialog_CommandLink1.Show();
        }
        #endregion

        #region MostrarInfoDoc()
        private void MostrarInfoDoc(Document doc)
        {
            TaskDialog.Show("Documento activo",
                    "Documento activo: " + doc.Title + "\n"
                    + "Nombre de vista activa: " + doc.ActiveView.Name);
        }
        #endregion

        #region AddNewRevision()
        private Revision AddNewRevision(Document document, string description, string issuedBy, string issuedTo)
        {
            //Creamos la Revision con los parametros indicados
            Revision newRevision = Revision.Create(document);
            newRevision.Description = description;
            newRevision.IssuedBy = issuedBy;
            newRevision.IssuedTo = issuedTo;
            /*newRevision.NumberType = RevisionNumberType.Alphanumeric*/; // REVISAR ACA
            newRevision.RevisionDate = DateTime.Now.ToShortDateString();
            return newRevision;
        }
        #endregion

        #region XYZString()
        private string XYZString(XYZ point)
        {
            return "(" + point.X + ", " + point.Y + ", " + point.Z + ")";
        }
        #endregion

        #region GetCenterline()
        private static Line GetCenterline(Wall wall)
        {
            //Creamos nuevas Options
            Options options = new Options();
            //Necesitamos obtener las referencias
            options.ComputeReferences = true;
            //Las lineas de eje de muro no son vicibles
            options.IncludeNonVisibleObjects = true;
            //Suponemos que la vista actual en Vista de plano.
            //Aplicamos esta vista a las Options
            options.View = wall.Document.ActiveView;
            //Obtenemos GeometryElement
            GeometryElement geoElem = wall.get_Geometry(options);
            //Iteramos buscandi Lines
            foreach (GeometryObject item in geoElem)
            {
                Line lineObj = item as Line;
                //De las dos lineas buscamos la que tiene Reference
                if (lineObj != null && lineObj.Reference != null)
                {
                    return lineObj;
                }
            }
            return null;
        }
        #endregion

        #region CrearSchema() ----- REVISAR "FieldBuilder"
        internal static void CrearSchema(Element element, Guid guidSchema)
        {

            Schema schema = Schema.Lookup(guidSchema);
            if (schema == null)
            {
                SchemaBuilder schemaBuilder = new SchemaBuilder(guidSchema);
                schemaBuilder.SetVendorId("FDeA");
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);//Establecemos acceso
                schemaBuilder.SetSchemaName("TestFilterExtensibleStorage");
                schemaBuilder.SetDocumentation("Empleado para test del filtro ExtensibleStorage");
                //Creanmos el campo para almacenar string
                /*
                FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField("TestFilterValue1", typeof(string));
                schema = schemaBuilder.Finish();
                */
            }
            Entity entity = new Entity(schema);
            entity.Set<string>("TestFilterValue1", "Poner un valor");
            if (element != null) element.SetEntity(entity);
        }
        #endregion
    }

    // -------------------------------------------- CLASES ADICIONALES ----------------------------------------------------

    #region WallSelectionFilterEtiquetas
    public class WallSelectionFilterEtiquetas : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            //Solo admitimos la clase Wall
            if (element is Wall)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    #endregion

    #region RoofSelectionFilter
    public class RoofSelectionFilter : ISelectionFilter
    {
        Document document = null;
        public RoofSelectionFilter(Document document)
        {
            this.document = document;
        }
        public bool AllowElement(Element element)
        {
            if (element is RoofBase)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            Element element = document.GetElement(refer.ElementId);
            if (element is RoofBase) return true;

            return false;
        }
    }
    #endregion

    #region FlippedWindowCheck
    public class FlippedWindowCheck : IPerformanceAdviserRule
    {
        private List<ElementId> m_FlippedWindows;

        private string m_name;

        private string m_description;

        public PerformanceAdviserRuleId m_Id = new PerformanceAdviserRuleId(new Guid("BC38854474284491BD03795675AC7386"));

        private FailureDefinitionId m_windowWarningId;

        private FailureDefinition m_windowWarning;

        public FlippedWindowCheck()
        {
            //Constructor. Completamos propiedades
            m_name = "Comprobación. Ventanas volteadas";
            m_description = "Regla para buscar cualquier ventana que esté volteada";
            m_windowWarningId = new FailureDefinitionId(new Guid("25570B8FD4AD42baBD78469ED60FB9A3"));
            m_windowWarning = FailureDefinition.CreateFailureDefinition(m_windowWarningId, FailureSeverity.Warning, "Algunas ventanas de este proyecto están volteadas.");
        }

        public void InitCheck(Document document)
        {
            //Creamos o reiniciamos la colección de ElementId
            if (m_FlippedWindows == null) m_FlippedWindows = new List<ElementId>();
            else m_FlippedWindows.Clear();
            return;
        }

        public void ExecuteElementCheck(Document document, Element element)
        {
            //Si es FamilyInstance
            if ((element is FamilyInstance familyInstance))
            {
                //Sis es FacingFlipped lo añadimos a la lista
                if (familyInstance.FacingFlipped) m_FlippedWindows.Add(familyInstance.Id);
            }
        }

        public void FinalizeCheck(Document document)
        {
            //Si hay ElementId en la coleccion 
            if (m_FlippedWindows.Count > 0)
            {
                //Creamos FailureMessage
                FailureMessage failureMessage = new FailureMessage(m_windowWarningId);
                //Asignamos lista de ElementId con falla
                failureMessage.SetFailingElements(m_FlippedWindows);

                //Definimos Transaction
                using (Transaction tx = new Transaction(document))
                {
                    //Iniciamos Transaction
                    tx.Start("Transaction ReglasPersonalizadas.Informe");

                    PerformanceAdviser.GetPerformanceAdviser().PostWarning(failureMessage);

                    //Confirmamos Transaction
                    tx.Commit();
                }
                m_FlippedWindows.Clear();
            }
        }

        public string GetDescription()
        {
            return m_description;
        }

        public ElementFilter GetElementFilter(Document document)
        {
            return new ElementCategoryFilter(BuiltInCategory.OST_Windows);
        }

        public string GetName()
        {
            return m_name;
        }

        public bool WillCheckElements()
        {
            return true;
        }
    }
    #endregion

    #region MyGraphicsService
    public class MyGraphicsService : ITemporaryGraphicsHandler
    {
        //Propiedad guid
        Guid _guid;
        //Propiedad ElemenId
        ElementId _elementId;
        //Constructor explicito
        public MyGraphicsService(Guid guid, ElementId elementId)
        {
            _guid = guid;
            _elementId = elementId;
        }
        public void OnClick(TemporaryGraphicsCommandData data)
        {
            //Npmbre completo del actual fichero GraficosTemp.dll
            string filename = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //Carpeta del actual GraficosTemp.dll
            string folder = (new System.IO.FileInfo(filename)).Directory.FullName;
            //Nombre completo del archivo csv
            string path = folder + "\\excel.csv";

            //Escritura del id en el fichero csv
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(_elementId);
            }

            //Arrancamos excel con el csv
            Process pr = new Process();
            pr.StartInfo = new ProcessStartInfo()
            {
                FileName = "excel.exe",
                Arguments = path
            };
            pr.Start();

        }
        public string GetName()
        { return "Vinculo a excel"; }
        public string GetDescription()
        { return "Graphics service. Excel"; }
        public string GetVendorId()
        { return "FdAA"; }
        public ExternalServiceId GetServiceId()
        { return ExternalServices.BuiltInExternalServices.TemporaryGraphicsHandlerService; }
        public Guid GetServerId()
        {
            return _guid;
        }
    }
    #endregion

    #region BeamSeleccionFilter
    public class BeamSeleccionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            //Filtramos a Armazon estructural
            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
            {
                return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ xyz)
        {
            return false;
        }
    }
    #endregion

    #region OpcionesCargaFamiliasMin
    class OpcionesCargaFamiliasMin : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            //Sobreescribimos los parametros existentes, si ya esta leida
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            //Devolvemos la Family recien cargada
            source = FamilySource.Family;
            //Sobreescribimos los parametros existentes, si ya esta leida
            overwriteParameterValues = true;
            return true;
        }

    }
    #endregion

    #region FiltroDeSeleccionDeMuros_1
    public class FiltroDeSeleccionDeMuros_1 : ISelectionFilter
    // Este filtro tendra la funcion de permitir la seleccion de muros que midan entre 3 y 8 metros.
    {
        public bool AllowElement(Element elemento)
        {
            // Verificamos si el elemento es un muro
            if (elemento is Wall muro)
            {
                // Intentamos obtener el parámetro de altura del muro
                Parameter alturaParametro = muro.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

                if (alturaParametro != null && alturaParametro.HasValue)
                {
                    // Convertimos la altura a metros
                    double alturaMuro = alturaParametro.AsDouble();
                    alturaMuro = UnitUtils.ConvertFromInternalUnits(alturaMuro, UnitTypeId.Meters);

                    // Devolvemos true si la altura está entre 3 y 8 metros
                    if (alturaMuro >= 3 && alturaMuro <= 8)
                    {
                        return true;
                    }
                }
            }
            // Si el elemento no es un muro o no cumple las condiciones, devolvemos false
            return false;
        }

        public bool AllowReference(Reference referencia, XYZ posicion)
        {
            // No trabajamos con referencias todavía, así que retornamos false.
            return false;
        }
    }
    #endregion

    #region FiltroDeSeleccionDeCarasPlanas_1

    public class FiltroDeSeleccionDeCarasPlanas_1 : ISelectionFilter
    // Este filtro tendra la funcion de permitir la seleccion de muros que midan entre 3 y 8 metros.
    {
        Document doc = null;
        public FiltroDeSeleccionDeCarasPlanas_1(Document doc)
        {
            this.doc = doc;
        }
        public bool AllowElement(Element elemento)
        {
            // No trabajamos con Element, retornamos siempre true.
            return true;
        }

        public bool AllowReference(Reference referencia, XYZ posicion)
        {
            
            Element elemento = doc.GetElement(referencia.ElementId);
            GeometryObject GeometriaDelObjeto = elemento.GetGeometryObjectFromReference(referencia);
            PlanarFace CaraPlana = GeometriaDelObjeto as PlanarFace;
            if (CaraPlana != null)
            {
                return true;
            }

            return false;
        }
    }
    #endregion

    #region FiltroDeSeleccionDeCategoriaMultiple
    public class FiltroDeSeleccionDeCategoriaMultiple : ISelectionFilter
    {
        private readonly List<string> categorias;

        public FiltroDeSeleccionDeCategoriaMultiple(params string[] categorias)
        {
            this.categorias = categorias.ToList();
        }

        public bool AllowElement(Element element)
        {
            return categorias.Contains(element.Category.Name);
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }
    #endregion

    #region FiltroDeSeleccionDeCategoriaMultiple_DiscriminacionDeMuros
    public class FiltroDeSeleccionDeCategoriaMultiple_DiscriminacionDeMuros : ISelectionFilter
    {
        private readonly string[] categorias;

        public FiltroDeSeleccionDeCategoriaMultiple_DiscriminacionDeMuros(params string[] categorias)
        {
            this.categorias = categorias;
        }

        public bool AllowElement(Element element)
        {
            if (element.Category.Name == "Walls")
            {
                Parameter alturaElementoParam = element.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

                // Permite seleccionar solo muros que tienen alturas conectadas a un nivel (propiedad de solo lectura)
                if (alturaElementoParam != null && alturaElementoParam.IsReadOnly)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (categorias.Contains(element.Category.Name))
            {
                return true;
            }

            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }
    #endregion

}

