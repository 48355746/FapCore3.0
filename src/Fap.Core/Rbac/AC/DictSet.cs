using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class DictSet : IDictSet
    {
        private IEnumerable<FapDict> _allDicts = new List<FapDict>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IDbSession _dbSession;


        internal DictSet(IDbSession dbSession)
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
                _allDicts = _dbSession.Query<FapDict>("select * from FapDict");
                //增加本身分类为字典
                var listCat = _dbSession.Query<FapDict>("select Category Code ,CategoryName Name,'FapDict' Category,'字典分类' CategoryName from FapDict group by Category,CategoryName");
                _allDicts = _allDicts.Union(listCat);
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out FapDict fapDict)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allDicts.FirstOrDefault<FapDict>(f => f.Fid == fid);
            if (result != null)
            {
                fapDict = result;
                return true;
            }
            fapDict = null;
            return false;
        }

        public IEnumerator<FapDict> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allDicts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allDicts.GetEnumerator();
        }


        public bool TryGetValueByCategory(string category, out IEnumerable<FapDict> fapDicts)
        {
            if (!_initialized)
            {
                Init();
            }
            fapDicts = _allDicts.Where(c => c.Category == category).OrderBy(c => c.SortBy);

            return fapDicts.Any();
        }


        public bool TryGetValueByCodeAndCategory(string code, string category, out FapDict fapDict)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allDicts.FirstOrDefault(c => c.Category == category && c.Code == code);
            if (result != null)
            {
                fapDict = result;
                return true;
            }
            fapDict = null;
            return false;
        }
    }
}
