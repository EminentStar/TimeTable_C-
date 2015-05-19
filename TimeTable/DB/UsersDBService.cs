using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using System.Data;
using TimeTable.Models;
using TimeTable.Useful_Functions;

namespace TimeTable.DB
{
    public class UsersDBService
    {
        //Singleton pattern
        private static UsersDBService usersDBService = new UsersDBService();

        private UsersDBService()
        {
        }
        public static UsersDBService GetInstance()
        {
            return usersDBService;
        }
        JKAppExceptions jkAppExceptions = JKAppExceptions.GetInstance();

        public int CheckPasswd(User paramUser, SqlCeConnection con)
        {
            int idCount = 0;

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE user_id = " + paramUser.User_id +
                                  " AND user_passwd = '" + paramUser.User_passwd + "'";
                idCount = (Int32)cmd.ExecuteScalar(); //return the number of the first row in the first column
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return idCount;
        }

        public int SearchID(SqlCeConnection con, int param_id)
        {
            int idCount = 0;

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;
                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE user_id = " + param_id;
                idCount = (Int32)cmd.ExecuteScalar(); //return the number of the first field in the first row
                tran.Commit();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return idCount;
        }

        public void SelectID(SqlCeConnection con)
        {
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM Users WHERE user_id != 100000";

                SqlCeDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("──────────────────────────────────────────────────────");
                Console.WriteLine("│  학번  │  성명  │ 휴대폰 번호 │                    이메일 주소                   │수강학점│관심과목│");
                Console.WriteLine("──────────────────────────────────────────────────────");

                while (reader.Read())
                {
                    Console.Write("│");
                    Console.Write(reader["user_id"].ToString().PadLeft(8, ' '));
                    Console.Write("│");
                    Console.Write(jkAppExceptions.KoreanPadLeft(reader["user_name"].ToString(), 8, ' '));
                    Console.Write("│");
                    Console.Write(reader["user_phone"].ToString().PadLeft(13, ' '));
                    Console.Write("│");
                    Console.Write(reader["user_email"].ToString().PadLeft(50, ' '));
                    Console.Write("│");
                    Console.Write(reader["course_count"].ToString().PadLeft(8, ' '));
                    Console.Write("│");
                    Console.Write(reader["favorite_count"].ToString().PadLeft(8, ' '));
                    Console.Write("│");
                    Console.WriteLine("\n──────────────────────────────────────────────────────");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public User FetchID(SqlCeConnection con, int paramUserID)
        {
            User tempUser = new User();

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM Users WHERE user_id = " + paramUserID;

                SqlCeDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    tempUser.User_id = Convert.ToInt32(reader["user_id"]);
                    tempUser.User_name = Convert.ToString(reader["user_name"]);
                    tempUser.User_passwd = Convert.ToString(reader["user_passwd"]);
                    tempUser.User_phone = Convert.ToString(reader["user_phone"]);
                    tempUser.User_email = Convert.ToString(reader["user_email"]);
                    tempUser.User_admin = Convert.ToInt32(reader["user_admin"]);
                    tempUser.Check_signed_in = Convert.ToInt32(reader["check_signed_in"]);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return tempUser;
        }

        public int InsertID(User param_SignUpForm, SqlCeConnection con)
        {
            int returnValue = -1;

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                //cmd.CommandText = "INSERT INTO Users VALUES(101939,'박준규','1234qwer','010-4242-3843','junkyu@naver.com',0,0)";
                //cmd.CommandText = "INSERT INTO Users VALUES(000000,'관리자','00000000','010-0000-0000','admin@gmail.com',1,0)";


                cmd.CommandText = "INSERT INTO Users VALUES(" + param_SignUpForm.User_id +
                                                           ",'" + param_SignUpForm.User_name +
                                                           "','" + param_SignUpForm.User_passwd +
                                                           "','" + param_SignUpForm.User_phone +
                                                           "','" + param_SignUpForm.User_email +
                                                           "'," + param_SignUpForm.User_admin +
                                                           "," + param_SignUpForm.Check_signed_in +
                                                           "," + param_SignUpForm.Course_count +
                                                           "," + param_SignUpForm.Favorite_count +
                                                           " )";
                // 쿼리 실행
                returnValue = cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return returnValue;
        }

        public void ChangeSession(SqlCeConnection con, int paramInt, int session)
        {
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;
                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "UPDATE Users SET check_signed_in = " + session + " WHERE user_id = " + paramInt;
                // 쿼리 실행
                cmd.ExecuteNonQuery();
                // 커밋(전송)
                tran.Commit();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int GetFavoriteCount(SqlCeConnection con, int paramUser_id)
        {
            int returnValue = 0;
            try
            {
                // 데이터베이스 커맨드 생성
                SqlCeCommand cmd = new SqlCeCommand();

                // 커맨드에 커낵션을 연결
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "Select favorite_count FROM Users WHERE user_id = " + paramUser_id;

                SqlCeDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnValue = Convert.ToInt32(reader["favorite_count"]);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return returnValue;
        }

        public void IncreaseFavoriteCount(SqlCeConnection con, int paramUser_id, int paramGrade)
        {
            try
            {
                // 데이터베이스 커맨드 생성
                SqlCeCommand cmd = new SqlCeCommand();

                // 커맨드에 커낵션을 연결
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "UPDATE Users SET favorite_count = favorite_count + " + paramGrade + "WHERE user_id = " + paramUser_id;

                // 쿼리 실행
                cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DecreaseFavoriteCount(SqlCeConnection con, int paramUser_id, int paramGrade)
        {
            try
            {
                // 데이터베이스 커맨드 생성
                SqlCeCommand cmd = new SqlCeCommand();

                // 커맨드에 커낵션을 연결
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "UPDATE Users SET favorite_count = favorite_count - " + paramGrade + "WHERE user_id = " + paramUser_id;

                // 쿼리 실행
                cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int GetCourseCount(SqlCeConnection con, int paramUser_id)
        {
            int returnValue = 0;
            try
            {
                // 데이터베이스 커맨드 생성
                SqlCeCommand cmd = new SqlCeCommand();

                // 커맨드에 커낵션을 연결
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "Select course_count FROM Users WHERE user_id = " + paramUser_id;

                SqlCeDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnValue = Convert.ToInt32(reader["course_count"]);
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return returnValue;
        }

        public void IncreaseCourseCount(SqlCeConnection con, int paramUser_id, int paramGrade)
        {
            try
            {
                // 데이터베이스 커맨드 생성
                SqlCeCommand cmd = new SqlCeCommand();

                // 커맨드에 커낵션을 연결
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "UPDATE Users SET course_count = course_count + " + paramGrade + "WHERE user_id = " + paramUser_id;

                // 쿼리 실행
                cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DecreaseCourseCount(SqlCeConnection con, int paramUser_id, int paramGrade)
        {
            try
            {
                // 데이터베이스 커맨드 생성
                SqlCeCommand cmd = new SqlCeCommand();

                // 커맨드에 커낵션을 연결
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "UPDATE Users SET course_count = course_count - " + paramGrade + "WHERE user_id = " + paramUser_id;

                // 쿼리 실행
                cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int DeleteUser(SqlCeConnection con, int paramUserID)
        {
            int rv = 0;
            try
            {
                // 데이터베이스 커맨드 생성
                SqlCeCommand cmd = new SqlCeCommand();

                // 커맨드에 커낵션을 연결
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "DELETE FROM Users WHERE user_id = " + paramUserID;
                // 쿼리 실행
                cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rv;
        }
        //public void CreateIDTable(SqlCeConnection con)
        //{
        //    try
        //    {
        //        // 데이터베이스 커맨드 생성
        //        SqlCeCommand cmd = new SqlCeCommand();

        //        // 커맨드에 커낵션을 연결
        //        cmd.Connection = con;

        //        // 트랜잭션 생성
        //        SqlCeTransaction tran = con.BeginTransaction();
        //        cmd.Transaction = tran;

        //        //쿼리 생성 : Insert 쿼리
        //        cmd.CommandText = "CREATE TABLE Users (user_id int, user_name nvarchar(30), user_passwd nvarchar(30), user_phone nvarchar(30), user_email nvarchar(100), user_admin bit, check_signed_in bit, PRIMARY KEY(user_id) );";

        //        // 쿼리 실행
        //        cmd.ExecuteNonQuery();

        //        // 커밋(전송)
        //        tran.Commit();

        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        //public void AlterIDTable(SqlCeConnection con)
        //{
        //    try
        //    {
        //        // 데이터베이스 커맨드 생성
        //        SqlCeCommand cmd = new SqlCeCommand();

        //        // 커맨드에 커낵션을 연결
        //        cmd.Connection = con;

        //        // 트랜잭션 생성
        //        SqlCeTransaction tran = con.BeginTransaction();
        //        cmd.Transaction = tran;

        //        //쿼리 생성 : Insert 쿼리
        //        cmd.CommandText = "ALTER TABLE Users ADD favorite_count int ";

        //        // 쿼리 실행
        //        cmd.ExecuteNonQuery();

        //        // 커밋(전송)
        //        tran.Commit();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
    }
}
