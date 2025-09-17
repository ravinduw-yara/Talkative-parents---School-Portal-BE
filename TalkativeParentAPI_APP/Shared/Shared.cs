using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TalkativeParentAPI_APP
{
    public static partial class Shared
    {
        public static TelemetryClient telemetryClient = new TelemetryClient();
    }

    public class SBMessageStatus
    {
        public int Status { get; set; }
        public int SbCount { get; set; }
    }
}
