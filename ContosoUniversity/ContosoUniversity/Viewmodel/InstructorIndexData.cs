using System.Collections.Generic;
using ContosoUniversity.Models;

/// <summary>
/// 教师列表中的办公室分配OfficeAssignment实体
/// 教师实体与办公室分配之间是一对一或者一堆零的关系，将使用饿汉模式来加载办公室分配实体
/// 饿汉模式适合于当我们需要主键表关联数据的时候，在这里，我们需要显示所有教师的办公室分配
/// </summary>
namespace ContosoUniversity.ViewModels
{
    /// <summary>
    /// 当用户选中一个教师的时候，需要显示这个教师相关的课程实体
    /// 教师和课程之间存在多对多的关系
    /// 我们将使用饿汉模式加载课程和相关的实体
    /// 在这里，延迟加载可能会更加有效，因为仅仅需要显示选中的教师的课程
    /// 实际上，这个例子展示了饿汉模式加载导航属性中的导航属性
    /// </summary>
    public class InstructorIndexData
    {
        /// <summary>
        /// 当用户选择课程后，相关的注册实体Enrollments将会显示出来
        /// Course 和 Enrollment 实体存在一对多的关系
        /// 我们将使用显示加载处理Enrollment实体，以及相关的学生Student实体
        /// </summary>
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}