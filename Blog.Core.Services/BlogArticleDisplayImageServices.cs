using Blog.Core.IServices.BASE;
using Blog.Core.Model;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using Blog.Core.Services.BASE;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Blog.Core.IServices
{
    /// <summary>
    /// ITasksLogServices
    /// </summary>	
    public class BlogArticleDisplayImageServices : BaseServices<BlogArticleDisplayImage>, IBlogArticleDisplayImageServices
    {
        private readonly IWebHostEnvironment _env;
        public BlogArticleDisplayImageServices(IWebHostEnvironment webHostEnvironment)
        {
            _env = webHostEnvironment;
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DelLoad(long id)
        {
           return await base.DeleteById(id);
        }

        /// <summary>
        /// 主图上传
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="page"></param>
        /// <param name="intPageSize"></param>
        /// <param name="runTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<List<BlogArticleDisplayImage>> Upload(long bid, List<string> imgUrlList){
            if (imgUrlList == null) return null;
           List<BlogArticleDisplayImage> filename=new List<BlogArticleDisplayImage>();
            foreach (var file in imgUrlList)
            {
                BlogArticleDisplayImage image = new BlogArticleDisplayImage() { 
                    BlogArticleId = bid,
                    ImagePath = file,
                    isMain = 1
                };
                var id =  await Add(image);
                var res = await base.QueryById(id);
                filename.Add(image);
            }
            return filename;
        }

        
      
    }
}
                    