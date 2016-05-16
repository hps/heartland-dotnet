namespace SecureSubmit.Abstractions
{
    public interface IHpsServicesConfig
    {
        string SecretApiKey { get; }
        string DeveloperId { get; }
        int DeviceId { get; }
        int LicenseId { get; }
        string Password { get; }
        int SiteId { get; }
        string SiteTrace { get; }
        string UserName { get; }
        string VersionNumber { get; }
        string ServiceUrl { get; }
    }
}
