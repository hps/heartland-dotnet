// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsCheckException.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS check exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Infrastructure
{
    using Entities;
    using System;
    using System.Collections.Generic;

    public class HpsCheckException :  Exception
    {
        /// <summary>Initializes a new instance of the <see cref="HpsCheckException"/> class.</summary>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="details">The check exception details.</param>
        /// <param name="code">The error code.</param>
        /// <param name="message">The message.</param>
        public HpsCheckException(int transactionId, List<HpsCheckResponseDetails> details, int code, string message)
            : base(message)
        {
            TransactionId = transactionId;
            Details = details;
            Code = code;
        }

        /// <summary>Gets or sets the transaction ID.</summary>
        public int TransactionId { get; set; }

        /// <summary>Gets or sets the transaction response code.</summary>
        public int Code { get; set; }

        /// <summary>Gets or sets the exception details.</summary>
        public List<HpsCheckResponseDetails> Details { get; set; }
    }
}
