using AutoMapper;
using Blog.Core.Common;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using Blog.Core.Services.BASE;
using SqlSugar;

namespace Blog.Core.Services
{
    public class BlogArticleServices : BaseServices<BlogArticle>, IBlogArticleServices
    {
        IMapper _mapper;
        public IBlogArticleDisplayImageServices _imageServices { get; set; }
        public BlogArticleServices(IMapper mapper, IBlogArticleDisplayImageServices imageServices)
        {
            this._mapper = mapper;
            _imageServices = imageServices;
        }


        /// <summary>
        /// 获取视图博客详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlogViewModels> GetBlogDetails(long id)
        {
            //var blogArticle = (await base.Query(a => a.bID == id)).FirstOrDefault();
            var blogArticle = (await base.QueryById(id));
            blogArticle = await NavData(blogArticle);
            BlogViewModels models = null;

            if (blogArticle != null)
            {
                blogArticle = await NavData(blogArticle, father: true, star: true);
                blogArticle.StarList = await ListNavData(blogArticle.StarList, father: true);
                models = _mapper.Map<BlogViewModels>(blogArticle);
                blogArticle.btraffic += 1;
                await base.Update(blogArticle, new List<string> { "btraffic" });
            }
            return models;

        }


        /// <summary>
        /// 获取博客列表
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 10)]
        public async Task<List<BlogArticle>> GetBlogs()
        {
            var bloglist = await base.Query(a => a.bID > 0, a => a.bID);
            return bloglist;

        }
        

        public async Task<List<BlogArticle>> ListNavData(List<BlogArticle> blogArticlelist, bool img = true, bool star = false, bool child = false, bool father = false)
        {
            var res = new List<BlogArticle>();
            if (blogArticlelist == null) return null;
            foreach (var blogArticle in blogArticlelist)
            {
                var data = await NavData(blogArticle,img,star,child,father);
                res.Add(data);
            }
            return res;
        }

        public async Task<BlogArticle> NavData(BlogArticle blogArticle, bool img = true, bool star = false, bool childs = false, bool father = false)
        {
            if (img)
            {
                blogArticle.DisplayImageData = await _imageServices.Query(s => s.BlogArticleId == blogArticle.bID);
            }
            if (star)
            {
                if (!string.IsNullOrEmpty(blogArticle.bstarList))
                {
                    List<long> starListIds = blogArticle.bstarList.Split(',').Select(long.Parse).ToList();
                    blogArticle.StarList = (await base.Query(a => starListIds.Contains(a.bID)));
                }
            }
            if (childs)
            {
                var Child = (await base.Query(a => a.bparentId == blogArticle.bID));
                Child = await ListNavData(Child,child:childs);
                blogArticle.Child = Child;
            }
            if (father)
            {
                if (blogArticle.bparentId!=null)
                {
                    blogArticle.Father = await base.QueryById(blogArticle.bparentId);
                }
               
                //blogArticle.Father = (await base.Query(a => a.bID == blogArticle.bparentId)).FirstOrDefault();
            }
            return blogArticle;
        }
    }
}
