using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace Enterprise.EmployeeManagement.DAL.RedisHelper
{
    public class RedisHelper
    {
        private static IDatabase redisDatabase;
        public static IDatabase database
        {
            get
            {
                return redisDatabase;
            }

        }
        static RedisHelper()
        {
            var connection = ConnectionMultiplexer.Connect("localhost:6379");
            redisDatabase = connection.GetDatabase();
        }
    }
}
