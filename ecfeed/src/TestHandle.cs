using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)] public sealed class TestHandle
    {
        [JsonIgnore] private readonly DataSession sessionData;
        [JsonIgnore] private readonly string id;
        [JsonIgnore] private bool pending;
        [JsonProperty(RequestFeedbackBody.Data)] private string data;
        [JsonProperty(RequestFeedbackBody.Status)] private string status;
        [JsonProperty(RequestFeedbackBody.Duration)] private int? duration;
        [JsonProperty(RequestFeedbackBody.Comment)] private string comment;
        [JsonProperty(RequestFeedbackBody.Custom)] private Dictionary<string, string> custom;

        internal TestHandle(DataSession sessionData, string data, string id)
        {
            this.sessionData = sessionData;
            this.data = data;
            this.id = id;
            this.pending = true;
        }

        public string addFeedback(bool status, int? duration = null, string comment = null, Dictionary<string, string> custom = null)
        {
            if (this.pending)
            {
                this.status = status ? RequestFeedbackBody.StatusPassed : RequestFeedbackBody.StatusFailed;
                this.duration = duration;
                this.comment = comment;
                this.custom = custom;

                sessionData.AddTest(id, this);

                this.pending = false;
            }

            return comment != null ? comment : "feedback";
        }

    }
}
    