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
            /* UIApplication es una clase que proporciona acceso a la aplicaci�n de Revit en ejecuci�n.
            A trav�s de ella puedes interactuar con la interfaz gr�fica de usuario y los documentos abiertos.
            commandData.Application contiene la instancia de la aplicaci�n actual de Revit que se pas� al comando.
            Este objeto encapsula toda la sesi�n de Revit, permitiendo que el comando interact�e con el entorno de Revit. */

            UIDocument uidoc = uiapp.ActiveUIDocument;
            /* UIDocument es una clase que proporciona acceso al documento activo que el usuario tiene abierto en Revit.
            Esto incluye elementos como la selecci�n actual del usuario y permite modificar el documento.
            uiapp.ActiveUIDocument devuelve el documento que est� activo en la ventana actual de Revit.
            A trav�s de este, puedes acceder a los elementos del proyecto que se encuentra abierto. */

            Application app = uiapp.Application;
            /* Application es una clase m�s baja en la jerarqu�a que UIApplication y proporciona acceso a funcionalidades del sistema,
            como la configuraci�n general de la aplicaci�n o las versiones de la API.
            Aqu� se est� almacenando en la variable app la aplicaci�n subyacente de Revit desde uiapp.Application. */

            Document doc = uidoc.Document;
            /* Document es una clase que representa el modelo de Revit (el archivo de proyecto que el usuario est� editando).
            Esta clase contiene toda la informaci�n del proyecto, incluyendo elementos del modelo, par�metros, vistas, etc.
            uidoc.Document devuelve el documento activo que est� siendo editado por el usuario en la interfaz gr�fica de Revit.
            A trav�s de doc, puedes acceder y modificar los elementos dentro del archivo de proyecto. */

            // ESTRUCTURA TRY CATCH BASICO.
            try
            {

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

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
            //En la API de Revit, una ICollection<T> es una interfaz que representa una colecci�n de objetos del tipo T.
            //Es similar a una lista o un conjunto, pero con una interfaz m�s b�sica,
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
                // Se agregan las l�neas al CurveLoop

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // SELECCION PUNTUAL CON PickObject
            try
            {
                // Seleccionamos un elemento completo en el modelo.
                // Lo que obtenemos aqu� es una "Reference" que apunta a un "Element".
                Reference ReferenciaPuntual_1 = uidoc.Selection.PickObject(ObjectType.Element, "Selecciona un elemento");

                // Seleccionamos una arista (Edge) de un elemento en el modelo.
                // Lo que obtenemos aqu� tambi�n es una "Reference", pero esta vez la referencia apunta a una arista espec�fica del elemento.
                Reference ReferenciaPuntual_2 = uidoc.Selection.PickObject(ObjectType.Edge, "Selecciona una arista");

                // Podemos obtener el elemento completo usando la "Reference" obtenida de la selecci�n de un "Element".
                // Hay dos maneras de hacerlo: usando el ElementId o directamente la Reference.

                // Opci�n 1: Obtener el Element usando el ElementId de la referencia.
                Element ElementoDesdeLaReferencia_1 = doc.GetElement(ReferenciaPuntual_1.ElementId);

                // Opci�n 2: Obtener el Element directamente usando la Reference (internamente usa el ElementId).
                Element ElementoDesdeLaReferencia_2 = doc.GetElement(ReferenciaPuntual_1);

                // Ambas l�neas anteriores devolver�n el mismo Element, ya que Reference.ElementId apunta al mismo elemento.

                // Cuando seleccionamos una arista, la referencia apunta a una parte espec�fica del elemento.
                // Podemos obtener el elemento completo desde la referencia de la arista.
                Element ElementoDesdeLaArista = doc.GetElement(ReferenciaPuntual_2.ElementId);

                // Obtener el objeto geom�trico (GeometryObject) desde la referencia.
                // En este caso, obtenemos la arista (Edge) a la que hace referencia.
                GeometryObject ObjetoGeometrico_1 = ElementoDesdeLaReferencia_1.GetGeometryObjectFromReference(ReferenciaPuntual_2);

                // Convertimos el GeometryObject a una arista (Edge), que es lo que seleccionamos.
                Edge Element_Borde_1 = ObjetoGeometrico_1 as Edge;

                // Ahora tenemos acceso a la arista espec�fica del elemento, a trav�s de la referencia.
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            // SELECCIONAMOS VARIOS ELEMENTOS - USO DE FUNCION FLECHA.
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

            // SELECCION CON FILTROS - El filtro esta en una clase nueva llamada "FiltroDeSeleccionDeMuros_1"
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

            // SELECCION CON FILTROS - Filtramos caras planas y mostramos su nombre y area.
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

            // SELECCION POR RECTANGULO "PickElementsByRectangle"
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

            // SELECCIONAR ELEMENTOS CON PickObjects Y FILTRO - Permite seleccionar por rectangulo
            try
            {
                ISelectionFilter selectionFilter = new FiltroDeSeleccionDeMuros_1();
                IList<Reference> referencesPre = new List<Reference>();
                List<ElementId> elementIds = new List<ElementId>();  // Inicializamos como lista vac�a

                // Nueva selecci�n de objetos. Partimos de preselcci�n anterior
                IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element,
                    selectionFilter, "Seleccionar elementos");

                // Necesitamos iDs. MODO LARGO
                foreach (Reference referenceTotal in references)
                {
                    elementIds.Add(referenceTotal.ElementId);
                }

                // Mostramos en pantalla la selecci�n actual
                uidoc.ShowElements(elementIds);

                // Incorporamos los IDs a selecci�n actual
                uidoc.Selection.SetElementIds(elementIds);

            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;  // Aseguramos que se devuelva el resultado fallido en caso de excepci�n
            }




            // AGREGAR UNA A UNA SELECCION PREVIA UNA NUEVA SELECCION
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

                //Nueva selecci�n de objetos. Partimos de preselcci�n anterior
                IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element,
                    selectionFilter, "Seleccionar elementos", referencesPre);
                // Podemos eliminar la selacci�n actual
                elementIds.Clear();
                // Necesitamos iDs. MODO LARGO
                foreach (Reference referenceTotal in references)
                {
                    elementIds.Add(referenceTotal.ElementId);
                }

                // Mostramos en pantalla la selecci�n actual
                uidoc.ShowElements(elementIds);

                // Incorporamos los IDs a selecci�n actual
                uidoc.Selection.SetElementIds(elementIds);
                return Result.Succeeded;
            }
            catch( Exception ex)
            {
                message = ex.ToString();
            }

            // ----------------------------------- FILTROS ------------------------------------

            // FILTROS RAPIDOS
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

            try
            {
                // Filtro que toma varios tipos diferentes
                List<Type> ListaElementType = new List<Type>() { typeof(Wall),  typeof(FamilyInstance) };



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
            try
            {

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

                // Crear las l�neas del rect�ngulo
                Line Linea_1 = Line.CreateBound(PuntoPB_1, PuntoPB_2);
                Line Linea_2 = Line.CreateBound(PuntoPB_2, PuntoPB_3);
                Line Linea_3 = Line.CreateBound(PuntoPB_3, PuntoPB_4);
                Line Linea_4 = Line.CreateBound(PuntoPB_4, PuntoPB_1);

                // Agregar las l�neas al CurveLoop
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

                // Iniciar la transacci�n para crear el suelo
                using (Transaction Transaccion_Suelos_1 = new Transaction(doc, "Crear suelos desde PickBox"))
                {
                    Transaccion_Suelos_1.Start();

                    // Crear el suelo
                    Floor.Create(doc, Lista_CurveLoop_1, Suelo_PorDefecto_1.Id, Nivel_1.Id);

                    // Confirmar la transacci�n
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

    public class FiltroDeSeleccionDeMuros_1 : ISelectionFilter
    // Este filtro tendra la funcion de permitir la seleccion de muros que midan entre 3 y 8 metros.
    {
        public bool AllowElement(Element elemento)
        {
            // Verificamos si el elemento es un muro
            if (elemento is Wall muro)
            {
                // Intentamos obtener el par�metro de altura del muro
                Parameter alturaParametro = muro.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);

                if (alturaParametro != null && alturaParametro.HasValue)
                {
                    // Convertimos la altura a metros
                    double alturaMuro = alturaParametro.AsDouble();
                    alturaMuro = UnitUtils.ConvertFromInternalUnits(alturaMuro, UnitTypeId.Meters);

                    // Devolvemos true si la altura est� entre 3 y 8 metros
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
            // No trabajamos con referencias todav�a, as� que retornamos false.
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
