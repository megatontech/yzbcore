using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Repository
{
    public class ConnectionProvider: IConnectionProvider
    {
        private string _connectionString;
        /// <summary>
        /// 加载配置文件，构建IConfigurationRoot
        /// </summary>
        private static readonly IConfigurationBuilder ConfigurationBuilder = new ConfigurationBuilder();

        /// <summary>
        /// 获取配置文件中的内容，继承自IConfiguration
        /// </summary>
        private static IConfigurationRoot _configuration;

        public ConnectionProvider()
        {
            
        }

        public IDbConnection GetDbConnection()
        {
            if (!string.IsNullOrEmpty(_connectionString))
            { 
                return new MySqlConnection(_connectionString); 
            }
            else 
            {
                //_configuration = ConfigurationBuilder
                //.SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile(cfg =>
                //{
                //    cfg.Path = "db.json";
                //    cfg.ReloadOnChange = true;
                //    cfg.Optional = false;
                //}).Build();
                //var url = _configuration.GetSection("url");
                ////读取json对象，在IConfiguration中，配置文件中的key都是扁平化的
                //var pAge = _configuration["person:age"];

                ////遍历配置文件中的所有元素
                //foreach (var keyValuePair in _configuration.AsEnumerable())
                //{
                //    Console.WriteLine($"{keyValuePair.Key} - {keyValuePair.Value}");
                //}
                _connectionString = ConfigHelper.GetSectionValue("dbconnstr");
                return new MySqlConnection(_connectionString); 
            }
            
        }
    }

    public interface IConnectionProvider 
    {
        public IDbConnection GetDbConnection();
    }
}
