using Microsoft.AspNetCore.Mvc;
using MVC_Demo.Models;
using MVC_Demo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Demo.Controllers
{
    //属性
    //java 注解
    //admin/控制器/方法
    public class TestController : Controller
    {
        //定义数据库上下文
        private readonly McStoreContext _db;

        /// <summary>
        /// 构造方法 构造注入
        /// </summary>
        public TestController(McStoreContext db)
        {
            _db = db;
        }

        // MVC约定大于配置
        //控制器名Test Views文件夹里面的Test文件夹对应的
        //方法名Index 视图名Index.cshtml 对应
        public IActionResult Index()
        {
            return View();
        }

        //返回类型IActionResult 
        //方法名
        //当我们添加特性路由 常规路由会失效
        [Route("/myview")]
        [Route("/test/myview")]
        public IActionResult MyView()
        {
            return View();
        }

        public IActionResult ViewDataDemo()
        {
            var name = "张飞";
            var number = 1;
            var date = new DateTime(2022, 09, 24);

            //1.ViewData传参
            //传递数据给View 弱类型传参
            ViewData["name"] = name;
            ViewData["number"] = number;
            ViewData["date"] = date;
            return View();
        }

        public IActionResult ViewBagDemo()
        {
            var name = "刘备";
            var number = 2;
            var date = new DateTime(2022, 09, 23);

            //2.ViewBag传参
            //传递数据给View 弱类型传参
            ViewBag.name = name;
            ViewBag.number = number;
            ViewBag.date = date;
            return View();
        }
        public IActionResult ViewModelDemo()
        {
            //强类型传参
            var person = new Person();
            person.Name = "关羽";
            person.Number = 3;
            person.Date = new DateTime(2022, 01, 02);

            //3.ViewModel传参
            return View(person);



            //调用add方法 替我们生成insert的SQL 产生一些资源消耗
            //_db.add(person)
            //_db.update(person)
            //_db.delete(person)
            //_db.getlist()
        }

        public IActionResult AddData()
        {
            //new一个分类对象  => 商品对象
            var model = new Category
            {
                //商品的属性
                CateName = "图书",
            };

            //使用规则
            //上下文对象.表名.操作方法 只是完成SQL构建 没有最终执行
            _db.Categories.Add(model);

            //保存更改 执行SQL 返回执行影响行数
            var row = _db.SaveChanges();
            if (row > 0)
            {
                //当我们return View() 他会去找页面
                //return View();
                return Content("添加成功");
            }
            else
            {
                return Content("添加失败");
            }
        }

        public IActionResult AddRange()
        {
            var list = new List<Category>();
            list.Add(new Category()
            {
                CateName = "美妆"
            });

            list.Add(new Category()
            {
                CateName = "电脑"
            });
            //使用规则
            //上下文对象.表名.操作方法 只是完成SQL构建 没有最终执行
            _db.Categories.AddRange(list);

            //保存更改 执行SQL 返回执行影响行数
            var row = _db.SaveChanges();
            if (row > 0)
            {
                //当我们return View() 他会去找页面
                //return View();
                return Content("添加成功");
            }
            else
            {
                return Content("添加失败");
            }
        }

        public IActionResult AddGoods()
        {
            //new一个分类对象  => 商品对象
            var model = new Good
            {
                //商品的属性
                Name = "图书",
                CateId = 1,
                Cover = "",
                Stock = 1,
                Price = 100,
                //CreateTime=DateTime.Now,
                CreateTime = new DateTime(2022, 10, 15),
                UpdateTime = null
            };

            //使用规则
            //上下文对象.表名.操作方法 只是完成SQL构建 没有最终执行
            _db.Goods.Add(model);

            //保存更改 执行SQL 返回执行影响行数
            var row = _db.SaveChanges();
            if (row > 0)
            {
                //当我们return View() 他会去找页面
                //return View();
                return Content("添加成功");
            }
            else
            {
                return Content("添加失败");
            }
        }

        public IActionResult GetData()
        {
            //上下文对象.表名.操作方法 查询所有数据 sql:select * from category
            var list = _db.Categories.ToList();
            //查询指定条件 id==1 分类 sql:select * from categroy where id=1
            //                             表达式：拉姆达 lambda表达式
            var list1 = _db.Categories.Where(x => x.Id == 1).ToList();

            var list2 = _db.Categories.Where(x => x.CateName == "手机").ToList();
            //查询带有“家”字的分类
            var list3 = _db.Categories.Where(x => x.CateName.Contains("家")).ToList();

            var list4 = _db.Categories.Where(x => x.Id > 2).ToList();

            //多重条件
            var list5 = _db.Categories.Where(x => x.Id > 2 && x.CateName == "手机").ToList();

            //查询单条数据
            // var model = _db.Categories.Where(x => x.Id == 1).FirstOrDefault(); //1
            var model = _db.Categories.FirstOrDefault(x => x.Id == 1); //2

            return new ObjectResult(model);
        }

        public IActionResult GetGoods()
        {
            //查询分类Id为1的所有商品数据 4.2.1
            var list = _db.Goods.Where(x => x.CateId == 1).ToList();

            //4.2.2 查询分类ID为2，并且价格大于2000的商品
            var list1 = _db.Goods.Where(x => x.CateId == 2 && x.Price > 2000).ToList();

            //4.2.3查询分类ID为1，或者分类Id为3的商品
            var list2 = _db.Goods.Where(x => x.CateId == 1 || x.CateId == 3).ToList();
            return new ObjectResult(new
            {
                list,
                list1,
                list2
            });
        }

        public IActionResult UpdateData()
        {
            //update category set catename='' where [id] = 4
            //1.把数据查询出来
            var model = _db.Categories.FirstOrDefault(x => x.Id == 4);
            if (model == null)
                return Content("没有找到数据");
            //2.修改数据
            model.CateName = "数码修改";
            //3.执行更新操作
            _db.Categories.Update(model);
            //4.保存
            var row = _db.SaveChanges();
            if (row > 0)
                return Content("成功");
            return Content("失败");
        }

        public IActionResult UpdateRange()
        {
            //1.把数据查询出来
            var list = _db.Categories.Where(x => x.Id > 1).ToList();
            //2.修改数据 遍历数组
            foreach (var item in list)
            {
                item.CateName = item.CateName + "（更新）";
            }

            //3.执行更新操作
            _db.Categories.UpdateRange(list);
            //4.保存
            var row = _db.SaveChanges();
            if (row > 0)
                return Content("成功");
            return Content("失败");
        }

        public IActionResult UpdateGoods()
        {
            var model = _db.Goods.FirstOrDefault(x => x.Id == 50);
            model.CateId = 2;
            _db.Goods.Update(model);
            _db.SaveChanges();
            return Content("成功");
        }
        /// <summary>
        /// 刪除一條數據
        /// </summary>
        /// <returns></returns>
        public IActionResult DeleteData()
        {
            //1先把数据查询出来
            var model = _db.Categories.FirstOrDefault(x => x.Id == 4);
            if (model == null)
                return Content("数据不存在");

            _db.Categories.Remove(model);

            var row = _db.SaveChanges();
            if (row > 0)
                return Content("成功");
            return Content("失敗");
        }
        /// <summary>
        /// 刪除多條
        /// </summary>
        /// <returns></returns>
        public IActionResult DeleteRange()
        {
            var list = _db.Categories.Where(x => x.Id > 1);

            _db.Categories.RemoveRange(list);

            var row = _db.SaveChanges();
            if (row > 0)
                return Content("成功");
            return Content("失敗");
        }
        /// <summary>
        /// 函数 有返回类型的函数
        /// </summary>
        /// <returns></returns>
        public IActionResult SortDemo()
        {
            //sql:select * from category where id>0 order by id
            //正序
            var list1 = _db.Categories
                .Where(x => x.Id > 0)
                .OrderBy(x => x.CateName)
                .ToList();

            //倒叙
            var list2 = _db.Categories
                .Where(x => x.Id > 0)
                .OrderByDescending(x => x.CateName)
                .ToList();

            //混合排序 当第一排序相同时，第二排序 第三排序
            var list3 = _db.Categories
               .Where(x => x.Id > 0)
               .OrderByDescending(x => x.CateName)
               .ThenByDescending(x => x.Id) //ThenBy
               .ToList();
            //return View();
            //return Content("成功");
            return new ObjectResult(list3);
        }
        //SoftGoods方法，查询商品数据并以分类Id倒序显示
        public IActionResult SoftGoods()
        {
            var list = _db.Goods.OrderByDescending(x => x.CateId).ToList();
            return new ObjectResult(list);
        }

        public IActionResult SkipTakeDemo()
        {
            //Skip、Take使用前必须排序
            //Skip: 跳过前几条  SQLServer版本2012以上才能使用
            //var list = _db.Goods.OrderByDescending(x => x.Id).Skip(5).ToList();
            //Take:取前几条
            var list1 = _db.Goods.OrderByDescending(x => x.Id).Take(10).ToList();

            //取6-10 每页5条 第二页的内容
            var list2 = _db.Goods.OrderByDescending(x => x.Id).Skip(5).Take(5).ToList();
            //取11-15 每页5条 第三页的内容
            var list3 = _db.Goods.OrderByDescending(x => x.Id).Skip(10).Take(5).ToList();
            var pageIndex = 2;  //第几页 页码
            var pageSize = 5;  //每页多少条
            // 在ToList之前 生成SQL
            //IQueryable 待执行SQL对象 等我们ToList的时候 会去数据库执行
            var list4 = _db.Goods.OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new ObjectResult(list1);
        }
        //完成实训5-2 SkipGoods 查询以商品金额倒序的3-7个商品
        public IActionResult SkipGoods()
        {
            //3-7
            var list = _db.Goods.OrderByDescending(x => x.Price)
                 //跳过多少个
                 .Skip(2)
                 //取多少个
                 .Take(5)
                 .ToList();
            return new ObjectResult(list);
        }

        public IActionResult AnyDemo()
        {
            //判断列表有无数据 有数据返回true 没有数据返回false
            var result = _db.Categories.Any();
            //判断分类表 有没有图书这个分类
            var result1 = _db.Categories.Where(x => x.CateName == "图书").Any();
            //简化
            var result2 = _db.Categories.Any(x => x.CateName == "图书");
            return new ObjectResult(result2);
        }
        //完成5.3 AnyGoods方法，判断商品金额为100的商品是否存在
        public IActionResult AnyGoods()
        {
            var result = _db.Goods.Any(x => x.Price == 100);
            return new ObjectResult(result);
        }
        /// <summary>
        /// 获取数据的数量
        /// </summary>
        /// <returns></returns>
        public IActionResult CountDemo()
        {
            //第一种
            var count = _db.Categories.Count();
            //第二种 带条件 获取分类名称为家具的数据 有多少
            var count1 = _db.Categories.Where(x => x.CateName == "家具").Count();
            //第三种
            var count2 = _db.Categories.Count(x => x.CateName == "家具");
            return new ObjectResult(count2);
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <returns></returns>
        public IActionResult SumDemo()
        {
            //获取整个表的库存总数 求和
            var sum = _db.Goods.Sum(x => x.Stock);
            //加上条件 只需要分类Id==2的库存总数
            var sum1 = _db.Goods.Where(x => x.CateId == 2).Sum(x => x.Stock);
            return new ObjectResult(sum1);
        }
        //5-4 在Test控制器添加CountGoods方法，查询商品金额为500-1000的商品数量
        public IActionResult CountGood()
        {
            var result = _db.Goods.Count(x => x.Price >= 500 && x.Price <= 1000);
            return new ObjectResult(result);
        }
        //5-5 在Test控制器添加SumGoods方法，查询分类Id为“1”的商品总额
        public IActionResult SumGoods()
        {
            var sum = _db.Goods.Where(x => x.CateId == 1).Sum(x => x.Price);
            var count = _db.Goods.Count(x => x.CateId == 1);
            //sum/count 平均
            var avg = sum / count;
            var avg1 = _db.Goods.Where(x => x.CateId == 1).Average(x => x.Price);
            return new ObjectResult(sum);
        }
        /// <summary>
        /// 分组统计GroupBy
        /// </summary>
        /// <returns></returns>
        public IActionResult GroupDemo()
        {
            var list = _db.Goods.GroupBy(x => x.CateId).Select(x => new
            {
                x.Key,
                Count = x.Count(),
                Sum = x.Sum(y => y.Price),
                Avg = x.Average(y => y.Price)
            });
            return new ObjectResult(list);
        }

        //5-6 在Test控制器添加StockGoods方法，统计每个分类的总库存
        public IActionResult StockGoods()
        {
            //上下文.表
            var list = _db.Goods.GroupBy(x => x.CateId).Select(x => new
            {
                分类Id = x.Key,
                总库存 = x.Sum(x => x.Stock)
            }).ToList();
            return new ObjectResult(list);
        }

        /// <summary>
        /// 级联 表之间的连接
        /// </summary>
        /// <returns></returns>
        public IActionResult JoinDemo()
        {
            var list1 = _db.Goods.Select(x => new
            {
                x.Id,
                x.Name,
                x.Price,
                x.CateId
            }).ToList();
            //sql select * from good inner into category on good.cateId = category.id
            // 内连接                         1：连接表           2：主表的关联字段  3：连接表的关联字段      4：组合在一起  x y只不过是变量 
            var list = _db.Goods.Join(_db.Categories, good => good.CateId, cateory => cateory.Id, (good, category) => new
            {
                good.Id,
                good.Name,
                good.CateId,
                category.CateName
            }).ToList();
            return new ObjectResult(list);
        }

        public IActionResult JoinGoods()
        {
            //完成
            //实训5.7 查询分类名称为“数码”的商品数据
            var list = _db.Goods.Join(_db.Categories, x => x.CateId, y => y.Id, (x, y) => new
            {
                x.Name,
                x.Id,
                y.CateName
            }).Where(x => x.CateName == "数码")
            .ToList();
            return new ObjectResult(list);
        }

    }

}
