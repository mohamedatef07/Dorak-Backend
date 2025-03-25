using System;
using Models.Enums;


namespace Models.Models
{ 

    public class ProviderAssignment
    {

            public int AssignmentID { get; set; } 
            public int ProviderID { get; set; } 
            public int CenterID { get; set; } 
            public DateTime StartDate { get; set; } 
            public DateTime EndDate { get; set; } 
            public AssignmentType AssignmentType { get; set; } 

            // Navigation Properties
            public virtual Provider Provider { get; set; }
            public virtual Center Center { get; set; } 
        }

    }



