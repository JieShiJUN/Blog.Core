using Blog.Core.IServices.BASE;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Core.IServices
{
    public interface IBlogArticleServices :IBaseServices<BlogArticle>
    {
        Task<List<BlogArticle>> GetBlogs();
        Task<BlogViewModels> GetBlogDetails(long id);

        Task<BlogArticle> GetBlogId(long id);

        Task<BlogArticle> NavData(BlogArticle blogArticle, bool img = true, bool star = false, bool child = false, bool father = false);

        Task<List<BlogArticle>> ListNavData(List<BlogArticle> blogArticlelist,bool img=true,bool star=false,bool child=false,bool father=false);
    }

}
