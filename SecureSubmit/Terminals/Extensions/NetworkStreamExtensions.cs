using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Terminals.Extensions {
    internal static class NetworkStreamExtensions {
        public static byte[] GetTerminalResponse(this NetworkStream stream) {
            var buffer = new byte[4096];
            int bytesReceived = stream.ReadAsync(buffer, 0, buffer.Length).Result;

            byte[] readBuffer = new byte[bytesReceived];
            Array.Copy(buffer, readBuffer, bytesReceived);

            var code = (ControlCodes)readBuffer[0];
            if (code == ControlCodes.NAK)
                return null;
            else if (code == ControlCodes.EOT)
                throw new HpsMessageException("Terminal returned EOT for the current message.");
            else if (code == ControlCodes.ACK) {
                return stream.GetTerminalResponse();
            }
            else if (code == ControlCodes.STX) {
                var queue = new Queue<byte>(readBuffer);

                // break off only one message
                var rec_buffer = new List<byte>();
                do {
                    rec_buffer.Add(queue.Dequeue());
                    if (rec_buffer[rec_buffer.Count - 1] == (byte)ControlCodes.ETX)
                        break;
                }
                while (true);

                // Should be the LRC
                rec_buffer.Add(queue.Dequeue());
                return rec_buffer.ToArray();
            }
            else throw new HpsMessageException(string.Format("Unknown message received: {0}", code));
        }
    }
}
