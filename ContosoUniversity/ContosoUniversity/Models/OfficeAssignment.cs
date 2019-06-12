using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class OfficeAssignment
    {
        /// <summary>
        /// 在 Instructor 和 OfficeAssignment 实体之间存在一对一或者一对零的关系
        /// 一个办公室分配仅仅存在于被分配给教师的时候。进而，它的主键也应该是 Instructor 实体的外键
        /// 但是 EF 并不能自动将 InstructorID 作为分配的主键，因为名字不符合约定，既不是 ID ，也不是类名加上 ID
        /// 因此，使用 Key 特性来标识这是一个主键
        /// </summary>
        [Key]
        // 在主键的属性名既不是 Id 也不是类名加上 ID 的时候，可以通过 Key 特性
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }

        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        /// <summary>
        /// Instructor 实体有一个可空的 OfficeAssignment 导航属性 ( 因为教师可能没有分配办公室 )
        /// 在 OfficeAssignment 实体上则有一个不可为空的 Instructor 导航属性 ( 因为办公室分配不可能在没有教师的情况下存在 )
        /// 当 Instructor 实体关联到 OfficeAssignment 实体的时候，它们可以通过导航属性相互引用对方
        /// </summary>
        public virtual Instructor Instructor { get; set; }
    }
}