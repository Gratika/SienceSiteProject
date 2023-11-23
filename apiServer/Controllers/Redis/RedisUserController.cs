using apiServer.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace apiServer.Controllers.Redis
{
    public class RedisUserController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisUserController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }
        public void AddUser(Users user)
        {
            var userKey = $"users:{user.Id}";
            var userFields = new HashEntry[]
            {
            new HashEntry("id", Convert.ToString(user.Id)),
            new HashEntry("login", user.login),
            new HashEntry("password", user.password),
            new HashEntry("email", user.email),
            new HashEntry("date_create", Convert.ToString(user.date_create)),
            new HashEntry("modified_date", Convert.ToString(user.modified_date)),
            new HashEntry("role_id", user.role_id),
            new HashEntry("access_token", user.access_token),
            new HashEntry("refresh_token", user.refresh_token),
            new HashEntry("email_is_checked", user.email_is_checked)
            };

            _database.HashSet(userKey, userFields);
        }
        public HashEntry[] GetUser(string key)
        {
            var db = _redis.GetDatabase();
            var hashEntries = db.HashGetAll(key);
            return hashEntries;
        }
        public bool IsUserUnique(string mypassword, string myemail)
        {
            var keys = _redis.GetServer("redis", 6379).Keys();

            // Итерация по всем ключам и получение данных
            foreach (var key in keys)
            {
                var userFields = _database.HashGetAll(key);
                // Проверка данных на совпадение              
                foreach (var hashEntry in userFields)
                {
                    if (hashEntry.Name.ToString() == "password" && hashEntry.Value == mypassword)
                    {
                        return false;
                    }
                    if (hashEntry.Name.ToString() == "email" && hashEntry.Value == myemail)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public void DeleteKey(string key)
        {
            var db = _redis.GetDatabase();
            bool result = db.KeyDelete(key);
        }
        public Users GetUsersRedis(string id)
        {
            Users user = new Users();
            HashEntry[] userFields = GetUser("users:" + id);
            if (userFields.Length != 0)
            {
                foreach (var hashEntry in userFields)
                {
                    var fieldName = hashEntry.Name.ToString();
                    var fieldValue = hashEntry.Value.ToString();
                    switch (fieldName)
                    {
                        case "id":
                            user.Id = fieldValue;
                            break;
                        case "login":
                            user.login = fieldValue;
                            break;
                        case "password":
                            user.password = fieldValue;
                            break;
                        case "email":
                            user.email = fieldValue;
                            break;
                        case "date_create":
                            user.date_create = Convert.ToDateTime(fieldValue);
                            break;
                        case "modified_date":
                            user.modified_date = Convert.ToDateTime(fieldValue);
                            break;
                        case "role_id":
                            user.role_id = fieldValue;
                            break;
                        case "access_token":
                            user.access_token = fieldValue;
                            break;
                        case "refresh_token":
                            user.refresh_token = fieldValue;
                            break;
                        case "email_is_checked":
                            user.email_is_checked = Convert.ToInt32(fieldValue);
                            break;
                    }
                };
                return user;
            }
            user.email = "0";
            return user;
        }
    }
}
