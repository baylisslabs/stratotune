using System;
using System.Linq;
using System.IO;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{   
    public class UserSettingsServiceImpl : IUserSettingsService
    {
        static Logger _log = LogManager.GetLogger("UserSettingsServiceImpl");
                     
        private readonly string DefaultName = "Default";
        private UserSettings _fallback;
        private string _connectionString;      
        private string _scriptPath;

        public UserSettingsServiceImpl (string connectionString, string scriptPath)
        {              
            _connectionString = connectionString;  
            _scriptPath = scriptPath;
            _fallback = UserSettingsExtensions.CreateWithName(this.DefaultName);
        }

        #region IUserSettingsService implementation
             
        public bool Configure()
        {
            _log.Debug("Configure()");
            try {
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    da.SetForeignKeys(true);
                    da.ExecuteScript(_scriptPath);                   
                    return true;
                }
            } catch (Exception ex) {
                _log.Error("configure failed",ex);
            }

            _log.Error("configure failed");
            
            return false;
        }
              
        public UserSettings[] RetrieveAll ()
        {
            _log.Debug("RetrieveAll()");
            try {
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    var usda = new UserSettingsDataAccess (da);
                    return usda.GetAllSettings ().OrderBy(us=>us.Id).ToArray ();
                }
            } catch (Exception ex) {
                _log.Error("RetrieveAll () failed",ex);
            }

            _log.Error("settings not retrieved");
                       
            return new UserSettings[] { _fallback };
        }

        public UserSettings RetrieveCurrent ()
        {
            _log.Debug("RetrieveCurrent()");
            try {
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    var usda = new UserSettingsDataAccess (da);
                    var result = usda.GetCurrentSetting();
                    if(result!=null) {
                        return result;
                    }
                }
            } catch (Exception ex) {
                _log.Error("RetrieveCurrent () failed",ex);
            }
                
            _log.Error("current setting not retrieved");

            return _fallback;
        }
              
        public int RetrieveCurrentId ()
        {
            _log.Debug("RetrieveCurrentId()");
            try {
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    var scda = new SystemConfigDataAccess(da);
                    return scda.Get_CurrentUserSetting_Id();                   
                }
            } catch (Exception ex) {
                _log.Error("RetrieveCurrent () failed",ex);
            }

            _log.Error("current id not retrieved");

            return _fallback.Id;
        }

        public bool SetCurrent (int id)
        {
            _log.Debug("SetCurrent({0})",id);
           
            try {
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    da.BeginTransaction();
                    var scda = new SystemConfigDataAccess(da);                  
                    if (scda.Update_CurrentUserSetting_Id(id)) {
                        da.Commit();
                        return true;
                    }
                }
            } catch (Exception ex) {
                _log.Error("SetCurrent () failed",ex);
            }       

            _log.Error("current id not updated");

            return false;
        }             
      
        public UserSettings Add (string name)
        {
            _log.Debug("Add({0})",name);
            try {
                var us = UserSettingsExtensions.CreateWithName(name);
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    da.BeginTransaction();
                    var usda = new UserSettingsDataAccess (da);
                    if (usda.Create(us)) {                      
                        da.Commit();
                        return us;
                    }                  
                }
            } catch (Exception ex) {
                _log.Error("Add () failed",ex);
            }

            _log.Error("row not inserted");

            return null;
        }

        public bool Delete (int id, out int newCurrentSettingId)
        {
            _log.Debug("Delete({0})",id); 

            newCurrentSettingId = 0;
            try {              
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    da.BeginTransaction();
                    var usda = new UserSettingsDataAccess (da);  
                    var scda = new SystemConfigDataAccess(da);
                    var sysConfig = scda.Get();
                    if(sysConfig!=null) {
                        newCurrentSettingId = sysConfig.CurrentUserSetting_Id;
                        if(sysConfig.CurrentUserSetting_Id==id) {
                            if (scda.Update_CurrentUserSetting_Id(sysConfig.DefaultUserSetting_Id)) {
                                newCurrentSettingId = sysConfig.DefaultUserSetting_Id;
                            }
                        }
                        if(usda.Delete(id)) {
                            da.Commit();
                            return true;
                        }
                    }
                }
            } catch (Exception ex) {
                _log.Error("Add () failed",ex);
            } 

            _log.Error("row not deleted");         
            return false;
        }

        public bool Update (UserSettings settings)
        {                  
            _log.Debug("Update({0})",settings.Id);

            try {                             
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    da.BeginTransaction();
                    var usda = new UserSettingsDataAccess (da);
                    if(usda.Update(settings)) {
                        da.Commit();
                        return true;
                    }
                }
            } catch (Exception ex) {
                _log.Error("Update () failed",ex);
            }  

            _log.Error("row not updated");

            return false;
        }

        public SystemConfig GetSystemConfig()
        {
            _log.Debug("GetSystemConfig()");
            try {
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    var scda = new SystemConfigDataAccess(da);
                    return scda.Get();               
                }
            } catch (Exception ex) {
                _log.Error("GetSystemConfig () failed",ex);
            }
            
            _log.Error("system config not retrieved");
            
            return null;
        }

        public bool SetHeadPhoneAlertDisabled(bool value)
        {       
            _log.Debug("SetHeadPhoneAlertDisabled({0})",value);
            
            try {
                using (var da = new SQLiteDataAccess(_connectionString)) {
                    da.BeginTransaction();
                    var scda = new SystemConfigDataAccess(da);                  
                    if (scda.Update_HeadPhoneAlertDisabled(value)) {
                        da.Commit();
                        return true;
                    }
                }
            } catch (Exception ex) {
                _log.Error("SetHeadPhoneAlertDisabled () failed",ex);
            }       
            
            _log.Error("system config not updated");
            
            return false;             
        }

		public AppFeedback GetAppFeedback()
		{
			_log.Debug("GetAppFeedback()");
			try {
				using (var da = new SQLiteDataAccess(_connectionString)) {
					var scda = new AppFeedbackDataAccess(da);
					return scda.Get();               
				}
			} catch (Exception ex) {
				_log.Error("GetAppFeedback () failed",ex);
			}

			_log.Error("app feedback not retrieved");

			return null;
		}

		public bool UpdateAppFeedback(AppFeedback appFeedback)
		{
			_log.Debug("UpdateAppFeedback()");

			try {                             
				using (var da = new SQLiteDataAccess(_connectionString)) {
					da.BeginTransaction();
					var usda = new AppFeedbackDataAccess (da);
					if(usda.Update(appFeedback)) {
						da.Commit();
						return true;
					}
				}
			} catch (Exception ex) {
				_log.Error("UpdateAppFeedback () failed",ex);
			}  

			_log.Error("row not updated");

			return false;
		}

        #endregion
    }
}

