using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class OauthException : Exception
    {
        static internal string FormatOauthExceptionMessage(string error, string? errorDescription)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append(error);
            if (errorDescription != null)
            {
                messageBuilder.Append(": ");
                messageBuilder.Append(errorDescription);
            }
            return messageBuilder.ToString();
        }
        public string Error { get; private set; }
        public string? ErrorDescription { get; private set; }
        public string? State { get; private set; }
        public string? ErrorUri { get; private set; }
        public OauthException(string error, string? errorDescription, string? state, string? errorUri) : base(FormatOauthExceptionMessage(error, errorDescription))
        {
            Error = error;
            ErrorDescription = errorDescription;
            State = state;
            ErrorUri = errorUri;
        }
    }
}
