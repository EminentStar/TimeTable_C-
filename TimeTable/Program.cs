using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DB;
using TimeTable.Models;
using TimeTable.Services;
using TimeTable.Useful_Functions;


namespace TimeTable
{
    class Program
    {
        static void Main(string[] args)
        {
            HomeService homeService = HomeService.GetInstance();
            UsersDBService u = UsersDBService.GetInstance();
            JKAppExceptions jkAppExceptions = JKAppExceptions.GetInstance();

            string dbName = "Data Source = database.sdf;";
            if (!File.Exists("database.sdf"))
            {
                SqlCeEngine engine = new SqlCeEngine(dbName);
                engine.CreateDatabase(); // db 파일 생성  -> 1번만 해야됨
            }
            SqlCeConnection con = new SqlCeConnection(dbName);
            con.Open();

            homeService.LoginEntry(con);
            Console.WriteLine("프로그램을 종료합니다.");

            con.Close();
        }
    }
}