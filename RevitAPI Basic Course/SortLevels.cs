using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace RevitAPI_Basic_Course
{
    [Transaction(TransactionMode.Manual)]
    public class SortLevels : IExternalCommand

    {
     
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> levels = collector.OfCategory(BuiltInCategory.OST_Views).Where(x => !(x as Autodesk.Revit.DB.View).IsTemplate
            && (x as Autodesk.Revit.DB.View).ViewType != ViewType.Elevation
            && (x as Autodesk.Revit.DB.View).ViewType != ViewType.ThreeD
            && (x as Autodesk.Revit.DB.View).ViewType != ViewType.Section).ToList();
            
            

            using (Autodesk.Revit.DB.Transaction transaction = new Autodesk.Revit.DB.Transaction(doc))
            {
                transaction.Start("Level");

                TaskDialog.Show("Уровни модели:", string.Join(Environment.NewLine, levels.Select(item => item.Name)));

                transaction.Commit();
            }
            return Result.Succeeded;

        }


    }
}