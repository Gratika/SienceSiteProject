using apiServer.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace apiServer.Controllers.Redis
{
    public class RedisAuthController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisAuthController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }
        public bool IsUserUnique(string mypassword, string myemail)
        {
            var keys = _redis.GetServer("redis", 6379).Keys();
            int check = 0;

            // Итерация по всем ключам и получение данных
            foreach (var key in keys)
            {
                var userFields = _database.HashGetAll(key);
                // Проверка данных на совпадение              
                foreach (var hashEntry in userFields)
                {
                    if (hashEntry.Name.ToString() == "password" && hashEntry.Value == mypassword)
                    {
                        check++;
                    }
                    if (hashEntry.Name.ToString() == "email" && hashEntry.Value == myemail)
                    {
                        check++;
                    }
                }
                check = 0;
            }
            if (check == 2)
            {
                return true;
            }
            return false;
        }
    }
}
