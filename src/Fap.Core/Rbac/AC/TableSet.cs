using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    public class TableSet : ITableSet
    {
        private IEnumerable<FapTable> _allTables = new List<FapTable>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal TableSet(IDbSession dbSession)
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

                _allTables = _dbSession.Query<FapTable>($"select * from FapTable where ProductUid in('FAP','HCM')").ToList();

                _initialized = true;
            }
        }
        public IEnumerator<FapTable> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allTables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allTables.GetEnumerator();
        }

        public bool TryGetValue(string fid, out FapTable fapTable)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allTables.FirstOrDefault<FapTable>(f => f.Fid == fid);
            if (result != null)
            {
                fapTable = result;
                return true;
            }
            fapTable = null;
            return false;
        }


        public bool TryGetValueByName(string tableName, out FapTable fapTable)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allTables.FirstOrDefault<FapTable>(f => f.TableName.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                fapTable = result;
                return true;
            }
            fapTable = null;
            return false;
        }

        public bool TryGetValueByCategory(string category, out IEnumerable<FapTable> tables)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allTables.Where<FapTable>(f => f.TableCategory.Equals(category, StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                tables = result;
                return true;
            }
            tables = null;
            return false;
        }
       
        public IEnumerable<FapTable> TryGetValue(Func<FapTable, bool> predicate)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allTables.Where(predicate);
            return result;
        }
    }
}
