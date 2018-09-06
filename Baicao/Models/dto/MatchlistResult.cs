using System;
using System.Collections.Generic;
namespace Baicao.Models.dto
{
    public class MatchlistResult : ApiResult
    {
        public MatchlistResult()
        {
            List = new List<UserInfoSimple>();
        }

        public List<UserInfoSimple> List { get; set; }
    }

    public class UserInfoSimple {
        public string Nickname
        {
            get;
            set;
        }

        public string HeadImgUrl
        {
            get;
            set;
        }

        public DateTime Invdate
        {
            get;
            set;
        }

        public int MatchType
        {
            get;
            set;
        }

        public string InvitionType
        {
            get;
            set;
        }

    }
}
