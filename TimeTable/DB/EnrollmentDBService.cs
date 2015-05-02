using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using TimeTable.Models;
using TimeTable.Useful_Functions;

namespace TimeTable.DB
{
    public class EnrollmentDBService
    {
        private static EnrollmentDBService enrollmentDBService = new EnrollmentDBService();
        private EnrollmentDBService()
        {
        }
        public static EnrollmentDBService GetInstance()
        {
            return enrollmentDBService;
        }

        JKAppExceptions jkAppExceptions = JKAppExceptions.GetInstance();
        UsersDBService usersDBService = UsersDBService.GetInstance();

        public int CheckLectureRegistered(SqlCeConnection con, int paramUserID, string paramCourse_code)
        {
            int returnValue = 0;
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM Enrollment WHERE user_id = " + paramUserID + " AND course_code = '" + paramCourse_code + "'";

                returnValue = (Int32)cmd.ExecuteScalar(); //return the number of the first row in the first column
            }
            catch (System.Exception ex)
            {
                //The lecture hasn't registered yet by the student
                //Console.WriteLine(ex.Message);
            }
            return returnValue;
        }

        public int SelectFavoriteLecture(SqlCeConnection con, int paramUser_id)
        {
            int returnValue = 0;
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM FavoriteEnrollment WHERE user_id = " + paramUser_id;

                returnValue = (Int32)cmd.ExecuteScalar(); //return the number of the first row in the first column

                SqlCeDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("{0} 의 관심과목 신청 학점 : {1}", paramUser_id, usersDBService.GetFavoriteCount(con, paramUser_id));

                LecturesAttributes();

                while (reader.Read())
                {
                    SelectLecturesLogic(reader);
                }
            }
            catch (System.Exception ex)
            {
                //Console.WriteLine(ex.Message);
                Console.WriteLine("관심과목 등록 목록이 없습니다.\n");
            }
            return returnValue;
        }

        public int SelectLecture(SqlCeConnection con, int paramUser_id)
        {
            int returnValue = 0;

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM Enrollment WHERE user_id = " + paramUser_id;
                
                returnValue = (Int32)cmd.ExecuteScalar(); //return the number of the first row in the first column
                
                SqlCeDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("{0} 의 수강신청 학점 : {1}",paramUser_id, usersDBService.GetCourseCount(con, paramUser_id));

                LecturesAttributes();

                while (reader.Read())
                {
                    SelectLecturesLogic(reader);
                }

                Console.WriteLine();
            }
            catch (System.Exception ex)
            {
                //Console.WriteLine(ex.Message);
                Console.WriteLine("수강 내역이 없습니다.\n");
            }
            return returnValue;
        }

        public void SelectLecturesLogic(SqlCeDataReader paramReader)
        {
            Console.Write("│"); Console.Write(paramReader["c_index"]);
            Console.Write("│"); Console.Write(paramReader["course_code"].ToString().PadLeft(8, ' '));
            Console.Write("│"); Console.Write(paramReader["class_number"].ToString().PadLeft(4, ' '));
            Console.Write("│"); Console.Write(jkAppExceptions.BytesPadLeft(paramReader["course_name"].ToString(), 25, ' '));
            Console.Write("│"); Console.Write(jkAppExceptions.BytesPadLeft(paramReader["comp_div"].ToString(), 8, ' '));
            Console.Write("│"); Console.Write(paramReader["grade"].ToString().PadLeft(14, ' '));
            Console.Write("│"); Console.Write(paramReader["year"].ToString().PadLeft(4, ' '));
            Console.Write("│"); Console.Write(jkAppExceptions.KoreanPadLeft(paramReader["relate_major"].ToString(), 16, ' '));
            Console.Write("│"); Console.Write(jkAppExceptions.KoreanPadLeft(paramReader["professor"].ToString(), 6, ' '));
            Console.Write("│"); Console.Write(jkAppExceptions.BytesPadLeft(paramReader["course_time"].ToString(), 16, ' '));
            Console.Write("│"); Console.Write(jkAppExceptions.BytesPadLeft(paramReader["classroom"].ToString(), 13, ' '));
            Console.Write("│\n");
            Console.WriteLine("────────────────────────────────────-──────────────────────────────────");
        }

        public void LecturesAttributes()
        {
            Console.WriteLine("──────────────────────────────────────────────────────────────────────");
            Console.WriteLine("│ NO │학수번호│분반│        교과목명         │이수구분│학점/이론/실습│학년│    주관학과    │교수명│요일 및 강의시간│   강의실    │");
            Console.WriteLine("──────────────────────────────────────────────────────────────────────");
        }

        public int GetGrade(SqlCeConnection con, int paramUser_id, int paramC_index)
        {
            int returnGrade = 0;
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM FavoriteEnrollment WHERE user_id = " + paramUser_id + " AND c_index = " + paramC_index;

                SqlCeDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnGrade = Convert.ToInt32(reader["grade"].ToString().Substring(0, 1));
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return returnGrade;
        }

        public Enrollment FetchFavoriteEnrollment(SqlCeConnection con, int paramUser_id, int paramC_index)
        {
            Enrollment tempEnrollment = null;

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM FavoriteEnrollment WHERE user_id = " + paramUser_id + " AND c_index = " + paramC_index;

                SqlCeDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    tempEnrollment = new Enrollment();
                    FetchLogic(tempEnrollment, reader);
                }
            }
            catch (System.Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
            return tempEnrollment;
        }

        public Enrollment FetchEnrollment(SqlCeConnection con, int paramUser_id, int paramC_index)
        {
            Enrollment tempEnrollment = null;

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM Enrollment WHERE user_id = " + paramUser_id + " AND c_index = " + paramC_index;

                SqlCeDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    tempEnrollment = new Enrollment();
                    FetchLogic(tempEnrollment, reader);
                }
            }
            catch (System.Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
            return tempEnrollment;
        }

        public Dictionary<string,string> FetchLectureDic(SqlCeConnection con, int paramUser_id)
        {
            Enrollment tempEnrollment = new Enrollment();
            Dictionary<string, string> lecTimeList = new Dictionary<string, string>();

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT * FROM Enrollment WHERE user_id = " + paramUser_id ;

                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lecTimeList.Add(reader["course_name"].ToString(), reader["course_time"].ToString());
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lecTimeList;
        }

        public void FetchLogic(Enrollment paramTempEnrollment, SqlCeDataReader paramReader)
        {
            paramTempEnrollment.C_index = Convert.ToInt32(paramReader["c_index"]);
            paramTempEnrollment.User_id = Convert.ToInt32(paramReader["user_id"]);
            paramTempEnrollment.Host_major = paramReader["host_major"].ToString();
            paramTempEnrollment.Course_code = paramReader["course_code"].ToString();
            paramTempEnrollment.Class_number = paramReader["class_number"].ToString();
            paramTempEnrollment.Course_name = paramReader["course_name"].ToString();
            paramTempEnrollment.Comp_div = paramReader["comp_div"].ToString();
            paramTempEnrollment.Grade = paramReader["grade"].ToString();
            paramTempEnrollment.Year = Convert.ToInt32(paramReader["year"]);
            paramTempEnrollment.Relate_major = paramReader["relate_major"].ToString();
            paramTempEnrollment.Professor = paramReader["professor"].ToString();
            paramTempEnrollment.Course_time = paramReader["course_time"].ToString();
            paramTempEnrollment.Classroom = paramReader["classroom"].ToString();
        }

        public void InsertFavoriteLecture(SqlCeConnection con, Enrollment paramEnrollment)
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
                cmd.CommandText = "INSERT INTO FavoriteEnrollment VALUES(" + paramEnrollment.C_index +
                                                         "," + paramEnrollment.User_id +
                                                        ",'" + paramEnrollment.Host_major +
                                                       "','" + paramEnrollment.Course_code +
                                                       "','" + paramEnrollment.Class_number +
                                                       "','" + paramEnrollment.Course_name +
                                                       "','" + paramEnrollment.Comp_div +
                                                       "','" + paramEnrollment.Grade +
                                                        "'," + paramEnrollment.Year +
                                                        ",'" + paramEnrollment.Relate_major +
                                                       "','" + paramEnrollment.Professor +
                                                       "','" + paramEnrollment.Course_time +
                                                       "','" + paramEnrollment.Classroom +
                                                       "','" + paramEnrollment.English +
                                                        "');";

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

        public int DeleteFavoriteLecture(SqlCeConnection con, int paramUser_id, int tempInt)
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
                cmd.CommandText = "DELETE FROM FavoriteEnrollment WHERE user_id = " +
                                    paramUser_id + " AND c_index = " + tempInt;
                // 쿼리 실행
                rv = cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rv;
        }

        public int DeleteLecture(SqlCeConnection con, int paramUser_id, int tempInt)
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
                cmd.CommandText = "DELETE FROM Enrollment WHERE user_id = " +
                                    paramUser_id + " AND c_index = " + tempInt;
                // 쿼리 실행
                rv = cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rv;
        }

        public void InsertLecture(SqlCeConnection con, Enrollment paramEnrollment)
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
                cmd.CommandText = "INSERT INTO Enrollment VALUES(" + paramEnrollment.C_index +
                                                         "," + paramEnrollment.User_id +
                                                        ",'" + paramEnrollment.Host_major +
                                                       "','" + paramEnrollment.Course_code +
                                                       "','" + paramEnrollment.Class_number +
                                                       "','" + paramEnrollment.Course_name +
                                                       "','" + paramEnrollment.Comp_div +
                                                       "','" + paramEnrollment.Grade +
                                                        "'," + paramEnrollment.Year +
                                                        ",'" + paramEnrollment.Relate_major +
                                                       "','" + paramEnrollment.Professor +
                                                       "','" + paramEnrollment.Course_time +
                                                       "','" + paramEnrollment.Classroom +
                                                       "','" + paramEnrollment.English +
                                                        "');";
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

        public void testQuery(SqlCeConnection con, string p)
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
                cmd.CommandText = p;
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

        public List<string> GetCourse_timeList(SqlCeConnection con, int paramUserID)
        {
            List<string> courseTimeList = new List<string>();

            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = con;

                // 트랜잭션 생성
                SqlCeTransaction tran = con.BeginTransaction();
                cmd.Transaction = tran;

                //쿼리 생성 : Insert 쿼리
                cmd.CommandText = "SELECT course_time FROM Enrollment WHERE user_id = " + paramUserID ;

                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    courseTimeList.Add(reader["course_time"].ToString());
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return courseTimeList;
        }

        public int DeleteAllLectures(SqlCeConnection con, int paramUserID)
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
                cmd.CommandText = "DELETE FROM Enrollment WHERE user_id = " + paramUserID ;
                // 쿼리 실행
                rv = cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return rv;
        }

        public int DeleteAllFavoriteLectures(SqlCeConnection con, int paramUserID)
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
                cmd.CommandText = "DELETE FROM FavoriteEnrollment WHERE user_id = " + paramUserID;
                // 쿼리 실행
                rv = cmd.ExecuteNonQuery();

                // 커밋(전송)
                tran.Commit();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rv;
        }

        //public void CreateEnrollmentTable(SqlCeConnection con)
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
        //        cmd.CommandText = "CREATE TABLE Enrollment (c_index int, user_id int ,host_major nvarchar(50),course_code nvarchar(10), class_number nvarchar(10),course_name nvarchar(50),comp_div nvarchar(10), grade nvarchar(20),year int, relate_major nvarchar(50), professor nvarchar(10),course_time nvarchar(30),classroom nvarchar(20),english nvarchar(10), PRIMARY KEY(course_code, user_id), FOREIGN KEY(user_id) REFERENCES Users(user_id));";

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

        //public void CreateFavoriteEnrollmentTable(SqlCeConnection con)
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
        //        cmd.CommandText = "CREATE TABLE FavoriteEnrollment (c_index int, user_id int,host_major nvarchar(50),course_code nvarchar(10), class_number nvarchar(10),course_name nvarchar(50),comp_div nvarchar(10), grade nvarchar(20),year int, relate_major nvarchar(50), professor nvarchar(10),course_time nvarchar(30),classroom nvarchar(20),english nvarchar(10),PRIMARY KEY(c_index, user_id),FOREIGN KEY(user_id) REFERENCES Users(user_id));";

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
