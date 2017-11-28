using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureSubmit.Fluent;

namespace SecureSubmit.Terminals.PAX {
    public class InputAccountBuilder : HpsBuilderAbstract<PaxDevice, InputAccountResponse> {
        private bool allowMagStripe = true;
        private bool allowManualEntry = true;
        private bool allowContactless = true;
        private bool allowScanner = true;
        private bool requireExpiry;
        private int timeout = 200;
        private EncryptionType encryptionType = EncryptionType.None;
        private int? keySlot;
        private int minAccountLength = 10;
        private int maxAccountLength = 19;

        public InputAccountBuilder AllowMagStripe(bool value) { allowMagStripe = value; return this; }
        public InputAccountBuilder AllowManualEntry(bool value) { allowManualEntry = value; return this; }
        public InputAccountBuilder AllowContactless(bool value) { allowContactless = value; return this; }
        public InputAccountBuilder AllowScanner(bool value) { allowScanner = value; return this; }
        public InputAccountBuilder RequireExpiry(bool value) { requireExpiry = value; return this; }
        public InputAccountBuilder WithTimeout(int value) { timeout = value; return this; }
        public InputAccountBuilder WithEncryptionType(EncryptionType value) { encryptionType = value; return this; }
        public InputAccountBuilder WithKeySlot(int? value) { keySlot = value; return this; }
        public InputAccountBuilder WithMinAccountLength(int value) { minAccountLength = value; return this; }
        public InputAccountBuilder WithMaxAccountLength(int value) { maxAccountLength = value; return this; }

        public InputAccountBuilder(PaxDevice device) : base(device) { }

        public override InputAccountResponse Execute() {
            base.Execute();

            var _params = new ArrayList();
            _params.Add(allowMagStripe ? "1" : "0");
            _params.Add(ControlCodes.FS);
            _params.Add(allowManualEntry ? "1" : "0");
            _params.Add(ControlCodes.FS);
            _params.Add(allowContactless ? "1" : "0");
            _params.Add(ControlCodes.FS);
            _params.Add(allowScanner ? "1" : "0");
            _params.Add(ControlCodes.FS);
            _params.Add(requireExpiry ? "1" : "0");
            _params.Add(ControlCodes.FS);
            _params.Add(timeout);
            _params.Add(ControlCodes.FS);
            _params.Add((int)encryptionType);
            _params.Add(ControlCodes.FS);
            if(keySlot.HasValue) _params.Add(keySlot);
            _params.Add(ControlCodes.FS);
            _params.Add(minAccountLength);
            _params.Add(ControlCodes.FS);
            _params.Add(maxAccountLength);
            _params.Add(ControlCodes.FS);
            _params.Add("04");
            _params.Add(ControlCodes.FS); // EDC

            var response = service.DoSend(PAX_MSG_ID.A30_INPUT_ACCOUNT, _params.ToArray());
            return new InputAccountResponse(response);
        }

        protected override void SetupValidations() {
            AddValidation(KeySlotValid, "Key slot value is invalid for the encryption type specified.");
        }

        private bool KeySlotValid() {
            if (encryptionType == EncryptionType.None || encryptionType == EncryptionType.VOLTAGE_E2EE)
                return keySlot == null;
            else if (encryptionType == EncryptionType.DUKPT)
                return keySlot >= 1 && keySlot <= 10;
            else
                return keySlot >= 2 && keySlot <= 89;
        }
    }
}
