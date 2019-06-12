using ContosoUniversity.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ContosoUniversity.DAL
{
    public class SchoolContext : DbContext
    {
        /// <summary>
        /// 这段代码使用DbSet类型的属性定义每一个实体集
        /// 在EF术语中，一个实体集典型的关联到一个数据库中的表，一个实体关联到表中的每一行
        /// </summary>
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // 在 Instructor 和 OfficeAssignment 之间一对一或者一对零的关系
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors).WithMany(i => i.Courses)
                .Map(t => t.MapLeftKey("CourseID")
                    .MapRightKey("InstructorID")
                    .ToTable("CourseInstructor"));

            // 此代码指示实体框架使用存储过程的插入。更新和删除操作上Department实体
            modelBuilder.Entity<Department>().MapToStoredProcedures();
        }
    }
}