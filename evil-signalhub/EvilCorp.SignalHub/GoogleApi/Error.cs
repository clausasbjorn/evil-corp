using System.Collections.Generic;

namespace EvilCorp.SignalHub.GoogleApi
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<ErrorDetails> Errors { get; set; }
    }

    public class ErrorDetails
    {
        public string Domain { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
    }
}