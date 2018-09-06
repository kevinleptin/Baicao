using System;
namespace Baicao.Models.dto
{
    public class ConsumerInfoResult : ApiResult
    {
        public ConsumerInfoResult()
        {
        }

        public string Dadacode
        {
            get;
            set;
        }

        public string Couponcode
        {
            get;
            set;
        }


    }
}
