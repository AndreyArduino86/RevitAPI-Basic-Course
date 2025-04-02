//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Mechanical;
//using Autodesk.Revit.DB.Plumbing;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RevitAPI_Basic_Course
//{
//    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
//    public class TwoElems : IExternalEventHandler
//    {
//        public static ElementId pipeDocId = new ElementId(0);
//        public static ElementId pipeId = new ElementId(0);

//        public static ElementId ductDocId = new ElementId(0);
//        public static ElementId ductId = new ElementId(0);

//        public static ElementId otherDocId = new ElementId(0);
//        public static ElementId otherId = new ElementId(0);
//        public void Execute(UIApplication uiapp)
//        {
//            // Get the handle of current document.
//            //UIDocument uidoc = commandData.Application.ActiveUIDocument;
//            UIDocument uidoc = uiapp.ActiveUIDocument;
//            Document doc = uiapp.ActiveUIDocument.Document;
//            Document linkDoc = ExClass.linkS[FormNew.comboValue];
//            Element linkElem = null;
//            Element pipeEl = null;
//            Reference pipeRef = null;

//            Element ductEl = null;
//            Reference ductRef = null;

//            try
//            {
//                using (Transaction _transaction_ = new Transaction(doc))
//                {
//                    //Selection selection = uidoc.Selection;
//                    if (!FormNew.radPipeMod)
//                    {
//                        pipeRef = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Select pipe in link");
//                        pipeEl = GetLinkedElem(doc, pipeRef);
//                        pipeDocId = pipeRef.ElementId;
//                    }
//                    else
//                    {
//                        // Get the element selection of current document.
//                        pipeRef = uidoc.Selection.PickObject(ObjectType.Element, "Select pipe");
//                        pipeEl = doc.GetElement(pipeRef);
//                        //pipeDocId = pipeRef.ElementId;
//                    }
//                    pipeId = pipeEl.Id;

//                    //Selection selection = uidoc.Selection;
//                    if (linkDoc.IsLinked)
//                    {
//                        Reference otherRef = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Select second object");
//                        linkElem = GetLinkedElem(doc, otherRef);
//                        otherDocId = otherRef.ElementId;
//                    }
//                    else
//                    {
//                        Reference otherRef = uidoc.Selection.PickObject(ObjectType.Element, "Select second object");
//                        linkElem = doc.GetElement(otherRef);
//                        //otherDocId = pipeRef.ElementId;
//                    }
//                    otherId = linkElem.Id;



//                    _transaction_.Start("Place openings");

//                    //FamilySymbol famSymb = GetFamilySymb(doc);
//                    //PlaceOpeningFamily(doc, intersection, famSymb, pipeEl);


//                    //XYZ ePoint2 = i
//                    List<XYZ> pointList = GetIntersectPoints(pipeEl, linkElem);
//                    XYZ sPoint2 = pointList.First();
//                    XYZ ePoint2 = pointList.Last();
//                    if (!FormNew.radPipeMod)
//                    {
//                        RevitLinkInstance revLinkElem = doc.GetElement(pipeRef.ElementId) as RevitLinkInstance;
//                        sPoint2 = CheckForTransform(doc, revLinkElem, sPoint2);
//                        ePoint2 = CheckForTransform(doc, revLinkElem, ePoint2);
//                    }
//                    //Pipe.Create(doc, systemTypeId, pipeTypeId, levelId, axizY2.GetEndPoint(0), axizY2.GetEndPoint(1));

//                    if (pipeEl.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_DuctCurves).Id) && pipeEl.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() != "Round Duct")
//                    {
//                        Duct pipeOpen = TwoElems.CreateDuctByTwoPoints(doc, sPoint2, ePoint2);
//                        pipeOpen.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM)
//                            .Set(pipeEl.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).AsDouble() + FormNew.dist);
//                        pipeOpen.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM)
//                            .Set(pipeEl.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).AsDouble() + FormNew.dist);
//                        pipeOpen.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
//                            .Set(pipeEl.Name + " " + pipeEl.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString());
//                        pipeOpen.get_Parameter(BuiltInParameter.DOOR_NUMBER)
//                           .Set(pipeId.ToString() + "." + pipeDocId.ToString() + "." + otherId.ToString() + "." + otherDocId.ToString());
//                        //pipeOpen.LookupParameter("Diameter").Set(pipeEl.LookupParameter("Diameter").AsDouble() + FormNew.dist); 
//                    }

//                    else if (pipeEl.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_DuctCurves).Id))
//                    {
//                        Duct pipeOpen = TwoElems.CreateRoundDuctByTwoPoints(doc, sPoint2, ePoint2);
//                        pipeOpen.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM)
//                            .Set(pipeEl.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM).AsDouble() + FormNew.dist);
//                        pipeOpen.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
//                            .Set(pipeEl.Name + " " + pipeEl.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString());
//                        pipeOpen.get_Parameter(BuiltInParameter.DOOR_NUMBER)
//                           .Set(pipeId.ToString() + "." + pipeDocId.ToString() + "." + otherId.ToString() + "." + otherDocId.ToString());
//                        //pipeOpen.LookupParameter("Diameter").Set(pipeEl.LookupParameter("Diameter").AsDouble() + FormNew.dist); 
//                    }

//                    else if (pipeEl.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves).Id))
//                    {
//                        Pipe pipeOpen = CreatePipeByTwoPoints(doc, sPoint2, ePoint2);
//                        pipeOpen.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)
//                            .Set(pipeEl.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble() + FormNew.dist);
//                        pipeOpen.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
//                            .Set(pipeEl.Name + " " + pipeEl.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString());
//                        pipeOpen.get_Parameter(BuiltInParameter.DOOR_NUMBER)
//                           .Set(pipeId.ToString() + "." + pipeDocId.ToString() + "." + otherId.ToString() + "." + otherDocId.ToString());
//                        //pipeOpen.LookupParameter("Diameter").Set(pipeEl.LookupParameter("Diameter").AsDouble() + FormNew.dist); 
//                    }
//                    else if (pipeEl.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Conduit).Id))
//                    {
//                        Pipe pipeOpen = CreatePipeByTwoPoints(doc, sPoint2, ePoint2);
//                        pipeOpen.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)
//                            .Set(pipeEl.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).AsDouble() + FormNew.dist);
//                        pipeOpen.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
//                            .Set(pipeEl.Name + " " + pipeEl.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString());
//                        pipeOpen.get_Parameter(BuiltInParameter.DOOR_NUMBER)
//                           .Set(pipeId.ToString() + "." + pipeDocId.ToString() + "." + otherId.ToString() + "." + otherDocId.ToString());
//                    }

//                    //pipeOpen.LookupParameter("Length").Set(intersection.GetCurveSegment(0).Length + FormNew.ladge);

//                    _transaction_.Commit();

//                }
//            }
//            catch { }

//            //return Result.Succeeded;
//        }
//        public string GetName()
//        {
//            return "R External Event Sample";
//        }

//        public static List<XYZ> GetIntersectPoints(Element pipeEl, Element linkElem)
//        {
//            SolidCurveIntersectionOptions solidCurveIntersectionOptions = new SolidCurveIntersectionOptions();
//            //Get geometry
//            Solid pipeSolid = null;
//            Solid otherSolid = null;
//            Curve pipeCur = null;
//            Options gOptions = new Options();
//            gOptions.DetailLevel = ViewDetailLevel.Fine;

//            Location pipeLoc = pipeEl.Location;
//            LocationCurve pipeLocCur = pipeLoc as LocationCurve;
//            pipeCur = pipeLocCur.Curve;

//            GeometryElement geom = pipeEl.get_Geometry(gOptions);
//            //Traverce Geometry
//            foreach (GeometryObject gPipe in geom)
//            {
//                pipeSolid = gPipe as Solid;
//            }

//            SolidCurveIntersection intersection = null;
//            //GeometryElement 
//            GeometryElement otherGeom = linkElem.get_Geometry(gOptions);
//            foreach (GeometryObject gOther in otherGeom)
//            {
//                GeometryInstance gInst = gOther as GeometryInstance;

//                if (gInst != null)
//                {
//                    GeometryElement gEle = gInst.GetInstanceGeometry();
//                    foreach (GeometryObject gO in gEle)
//                    {
//                        //otherSolid = gO as Solid;
//                        try
//                        {
//                            otherSolid = gO as Solid;
//                            intersection = otherSolid.IntersectWithCurve(pipeCur, solidCurveIntersectionOptions);
//                            if (intersection.SegmentCount > 0)
//                            {
//                                break;
//                            }
//                        }
//                        catch
//                        {
//                            continue;
//                        }
//                    }
//                }
//                else
//                {
//                    otherSolid = gOther as Solid;
//                    if (otherSolid.IntersectWithCurve(pipeCur, solidCurveIntersectionOptions).Count() > 0)
//                        intersection = otherSolid.IntersectWithCurve(pipeCur, solidCurveIntersectionOptions);
//                }
//            }

//            XYZ startPoint = intersection.GetCurveSegment(0).GetEndPoint(0);
//            XYZ endPoint = intersection.GetCurveSegment(0).GetEndPoint(1);
//            XYZ interP = (startPoint + endPoint) * 0.5;
//            XYZ pipeDir = (endPoint - startPoint).Normalize().Multiply(FormNew.ladge);//FormNew.ladge);
//            XYZ sPoint2 = startPoint.Add(pipeDir.Negate());
//            XYZ ePoint2 = endPoint.Add(pipeDir);
//            List<XYZ> pointList = new List<XYZ>(2);
//            pointList.Add(sPoint2);
//            pointList.Add(ePoint2);
//            return pointList;
//        }
//        /// <summary>
//        /// Get selected element in linked file
//        /// </summary>
//        public static Element GetLinkedElem(Document doc, Reference refer)
//        {
//            RevitLinkInstance revLinkElem = doc.GetElement(refer.ElementId) as RevitLinkInstance;
//            Document linkDocElem = revLinkElem.GetLinkDocument();
//            Element linkElem = linkDocElem.GetElement(refer.LinkedElementId);
//            return linkElem;
//        }
//        /// <summary>
//        /// Check link for transform
//        /// </summary>
//        public static XYZ CheckForTransform(Document doc, RevitLinkInstance revLinkElem, XYZ point)
//        {
//            //RevitLinkInstance revLinkElem = doc.GetElement(refer.ElementId) as RevitLinkInstance;
//            //Document linkDocElem = revLinkElem.GetLinkDocument();
//            XYZ linkOrigin = revLinkElem.GetTotalTransform().Origin;
//            if (linkOrigin.DistanceTo(new XYZ(0, 0, 0)) < 1)
//            {
//                return point;
//            }
//            else
//            {

//                return revLinkElem.GetTotalTransform().OfPoint(point);
//            }
//        }
//        /// <summary>
//        /// Create pipe by 2 points
//        /// </summary>
//        public static Pipe CreatePipeByTwoPoints(Document doc, XYZ point1, XYZ point2)
//        {
//            //get the system type id of the duct
//            ///  The system type id of the pipe
//            ElementId systemTypeId = ElementId.InvalidElementId;
//            ElementClassFilter systemTypeFilter = new ElementClassFilter(typeof(MEPSystemType));
//            FilteredElementCollector C = new FilteredElementCollector(doc);
//            C.WherePasses(systemTypeFilter);
//            foreach (MEPSystemType type in C)
//            {
//                if (type.SystemClassification == MEPSystemClassification.DomesticColdWater)
//                {
//                    systemTypeId = type.Id;
//                    break;
//                }
//            }

//            //get the system type id of the level
//            ///  The system type id of the level
//            ElementId levelId = ElementId.InvalidElementId;
//            ElementClassFilter levelFilter = new ElementClassFilter(typeof(Level));
//            FilteredElementCollector levelC = new FilteredElementCollector(doc);
//            levelC.WherePasses(levelFilter);
//            foreach (Level lev in levelC)
//            {
//                if (lev != null)
//                {
//                    levelId = lev.Id;
//                    break;
//                }
//            }

//            ///  The type id of the pipe
//            ElementId pipeTypeId = ElementId.InvalidElementId;
//            FilteredElementCollector pipeS = new FilteredElementCollector(doc);
//            IList<Element> pipeList = pipeS.OfCategory(BuiltInCategory.OST_PipeCurves)
//                .WhereElementIsElementType()
//                .ToElements();
//            foreach (Element pipeTypeElem in pipeList)
//            {
//                //PipeType revIns = pipeTypeElem.Id as PipeType;
//                if (pipeTypeElem.Name == "Y_Opening")
//                {
//                    pipeTypeId = pipeTypeElem.Id;
//                    break;
//                }
//            }
//            if (pipeTypeId == ElementId.InvalidElementId)
//            {
//                pipeTypeId = (pipeList.First() as PipeType).Duplicate("Y_Opening").Id;
//            }

//            return Pipe.Create(doc, systemTypeId, pipeTypeId, levelId, point1, point2);
//        }

//        public static Duct CreateDuctByTwoPoints(Document doc, XYZ point1, XYZ point2)
//        {
//            //get the system type id of the duct
//            ///  The system type id of the pipe
//            ElementId systemTypeId = ElementId.InvalidElementId;
//            ElementClassFilter systemTypeFilter = new ElementClassFilter(typeof(MEPSystemType));
//            FilteredElementCollector C = new FilteredElementCollector(doc);
//            C.WherePasses(systemTypeFilter);
//            foreach (MEPSystemType type in C)
//            {
//                if (type.SystemClassification == MEPSystemClassification.SupplyAir)
//                {
//                    systemTypeId = type.Id;
//                    break;
//                }
//            }

//            //get the system type id of the level
//            ///  The system type id of the level
//            ElementId levelId = ElementId.InvalidElementId;
//            ElementClassFilter levelFilter = new ElementClassFilter(typeof(Level));
//            FilteredElementCollector levelC = new FilteredElementCollector(doc);
//            levelC.WherePasses(levelFilter);
//            foreach (Level lev in levelC)
//            {
//                if (lev != null)
//                {
//                    levelId = lev.Id;
//                    break;
//                }
//            }

//            ///  The type id of the pipe
//            ElementId pipeTypeId = ElementId.InvalidElementId;
//            FilteredElementCollector ductsS = new FilteredElementCollector(doc);
//            IList<Element> pipeList = ductsS.OfCategory(BuiltInCategory.OST_DuctCurves)
//                .WhereElementIsElementType()
//                .ToElements();
//            foreach (Element ductTypeElem in pipeList)
//            {
//                //DuctType revIns = pipeTypeElem.Id as PipeType;
//                if (ductTypeElem.Name == "Y_Opening2")
//                {
//                    pipeTypeId = ductTypeElem.Id;
//                    break;
//                }
//            }
//            if (pipeTypeId == ElementId.InvalidElementId)
//            {
//                pipeTypeId = (pipeList.First() as DuctType).Duplicate("Y_Opening2").Id;
//            }

//            return Duct.Create(doc, systemTypeId, pipeTypeId, levelId, point1, point2);
//        }


//        public static Duct CreateRoundDuctByTwoPoints(Document doc, XYZ point1, XYZ point2)
//        {
//            //get the system type id of the duct
//            ///  The system type id of the pipe
//            ElementId systemTypeId = ElementId.InvalidElementId;
//            ElementClassFilter systemTypeFilter = new ElementClassFilter(typeof(MEPSystemType));
//            FilteredElementCollector C = new FilteredElementCollector(doc);
//            C.WherePasses(systemTypeFilter);
//            foreach (MEPSystemType type in C)
//            {
//                if (type.SystemClassification == MEPSystemClassification.SupplyAir)
//                {
//                    systemTypeId = type.Id;
//                    break;
//                }
//            }

//            //get the system type id of the level
//            ///  The system type id of the level
//            ElementId levelId = ElementId.InvalidElementId;
//            ElementClassFilter levelFilter = new ElementClassFilter(typeof(Level));
//            FilteredElementCollector levelC = new FilteredElementCollector(doc);
//            levelC.WherePasses(levelFilter);
//            foreach (Level lev in levelC)
//            {
//                if (lev != null)
//                {
//                    levelId = lev.Id;
//                    break;
//                }
//            }

//            ///  The type id of the pipe
//            ElementId pipeTypeId = ElementId.InvalidElementId;
//            FilteredElementCollector ductsS = new FilteredElementCollector(doc);
//            IList<Element> pipeList = ductsS.OfCategory(BuiltInCategory.OST_DuctCurves)
//                .WhereElementIsElementType()
//                .ToElements();
//            foreach (Element ductTypeElem in pipeList)
//            {
//                //DuctType revIns = pipeTypeElem.Id as PipeType;
//                if (ductTypeElem.Name == "Y_Opening3")
//                {
//                    pipeTypeId = ductTypeElem.Id;
//                    break;
//                }
//            }
//            if (pipeTypeId == ElementId.InvalidElementId)
//            {
//                pipeTypeId = (pipeList.First() as DuctType).Duplicate("Y_Opening3").Id;
//            }

//            return Duct.Create(doc, systemTypeId, pipeTypeId, levelId, point1, point2);
//        }


//        /// <summary>
//        /// Get family symbol
//        /// </summary>
//        public FamilySymbol GetFamilySymb(Document doc)
//        {
//            //Family Symbol
//            FilteredElementCollector collecF = new FilteredElementCollector(doc);
//            IList<Element> symbols = collecF.OfClass(typeof(FamilySymbol))
//                .WhereElementIsElementType()
//                .ToElements();
//            FamilySymbol famSymb = null;
//            foreach (Element fam in symbols)
//            {
//                if (fam.Name == "Y_Opening")
//                {
//                    famSymb = fam as FamilySymbol;
//                    break;
//                }
//            }
//            if (famSymb == null)
//            { TaskDialog.Show("Worning", "Paste provided opening family into project"); }
//            if (!famSymb.IsActive)
//            {
//                famSymb.Activate();
//            }
//            return famSymb;
//        }
//        /// <summary>
//        /// Place and rotate opening family
//        /// </summary>
//        public void PlaceOpeningFamily(
//            Document doc,
//            SolidCurveIntersection intersection,
//            FamilySymbol famSymb,
//            Element pipeEl)
//        {
//            XYZ interP = (intersection.GetCurveSegment(0).GetEndPoint(0) + intersection.GetCurveSegment(0).GetEndPoint(1)) * 0.5;

//            XYZ startPoint = intersection.GetCurveSegment(0).GetEndPoint(0);
//            XYZ endPoint = intersection.GetCurveSegment(0).GetEndPoint(1);

//            XYZ pipeDir = (endPoint - startPoint).Normalize();
//            XYZ zeroVec = new XYZ(1, 0, 0);
//            XYZ zPlane = new XYZ(0, 0, 1);
//            XYZ yPlane = new XYZ(0, 1, 0);
//            Line axiz = Line.CreateBound(interP, interP + zPlane);
//            double ang = pipeDir.AngleOnPlaneTo(zeroVec, zPlane);

//            Line axizY = Line.CreateBound(new XYZ(endPoint.X, endPoint.Y, interP.Z), new XYZ(startPoint.X, startPoint.Y, interP.Z));

//            Transform rot = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI / 2, interP);
//            //Curve cRot = pipeCur.CreateTransformed(rot);
//            Line axizY2 = axizY.CreateTransformed(rot) as Line;
//            XYZ prot = axizY2.GetEndPoint(0);
//            //XYZ pipeDir = (endPoint - startPoint).Normalize();
//            double angz = pipeDir.AngleTo((axizY.GetEndPoint(1) - axizY.GetEndPoint(0)).Normalize());
//            //double angz = pipeDir.AngleOnPlaneTo(axizY.GetEndPoint(0), axizY.GetEndPoint(1));// zPlane, yPlane);
//            //ang = ang * (180 / Math.PI);

//            Element famInst = doc.Create.NewFamilyInstance(interP, famSymb, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
//            famInst.LookupParameter("Diameter").Set(pipeEl.LookupParameter("Diameter").AsDouble() + FormNew.dist);
//            famInst.LookupParameter("Length").Set(intersection.GetCurveSegment(0).Length + FormNew.ladge);
//            ElementTransformUtils.RotateElement(doc, famInst.Id, axiz, ang * -1);
//            ElementTransformUtils.RotateElement(doc, famInst.Id, axizY2, angz * -1);
//        }

//    }
//}
