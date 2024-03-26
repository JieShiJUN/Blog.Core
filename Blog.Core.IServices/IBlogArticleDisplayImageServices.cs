using Blog.Core.IServices.BASE;
using Blog.Core.Model;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using System.Collections.Generic;

namespace Blog.Core.IServices
{
    /// <summary>
    /// ITasksLogServices
    /// </summary>	
    public interface IBlogArticleDisplayImageServices : IBaseServices<BlogArticleDisplayImage>
	{
        /// <summary>
        /// 主图上传
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="page"></param>
        /// <param name="intPageSize"></param>
        /// <param name="runTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
		public Task<List<BlogArticleDisplayImage>> Upload(long bid, List<string> imgUrlList);

        /// <summary>
        /// 移除图片
        /// </summary>
        /// <param name="bid"></param>
        /// <param name="imgUrlList"></param>
        /// <returns></returns>

        public Task<bool> DelLoad(long id);
    }
}
                    