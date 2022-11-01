﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Authgear.Xamarin
{
    public class ServerException : Exception
    {
        public string Name { get; private set; }
        public string Reason { get; private set; }
        public JsonDocument? Info { get; private set; }
        public ServerException(string name, string reason, string message, JsonDocument? info) : base(message)
        {
            Name = name;
            Reason = reason;
            Info = info;
        }
    }
}
