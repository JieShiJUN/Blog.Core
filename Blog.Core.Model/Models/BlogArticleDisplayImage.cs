using SqlSugar;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 博客文章展示图
    /// </summary>
    public class BlogArticleDisplayImage
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsNullable = false, IsIdentity = false, IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 图片路径或标识
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string ImagePath { get; set; }

        /// <summary>
        /// 图片描述
        /// </summary>
        [SugarColumn(Length = 500, IsNullable = true)]
        public string Description { get; set; }

        // 其他可能的属性...

        /// <summary>
        /// 是否為展示廳
        /// </summary>
        public int isMain { get; set; }

        /// <summary>
        /// 外键，指向博客文章
        /// </summary>
        public long BlogArticleId { get; set; }

        /// <summary>
        /// 导航属性，表示该展示图所属的博客文章
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(BlogArticleId))]
        public BlogArticle BlogArticle { get; set; }
    }
}