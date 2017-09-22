using System;

namespace FacebookApi.Core.Models
{
    public class TokenInfoModel
    {
        public string Token { get; internal set; }
        public string TokenPortion { get; set; }
        public DateTime? RequestedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
    }
}