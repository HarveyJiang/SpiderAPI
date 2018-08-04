using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderAPI.Models
{

    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Write(false)]
        public string creator { get; set; }
        [Write(false)]
        public string editor { get; set; }
        [Write(false)]
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        [Write(false)]
        public DateTime EditingTime { get; set; }

        public BaseModel()
        {
            this.EditingTime = this.CreationTime;
        }

    }
    [Table("spider_basic")]
    public class SpiderBasic : BaseModel
    {
        //public int Id { get; set; }
        //public string Name { get; set; }
        public string Domain { get; set; }
        public SpiderTypeEnum SpiderType { get; set; } = SpiderTypeEnum.SINGE_PAGE;
        public bool IsDuplicate { get; set; }
        public bool IsProxy { get; set; }
        public bool IsLogin { get; set; }
        public string LoginUrl { get; set; }
        public string LoginArgs { get; set; }
        public bool IsScheduled { get; set; }
        public string CronExpression { get; set; }
        public string Tags { get; set; }
        public int ScrapySettingId { get; set; }
        public int SpiderStartUrlsId { get; set; }
        public string Status { get; set; }
        [Write(false)]
        public ScrapySetting ScrapySetting { get; set; }
        [Write(false)]
        public ScrapySetting SpiderStartUrls { get; set; }

        public SpiderBasic()
        {
            ScrapySetting = new ScrapySetting();
            SpiderStartUrls = new ScrapySetting();
        }

        public enum SpiderTypeEnum
        {
            /// <summary>
            /// 列表页面
            /// </summary>
            LIST_PAGE = 1,
            /// <summary>
            /// 单页面爬
            /// </summary>
            SINGE_PAGE = 2,
            /// <summary>
            /// link 
            /// </summary>
            LINK_PAGE = 3,
            /// <summary>
            /// API 接口
            /// </summary>
            API_PAGE = 4
        }
    }
    [Table("scrapy_setting")]
    public class ScrapySetting : BaseModel
    {
        //public string Id { get; set; }
        public int CONCURRENT_REQUESTS { get; set; }
        public string LOG_LEVEL { get; set; }
        public float DOWNLOAD_DELAY { get; set; }
        public int DOWNLOAD_TIMEOUT { get; set; }
        public int CLOSESPIDER_TIMEOUT { get; set; }
        public int CLOSESPIDER_ITEMCOUNT { get; set; }
        public int CLOSESPIDER_PAGECOUNT { get; set; }
        public string HTTPERROR_ALLOWED_CODES { get; set; }
        public bool LOG_ENABLED { get; set; }
        public bool COOKIES_ENABLED { get; set; }
        public bool REDIRECT_ENABLED { get; set; }
        public bool ROBOTSTXT_OBEY { get; set; }
    }

    [Table("spider_start_urls")]
    public class SpiderStartUrls : BaseModel
    {
        [Write(false)]
        new public string Name { get; set; }
        public string Params { get; set; }
        public string XPath { get; set; }
        public string Url { get; set; }
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public class Result
    {
        //public IEnumerable<dynamic> Data { get; set; } = new List<dynamic>();
        public dynamic Data { get; set; }
        public bool Succeed { get; set; } = true;
        public string Message { get; set; } = "ok";
        public string StackTrace { get; set; } = "";
        /// <summary>
        /// 总条数
        /// </summary>
        public int? Count { get; set; } = 0;
        

    }

    public class Condition
    {

        public Dictionary<string, string> FieldAndKeyWord = new Dictionary<string, string>();
        public Dictionary<string, int?> Pagination = new Dictionary<string, int?>();
        public Dictionary<string, string> Sort = new Dictionary<string, string>();

        public Condition()
        {
            Pagination.Add("PageSize", 10);
            Pagination.Add("PageIndex", 1);
            //Sort.Add("Id", "desc");
        }


    }
}
