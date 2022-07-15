using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class UserInfo
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = "";
        [JsonPropertyName("https://authgear.com/claims/user/is_anonymous")]
        public bool IsAnonymous { get; set; }
        [JsonPropertyName("https://authgear.com/claims/user/is_verified")]
        public bool IsVerified { get; set; }
        [JsonPropertyName("https://authgear.com/claims/user/can_reauthenticate")]
        public bool CanReauthenticate { get; set; }
        [JsonPropertyName("custom_attributes")]
        public Dictionary<string, object>? CustomAttributes { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("email_verified")]
        public bool? EmailVerified { get; set; }
        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("phone_number_verified")]
        public bool? PhoneNumberVerified { get; set; }
        [JsonPropertyName("preferred_username")]
        public string? PreferredUsername { get; set; }
        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }
        [JsonPropertyName("middle_name")]
        public string? MiddleName { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }
        [JsonPropertyName("picture")]
        public string? Picture { get; set; }
        [JsonPropertyName("profile")]
        public string? Profile { get; set; }
        [JsonPropertyName("website")]
        public string? Website { get; set; }
        [JsonPropertyName("gender")]
        public string? Gender { get; set; }
        [JsonPropertyName("birthdate")]
        public string? Birthdate { get; set; }
        [JsonPropertyName("zoneinfo")]
        public string? Zoneinfo { get; set; }
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
        [JsonPropertyName("address")]
        public UserInfoAddress? address { get; set; }
    }
}
