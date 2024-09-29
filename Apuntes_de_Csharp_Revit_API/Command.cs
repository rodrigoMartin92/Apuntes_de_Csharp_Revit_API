
/* -------------------------------------------- INDICE --------------------------------------------
    
    
    1.1 VARIABLES INICIALES
    1.2 TRABAJO CON VARIABLES "elements" y "message"
    1.3 TRABAJANDO CON TRANSACCION
    1.4 TRABAJANDO "JournalingMode.UsingCommandData"
    1.5 OBTENCIOND DE ELEMENTOS Y SUS Ids

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


 */




#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
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

            // 1.1 -------------------------------------------- VARIABLES INICIALES ----------------------------------------------------

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

            // ESTRUCTURA TRY CATCH BASICO.
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // 1.2 -------------------------------------- TRABAJO CON VARIABLES "elements" y "message" -----------------------------------

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

            // 1.3 ---------------------------------------------- TRABAJANDO CON TRANSACCION --------------------------------------------------

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

            // 1.4 ----------------------------------- TRABAJANDO "JournalingMode.UsingCommandData" ------------------------------------

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

            // 1.5 ----------------------------------- OBTENCIOND DE ELEMENTOS Y SUS Ids ------------------------------------

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


            // 2.0 ----------------------------------- SELECCION EN EL MODELO DE REVIT ------------------------------------

            // 2.1 SELECCION LIBRE DEL PUNTO
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


            // 2.2 SELECCION CONDICIONADA DEL PUNTO
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

            // 2.3 SELECCION POR RECTANGULO "TODO INCLUIDO" y "TODO LO INCLUIDO Y CORTADO"
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

            // 2.4 CREACION DE UN CurveArray A TRAVES DE UN PickBox
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

            // 2.5 SELECCION PUNTUAL CON PickObject
            try
            {
                // Seleccionamos un elemento completo en el modelo.
                // Lo que obtenemos aquí es una "Reference" que apunta a un "Element".
                Reference ReferenciaPuntual_1 = uidoc.Selection.PickObject(ObjectType.Element, "Selecciona un elemento");

                // Seleccionamos una arista (Edge) de un elemento en el modelo.
                // Lo que obtenemos aquí también es una "Reference", pero esta vez la referencia apunta a una arista específica del elemento.
                Reference ReferenciaPuntual_2 = uidoc.Selection.PickObject(ObjectType.Edge, "Selecciona una arista");

                // Podemos obtener el elemento completo usando la "Reference" obtenida de la selección de un "Element".
                // Hay dos maneras de hacerlo: usando el ElementId o directamente la Reference.

                // Opción 1: Obtener el Element usando el ElementId de la referencia.
                Element ElementoDesdeLaReferencia_1 = doc.GetElement(ReferenciaPuntual_1.ElementId);

                // Opción 2: Obtener el Element directamente usando la Reference (internamente usa el ElementId).
                Element ElementoDesdeLaReferencia_2 = doc.GetElement(ReferenciaPuntual_1);

                // Ambas líneas anteriores devolverán el mismo Element, ya que Reference.ElementId apunta al mismo elemento.

                // Cuando seleccionamos una arista, la referencia apunta a una parte específica del elemento.
                // Podemos obtener el elemento completo desde la referencia de la arista.
                Element ElementoDesdeLaArista = doc.GetElement(ReferenciaPuntual_2.ElementId);

                // Obtener el objeto geométrico (GeometryObject) desde la referencia.
                // En este caso, obtenemos la arista (Edge) a la que hace referencia.
                GeometryObject ObjetoGeometrico_1 = ElementoDesdeLaReferencia_1.GetGeometryObjectFromReference(ReferenciaPuntual_2);

                // Convertimos el GeometryObject a una arista (Edge), que es lo que seleccionamos.
                Edge Element_Borde_1 = ObjetoGeometrico_1 as Edge;

                // Ahora tenemos acceso a la arista específica del elemento, a través de la referencia.
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // 2.6 SELECCIONAMOS VARIOS ELEMENTOS - USO DE FUNCION FLECHA.
            try
            {
                IList<Reference> Referencias_1 = uidoc.Selection.PickObjects(ObjectType.Element, "Selecciona varios elementos");
                // Obtenemos una lista de referencias, pero estas referencias son a Elements.

                // Primera manera de agregar los Element a una lista, sin funcion flecha.
                List<string> ListaElements_1 = new List<string>(); // Inicializar la lista de strings
                foreach (Reference Referencia in Referencias_1)
                {
                    // Obtenemos el Element a partir de la referencia y accedemos a su nombre
                    Element elemento = doc.GetElement(Referencia.ElementId);

                    // Agregamos el nombre del elemento a la lista
                    ListaElements_1.Add(elemento.Name);
                }

                // Segunda manera de agregar los Element a una lista, con funcion flecha.
                List<string> ListaElements_2 = Referencias_1.Select(Referencia => doc.GetElement(Referencia.ElementId).Name).ToList();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // 2.7 SELECCION CON FILTROS - El filtro esta en una clase nueva llamada "FiltroDeSeleccionDeMuros_1"
            try
            {
                // Creamos una instancia de filtro usando una clase de filtro ya creada
                ISelectionFilter FiltroDeSeleccionDeMuro_1 = new FiltroDeSeleccionDeMuros_1();

                // Seleccionamos un muro usando la instancia del filtro creada, y obtenemos la referencia del mismo.
                Reference SeleccionDeMuroFiltrado_1 = uidoc.Selection.
                    PickObject(ObjectType.Element, FiltroDeSeleccionDeMuro_1, "Seleccione un muro");
                // Obtenemos el Element desde la referencia
                Element ElementoSeleccionado_1 = doc.GetElement(SeleccionDeMuroFiltrado_1.ElementId);

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
            }
            catch (Exception ex) 
            { 
                message = ex.ToString(); 
            }

            // 2.8 SELECCION CON FILTROS - Filtramos caras planas y mostramos su nombre y area.
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
            catch(Exception ex)
            {
                message = ex.ToString();
            }

            // 2.9 SELECCION POR RECTANGULO "PickElementsByRectangle"
            try
            {
                // Creamos una instancia de filtro usando una clase de filtro ya creada
                ISelectionFilter FiltroDeSeleccionDeMuro_1 = new FiltroDeSeleccionDeMuros_1();

                // Seleccionamos varios muros usando la instancia del filtro creado, y los colocamos en una lista de Element.
                IList<Element> SeleccionDeMurosFiltrados_2 = uidoc.Selection.
                    PickElementsByRectangle(FiltroDeSeleccionDeMuro_1, "Seleccione muros");
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // 2.10 SELECCIONAR ELEMENTOS CON PickObjects Y FILTRO - Permite seleccionar por rectangulo
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




            // 2.11 AGREGAR UNA A UNA SELECCION PREVIA UNA NUEVA SELECCION
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
            catch( Exception ex)
            {
                message = ex.ToString();
            }

            // 3.0 ----------------------------------- FILTROS ------------------------------------

            // 3.1 FILTROS RAPIDOS
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

            try
            {
                // Elementos cortados o incluidos en una BuildingBox
                double ValorCreciente = 10;
                Outline Outline_1 = new Outline(new XYZ (0,0,0), new XYZ(ValorCreciente,ValorCreciente, ValorCreciente));

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

            // FILTRO MULTICATEGORIA
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

            // Filtro que excluye los elementos seleccionados
            try
            {
                // Seleccionamos elementos con una seleccion por rectangulo.
                IList<Element> ElementosSeleccionados = uidoc.Selection.PickElementsByRectangle("Selecciona elementos");
                ICollection<ElementId> ElementosSeleccionados_Ids = ElementosSeleccionados.Select(Elementos => Elementos.Id).ToList();
                ExclusionFilter FiltroDeExclucion = new ExclusionFilter(ElementosSeleccionados_Ids);
                
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

            // CREACION DE ESQUEMAS Y FILTRO QUE BUSCA DETERMINADOS ESQUEMAS
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

            // 3.2 FILTROS LENTOS
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




            // 4.0 ----------------------------------- CREACION DE ELEMENTOS ------------------------------------

            // 4.1 CREACION DE UN MURO

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

            // 4.2 CREACION DE UN SUELO.

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

            // 4.3 MODIFICACION DEL SUELO - Video RAPI 8.3
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

            // 4.4 CREACION DE PLATAFORMAS DE NIVELACION.
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

            // CREACION DE CRISTALERAS
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

            // BORRADO DE ELEMENTOS
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

            // EDICION DE ELEMENTOS.

            // CAMBIAR UBICACION DEL ELEMENTO.
            // En este caso se cambia la ubicacion de una columna cuya ubicacion esta basada en un punto
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

            // Ahora cambiamos la ubicacion del "LocationPoint"
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

            // Movemos en este caso la "LocationCurve"
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

            // En este caso creamos una linea y hacemos que la location curve sea igual que la linea creada, es otro metodo diferente.
            // Podemos usar este metodo para torcer un poco las cosas, creando location curves inclinadas en Z,
            // pero el resultado es inestable.
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


            // Copiar elementos
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

            // ROTAR ELEMENTOS
            // Rotamos elementos segun un eje creado con un "CreateUnbound". En este caso el eje pasa por el centro de la viga.
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

            // ROTAR ELEMENTO SEGUN UN PLANO INCLINADO
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

            // SIMETRIA A UN PLANO
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

            // CREACION DE MATRICES
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

            // TRANSFORMAR ELEMENTOS
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

            // TRANSFORMAR DESDE UNA VISTA 3D
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

            // ALINEAR ELEMENTOS
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






            #region FAMILIAS
            // CARGAR UN SYMBOL EN ESPECIFICO -  RAPI 8-10
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





            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------
            // -----------------------------------  ------------------------------------



            return Result.Succeeded; // Muestra que la clase tuvo exito en su ejecucion
        }

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
                FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField("TestFilterValue1", typeof(string));
                schema = schemaBuilder.Finish();
            }
            Entity entity = new Entity(schema);
            entity.Set<string>("TestFilterValue1", "Poner un valor");
            if (element != null) element.SetEntity(entity);
        }

    }

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
}
