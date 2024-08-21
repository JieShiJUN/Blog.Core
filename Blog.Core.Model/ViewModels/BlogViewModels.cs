using Blog.Core.Model.Models;
using SqlSugar;
using System;

namespace Blog.Core.Model.ViewModels
{
    /// <summary>
    /// 博客信息展示类
    /// </summary>
    public class BlogViewModels
    {

        /// <summary>
        /// 節點等級
        /// </summary>
        public int? bnodeLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long bID { get; set; }
        /// <summary>创建人
        /// 
        /// </summary>
        public string bsubmitter { get; set; }

        /// <summary>博客标题
        /// 
        /// </summary>
        public string btitle { get; set; }

        /// <summary>摘要
        /// 
        /// </summary>
        public string digest { get; set; }

        /// <summary>
        /// 上一篇
        /// </summary>
        public string previous { get; set; }

        /// <summary>
        /// 上一篇id
        /// </summary>
        public long previousID { get; set; }

        /// <summary>
        /// 下一篇
        /// </summary>
        public string next { get; set; }

        /// <summary>
        /// 下一篇id
        /// </summary>
        public long nextID { get; set; }

        /// <summary>类别
        /// 
        /// </summary>
        public string bcategory { get; set; }

        /// <summary>内容
        /// 
        /// </summary>
        public string bcontent { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int btraffic { get; set; }
        /// <summary>
        /// 推荐等级
        /// </summary>
        public int? bstarLevel { get; set; }

        /// <summary>
        /// 父节点id
        /// </summary>
        public long? bparentId { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int bcommentNum { get; set; }

        /// <summary> 修改时间
        /// 
        /// </summary>
        public DateTime bUpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime bCreateTime { get; set; }
        /// <summary>备注
        /// 
        /// </summary>
        public string bRemark { get; set; }

        public int bDisplayImageId { get; set; }

        public string bImageslist { get; set; }

        public List<string> imageUrlList { get; set; }

        public UploadFileDto image { get; set; }
        /// <summary>
        /// 推薦列表實體
        /// </summary>
        public List<BlogArticle> StarList { get; set; }

        public List<BlogArticleDisplayImage> DisplayImageData { get; set; }

        public BlogViewModels Father { get; set; }

        /// <summary>
        /// 相关推荐
        /// </summary>
        public string bstarList { get; set; }

        public List<BlogArticle> Child { get; set; }

        public List<BlogArticleComment> Comments { get; set; }


    }
}
