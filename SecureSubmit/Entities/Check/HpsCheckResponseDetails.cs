// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsCheckResponseDetails.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS check response details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsCheckResponseDetails
    {
        /// <summary>Gets or sets the message Type. Indicates type of response information.</summary>
        public string MessageType { get; set; }

        /// <summary>Gets or sets the response code. Check Processor response code(s). Can come from an overall single
        /// response code or from a detail of response information. Overall and detail response codes separated by hyphen (-).</summary>
        public string Code { get; set; }

        /// <summary>Gets or sets the processor message.</summary>
        public string Message { get; set; }

        /// <summary>Gets or sets the processor field number in error.</summary>
        public string FieldNumber { get; set; }

        /// <summary>Gets or sets the processor field name in error.</summary>
        public string FieldName { get; set; }
    }
}
