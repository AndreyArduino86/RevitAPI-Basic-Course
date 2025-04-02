using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAPI_Basic_Course
{
    [Transaction(TransactionMode.Manual)]
    public class DiologForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using (var Form1 = new Form1())

            {
                Form1.ShowDialog();
            }
            return Result.Succeeded;
        }
    }


}


