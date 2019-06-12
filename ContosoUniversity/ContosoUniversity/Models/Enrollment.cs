using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    /// <summary>
    /// Grade属性是枚举
    /// </summary>
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        public int EnrollmentID { get; set; }

        /// <summary>
        /// CourseID属性也是外键，关联的导航属性为Course
        /// 一个Enrollment关联一个Course实体
        /// </summary>
        public int CourseID { get; set; }

        /// <summary>
        /// StudentID属性是外键，关联的导航属性为Student。
        /// 一个Enrollment关联一个Student
        /// 所以这个属性只能持有一个Student实体
        /// (与前面所看到的Student.Enrollments 导航属性不同，Student中可以容纳多个Enrollment实体)
        /// </summary>
        public int StudentID { get; set; }

        /// <summary>
        /// 问号后Grade类型声明指示：Grade属性是可以为'Null'
        /// 评级为null是不同于评级为零
        /// null表示一个等级未知或者尚未分配
        /// </summary>
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        /// <summary>
        /// 注册记录面向一门课程，因此存在 CourseID 外键属性和 Course 导航属性
        /// </summary>
        public virtual Course Course { get; set; }

        /// <summary>
        /// 注册记录面向一名学生，因此存在 StudentID 外键属性和 Student 导航属性
        /// </summary>
        public virtual Student Student { get; set; }
    }
}