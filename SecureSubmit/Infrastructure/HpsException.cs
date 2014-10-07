// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsException.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Infrastructure
{
    using System;

    /// <summary>The HPS exception.</summary>
    public class HpsException : Exception
    {
        public HpsException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}