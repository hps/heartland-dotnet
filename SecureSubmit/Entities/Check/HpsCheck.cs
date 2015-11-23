namespace SecureSubmit.Entities
{
    using Hps.Exchange.PosGateway.Client;

    public class HpsCheck
    {
        /// <summary>Check routing number.</summary>
        public string RoutingNumber { get; set; }

        /// <summary>Check account number.</summary>
        public string AccountNumber { get; set; }

        /// <summary>Check number.</summary>
        public string CheckNumber { get; set; }

        /// <summary>MICR number.</summary>
        public string MicrNumber { get; set; }

        /// <summary>Account Type: Checking, Savings. NOTE: If processing with Colonnade, Account Type must be specified.</summary>
        public accountTypeType? AccountType { get; set; }

        /// <summary>Data Entry Mode indicating whether the check data was manually entered or obtained from a check reader.</summary>
        public dataEntryModeType? DataEntryMode { get; set; }

        /// <summary>Check type.</summary>
        public checkTypeType? CheckType { get; set; }

        /// <summary>Indicates check verify. Requires processor setup to utilize. Please contact your HPS representative
        /// for more information on the GETI eBronze program.</summary>
        public bool CheckVerify { get; set; }

        /// <summary>Indicates check verify. Requires processor setup to utilize. Please contact your HPS representative
        /// for more information on the GETI eBronze program.</summary>
        public bool AchVerify { get; set; }

        /// <summary>NACHA Standard Entry Class Code. NOTE: If processing with Colonnade, SECCode is required for Check
        /// Sale transactions.</summary>
        public string SecCode { get; set; }

        /// <summary>The check holder.</summary>
        public HpsCheckHolder CheckHolder { get; set; }
    }
}
