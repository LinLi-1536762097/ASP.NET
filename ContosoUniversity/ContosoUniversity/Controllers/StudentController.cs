using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Student
        /// <summary>
        /// 这段代码从URL中接收名为'sortOrder的参数'，这个参数有ASP.NET MVC作为参数传递给Action方法
        /// 这个参数可以说'Name'或者是'Date'，可能还有一个空格隔开的 desc 来指定排序
        /// 
        /// 当第一次请求Index的时候，没有参数，学生使用 LastName 的升序排序。
        /// 这是通过 switch 的 default 代码段指定的，当用户点击一个列的标题链接的时候，合适的 sortOrder 值需要通过查询字符串传递出来
        /// 
        /// 方法又增加了一个 page 参数
        /// 当第一次显示这个页面的时候，或者用户没有点击分页链接的时候，page的参数将会是null。
        /// 如果分页链接将被点击了，page参数将会包含需要显示的页码。
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // ViewBag中的CurrentSort属性用来提供当前的排序顺序
            // 它必须被包含到当前的分页链接中，以便在分页处理过程中保持当前的排序规则
            ViewBag.CurrentSort = sortOrder;

            // 这里使用了条件语句，第一个用来指定当sortOrder参数为null或者为空的时候，ViewBag.NameSortParm 应用被设置为 Name desc
            // 其他情况下，应该被设置为空串
            // 这里有四种可能，依赖于当前的排序情况：
            //      1.如果当前的排序规则为 LastName 升序，那么，LastName 链接应该设置为降序，Enrollment Date 链接必须被设置为按日期升序。
            //      2.如果当前的排序规则为 LastName 降序，那么，LastName 链接应该设置为升序，排序串应该为空串，日期为升序。
            //      3.如果当前排序的规则为 Date 升序，那么，链接应该为 LastName 升序和日期升序。
            //      4.如果当前的排序规则为 Date 降序，那么，链接应该为 LastName 升序和日期降序。
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            // 其它的 ViewBag 属性为视图提供当前的过滤串
            // 因为这个过滤串在页面被重新显示的时候，必须重新回到文本框中
            // 另外，这个串也必须包含在分页链接中，以便在分页过程中，保持过滤效果
            // 最后，如果在分页的过程中修改了过滤串，那么页码将会回到第一页
            // 因为新的过滤规则返回了不同的数据，很可能原来的页码在这时候已经不再存在了
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            // 现在，为 Index 方法增加了一个参数 searchString ，LINQ 语句中也增加了一个 where 子句
            // 来选择在 FirstName 或者 LastName 中包含过滤字符串的学生
            // 搜索串来自文本框的输入，后面需要你在视图中加入它
            // 增加的 where 条件子句仅仅在提供了搜索串的情况下才会被处理
            var students = from s in db.Students
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:  // Name ascending 
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            // 在方法的最后，查询学生的表达式被转换为 PagedList
            // 而不再是通常的 List，这样传递到视图中的就是支持分页的集合
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            // ToPagedList 方法需要一个页码值，两个问号用来为可空的页码提供一个默认值
            // 表达式 ( page ?? 1 ) 意味着如果 page 有值得话返回这个值，如果是 null 的话，返回 1
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // 使用 Find 方法来获取单个的 Student 实体，使用传递给方法的 id 关键字。
            // Id 来自 Index 页面中的超级链接提供的查询字符串
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        /// <summary>
        /// 这些代码将通过 ASP.NET MVC 模型绑定创建的实体对象加入到 Students 集合中，然后保存修改到数据库中
        /// 模型绑定是 ASP.NET MVC 的一个功能用于简化你获取通过表单提交的数据
        /// 模型绑定转换提交的表单数据到 .NET 中的数据类型，通过 Action 方法的参数传递进来
        /// 在这里，模型绑定通过表单数据为你实例化了一个 Student 的实体对象实例 
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")]Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        /// <summary>
        /// 数据库上下文对象维护内存中的对象与数据库中数据行之间的同步
        /// 这些信息在调用SaveChanges方法被调用的时候使用
        /// 例如，当使用Add方法传递一个新的实体对象的时候，实体的状态被设置为Added
        /// 在调用SavChanges方法的时候，数据库上下文使用SQL命令Insert来插入数据
        /// 
        /// 
        /// 实体的状态可能为如下之一：
        ///     Added.实体在数据库中不存在。SaveChanges 方法必须执行 Insert 命令
        ///     Unchanged.在调用 SaveChanges 的时候不需要做任何事情，当从数据库读取数据的时候，实体处于此状态。
        ///     Modified.某些或者全部的实体属性被修改过.SaveChanges方法需要执行 Update 命令。
        ///     Deleted.实体标记为已删除，SaveChanges 方法必须执行 Delete 命令。
        ///     Detached.实体的状态没有被数据库上下文管理。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Student/Delete/5
        // 此代码接受'可选参数'
        /// <summary>
        /// 模板生成的 HttpGet Delete 方法使用 Find 方法来获取 Student 实体
        /// 像在Details 和 Edit 方法中一样
        /// 
        /// 像在更新和创建操作中一样，删除操作也需要两个方法
        /// GET 方法用于显示一个视图，使用户可以允许或者取消删除操作
        /// 如果用户允许删除操作，那么，将会发出一个 Post 请求
        /// HttpPost Delete 方法将会被调用，然后执行实际的删除操作
        /// 
        /// 这段代码接收一个可选的 bool 类型参数，这个参数用来表示是在更新失败之后调用这个方法
        /// 在页面请求中被调用的时候，这个参数为 null ( false )
        /// 当通过 HttpPost Delete 方法更新数据库失败后，被调用的时候，参数被设置为 true，错误信息被传递到视图中
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        /// <summary>
        /// 这段代码获取选中的实体，然后调用 Remove 方法将实体的状态设置为 Deleted
        /// 当调用 SaveChanged 的时候，SQL 命令 Delete 被生成并执行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return RedirectToAction("Index");
        }

        // 关闭数据库连接
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
