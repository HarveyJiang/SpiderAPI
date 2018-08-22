using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
    [Table("spider")]
    public class Spider : BaseModel
    {
        //public int Id { get; set; }
        //public string Name { get; set; }
        public bool AllowedDomain { get; set; } = false;
        public string Domains { get; set; }
        public SpiderTypeEnum SpiderType { get; set; } = SpiderTypeEnum.SINGE_PAGE;
        public bool IsDuplicate { get; set; }
        public bool IsProxy { get; set; }
        public bool IsLogin { get; set; }
        public string LoginUrl { get; set; }
        public string LoginArgs { get; set; }
        public bool IsScheduled { get; set; }
        public string CronExpression { get; set; }
        public string Tags { get; set; }
        public string ScrapySettings { get; set; }
        public SpiderStatusEnum Status { get; set; } = SpiderStatusEnum.NEW;
        [Write(false)]
        public List<SpiderStartUrls> SpiderStartUrls { get; set; }

        public Spider()
        {
            SpiderStartUrls = new List<SpiderStartUrls>();
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

        public enum SpiderStatusEnum
        {
            NEW,
            RUNNING,
            STOP,
            FINISHED,
            EXCEPTION
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
        public string Url { get; set; }
        public string RequestMethod { get; set; }
        public string RequestEncoding { get; set; }
        public string RequestParams { get; set; }
        public string ListFields { get; set; }
        public string DetailFields { get; set; }
        public string ListInfo { get; set; }
        public string PageInfo { get; set; }
        public int SpiderId { get; set; }

    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public class Result
    {
        //public IEnumerable<dynamic> Data { get; set; } = new List<dynamic>();
        public dynamic Data { get; set; }
        public bool Succeed { get; set; } = true;
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.success;
        public string Message { get; set; } = "执行成功。";
        public string StackTrace { get; set; } = "";
        /// <summary>
        /// 总条数
        /// </summary>
        public int? Count { get; set; } = 0;


        public enum MessageTypeEnum
        {
            success,
            warning,
            info,
            error,
        }

    }

    public class Page<T>
    {
        public long CurrentPage { get; set; }
        public long TotalPages { get; set; }
        public long TotalItems { get; set; }
        public long ItemsPerPage { get; set; }
        public List<T> Items { get; set; }
    }



    public class Condition<T>
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 10;
        public string Sorts { get; set; }
        public string Key { get; set; }
        public string Fields { get; set; }


    }
}
