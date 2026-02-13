namespace EasyUmp
{
    /// <summary>
    /// Standardized error messages.
    /// </summary>
    public static class ErrorMessages
    {
        public const string NotSupportedMessage = "UMP is only supported on Android devices.";
        public const string UnknownUmpError = "Unknown UMP error.";
        public const string MalformedUmpError = "Malformed UMP error payload.";
        public const string OperationInProgress = "Another UMP operation is already in progress.";
        public const string EditorSimulatedFailure = "Editor simulated failure.";
    }
}
