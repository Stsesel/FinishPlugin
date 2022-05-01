using Autodesk.Revit.UI;

namespace FinishPlugin
{
    internal class RoomsData
    {
        private ExternalCommandData commandData;

        public RoomsData(ExternalCommandData commandData)
        {
            this.commandData = commandData;
        }
    }
}