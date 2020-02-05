using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTS.Parser;

namespace VTS.Web.Helpers
{
    public class DataAIS
    {
        public long Id { get; set; }
        public string Tipe { get; set; }
        public AisMessage Data { get; set; }
    }
    public interface INoSQLDataAccess
    {
        bool InsertBulkData<T>(IEnumerable<T> data);
        bool InsertData<T>(T data);
        bool DeleteAllData<T>();
        bool DeleteData<T>(long id);
        bool DeleteDataBulk<T>(IEnumerable<T> Ids);
        List<T> GetAllData<T>();
        T GetDataById<T>(long Id);
        List<T> GetDataByIds<T>(params long[] Ids);
        List<T> GetAllData<T>(int Limit);
        List<T> GetDataByStartId<T>(int Limit, long StartId);
        long GetSequence<T>();
    }
    public class RedisDB : INoSQLDataAccess
    {
        public int DbSelected = 1;
        public static IRedisClientsManager redisManager;
        string RedisConStr { set; get; }
        public RedisDB(string RedisConnectionString, int DBSel = 1)
        {
            DbSelected = DBSel;
            RedisConStr = RedisConnectionString;

            if (redisManager == null)
            {
                redisManager = new PooledRedisClientManager(DbSelected, RedisConStr);

            }

        }

        public bool InsertBulkData<T>(IEnumerable<T> data)
        {


            using (var redis = redisManager.GetClient())
            {
                var redisStream = redis.As<T>();
                redisStream.StoreAll(data);
                return true;
            }
        }

        public bool InsertData<T>(T data)
        {
            try
            {
                if (data == null) return false;

                using (var redis = redisManager.GetClient())
                {
                    var redisStream = redis.As<T>();
                    redisStream.Store(data);
                    return true;
                }
            }
            catch
            {
                //print ke log
                //throw;
                return false;
            }
        }

        public bool DeleteData<T>(long id)
        {
            try
            {

                using (var redis = redisManager.GetClient())
                {
                    var redisStream = redis.As<T>();
                    redisStream.DeleteById(id);
                    return true;
                }
            }
            catch
            {
                //print ke log
                //throw;
                return false;
            }
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public bool DeleteAllData<T>()
        {
            try
            {

                using (var redis = redisManager.GetClient())
                {
                    IRedisTypedClient<T> redis1 = redis as IRedisTypedClient<T>;

                    var datas = redis1.GetAll();
                    foreach (var item in datas)
                    {
                        redis1.Delete(item);
                    }

                    return true;
                }
            }
            catch
            {
                //print ke log
                //throw;
                return false;
            }
        }
        public bool DeleteDataBulk<T>(IEnumerable<T> Ids)
        {
            try
            {

                using (var redis = redisManager.GetClient())
                {
                    var redisStream = redis.As<T>();
                    redisStream.DeleteByIds(Ids);
                    return true;
                }
            }
            catch
            {
                //print ke log
                //throw;
                return false;
            }
        }

        public T GetDataById<T>(long Id)
        {

            using (var redis = redisManager.GetClient())
            {
                var redisStream = redis.As<T>();

                return redisStream.GetById(Id);

            }
        }
        public List<T> GetDataByIds<T>(params long[] Ids)
        {

            using (var redis = redisManager.GetClient())
            {
                var redisStream = redis.As<T>();

                var data = from c in redisStream.GetByIds(Ids)
                           select c;
                return data.ToList();

            }
        }
        public List<T> GetDataByStartId<T>(int Limit, long StartId)
        {

            using (var redis = redisManager.GetClient())
            {
                var redisStream = redis.As<T>();

                var data = from c in redisStream.GetAll()
                           where Convert.ToInt32(GetPropValue(c, "Id")) >= StartId
                           select c;
                return data.Take(Limit).ToList();
            }

        }
        public List<T> GetAllData<T>(int Limit)
        {

            using (var redis = redisManager.GetClient())
            {
                var redisStream = redis.As<T>();

                var data = from c in redisStream.GetAll()
                           select c;

                return data == null ? null : data.TakeLast(Limit).ToList();

            }
        }
        public List<T> GetAllData<T>()
        {

            using (var redis = redisManager.GetClient())
            {

                var redisStream = redis.As<T>();

                var data = from c in redisStream.GetAll()
                           select c;
                return data.ToList();

            }
        }

        public long GetSequence<T>()
        {

            using (var redis = redisManager.GetClient())
            {

                var redisStream = redis.As<T>();
                long Id = redisStream.GetNextSequence();
                return Id;

            }
        }

    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(toCheck)) return false;
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}