/// <summary>
/// 在课程Course和教师Instructor之间存在多对多的关系，这意味着我们不需要直接访问表之间的关联
/// 而是通过增加或者删除Instructor. Course 实体来完成
/// </summary>

namespace ContosoUniversity.ViewModels
{
    public class AssignedCourseData
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }
    }
}