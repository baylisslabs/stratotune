using System;
using System.Globalization;

using SQLite;


namespace bit.projects.iphone.chromatictuner.model
{
    [Table("App_Feedback")]
    public class AppFeedbackDao
    {
		private const string dateTimeFormat = "yyyy-MM-dd hh:mm:ss";

        [PrimaryKey]  
		public int Id { get; set; }
		public string VersionAtLastUsage { get; set; }
		public string VersionFirstUsedTimeStamp { get; set; }
		public int UsesCount { get; set; }
		public int SignificantUsesCount { get; set; }
		public string DoNotRemindBeforeTime { get; set; }
		public bool RatingFlowCompleted { get; set; }	
		     
		public AppFeedbackDao ()
        {
        }

		public AppFeedbackDao (AppFeedback obj)
        {
            this.Id = obj.Id;
			this.VersionAtLastUsage = obj.VersionAtLastUsage;
			this.VersionFirstUsedTimeStamp = fromDateTime (obj.VersionFirstUsedTimeStamp);
			this.UsesCount = obj.UsesCount;
			this.SignificantUsesCount = obj.SignificantUsesCount;
			this.DoNotRemindBeforeTime = fromDateTime(obj.DoNotRemindBeforeTime);
			this.RatingFlowCompleted = obj.RatingFlowCompleted;
        }
        
		public AppFeedback ToObject()
        {
			return new AppFeedback()
            {
				Id = this.Id,
				VersionAtLastUsage = this.VersionAtLastUsage,
				VersionFirstUsedTimeStamp = toDateTime(this.VersionFirstUsedTimeStamp),
				UsesCount = this.UsesCount,
				SignificantUsesCount = this.SignificantUsesCount,
		        DoNotRemindBeforeTime = toDateTime(this.DoNotRemindBeforeTime),
				RatingFlowCompleted = this.RatingFlowCompleted
            };
        }

		private string fromDateTime(DateTime? dateTime)
		{
			if (dateTime == null) {
				return null;
			}
			return dateTime.Value.ToString (dateTimeFormat);
		}

		private DateTime? toDateTime(string dbString)
		{
			if (string.IsNullOrWhiteSpace(dbString)) {
				return null;
			}
			return DateTime.ParseExact (dbString, dateTimeFormat, CultureInfo.InvariantCulture);
		}
    }
}

