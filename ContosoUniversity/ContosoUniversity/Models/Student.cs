//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ContosoUniversity.Models
//{
//    /// <summary>
//    ///  学生(Student)实体和注册(Enrollment)实体之间存在一对多的关联
//    ///  课程(Course)和注册(Enrollment)之间也存在一对多的关联
//    /// 或者可以说：
//    ///  一个学生可以注册许多个课程，一个课程也可以被许多学生注册
//    /// </summary>

//    public class Student
//    {
//        /// <summary>
//        /// ID属性将会映射到数据库中关联到这个类的表的主键
//        /// 默认情况下，EF将属性为名 'ID'或者名为 '类名ID'的属性作为主键
//        /// </summary>
//        public int ID { get; set; }

//        [Required]
//        [Display(Name = "Last Name")]
//        [StringLength(50, MinimumLength = 1)]
//        public string LastName { get; set; }

//        [Required]
//        // 假设希望用户在输入名字的时候不能超过50个字符长度
//        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
//        // 还可以通过特性来控制模型类及属性如何映射到数据库
//        // 假设我们已经使用FirstMidName来表示名字，因为属性也包含了中间名
//        // 但是我们希望数据库中的列名为'FirstName'，因为编写本地查询的用户习惯使用这个名字
//        // 要完成这种映射。我们可以使用'Column'特性

//        // Column 特性在数据库创建的时候被使用
//        // Student的属性FirstMidName在数据库中将被命名为'FirstName'
//        [Column("FirstName")]
//        [Display(Name = "First Name")]
//        public string FirstMidName { get; set; }

//        [DataType(DataType.Date)]
//        // 对于学生注册日期来说，虽然我们只关心注册的日期，但是现在的页面在日期之后还显示了时间
//        // 通过使用数据标注特性，可以通过一点代码就可以在所有的地方修补这个问题
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Enrollment Date")]
//        public DateTime EnrollmentDate { get; set; }

//        // FullName 是计算属性，可以返回通过串联两个其他属性创建的值
//        // 因此只有get访问器，但FullName没有在数据库中生成列
//        [Display(Name = "Full Name")]
//        public string FullName
//        {
//            get
//            {
//                return LastName + ", " + FirstMidName;
//            }
//        }

//        public string Secret { get; set; }

//        public virtual ICollection<Enrollment> Enrollments { get; set; }
//    }
//}

// 对 Student.cs 文件进行类似的修改，Student派生自Person

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Student : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}