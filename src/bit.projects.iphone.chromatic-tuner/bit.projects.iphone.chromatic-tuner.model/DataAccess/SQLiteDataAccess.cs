using System;
using System.Collections.Generic;
using System.IO;

using SQLite;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{
    public class SQLiteDataAccess : IDisposable
    {
        private static Logger _log = LogManager.GetLogger("bit.projects.iphone.chromatictuner.model.SQLiteDataAccess");
        private static Dictionary<string,SQLiteConnection> _cached;
        private static object _lock = new object();

        private SQLiteConnection _sqliteConnection;              
        private bool _isDisposed;
             
        static SQLiteDataAccess ()
        {
            _cached = new Dictionary<string, SQLiteConnection>();
        }

        public SQLiteDataAccess (string connectionString)
        {
            _sqliteConnection = getCached(connectionString);
            if (_sqliteConnection == null) {
                _log.Debug ("Opening New SQLiteConnection('{0}')", connectionString);

                _sqliteConnection = new SQLiteConnection (
                    databasePath: connectionString,
                    openFlags: SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite,
                    storeDateTimeAsTicks: true);  

                _sqliteConnection = testAndSetCached(connectionString,_sqliteConnection);                                         
            }
        }
            
        public void BeginTransaction ()
        {
            usingConnection((conn)=>
            {
                _log.Debug("BeginTransaction()");
                conn.BeginTransaction();
            });
        }

        public void Commit ()
        {
            usingConnection((conn)=>
            {
                _log.Debug("Commit()");
                conn.Commit();
            });
        }

        public void ExecuteQuery<T>(Action<SQLiteCommand> cmd, Action<List<T>> onData)
        {
            usingConnection((conn)=>
            {
                var command = conn.CreateCommand(null); 
                cmd(command);
                _log.Debug(()=>String.Format("ExecuteQuery('{0}')",command.ToString()));
                var rows = command.ExecuteQuery<T>();                              
                _log.Debug("Returned {0} Rows",rows.Count);
                onData(rows);
            });
        }

        public void ExecuteScalar<T>(Action<SQLiteCommand> cmd, Action<T> onData)
        {
            usingConnection((conn)=>
            {
                var command = conn.CreateCommand(null); 
                cmd(command);
                _log.Debug(()=>String.Format("ExecuteScalar('{0}')",command.ToString()));
                var data = command.ExecuteScalar<T>();                              
                _log.Debug("Returned 1 Rows");
                onData(data);
            });
        }

        public void ExecuteNonQuery(Action<SQLiteCommand> cmd, Action<SQLiteConnection,int> onRowsAffected)
        {
            usingConnection((conn)=>
            {
                var command = conn.CreateCommand(null); 
                cmd(command);
                _log.Debug(()=>String.Format("ExecuteNonQuery('{0}')",command.ToString()));
                var rowsAffected = command.ExecuteNonQuery();               
                _log.Debug("Rows affected: {0}",rowsAffected);
                onRowsAffected(conn,rowsAffected);
            });
        }

        public void CreateTable<T> ()
        {
            usingConnection((conn)=>
            {
                _log.Debug(()=>String.Format("CreateTable(IfNotExists): {0}",typeof(T)));
                conn.CreateTable<T>();
            });
        }

        public void Insert<T> (T obj, Action<T> onSuccess)
        {
            usingConnection((conn)=>
            {
                _log.Debug(()=>String.Format("Insert: {0}",typeof(T)));
                if (conn.Insert(obj)!=0) {
                    onSuccess(obj);
                }
            });
        }

        public void Update<T> (T obj, Action<SQLiteConnection,int> onRowsAffected)
        {
            usingConnection((conn)=>
            {
                _log.Debug(()=>String.Format("Update: {0}",typeof(T)));
                onRowsAffected(conn,conn.Update(obj));
            });
        }

        public void Delete<T> (T obj, Action<SQLiteConnection,int> onRowsAffected)
        {
            usingConnection((conn)=>
            {
                _log.Debug(()=>String.Format("Delete: {0}",typeof(T)));
                onRowsAffected(conn,conn.Delete(obj));
            });
        }

        public void ExecuteScript (string scriptPath)
        {
            usingConnection((conn)=>
            {
                // TODO: - handle embedded ;
                var script = File.ReadAllText(scriptPath);                 
                var lines = script.Split(new [] {';'},StringSplitOptions.RemoveEmptyEntries);
                foreach(var line in lines) {
                    var trimmed = line.Trim();
                    if(trimmed.Length>0) {
                        _log.Debug(()=>String.Format("Execute: {0};",trimmed));
                        conn.Execute(trimmed+";");
                    }
                }
            });
        }

        public bool SetForeignKeys(bool enabled)
        {
            bool success = false;
            usingConnection((conn)=>
            {
                _log.Debug(()=>String.Format("SetForeignKeys({0})",enabled));
                conn.Execute(String.Format("PRAGMA foreign_keys = {0};",enabled?"ON":"OFF"));
                success = (conn.ExecuteScalar<int>("PRAGMA foreign_keys")==1);
            });
            _log.Debug("SetForeignKeys()=>{0}",success);
            return success;
        }

        public void Close ()
        {         
            usingConnection((conn)=>
            {               
                try {
                    try {
                        if (_sqliteConnection.IsInTransaction) {
                            _log.Debug("Rollback()");
                            _sqliteConnection.Rollback();
                        }
                    }
                    catch(Exception ex) {
                        _log.Error("Rollback() failed",ex);
                    }                
                } finally {              
                    _sqliteConnection = null;
                }
            });
        }

        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool isDisposing)
        {
            try {
                if (!_isDisposed) {
                    if (isDisposing) {
                        Close ();                       
                    }
                }
            } finally {
                _isDisposed = true;
            }
        }
               
        private void usingConnection (Action<SQLiteConnection> action)
        {         
            if (_sqliteConnection != null) {
                action (_sqliteConnection);         
            } else {
                _log.Error("Attempt to use closed connection");
            }
        }

        private static SQLiteConnection getCached (string connectionString)
        {
            lock (_lock) {
                if (_cached.ContainsKey (connectionString)) {
                    return _cached[connectionString];
                }
                return null;
            }
        }

        private static SQLiteConnection testAndSetCached (string connectionString, SQLiteConnection connection)
        {
            lock (_lock) {
                if (_cached.ContainsKey (connectionString)) {
                    return _cached[connectionString];
                } else {
                    _cached[connectionString] = connection;
                    return connection;
                }
            }
        }

    }
}

