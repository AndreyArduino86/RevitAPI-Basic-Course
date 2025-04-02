using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace RevitAPI_Basic_Course
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class LinkSectionBox : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            Document doc = app.ActiveUIDocument.Document;
            Selection sel = app.ActiveUIDocument.Selection;
            Element elem = null;
            double xMinPoint = 1000000;
            double yMinPoint = 1000000;
            double zMinPoint = 1000000;
            double xMaxPoint = -1000000;
            double yMaxPoint = -1000000;
            double zMaxPoint = -1000000;

            BoundingBoxXYZ boxXYZ = null;
            IList<Reference> refList = sel.PickObjects(ObjectType.LinkedElement, "Select link elements");
            foreach (Reference refer in refList)
            {
                //elem = TwoElems.GetLinkedElem(doc, refer);
                boxXYZ = elem.get_BoundingBox(doc.ActiveView);

                RevitLinkInstance revLinkElem = doc.GetElement(refer.ElementId) as RevitLinkInstance;
                XYZ linkOrigin = revLinkElem.GetTotalTransform().Origin;
                if (linkOrigin.DistanceTo(new XYZ(0, 0, 0)) > 1)
                {
                    XYZ midPoint = (boxXYZ.Max + boxXYZ.Min) / 2;
                    // XYZ midTransPoint = TwoElems.CheckForTransform(doc, revLinkElem, midPoint);
                    // XYZ transVec = midTransPoint - midPoint;
                    // Transform trans1 = Transform.CreateTranslation(transVec);
                    // boxXYZ.Max = trans1.OfPoint(boxXYZ.Max);
                    //  boxXYZ.Min = trans1.OfPoint(boxXYZ.Min);
                }
                if (boxXYZ.Min.X < xMinPoint)
                {
                    xMinPoint = boxXYZ.Min.X;
                }
                if (boxXYZ.Min.Y < yMinPoint)
                {
                    yMinPoint = boxXYZ.Min.Y;
                }
                if (boxXYZ.Min.Z < zMinPoint)
                {
                    zMinPoint = boxXYZ.Min.Z;
                }

                if (boxXYZ.Max.X > xMaxPoint)
                {
                    xMaxPoint = boxXYZ.Max.X;
                }
                if (boxXYZ.Max.Y > yMaxPoint)
                {
                    yMaxPoint = boxXYZ.Max.Y;
                }
                if (boxXYZ.Max.Z > zMaxPoint)
                {
                    zMaxPoint = boxXYZ.Max.Z;
                }
            }
            boxXYZ.Min = new XYZ(xMinPoint, yMinPoint, zMinPoint);
            boxXYZ.Max = new XYZ(xMaxPoint, yMaxPoint, zMaxPoint);

            //List<Document> docS = new List<Document>();
            //FilteredElementCollector links = new FilteredElementCollector(doc);
            //IList<Element> linksS = links.OfCategory(BuiltInCategory.OST_RvtLinks)
            //    .WhereElementIsNotElementType()
            //    .ToElements();
            //foreach (Element linkElem in linksS)
            //{
            //    RevitLinkInstance revIns = linkElem as RevitLinkInstance;
            //    RevitLinkType revType = doc.GetElement(linkElem.Id) as RevitLinkType;
            //    try
            //    {
            //        Document linkDoc = revIns.GetLinkDocument();
            //        if (linkDoc != null)
            //        {
            //            docS.Add(linkDoc);
            //        }

            //    }
            //    catch { }
            //}
            //docS.Add(doc);
            //ActivateUserView(app);
            //View3D actView = doc.ActiveView as View3D;
            //View3D view = null;

            ActivateUserView(app);

            //view = actView as View3D;

            Transaction trans = new Transaction(doc);
            trans.Start("LinkSectionBox");
            if (boxXYZ != null)
            {
                ExpandSectionBox(doc.ActiveView as View3D, boxXYZ);

                //ElementId sectBoxId = new ElementId(doc.ActiveView.Id.IntegerValue - 1);

            }

            trans.Commit();
            app.ActiveUIDocument.ShowElements(new ElementId(doc.ActiveView.Id.IntegerValue - 1));
            return Result.Succeeded;
        }
        private void ExpandSectionBox(View3D view, BoundingBoxXYZ sectionBox)
        {
            // The original section box
            //BoundingBoxXYZ viewSectionBox = view.GetSectionBox();
            //View3D view = actView as View3D;
            // Expand the section box (doubling in size in all directions while preserving the same center and orientation)
            XYZ deltaXYZ = sectionBox.Max - sectionBox.Min;
            sectionBox.Max += new XYZ(1, 1, 1);//deltaXYZ / 5;
            sectionBox.Min -= new XYZ(1, 1, 1);//deltaXYZ / 5;
            //After resetting the section box, it will be shown in the view.
            //It only works when the Section Box is active
            view.SetSectionBox(sectionBox);
        }
        private void ActivateUserView(UIApplication app)
        {

            Document doc = app.ActiveUIDocument.Document;
            if (doc.GetElement(doc.ActiveView.GetTypeId()).Name != "3D View")
            {
                FilteredElementCollector docCollector = new FilteredElementCollector(doc);
                IList<Element> view3dList = docCollector.OfClass(typeof(View3D))
                    .WhereElementIsNotElementType()
                    .ToElements();
                foreach (Element view in view3dList)
                {
                    if (doc.IsWorkshared)
                    {
                        if (view.Name == "{3D - " + app.Application.Username + "}")
                        {
                            app.ActiveUIDocument.ActiveView = (view as View3D);
                            break;
                        }
                    }
                    else
                    {
                        if (view.Name.Contains("{3D}"))
                        {
                            app.ActiveUIDocument.ActiveView = (view as View3D);
                            break;
                        }
                    }
                }
            }

        }

    }
}

