using System;


namespace Models.Models
{ 

    public class ProviderAssignment
    {


        [Key]
        public int AssignmentID { get; set; }

        [ForeignKey("Provider")]
        public int ProviderID { get; set; }

        [ForeignKey("Center")]
        public int CenterID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } 

        [Required]
        public string AssignmentType { get; set; }


        public Provider Provider { get; set; }
        public Center Center { get; set; }
    }



}