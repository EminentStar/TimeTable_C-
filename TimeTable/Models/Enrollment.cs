using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTable.Models
{
    public class Enrollment
    {
        private int c_index;           
        private int user_id;           
        private string host_major;     
        private string course_code;    
        private string class_number;   
        private string course_name;    
        private string comp_div;       
        private string grade;          
        private int year;              
        private string relate_major;   
        private string professor;      
        private string course_time;    
        private string classroom;      
        private string english;        

        public Enrollment()
        {

        }
        public Enrollment(int paramC_index, int paramUser_id, string paramHost_major,
                          string paramCourse_code, string param_class_number,
                          string paramCourse_name, string paramComp_div, string paramGrade,
                          int paramYear, string paramRelate_major, string paramProfessor,
                          string paramCourse_time, string paramClassroom)
        {
            this.c_index = paramC_index;
            this.user_id = paramUser_id;
            this.host_major = paramHost_major;
            this.course_code = paramCourse_code;
            this.class_number = param_class_number;
            this.course_name = paramCourse_name;
            this.comp_div = paramComp_div;
            this.grade = paramGrade;
            this.year = paramYear;
            this.relate_major = paramRelate_major;
            this.professor = paramProfessor;
            this.course_time = paramCourse_time;
            this.classroom = paramClassroom;
        }

        public Enrollment(int paramC_index, int paramUser_id, string paramHost_major,
                          string paramCourse_code, string param_class_number,
                          string paramCourse_name, string paramComp_div, string paramGrade,
                          int paramYear, string paramRelate_major, string paramProfessor,
                          string paramCourse_time, string paramClassroom, string paramEnglish)
        {
            this.c_index = paramC_index;
            this.user_id = paramUser_id;
            this.host_major = paramHost_major;
            this.course_code = paramCourse_code;
            this.class_number = param_class_number;
            this.course_name = paramCourse_name;
            this.comp_div = paramComp_div;
            this.grade = paramGrade;
            this.year = paramYear;
            this.relate_major = paramRelate_major;
            this.professor = paramProfessor;
            this.course_time = paramCourse_time;
            this.classroom = paramClassroom;
            this.english = paramEnglish;
        }

        public int C_index
        {
            get { return c_index; }
            set { c_index = value; }
        }

        public int User_id
        {
            get { return user_id; }
            set { user_id = value; }
        }

        public string Host_major
        {
            get { return host_major; }
            set { host_major = value; }
        }

        public string Course_code
        {
            get { return course_code; }
            set { course_code = value; }
        }

        public string Class_number
        {
            get { return class_number; }
            set { class_number = value; }
        }

        public string Course_name
        {
            get { return course_name; }
            set { course_name = value; }
        }

        public string Comp_div
        {
            get { return comp_div; }
            set { comp_div = value; }
        }

        public string Grade
        {
            get { return grade; }
            set { grade = value; }
        }

        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        public string Relate_major
        {
            get { return relate_major; }
            set { relate_major = value; }
        }

        public string Professor
        {
            get { return professor; }
            set { professor = value; }
        }

        public string Course_time
        {
            get { return course_time; }
            set { course_time = value; }
        }

        public string Classroom
        {
            get { return classroom; }
            set { classroom = value; }
        }

        public string English
        {
            get { return english; }
            set { english = value; }
        }
    }
}
