using System;

namespace SecureSubmit.Terminals.Abstractions {
    interface IDeviceMessage {
        byte[] GetSendBuffer();
    }
}
