using Blog.Core.Model.ViewModels;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 博客文章
    /// </summary>
    public class BlogArticle
    {
        /// <summary>
        /// 主键
        /// </summary>
        /// 这里之所以没用RootEntity，是想保持和之前的数据库一致，主键是bID，不是Id
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false)]
        public long bID { get; set; }

        /// <summary>
        /// 節點等級
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? bnodeLevel { get; set; }

        /// <summary>
        /// 推薦等級
        /// </summary>  /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? bstarLevel { get; set; }

        /// <summary>
        /// 父節點
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? bparentId { get; set; }
       
        
        /// <summary>
        /// 父節點實體
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(bparentId))]
        public BlogArticle Father { get; set; }

        /// <summary>
        /// 相关推荐
        /// </summary>
        [SugarColumn(Length = 2000, IsNullable = true)]
        public string bstarList { get; set; }

        /// <summary>
        /// 推薦列表實體
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(bstarList))]
        public List<BlogArticle> StarList { get; set; }

        /// <summary>
        /// 展示图
        /// </summary>
        [SugarColumn(Length = 256, IsNullable = true)]
        public string bImageslist { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(Length = 600, IsNullable = true)]
        public string bsubmitter { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(bsubmitter))]
        public SysUserInfo User { get; set; }

        /// <summary>
        /// 标题blog
        /// </summary>
        [SugarColumn(Length = 256, IsNullable = true)]
        public string btitle { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [SugarColumn(Length = 2000, IsNullable = true)]
        public string bcategory { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [SugarColumn(Length = 2000, IsNullable = true)]
        public string bcontent { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int btraffic { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int bcommentNum { get; set; }

        /// <summary> 
        /// 修改时间
        /// </summary>
        public DateTime bUpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime bCreateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 2000, IsNullable = true)]
        public string bRemark { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? IsDeleted { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(BlogArticleDisplayImage.ImagePath))]
        public List<string> imageUrlList { get; set; }
        /// <summary>
        /// 评论
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(BlogArticleComment.bID))]
        public List<BlogArticleComment> Comments { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(BlogArticleDisplayImage))]
        public List<BlogArticleDisplayImage> DisplayImageData { get; set; }

    }
}