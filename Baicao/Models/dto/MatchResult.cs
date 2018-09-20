using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Baicao.Models.dto
{
    public class MatchResult : ApiResult
    {
        public MatchResult()
        {
            MatchType = -1;
        }
        public int MatchType { get; set; }

    }
}