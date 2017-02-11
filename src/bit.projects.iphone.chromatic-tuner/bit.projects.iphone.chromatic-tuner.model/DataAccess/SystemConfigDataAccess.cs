using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{  
    public class SystemConfigDataAccess
    {     
        private SQLiteDataAccess _dataAccess;      

        public static readonly int SYSTEM_CONFIG_ID = 1;

        public SystemConfigDataAccess (SQLiteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void CreateTableIfNotExists()
        {
            _dataAccess.CreateTable<SystemConfigDao>();
        }
               
        public SystemConfig Get()
        {
            var result = new List<SystemConfig>();
            _dataAccess.ExecuteQuery<SystemConfigDao>(
                cmd=>{
                    cmd.CommandText = @"select * from system_config where id = @id;";
                    cmd.Bind("@id",SYSTEM_CONFIG_ID);
                },
                dataRows=>{
                    result.AddRange(dataRows.Select((row)=>row.ToObject()));
                }
            );

            return result.FirstOrDefault();
        }

        public int Get_CurrentUserSetting_Id()
        {
            int result = 0;
            _dataAccess.ExecuteScalar<int>(
                cmd=>{
                    cmd.CommandText = @"select CurrentUserSetting_Id from system_config where id = @id;";
                    cmd.Bind("@id",SYSTEM_CONFIG_ID);
                },
                data=>{
                    result = data;
                }
            );
            
            return result;
        }

        public bool Create(SystemConfig settings)
        {
            bool success = false;
            _dataAccess.Insert(
                new SystemConfigDao(settings),
                updatedObj=>{
                    settings.Id = updatedObj.Id;
                    success = true;
                }
            );
            return success;
        }

        public bool Update(SystemConfig settings)
        {
            bool success = false;
            _dataAccess.Update(
                new SystemConfigDao(settings),
                (conn,rowsAffected)=> {
                    success = (rowsAffected>0);
                }
            );
            return success;
        }               
               
        /* nb. named param binding in update set section not working, check */
        public bool Update_CurrentUserSetting_Id(int id)
        {
            bool success = false;
            _dataAccess.ExecuteNonQuery(
                cmd=>{
                    cmd.CommandText = @"update system_config set CurrentUserSetting_Id = ? where id = ?;";
                    cmd.Bind(id);
                    cmd.Bind(SYSTEM_CONFIG_ID);                   
                },
                (conn,rowsAffected)=> {
                    success = (rowsAffected>0);
                }
            );
            return success;
        }      

        /* nb. named param binding in update set section not working, check */
        public bool Update_HeadPhoneAlertDisabled(bool value)
        {
            bool success = false;
            _dataAccess.ExecuteNonQuery(
                cmd=>{
                cmd.CommandText = @"update system_config set HeadPhoneAlertDisabled = ? where id = ?;";
                cmd.Bind(value);
                cmd.Bind(SYSTEM_CONFIG_ID);                   
            },
            (conn,rowsAffected)=> {
                success = (rowsAffected>0);
            }
            );
            return success;
        }      
    }
}

