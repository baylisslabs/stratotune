using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{
    public class UserSettingsDataAccess
    {
        private SQLiteDataAccess _dataAccess;
              
        public UserSettingsDataAccess (SQLiteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void CreateTableIfNotExists()
        {
            _dataAccess.CreateTable<UserSettingsDao>();
        }

        public int NumSettings ()
        {
            int result = 0;
            _dataAccess.ExecuteScalar<int>(
                cmd=>{
                    cmd.CommandText = @"select count(1) from user_settings;";
                },
                data=>{
                    result = data;
                }
            );
            return result;
        }

        public IEnumerable<UserSettings> GetAllSettings()
        {
            var result = new List<UserSettings>();
            _dataAccess.ExecuteQuery<UserSettingsDao>(
                cmd=>{
                    cmd.CommandText = @"select * from user_settings;";
                },
                dataRows=>{
                    result.AddRange(dataRows.Select((row)=>row.ToObject()));
                }
            );
            return result;
        }

        public UserSettings GetCurrentSetting()
        {
            var result = new List<UserSettings>();
            _dataAccess.ExecuteQuery<UserSettingsDao>(
                cmd=>{
                    cmd.CommandText = @"select us.* from user_settings us, system_config sc where us.id = sc.currentusersetting_id and sc.id = @id;";                  
                    cmd.Bind("@id",SystemConfigDataAccess.SYSTEM_CONFIG_ID);
                },
                dataRows=>{
                    result.AddRange(dataRows.Select((row)=>row.ToObject()));
                }
            );

            return result.FirstOrDefault();
        }

        public bool Create(UserSettings settings)
        {
            bool success = false;
            _dataAccess.Insert(
                new UserSettingsDao(settings),
                updatedObj=>{
                    settings.Id = updatedObj.Id;
                    success = true;
                }
            );
            return success;
        }

        public bool Update(UserSettings settings)
        {
            bool success = false;
            _dataAccess.Update(
                new UserSettingsDao(settings),
                (conn,rowsAffected)=> {
                    success = (rowsAffected>0);
                }
            );
            return success;
        }
               
        public bool Delete(int id)
        {
            bool success = false;
            _dataAccess.ExecuteNonQuery(
                cmd=>{
                cmd.CommandText = "delete from user_settings where id = @id and not DisallowDelete;";
                    cmd.Bind("@id",id);
                },           
                (conn,rowsAffected)=> {
                    success = (rowsAffected>0);
                }
            );
            return success;
        }
    }
}

