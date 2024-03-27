using AutoMapper;
using Blog.Core.Common;
using Blog.Core.IRepository.Base;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using Blog.Core.Services.BASE;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        /// 实体模型导航属性获取值
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<BlogArticle> NavData(BlogArticle blogArticle) {
            ///父节点
            blogArticle.Father = (await base.Query(a => a.bID == blogArticle.bparentId)).FirstOrDefault();

            ///推荐列表
            if (!string.IsNullOrEmpty(blogArticle.bstarList))
            {
                List<long> starListIds = blogArticle.bstarList.Split(',').Select(long.Parse).ToList();
                blogArticle.StarList = (await base.Query(a => starListIds.Contains(a.bID)));
            }
                blogArticle.DisplayImageData = await _imageServices.Query(s => s.BlogArticleId == blogArticle.bID);
            return blogArticle;
        }

        /// <summary>
        /// 获取视图博客详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlogViewModels> GetBlogDetails(long id)
        {
            var blogArticle = (await base.Query(a => a.bID == id)).FirstOrDefault();
            blogArticle = await NavData(blogArticle);
            BlogViewModels models = null;

            if (blogArticle != null)
            {
                blogArticle.StarList = await ListNavData(blogArticle.StarList);
                models = _mapper.Map<BlogViewModels>(blogArticle);
                blogArticle.btraffic += 1;
                await base.Update(blogArticle, new List<string> { "btraffic" });
            }
            return models;

        }

        /// <summary>
        /// 根据id导航属性赋值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlogArticle> GetBlogId(long id)
        {
            var blogArticle = (await base.Query(a => a.bID == id)).FirstOrDefault();
            blogArticle = await NavData(blogArticle);
            return blogArticle;
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

        public async Task<List<BlogArticle>> ListNavData(List<BlogArticle> blogArticlelist)
        {
            var res = new List<BlogArticle>();
            if (blogArticlelist == null) return null;
            foreach (var blogArticle in blogArticlelist)
            {
               var data= await NavData(blogArticle);
                res.Add(data);
            }
            return res;
        }
    }
}
