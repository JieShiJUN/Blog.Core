using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Blog.Core.Common.Helper;
using Blog.Core.IServices;
using Blog.Core.Model;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using Blog.Core.SwaggerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Serilog;
using SkyWalking.NetworkProtocol.V3;
using SqlSugar;
using StackExchange.Profiling;
using ZstdSharp.Unsafe;
using static Blog.Core.Extensions.CustomApiVersion;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// 博客管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Blog")]
    public class BlogController : BaseApiController
    {
        public IBlogArticleServices _blogArticleServices { get; set; }
        public IBlogArticleDisplayImageServices _imageServices { get; set; }
        private readonly ILogger<BlogController> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// 
        public BlogController(ILogger<BlogController> logger, IBlogArticleDisplayImageServices imageServices)
        {
            _logger = logger;
            _imageServices = imageServices;
        }


        /// <summary>
        /// 获取博客列表【无权限】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="bcategory"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<BlogArticle>>> Get(int id, int page = 1, string bcategory = "技术博文", string key = "")
        {
            int intPageSize = 6;
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            Expression<Func<BlogArticle, bool>> whereExpression = a => (a.bcategory == bcategory && a.IsDeleted == false) && ((a.btitle != null && a.btitle.Contains(key)) || (a.bID != null && a.bID.ToString().Contains(key) || (a.bcontent != null && a.bcontent.Contains(key))));

            var res = new PageModel<BlogArticle>();
            var pageModelBlog = await _blogArticleServices.QueryPage(whereExpression, page, intPageSize, " bID desc ");

            using (MiniProfiler.Current.Step("获取成功后，开始处理最终数据"))
            {
                var list = new List<BlogArticle>();
                foreach (var item in pageModelBlog.data)
                {
                    if (!string.IsNullOrEmpty(item.bcontent))
                    {
                        var blog =await _blogArticleServices.NavData(item);
                        blog.bRemark = (HtmlHelper.ReplaceHtmlTag(blog.bcontent)).Length >= 200 ? (HtmlHelper.ReplaceHtmlTag(blog.bcontent)).Substring(0, 200) : (HtmlHelper.ReplaceHtmlTag(blog.bcontent));
                        int totalLength = 500;
                        if (blog.bcontent.Length > totalLength)
                        {
                            blog.bcontent = blog.bcontent.Substring(0, totalLength);
                        }
                        list.Add(blog);
                    }
                }
                res.data = list;
            }

            return SuccessPage(res);
        }


        /// <summary>
        /// 获取博客详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[Authorize(Policy = "Scope_BlogModule_Policy")]
      /*  [Authorize]*/
        public async Task<MessageModel<BlogViewModels>> Get(long id)
        {
            return Success(await _blogArticleServices.GetBlogDetails(id));
        }


        /// <summary>
        /// 获取详情【无权限】
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DetailNuxtNoPer")]
        public async Task<MessageModel<BlogViewModels>> DetailNuxtNoPer(long id)
        {
            return Success(await _blogArticleServices.GetBlogDetails(id));
        }

        [HttpGet]
        [Route("GoUrl")]
        public async Task<IActionResult> GoUrl(long id = 0)
        {
            var response = await _blogArticleServices.QueryById(id);
            if (response != null && response.bsubmitter.IsNotEmptyOrNull())
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
                if (Regex.IsMatch(response.bsubmitter, Url))
                {
                    response.btraffic += 1;
                    await _blogArticleServices.Update(response);
                    return Redirect(response.bsubmitter);
                }

            }

            return Ok();
        }

        [HttpGet]
        [Route("GetBlogsByTypesForMVP")]
        public async Task<MessageModel<List<BlogArticle>>> GetBlogsByTypesForMVP(string types = "", int id = 0)
        {
            if (types.IsNotEmptyOrNull())
            {
                var blogs = await _blogArticleServices.Query(d => d.bcategory != null && types.Contains(d.bcategory) && d.IsDeleted == false, d => d.bID, false);
                return Success(blogs);
            }
            return Success(new List<BlogArticle>() { });
        }

        [HttpGet]
        [Route("GetBlogByIdForMVP")]
        public async Task<MessageModel<BlogArticle>> GetBlogByIdForMVP(long id = 0)
        {
            if (id > 0)
            {
                return Success(await _blogArticleServices.QueryById(id));
            }
            return Success(new BlogArticle());
        }

        /// <summary>
        /// 获取博客测试信息 v2版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        ////MVC自带特性 对 api 进行组管理
        //[ApiExplorerSettings(GroupName = "v2")]
        ////路径 如果以 / 开头，表示绝对路径，反之相对 controller 的想u地路径
        //[Route("/api/v2/blog/Blogtest")]
        //和上边的版本控制以及路由地址都是一样的

        [CustomRoute(ApiVersions.V2, "Blogtest")]
        public MessageModel<string> V2_Blogtest()
        {
            return Success<string>("我是第二版的博客信息");
        }

          /// <summary>
        /// 添加博客【无权限】
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = "Scope_BlogModule_Policy")]
       /* [Authorize]*/
        public async Task<MessageModel<string>> Post([FromBody] BlogArticle blogArticle)
        {
            if (blogArticle.btitle.Length > 5 && blogArticle.bcontent.Length > 50)
            {
               
                blogArticle.bCreateTime = DateTime.Now;
                blogArticle.bUpdateTime = DateTime.Now;
                blogArticle.IsDeleted = false;
                var id = (await _blogArticleServices.Add(blogArticle));
                var model = await _blogArticleServices.QueryById(id);
                var images = await _imageServices.Upload(model.bID, model.imageUrlList);
                StringBuilder builder = new StringBuilder();
                foreach (var item in images)
                {
                    builder.Append(item.Id).Append(",");
                }
                if (builder.Length > 0)
                {
                    builder.Length--; // 删除最后一个字符（即逗号）
                }
                model.bImageslist=builder.ToString();
                await _blogArticleServices.Update(model);
                return id > 0 ? Success<string>(id.ObjToString()) : Failed("添加失败");
            }
            else
            {
                return Failed("文章标题不能少于5个字符，内容不能少于50个字符！");
            }
        }



  /*      /// <summary>
        /// 獲取首頁推薦
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMainStar")]
        public async Task<MessageModel<BlogViewModels>> GetMainStar(string bcategory)
        {
            if (!string.IsNullOrWhiteSpace(bcategory))
            {

                Expression<Func<BlogArticle, bool>> whereExpression = a => (a.bcategory == bcategory && a.IsDeleted == false) && ((a.bstarLevel != null && a.bstarLevel>=0));

                var pageModelBlog = await _blogArticleServices.QueryPage(whereExpression, 1, 10, " bstarLevel desc ");

                using (MiniProfiler.Current.Step("获取成功后，开始处理最终数据"))
                {
                    foreach (var item in pageModelBlog.data)
                    {
                        if (!string.IsNullOrEmpty(item.bcontent))
                        {
                            item.bRemark = (HtmlHelper.ReplaceHtmlTag(item.bcontent)).Length >= 200 ? (HtmlHelper.ReplaceHtmlTag(item.bcontent)).Substring(0, 200) : (HtmlHelper.ReplaceHtmlTag(item.bcontent));
                            int totalLength = 500;
                            if (item.bcontent.Length > totalLength)
                            {
                                item.bcontent = item.bcontent.Substring(0, totalLength);
                            }
                        }
                    }
                }
                 return SuccessPage(pageModelBlog);
            }
        }*/


        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddForMVP")]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<string>> AddForMVP([FromBody] BlogArticle blogArticle)
        {
            blogArticle.bCreateTime = DateTime.Now;
            blogArticle.bUpdateTime = DateTime.Now;
            blogArticle.IsDeleted = false;
            var id = (await _blogArticleServices.Add(blogArticle));
            return id > 0 ? Success<string>(id.ObjToString()) : Failed("添加失败");
        }
        /// <summary>
        /// 更新博客信息
        /// </summary>
        /// <param name="BlogArticle"></param>
        /// <returns></returns>
        // PUT: api/User/5
        [HttpPut]
        [Route("Update")]
        [Authorize(Permissions.Name)]
        public async Task<MessageModel<string>> Put([FromBody] BlogArticle BlogArticle)
        {
            if (BlogArticle != null && BlogArticle.bID > 0)
            {
                var model = await _blogArticleServices.QueryById(BlogArticle.bID);

                if (model != null)
                {
                    model.btitle = BlogArticle.btitle;
                    model.bcategory = BlogArticle.bcategory;
                    model.bsubmitter = BlogArticle.bsubmitter;
                    model.bcontent = BlogArticle.bcontent;
                    model.btraffic = BlogArticle.btraffic;
                    model.bnodeLevel = BlogArticle.bnodeLevel;
                    model.bstarLevel = BlogArticle.bstarLevel;
                    model.bstarList = BlogArticle.bstarList;
                    model.bparentId = BlogArticle.bparentId;
                    var images = await _imageServices.Upload(model.bID, BlogArticle.imageUrlList);
                    if (images!=null)
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (var item in images)
                        {
                            builder.Append(item.Id).Append(",");
                        }
                        if (builder.Length > 0)
                        {
                            builder.Length--; // 删除最后一个字符（即逗号）
                        }
                        model.bImageslist = builder.ToString();
                    }
                  

                    if (await _blogArticleServices.Update(model))
                    {
                        return Success<string>(BlogArticle?.bID.ObjToString());
                    }
                }
            }
            return Failed("更新失败");
        }



        /// <summary>
        /// 删除博客
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Permissions.Name)]
        [Route("Delete")]
        public async Task<MessageModel<string>> Delete(long id)
        {
            if (id > 0)
            {
                var blogArticle = await _blogArticleServices.QueryById(id);
                if (blogArticle == null)
                {
                    return Failed("查询无数据");
                }
                blogArticle.IsDeleted = true;
                return await _blogArticleServices.Update(blogArticle) ? Success(blogArticle?.bID.ObjToString(), "删除成功") : Failed("删除失败");
            }
            return Failed("入参无效");
        }
        /// <summary>
        /// apache jemeter 压力测试
        /// 更新接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ApacheTestUpdate")]
        public async Task<MessageModel<bool>> ApacheTestUpdate()
        {
            return Success(await _blogArticleServices.Update(new { bsubmitter = $"laozhang{DateTime.Now.Millisecond}", bID = 1 }), "更新成功");
        }



        #region 新增

        /// <summary>
        /// 獲取首頁推薦
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMainStar")]
        public async Task<MessageModel<List<BlogArticle>>> GetMainStar(string bcategory)
        {
            var res = new List<BlogArticle>();
            if (!string.IsNullOrWhiteSpace(bcategory))
            {

                var blogs = await _blogArticleServices.Query(d => d.bcategory == bcategory && d.bstarLevel>=0 && d.IsDeleted == false, d => d.bstarLevel, false);
                res = await _blogArticleServices.ListNavData(blogs);
            }
            return Success(res);
        }

        /// <summary>
        /// 獲取所有子節點
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSubBlog")]
        public async Task<MessageModel<List<BlogArticle>>> GetSubBlog([FromBody] BlogArticle blogArticle)
        {
            var res = new List<BlogArticle>();
            if (blogArticle != null && blogArticle.bID > 0)
            {
                var blogs = await _blogArticleServices.Query(d => d.bcategory == blogArticle.bcategory && d.bparentId == blogArticle.bID && d.IsDeleted == false, d => d.bID, false);
                res = await _blogArticleServices.ListNavData(blogs);
            }
            return Success(res);
        }


        /// <summary>
        /// 獲取所有一级节点
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubBlog")]
        public async Task<MessageModel<List<BlogArticle>>> GetAllBlog([FromBody] BlogArticle blogArticle)
        {
            var blogs = new List<BlogArticle>();
            if (blogArticle!=null)
            {
                 blogs = await _blogArticleServices.Query(d => d.bcategory == blogArticle.bcategory && d.bnodeLevel == 1 && d.IsDeleted == false, d => d.bID, false);
            }
             blogs = await _blogArticleServices.Query(d=>d.bnodeLevel==1 && d.IsDeleted == false, d => d.bID, false);
            
            return Success(blogs);
        }


        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DelBlogImages")]
        public async Task<bool> DelBlogImages(long id)
        {
            return await _imageServices.DelLoad(id);
        }
        





        #endregion










    }
}