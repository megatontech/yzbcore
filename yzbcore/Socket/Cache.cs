using K4os.Compression.LZ4.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yzbcore.Bussiness;
using yzbcore.Models;

namespace yzbcore.Socket
{
    public class Cache : ICache
    {
        public IEquipmentRepository _equipmentRepository;
        public IBirdhouseRepository _birdhouse;
        public Cache(IBirdhouseRepository birdhouse, IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
            _birdhouse = birdhouse;
        }

        public Task DelCache()
        {
            throw new NotImplementedException();
        }

        public Task<UserCacheModel> FillCacheWithToken(string token, Member member)
        {
            UserCacheModel model = new UserCacheModel();
            model.token = token;
            model.member = member;
            model.id = member.id;
            model.birdhouses = new List<Birdhouse>();
            #region birdhouses
            model.birdhouses.AddRange(_birdhouse.GetAllByMember(member.id.ToInt()));
            #endregion
            #region CacheModel
            model.cacheModels = new List<CacheModel>();
            foreach (var item in model.birdhouses)
            {
                if (CacheUntity.Exists(item.equipment_id))
                {
                    var devicecache = CacheUntity.GetCache<CacheModel>(item.equipment_id);
                    model.cacheModels.Add(devicecache);
                }
                else 
                {
                    var cacheModel = new CacheModel();
                    cacheModel.serial = item.equipment_id;
                    var equip = _equipmentRepository.GetCacheModelByserial(cacheModel.serial);
                    cacheModel.bind_status = equip.bind_status;
                    cacheModel.birdhouse_name = equip.birdhouse_name;
                    cacheModel.phone = 5;
                    cacheModel.sms = 5;
                    cacheModel.smsEnableFlag = true;
                    cacheModel.call_num = 0;
                    cacheModel.max_humidity = item.max_humidity;
                    cacheModel.min_humidity = item.min_humidity;
                    cacheModel.max_temperature = item.max_temperature;
                    cacheModel.min_temperature = item.min_temperature;
                    cacheModel.warning_ammonia_concentration = item.warning_ammonia_concentration;
                    cacheModel.warning_negative_pressure = item.warning_negative_pressure;
                    cacheModel.name = item.name;
                    cacheModel.status = equip.status;
                    cacheModel.username = member.username;
                    cacheModel.warning_mobile = equip.warning_mobile;
                    cacheModel.currentTemp = "0";
                    cacheModel.currentHumidity = "0";
                    cacheModel.currentPower = "0";
                    cacheModel.setDate = DateTime.Now.Date;
                    model.cacheModels.Add(cacheModel);
                }
                
            }
            
            #endregion

            return Task.FromResult( model);
        }

        public Task<IEnumerable<CacheModel>> GetCache()
        {
            List<CacheModel> models = new List<CacheModel>();
            return _equipmentRepository.CacheList();
        }

        public Task<CacheModel> GetCacheByToken(string token)
        {
            throw new NotImplementedException();
        }

        public Task InitCache()
        {
            throw new NotImplementedException();
        }

        public Task InsertCache()
        {
            throw new NotImplementedException();
        }

        public Task InsertCache(UserCacheModel model)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCache(CacheModel cachemodel)
        {
            throw new NotImplementedException();
        }

        Task<UserCacheModel> ICache.GetCacheByToken(string token)
        {
            throw new NotImplementedException();
        }
    }
    public interface ICache
    {
        public Task UpdateCache(CacheModel cachemodel);
        public Task InsertCache(UserCacheModel model);
        public Task InitCache();
        public Task DelCache();
        public Task<IEnumerable<CacheModel>> GetCache();
        public Task<UserCacheModel> GetCacheByToken(string token);
        public Task<UserCacheModel> FillCacheWithToken(string token,Member member);
       

    }
    public class CacheUntity
    {
        private static ICacheHelper _cache = new RedisCacheHelper();


        private static bool isInited = false;
        public static void Init(ICacheHelper cache)
        {
            if (isInited)
                return;
            _cache.Dispose();
            _cache = cache;
            isInited = true;
        }


        public static bool Exists(string key)
        {
            return _cache.Exists(key);
        }


        public static T GetCache<T>(string key) where T : class
        {
            return _cache.GetAsync<T>(key).Result;
        }


        public static void SetCache(string key, object value)
        {
            _cache.AddAsync(key, value);
        }


        public static void SetCache(string key, object value, DateTimeOffset expiressAbsoulte)
        {
            _cache.SetCache(key, value, expiressAbsoulte);
        }


        //public void SetCache(string key, object value, double expirationMinute)
        //{


        //}


        public static void RemoveCache(string key)
        {
            _cache.RemoveCache(key);
        }


    }


    public class MemoryCacheHelper : ICacheHelper
    {
        public MemoryCacheHelper()//这里可以做成依赖注入，但没打算做成通用类库，所以直接把选项直接封在帮助类里边
        {
            //this._cache = new MemoryCache(options);
            this._cache = new MemoryCache(new MemoryCacheOptions());
        }


        private IMemoryCache _cache;


        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            object v = null;
            return this._cache.TryGetValue<object>(key, out v);
        }


        public T GetCache<T>(string key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            T v = null;
            this._cache.TryGetValue<T>(key, out v);


            return v;
        }


        public void SetCache(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);
            this._cache.Set<object>(key, value);
        }


        public void SetCache(string key, object value, double expirationMinute)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);
            DateTime now = DateTime.Now;
            TimeSpan ts = now.AddMinutes(expirationMinute) - now;
            this._cache.Set<object>(key, value, ts);
        }


        public void SetCache(string key, object value, DateTimeOffset expirationTime)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            object v = null;
            if (this._cache.TryGetValue(key, out v))
                this._cache.Remove(key);


            this._cache.Set<object>(key, value, expirationTime);
        }


        public void RemoveCache(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            this._cache.Remove(key);
        }


        public void Dispose()
        {
            if (_cache != null)
                _cache.Dispose();
            GC.SuppressFinalize(this);
        }

        bool ICacheHelper.SetCache(string key, object value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddAsync(string key, object value)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }
    }
    public class RedisCacheHelper : ICacheHelper
    {
        public RedisCacheHelper(/*RedisCacheOptions options, int database = 0*/)//这里可以做成依赖注入，但没打算做成通用类库，所以直接把连接信息直接写在帮助类里
        {
            RedisCacheOptions options = new RedisCacheOptions();
            options.Configuration = "127.0.0.1:6379";
            options.InstanceName = "yzb";
            //options.ConfigurationOptions.ConnectTimeout = 15000;
            //options.ConfigurationOptions.SyncTimeout = 5000;
            //options.ConfigurationOptions.ResponseTimeout = 15000;
            //options.ConfigurationOptions.EndPoints = { connectionString }// connectionString 为IP:Port 如”192.168.2.110:6379”
            //var csredis = new CSRedis.CSRedisClient("127.0.0.1:6379,defaultDatabase=1,poolsize=50,ssl=false,writeBuffer=10240,prefix=yzb");
            //RedisHelper.Initialization(csredis);
            //var config = new ConfigurationOptions
            //{
            //    AbortOnConnectFail = false,
            //    AllowAdmin = true,
            //    ConnectTimeout = 15000,
            //    SyncTimeout = 5000,
            //    ResponseTimeout = 15000,
            //    Password = "Pwd",//Redis数据库密码
            //    EndPoints = { "127.0.0.1:6379" }// connectionString 为IP:Port 如”192.168.2.110:6379”
            //};
            int database = 0;
            _connection = ConnectionMultiplexer.Connect(options.Configuration);
            _cache = _connection.GetDatabase(database);
            _instanceName = options.InstanceName;
        }


        private IDatabase _cache;


        private ConnectionMultiplexer _connection;


        private readonly string _instanceName;


        private string GetKeyForRedis(string key)
        {
            return _instanceName + key;
        }


        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            return _cache.KeyExists(GetKeyForRedis(key));
        }


        public T GetCache<T>(string key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            var value = _cache.StringGet(GetKeyForRedis(key));
            if (!value.HasValue)
                return default(T);


            return JsonConvert.DeserializeObject<T>(value);
        }


        public bool SetCache(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            if (Exists(GetKeyForRedis(key)))
                RemoveCache(GetKeyForRedis(key));


            _cache.StringSet(GetKeyForRedis(key), JsonConvert.SerializeObject(value));
            return true;
        }
        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public async Task<bool> AddAsync(string key, object value)
        {
            return await Task.Run(() => SetCache(key, value));
        }
        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            return await Task.Run(() => GetCache<T>(key));
        }
        public void SetCache(string key, object value, DateTimeOffset expiressAbsoulte)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            if (Exists(GetKeyForRedis(key)))
                RemoveCache(GetKeyForRedis(key));


            _cache.StringSet(GetKeyForRedis(key), JsonConvert.SerializeObject(value), expiressAbsoulte - DateTime.Now);
        }


        public void SetCache(string key, object value, double expirationMinute)
        {
            if (Exists(GetKeyForRedis(key)))
                RemoveCache(GetKeyForRedis(key));


            DateTime now = DateTime.Now;
            TimeSpan ts = now.AddMinutes(expirationMinute) - now;
            _cache.StringSet(GetKeyForRedis(key), JsonConvert.SerializeObject(value), ts);
        }


        public void RemoveCache(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));


            _cache.KeyDelete(GetKeyForRedis(key));
        }


        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            GC.SuppressFinalize(this);
        }
        
    }


    public interface ICacheHelper
    {
        bool Exists(string key);


        T GetCache<T>(string key) where T : class;


        bool SetCache(string key, object value);


        void SetCache(string key, object value, DateTimeOffset expiressAbsoulte);//设置绝对时间过期


        //void SetCache(string key, object value, double expirationMinute);  //设置滑动过期， 因redis暂未找到自带的滑动过期类的API，暂无需实现该接口


        void RemoveCache(string key);
        public  Task<bool> AddAsync(string key, object value);
        public Task<T> GetAsync<T>(string key) where T : class;
        void Dispose();
    }

}
