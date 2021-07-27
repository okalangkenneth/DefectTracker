using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Defect_Tracker.Shared
{
    public class Defect
    {
        public int Id { get; set; }
        [Required]
        public string DefectStatus { get; set; }
        [Required]
        public DateTime DefectAt { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2,

        ErrorMessage ="Description must be a minimum of 2 and maximum of 50 characters.")]
        public string DefectDescription { get; set; }
        [Required]
        [EmailAddress]
        public string DefectReporterEmail { get; set; }
        public string DefectGuid { get; set; }
        public List<DefectDetail> DefectDetails {get; set;}
    }
    public class DefectDetail
    {
        public int Id { get; set; }
        public int DefectId { get; set; }
        public DateTime DefectAt { get; set; }
        public string DefectDescription { get; set; }
    }
}

