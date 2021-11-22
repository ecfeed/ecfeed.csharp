namespace EcFeed
{
    internal static class HelperMessageStatus
    {
        internal static bool IsTransmissionFinished(MessageStatus messageStatus)
        {
            return messageStatus.Status.Contains("END_DATA");
        }
    }
}