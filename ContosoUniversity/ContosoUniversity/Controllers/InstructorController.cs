using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;

/// <summary>
/// 在修改教师信息的时候，我们希望也能够修改教师的办公室分配
/// 教师实体Instructor和办公室分配OfficeAssignment存在一对一或者一对零的关系
/// 这就意味着我们必须处理如下的状态：
///     - 如果教师原来存在一个办公室分配，但是用户删除了它，那么，你必须移除并且删除这个办公室分配 OfficeAssignment 实体。
///     - 如果教师原来没有办公室分配，但是用户输入了一个，你必须创建一个新的办公室分配。
///     - 如果用户修改了原来的办公室分配，你必须修改当前的办公分配 OfficeAssignment 实体。
/// </summary>

namespace ContosoUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private SchoolContext db = new SchoolContext();

        /// <summary>
        /// 方法通过查询串接收一个可选的教师id和选中的课程
        /// 然后将所有需要的数据传递给视图
        /// 查询串通过页面上的Select超级链接提供
        /// </summary>
        /// <param name="id"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        // GET: Instructor
        public ActionResult Index(int? id, int? courseID)
        {
            // 代码首先创建ViewModel的实例，然后将教师实体列表保存在其中
            // 代码使用饿汉模式加载 Instructor.OfficeAssignment 和 Instructor.Courses 导航属性
            // 对于关联的 Course 实体，通过在 Inclue 中使用 Select 方法饿汉模式加载，结果使用 LastName 进行排序
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department))
                .OrderBy(i => i.LastName);

            // 如果某个教师被选中了，选中的教师从ViewModel中的教师列表中被选出
            // 视图模型的Courses属性通过教师的Courses属性加载相关的Course实体
            if (id != null)
            {
                ViewBag.InstructorID = id.Value;

                // Where 方法返回一个集合，但是这里的情况将仅仅返回一个教师实体
                // Single方法将集合转化为一个单个的实体，以便访问这个实体的Course属性

                // 在知道集合中仅仅包含一个实体的时候，可以使用Single方法
                // Single方法在集合中为空的时候将会抛出异常，或者在集合中包含多于一个实体的时候也会抛出异常
                // 另外一个替换的方法是'SingleOrDefault'方法，在集合为空的时候，这个方法放回Null
                // 实际上，在这里还是会抛出异常 ( 试图在空引用上访问 Courses 属性的时候 )
                // 异常的信息将会简单地说明这个问题，在调用 Single 方法的时候，还可以传递一个条件来代替通过 Where 传递的条件
                viewModel.Courses = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single().Courses;
            }

            // 在获取教师列表的时候，使用饿汉模式加载 Courses 导航属性值，以及 Department 导航属性的值
            // 然后将结果保存到视图模型的 Courses 集合中，再从这个集合的一个实体中访问注册实体
            // 因为没有对Course.Enrollements 属性指定饿汉加载，出现在页面上时将使用延迟加载

            // 如果仅仅禁用延迟加载而不采取其他的措施，Enrollments 属性将是 null ，而不管实际上有多少注册
            // 在这种情况下，就必须要么指定饿汉加载，要么指定显式加载
            // 已经见到了如何使用饿汉加载，因为展示如何使用显式加载，将 Index 方法中替换为如下的代码，这里使用显式加载来读取 Enrollments 属性
            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                // Lazy loading
                //viewModel.Enrollments = viewModel.Courses.Where(
                //    x => x.CourseID == courseID).Single().Enrollments;
                // Explicit loading
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();

                // 在获取了选中的Course实体后，新的代码显示加载课程的Enrolments导航属性
                db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();

                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    // 然后显式加载每个注册 Enrollment 实体相关的学生 Student 实体。
                    db.Entry(enrollment).Reference(x => x.Student).Load();
                }

                viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<Course>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructor/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,HireDate,OfficeAssignment")]Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.Courses = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = db.Courses.Find(int.Parse(course));
                    instructor.Courses.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        /// <summary>
        /// 脚手架生成的代码不是我们希望的，它还生成了一个下拉列表，
        /// 但是我们希望是文本框，将这个方法使用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.ID == id)
                .Single();
            PopulateAssignedCourseData(instructor);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = db.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        // POST: Instructor/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        /// <summary>
        /// 从数据库中获取了教师Instructor实体，并且预先加载了相关的办公司分配 OfficeAssignment 和课程 Course 导航属性。如同在 HttpGet 的 Edit 方法一样
        /// 使用通过模型绑定获取的数据，更新 Instructor 实体，除了课程 Course 导航属性之外
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedCourses"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instructorToUpdate = db.Instructors
               .Include(i => i.OfficeAssignment)
               .Include(i => i.Courses)
               .Where(i => i.ID == id)
               .Single();

            if (TryUpdateModel(instructorToUpdate, "",
               new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                    {
                        instructorToUpdate.OfficeAssignment = null;
                    }
                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }
        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            // 如果复选框没有被选中，在UpdateInstructorCourses方法中，使用一个空的集合来初始化Courses导航属性
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.CourseID));
            foreach (var course in db.Courses)
            {
                // 然而，代码遍历数据库中所有的课程，如果课程的复选框被选中了，但是没有包含在教师Instructor的Courses集合中
                // 这个课程将会被加入到导航属性集合中
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }

                // 如果课程没有被选中，但是在教师的导航属性Courses集合中，就从集合属性中删除掉
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Remove(course);
                    }
                }
            }
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = db.Instructors
              .Include(i => i.OfficeAssignment)
              .Where(i => i.ID == id)
              .Single();

            db.Instructors.Remove(instructor);

            var department = db.Departments
                .Where(d => d.InstructorID == id)
                .SingleOrDefault();
            if (department != null)
            {
                department.InstructorID = null;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
