using System.Runtime.Serialization;

namespace UnityBuildAutomation.SourceControl
{
    [Serializable]
    internal class SourceControlException : Exception
    {
        public SourceControlException()
        {
        }

        public SourceControlException(string? message) : base(message)
        {
        }

        public SourceControlException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}