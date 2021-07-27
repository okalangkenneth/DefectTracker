using System.Collections.Generic;

namespace Defect_Tracker.Shared
{
    public class DefectStatus
    {
        public string ID { get; set; }
        public string Text { get; set; }

        public static List<DefectStatus> Statuses = new List<DefectStatus>() {
        new DefectStatus(){ ID= "New", Text= "New" },
        new DefectStatus(){ ID= "Open", Text= "Open" },
        new DefectStatus(){ ID= "Urgent", Text= "Urgent" },
        new DefectStatus(){ ID= "Closed", Text= "Closed" },
        };
    }

}
