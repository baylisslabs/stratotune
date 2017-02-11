using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{
	public class AppFeedbackDataAccess
	{
		private SQLiteDataAccess _dataAccess;      

		public static readonly int APP_FEEDBACK_ID = 1;

		public AppFeedbackDataAccess (SQLiteDataAccess dataAccess)
		{
			_dataAccess = dataAccess;
		}

		public AppFeedback Get()
		{
			var result = new List<AppFeedback>();
			_dataAccess.ExecuteQuery<AppFeedbackDao>(
				cmd=>{
				cmd.CommandText = @"select * from app_feedback where id = @id;";
				cmd.Bind("@id",APP_FEEDBACK_ID);
			},
			dataRows=>{
				result.AddRange(dataRows.Select((row)=>row.ToObject()));
			}
			);

			return result.FirstOrDefault();
		}

		public bool Update(AppFeedback appFeedback)
		{
			bool success = false;
			_dataAccess.Update(
				new AppFeedbackDao(appFeedback),
				(conn,rowsAffected)=> {
				success = (rowsAffected>0);
			}
			);
			return success;
		}
	}
}

