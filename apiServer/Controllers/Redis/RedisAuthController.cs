using apiServer.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisAuthController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly RedisUserController _redisUser;

        public RedisAuthController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
            _redisUser = new RedisUserController("redis:6379,abortConnect=false");
        }
        [HttpPost("IsUserUnique")]
        public Users IsUserUnique(string mypassword, string myemail)
        {
            var keys = _redis.GetServer("redis", 6379).Keys();
            int check = 0;
            string id = "";

            // Итерация по всем ключам и получение данных
            foreach (var key in keys)
            {
                var userFields = _database.HashGetAll(key);
                // Проверка данных на совпадение              
                foreach (var hashEntry in userFields)
                {
                    if (hashEntry.Name.ToString() == "id")
                    {
                        id = hashEntry.Value;
                    }
                    if (hashEntry.Name.ToString() == "password" && hashEntry.Value == mypassword)
                    {
                        check++;
                    }
                    if (hashEntry.Name.ToString() == "email" && hashEntry.Value == myemail)
                    {
                        check++;
                    }
                    if (check == 2)
                    {
                        return _redisUser.GetUsersRedis(id);
                    }
                }
                check = 0;
            }

            return null;
        }
        [HttpPost("AddUser")]
        public void AddUser(Users user)
        {
            _redisUser.AddUser(user);
        }
    }
}
