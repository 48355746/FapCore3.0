using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.MetaData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    public class ColumnSet:IColumnSet
    {
        private IEnumerable<FapColumn> _allColumns = new List<FapColumn>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal ColumnSet(IDbSession dbSession)
        {
            _dbSession = dbSession;
            Init();
        }
        public void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }
        private void Init()
        {
            if (_initialized) return;
            lock (Locker)
            {
              
                    _allColumns = _dbSession.Query<FapColumn>("select * from FapColumn");
              
                _initialized = true;
            }
        }
        public IEnumerator<FapColumn> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allColumns.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allColumns.GetEnumerator();
        }

        public bool TryGetValue(string fid, out FapColumn fapColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allColumns.FirstOrDefault<FapColumn>(f => f.Fid == fid);
            if (result != null)
            {
                fapColumn = result;
                return true;
            }
            fapColumn = null;
            return false;
        }


        public bool TryGetValueByTable(string tableName, out List<FapColumn> fapColumns)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allColumns.Where<FapColumn>(c => c.TableName.Equals(tableName,StringComparison.CurrentCultureIgnoreCase));
            if (result != null&&result.Any())
            {
                fapColumns = result.ToList<FapColumn>();
                return true;
            }
            fapColumns = null;
            return false;
        }


        public bool TryGetValue(int id, out FapColumn fapColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allColumns.FirstOrDefault<FapColumn>(f => f.Id == id);
            if (result != null)
            {
                fapColumn = result;
                return true;
            }
            fapColumn = null;
            return false;
        }
    }
}
