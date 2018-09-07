using System;
namespace Baicao.Models.dto
{
    public class matchdto
    {
        public matchdto()
        {
        }
        /// <summary>
        /// 发起人的openid
        /// </summary>
        public string openid
        {
            get;
            set;
        }

        /// <summary>
        /// 别人的拼搭码
        /// </summary>
        public string dadacode
        {
            get;
            set;
        }

    }
}
