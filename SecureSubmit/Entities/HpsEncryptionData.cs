// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsEncryptionData.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Defines the HpsEncryptionData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    /// <summary>
    /// The HPS encryption data.
    /// </summary>
    public class HpsEncryptionData
    {
        /// <summary>Gets or sets the encryption version used on the supplied data.</summary>
        public string Version { get; set; }

        /// <summary>Gets or sets the encrypted track number (optional). This is required in certain
        /// encryption versions when supplying track data and indicates which track has been supplied.</summary>
        public string EncryptedTrackNumber { get; set; }

        /// <summary>Gets or sets the KTB (optional). This is required in certain encryption versions; 
        /// the <u>K</u>ey <u>T</u>ransmission <u>B</u>lock (KTB) used at the point of sale.</summary>
        public string Ktb { get; set; }

        /// <summary>Gets or sets the KSN (optional). This is required in certain encryption versions; 
        /// the <u>K</u>ey <u>S</u>erial <u>N</u>umber (KSN) used at the point of sale.</summary>
        public string Ksn { get; set; }
    }
}