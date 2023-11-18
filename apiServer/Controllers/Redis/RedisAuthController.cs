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
        public Users IsUserUnique(string mypassword, string myemail)
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
                    if (check == 2)
                    {
                        return GetUsersRedis(userFields);
                    }
                }
                check = 0;
            }

            return null;
        }
        public HashEntry[] GetUser(string key)
        {
            var db = _redis.GetDatabase();
            var hashEntries = db.HashGetAll(key);
            return hashEntries;
        }
        public Users GetUsersRedis(HashEntry[] userFields)
        {
            Users user = new Users();
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
                        case "firstname":
                            //user.firstname = fieldValue;
                            break;
                        case "name":
                           // user.name = fieldValue;
                            break;
                        case "birthday":
                           // user.birthday = Convert.ToDateTime(fieldValue);
                            break;
                        default:
                            // Обработка неизвестных полей, если необходимо
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
