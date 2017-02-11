using System;
using System.Collections.Generic;

namespace bit.projects.iphone.chromatictuner.model
{                 
    public class AppFeedback
    {         
		public int Id { get; set; }      
		public string VersionAtLastUsage { get; set; }
		public DateTime? VersionFirstUsedTimeStamp { get; set; }
		public int UsesCount { get; set; }
		public int SignificantUsesCount { get; set; }
		public DateTime? DoNotRemindBeforeTime { get; set; }
		public bool RatingFlowCompleted { get; set; }			   
    }   
}

