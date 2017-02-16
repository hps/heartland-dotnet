using System;
using SecureSubmit.Terminals.PAX;

namespace SecureSubmit.Terminals.Abstractions {
    public interface IDeviceResponse {
        string Status { get; set; }
        string Command { get; set; }
        string Version { get; set; }
        string DeviceResponseCode { get; set; }
        string DeviceResponseText { get; set; }
    }
}
