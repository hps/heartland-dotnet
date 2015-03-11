// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsCreditCard.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Defines the HpsCreditCardInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>The HPS credit card info.</summary>
    public class HpsCreditCard
    {
        /// <summary>AMEX Regex</summary>
        private static readonly Regex AmexRegex = new Regex(@"^3[47][0-9]{13}$", RegexOptions.Compiled);
        
        /// <summary>MasterCard Regex</summary>
        private static readonly Regex MasterCardRegex = new Regex(@"^5[1-5][0-9]{14}$", RegexOptions.Compiled);
        
        /// <summary>Visa Regex</summary>
        private static readonly Regex VisaRegex = new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$", RegexOptions.Compiled);

        /// <summary>Diners Club Regex</summary>
        private static readonly Regex DinersClubRegex = new Regex(@"^3(?:0[0-5]|[68][0-9])[0-9]{11}$", RegexOptions.Compiled);

        /// <summary>Route Club Regex</summary>
        private static readonly Regex RouteClubRegex = new Regex(@"^(2014|2149)", RegexOptions.Compiled);

        /// <summary>Discover Club Regex</summary>
        private static readonly Regex DiscoverRegex = new Regex(@"^6(?:011|5[0-9]{2})[0-9]{12}$", RegexOptions.Compiled);

        /// <summary>JCB Regex</summary>
        private static readonly Regex JcbRegex = new Regex(@"^(?:2131|1800|35\d{3})\d{11}$", RegexOptions.Compiled);
        
        /// <summary>Private HashTable containing all the REGEX tests (by card type name)</summary>
        private Hashtable regexHash;

        /// <summary>Gets or sets the credit card number.</summary>
        public string Number { get; set; }

        /// <summary>Gets or sets the card security code.</summary>
        public string Cvv { get; set; }

        /// <summary>Gets or sets the expiration month.</summary>
        public int ExpMonth { get; set; }

        /// <summary>Gets or sets the expiration year.</summary>
        public int ExpYear { get; set; }

        /// <summary>Gets the credit card type; based on the credit card number.</summary>
        public string CardType
        {
            get
            {
                string cardType = "Unknown";

                try
                {
                    string cardNum = this.Number.Replace(" ", string.Empty).Replace("-", string.Empty);
                    foreach (string cardTypeName in this.RegexHash.Keys)
                    {
                        if (((Regex)this.RegexHash[cardTypeName]).IsMatch(cardNum))
                        {
                            cardType = cardTypeName;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                }

                return cardType;
            }
        }

        /// <summary>Gets or sets the HashTable containing all the REGEX tests (by card type name)</summary>
        private Hashtable RegexHash
        {
            get
            {
                if (this.regexHash == null)
                {
                    this.regexHash = new Hashtable();
                    this.regexHash.Add("Amex", AmexRegex);
                    this.regexHash.Add("MasterCard", MasterCardRegex);
                    this.regexHash.Add("Visa", VisaRegex);
                    this.regexHash.Add("DinersClub", DinersClubRegex);
                    this.regexHash.Add("EnRoute", RouteClubRegex);
                    this.regexHash.Add("Discover", DiscoverRegex);
                    this.regexHash.Add("Jcb", JcbRegex);
                }

                return this.regexHash;
            }

            set
            {
                this.regexHash = value;
            }
        }
    }
}