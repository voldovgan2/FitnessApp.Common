namespace FitnessApp.Common.ServiceBus.Nats
{
    public static class Topic
    {
        public const string NEW_USER_REGISTERED = "NEW_USER_REGISTERED";
        public const string NEW_FOLLOW_REQUEST = "NEW_FOLLOW_REQUEST";
        public const string FOLLOW_REQUEST_CONFIRMED = "FOLLOW_REQUEST_CONFIRMED";
    }
}