using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// 在Department实体中，Column特性被用来改变SQL数据类型映射，以便在数据库中使用SQL Server的money数据类型
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        // 增加跟踪属性
        // Timestamp特性指定随后的列将会被包含在Update或者Delete语句的Where子句
        [Timestamp]
        public byte[] RowVersion { get; set; }


        /// <summary>
        /// 一个系可能有或者没有一个系主任，系主任通常是教师
        /// 进而InstructorID属性包含了一个教师实体的外键
        /// 加在int类型后面的问号表示这是一个可空类型
        /// 导航属性名为Administrator，实际保存Instrctor实体的引用
        /// </summary>
        public int? InstructorID { get; set; }
        public virtual Instructor Administrator { get; set; }

        /// <summary>
        /// 一个系可能有多个课程，所以Courses是导航属性
        /// </summary>
        public virtual ICollection<Course> Courses { get; set; }
    }
}