//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ContosoUniversity.Models
//{
//    public class Instructor
//    {
//        public int ID { get; set; }

//        [Display(Name = "Last Name"), StringLength(50, MinimumLength = 1)]
//        public string LastName { get; set; }

//        [Column("FirstName"), Display(Name = "First Name"), StringLength(50, MinimumLength = 1)]
//        public string FirstMidName { get; set; }

//        [DataType(DataType.Date), Display(Name = "Hire Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        public DateTime HireDate { get; set; }

//        [Display(Name = "Full Name")]
//        public string FullName
//        {
//            get { return LastName + ", " + FirstMidName; }
//        }

//        /// <summary>
//        /// Courses 和 OfficeAssignment 属性是导航属性，像我们在前面说明的，它们典型地被定义为虚拟的 virtual，以便 EF 的延迟加载特性可以提供帮助
//        /// 如果导航属性可以包含多个实体，它的类型必须为 ICollection
//        /// </summary>
//        public virtual ICollection<Course> Courses { get; set; }

//        /// <summary>
//        /// 一个教师可以教授多门课程，所以 Courses 被定义为 Course 的集合
//        /// 另一方面，一个教师仅有一间办公室，所以 OfficeAssignment 被定义为单个的 OfficeAssignment 实体 ( 如果没有办公室的话，可能为 null )
//        /// </summary>
//        public virtual OfficeAssignment OfficeAssignment { get; set; }
//    }
//}


// 在 Instructor.cs 文件中，将 Instructor 从 Person 派生出来，删除 key 和 name 字段

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Instructor : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}