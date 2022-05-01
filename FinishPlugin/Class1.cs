using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class AddRoom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Создаю помещения в заданных границах стен
            List<Room> rooms = new List<Room>();

            //Отбираю уровень для помещения
            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction ts1 = new Transaction(doc, "New room creation");
            ts1.Start();
            foreach (var level in levels)
            {
                doc.Create.NewRooms2(level);
            }
            ts1.Commit();


            //Отбираю помещения
            rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfType<Room>()
                .ToList();


            Transaction ts2 = new Transaction(doc, "New tag creation");

            ts2.Start();
            int roomcounter = 0;
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            filteredElementCollector.OfClass(typeof(FamilySymbol));
            filteredElementCollector.OfCategory(BuiltInCategory.OST_RoomTags);
            var roomtagtypes = filteredElementCollector.Cast<RoomTagType>().ToList<RoomTagType>();
            foreach (var room in rooms)
            {
                roomcounter++;
                string Name = room.Level.Name.Substring(6);
                room.Name = $"{Name}_{ room.Number}";
                //newTag.get_Parameter(BuiltInParameter.TAG_TAG).Set($"{room.Level}_{roomcounter}");
                LocationPoint locationPoint = room.Location as LocationPoint;
                UV point = new UV(locationPoint.Point.X, locationPoint.Point.Y);
                //public RoomTag NewRoomTag(LinkElementId roomId, DB.UV point, DB.ElementId viewId);
                RoomTag newTag = doc.Create.NewRoomTag(new LinkElementId(room.Id), point, null);              
            }
            ts2.Commit();
            return Result.Succeeded;

        }

    }
}
