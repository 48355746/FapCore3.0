using Ardalis.GuardClauses;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Infrastructure.Cache
{
    public  class DistributedCacheService : ICacheService
    {
        protected IDistributedCache _cache;

        public DistributedCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public DistributedCacheService()
        {
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public bool Add(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _cache.Set(key, Serialize(value));
            return true;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _cache.Set(key, Serialize(value),
                    new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(expiresSliding)
                    .SetAbsoluteExpiration(expiressAbsoulte)
                    );

            return true;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (isSliding)
                _cache.Set(key,
                   Serialize(value),
                    new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(expiresIn)
                    );
            else
                _cache.Set(key, Serialize(value),
                    new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(expiresIn)
                );

            return true;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _cache.Remove(key);

            return true;
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            keys.ToList().ForEach(item => _cache.Remove(item));
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get(key) as T;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get(key);
        }
        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            var dict = new Dictionary<string, object>();

            keys.ToList().ForEach(item => dict.Add(item, _cache.Get(item)));

            return dict;
        }
       

        public async Task<bool> AddAsync(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            await _cache.SetAsync(key, Serialize(value));
            return true;
        }

        public async Task<bool> AddAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            Guard.Against.NullOrWhiteSpace(key,key);
            Guard.Against.Null(value, "value");
            await _cache.SetAsync(key, Serialize(value),
               new DistributedCacheEntryOptions()
               .SetSlidingExpiration(expiresSliding)
               .SetAbsoluteExpiration(expiressAbsoulte)
               );
            return true;
        }

        public async Task<bool> AddAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (isSliding)
               await _cache.SetAsync(key,
                   Serialize(value),
                    new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(expiresIn)
                    );
            else
              await  _cache.SetAsync(key, Serialize(value),
                    new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(expiresIn)
                );

            return true;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            Guard.Against.Null(key, "key");
            await _cache.RemoveAsync(key);
            return true;
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys)
        {
            Guard.Against.Null(keys, "keys");
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key);
            }
            
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            Guard.Against.Null(key, "key");
            var result=await _cache.GetAsync(key);
            if (result != null)
            {
                return Deserialize(result) as T;
            }
            return null;
        }

        public async Task<object> GetAsync(string key)
        {
            Guard.Against.Null(key, "key");
            var result = await _cache.GetAsync(key);
            if (result != null)
            {
                return Deserialize(result);
            }
            return null;
        }
       


        private static byte[] Serialize(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using MemoryStream stream = new MemoryStream();
            bf.Serialize(stream, obj);
            byte[] datas = stream.ToArray();
            return datas;
        }
        private static object Deserialize(byte[] datas)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using MemoryStream stream = new MemoryStream(datas, 0, datas.Length);
            object obj = bf.Deserialize(stream);

            return obj;
        }

       
    }
}
