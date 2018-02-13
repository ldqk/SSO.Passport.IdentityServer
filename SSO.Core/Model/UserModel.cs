using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SSO.Core.Model
{
    public class UserModel
    {
        [JsonProperty("user")]
        public UserInfoLoginModel User { get; set; }
        [JsonProperty("acl")]
        public List<AccessControl> Acl { get; set; }
        [JsonProperty("menus")]
        public List<Menu> Menus { get; set; }
    }
    public class UserInfoLoginModel
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("Locked")]
        public bool Locked { get; set; }
        [JsonProperty("RegisterTime")]
        public DateTime RegisterTime { get; set; }
        [JsonProperty("LastLoginTime")]
        public DateTime LastLoginTime { get; set; }
        [JsonProperty("AccessKey")]
        public string AccessKey { get; set; }
        [JsonProperty("IsMaster")]
        public bool IsMaster { get; set; }
        [JsonProperty("IsPreset")]
        public bool IsPreset { get; set; }
        [JsonProperty("Avatar")]
        public string Avatar { get; set; }
        [JsonProperty("HasPermission")]
        public bool HasPermission { get; set; }
    }
}