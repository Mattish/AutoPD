using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using System.Threading;

namespace Perfect_Dark_Automation {
    public static class Redis {
        public static string host = "127.0.0.1";
        public static string key = "";
        public static int port = 6379;
        public static Thread redis;
        public static int lastCount;
        delegate List<string> GetNewHashesDelegate(out int i, int lastCount, string key);

        public static List<string> GetNewHashes(out int count, int lastCount, string key) {
            try {
                using (var redisClient = new RedisClient(host, port)) {
                    count = redisClient.GetSortedSetCount(key);
                    List<string> list = redisClient.GetRangeFromSortedSet(key, lastCount, count);
                    return list;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
            count = -1;
            return null;
        }

        public static void Do_It() {
            int i;
            GetNewHashesDelegate gnhd = new GetNewHashesDelegate(GetNewHashes);
            AsyncCallback acb = new AsyncCallback(GetNewHashesCallBack);
            IAsyncResult result = gnhd.BeginInvoke(out i, lastCount, key, acb, gnhd);
        }

        public static void GetNewHashesCallBack(IAsyncResult result) {
            GetNewHashesDelegate gnhd = (GetNewHashesDelegate)result.AsyncState;
            int count;
            List<string> list = gnhd.EndInvoke(out count, result);
            lastCount = count;
            if (list.Count > 0)
                Log.WriteLine("Redis update - Got " + list.Count + " new hashes");
            ProcessNewHashes(list);
        }

        public static void ProcessNewHashes(List<string> list) {
            foreach (string hash in list) {
                if (hash.Length > 61) {
                    Filters.Add(new Filter("", "", hash, false));
                }
            }
            Filters.UpdateLabel();
        }

        public static int CheckRedisServer(string host, int port, string key) {
            try {
                using (var redisClient = new RedisClient(host, port)) {
                    Redis.host = host;
                    Redis.port = port;
                    Redis.key = key;
                    int i = redisClient.GetSortedSetCount(key);
                    return i;
                }
            }
            catch (Exception) {
                return -1;
            }
        }

    }
}
