using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        /// <summary>
        /// 在CourseID属性上的DatabaseGenerated属性，使用了None参数
        /// 指定主键将由用户提供，而不是通过数据库自动生成
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        /// <summary>
        /// 一个课程属于一个系，所以存在一个DepartmentID外键和一个Department导航属性
        /// </summary>
        public int DepartmentID { get; set; }
        public virtual Department Department { get; set; }

        /// <summary>
        /// Enrollments属性是导航属性
        /// 一个Course实体可以关联多个注册实体
        /// 也就是说：一个课程可以被多个学生注册，所以存在一个Enrollments导航属性
        /// </summary>
        public virtual ICollection<Enrollment> Enrollments { get; set; }

        /// <summary>
        /// 一个课程可能被多名教师教授，所以这里存在一个Instructors属性
        /// </summary>
        public virtual ICollection<Instructor> Instructors { get; set; }
    }
}