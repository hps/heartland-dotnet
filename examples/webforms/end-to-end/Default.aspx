<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="end_to_end.Default" %>

<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="SecureSubmit C# WebForms end-to-end payment example using tokenization.">
    <meta name="author" content="Clayton Hunt">
    <title>Simple Payment Form Demo</title>

	<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script type="text/javascript" src="https://api2.heartlandportico.com/SecureSubmit.v1/token/2.1/securesubmit.js"></script>

    <link href="http://maxcdn.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css" rel="stylesheet">
</head>
<body>
<div class="container">
<h1>PHP SecureSubmit Example</h1>
<form class="payment_form form-horizontal" id="payment_form" action="Default.aspx">
<h2>Billing Information</h2>
<div class="form-group">
	<label for="FirstName" class="col-sm-2 control-label">First Name</label>
	<div class="col-sm-10">
		<input type="text" name="FirstName" />
	</div>
</div>
<div class="form-group">
	<label for="LastName" class="col-sm-2 control-label">Last Name</label>
	<div class="col-sm-10">
		<input type="text" name="LastName" />
	</div>
</div>
<div class="form-group">
	<label for="PhoneNumber" class="col-sm-2 control-label">Phone Number</label>
	<div class="col-sm-10">
		<input type="text" name="PhoneNumber" />
	</div>
</div>
<div class="form-group">
	<label for="Email" class="col-sm-2 control-label">Email</label>
	<div class="col-sm-10">
		<input type="text" name="Email" />
	</div>
</div>
<div class="form-group">
	<label for="Address" class="col-sm-2 control-label">Address</label>
	<div class="col-sm-10">
		<input type="text" name="Address" />
	</div>
</div>
<div class="form-group">
	<label for="City" class="col-sm-2 control-label">City</label>
	<div class="col-sm-10">
		<input type="text" name="City" />
	</div>
</div>
<div class="form-group">
	<label for="State" class="col-sm-2 control-label">State</label>
	<div class="col-sm-10">
		<select Name="State">
			<option value="AL">Alabama</option>
			<option value="AK">Alaska</option>
			<option value="AZ">Arizona</option>
			<option value="AR">Arkansas</option>
			<option value="CA">California</option>
			<option value="CO">Colorado</option>
			<option value="CT">Connecticut</option>
			<option value="DE">Delaware</option>
			<option value="DC">District Of Columbia</option>
			<option value="FL">Florida</option>
			<option value="GA">Georgia</option>
			<option value="HI">Hawaii</option>
			<option value="ID">Idaho</option>
			<option value="IL">Illinois</option>
			<option value="IN">Indiana</option>
			<option value="IA">Iowa</option>
			<option value="KS">Kansas</option>
			<option value="KY">Kentucky</option>
			<option value="LA">Louisiana</option>
			<option value="ME">Maine</option>
			<option value="MD">Maryland</option>
			<option value="MA">Massachusetts</option>
			<option value="MI">Michigan</option>
			<option value="MN">Minnesota</option>
			<option value="MS">Mississippi</option>
			<option value="MO">Missouri</option>
			<option value="MT">Montana</option>
			<option value="NE">Nebraska</option>
			<option value="NV">Nevada</option>
			<option value="NH">New Hampshire</option>
			<option value="NJ">New Jersey</option>
			<option value="NM">New Mexico</option>
			<option value="NY">New York</option>
			<option value="NC">North Carolina</option>
			<option value="ND">North Dakota</option>
			<option value="OH">Ohio</option>
			<option value="OK">Oklahoma</option>
			<option value="OR">Oregon</option>
			<option value="PA">Pennsylvania</option>
			<option value="RI">Rhode Island</option>
			<option value="SC">South Carolina</option>
			<option value="SD">South Dakota</option>
			<option value="TN">Tennessee</option>
			<option value="TX">Texas</option>
			<option value="UT">Utah</option>
			<option value="VT">Vermont</option>
			<option value="VA">Virginia</option>
			<option value="WA">Washington</option>
			<option value="WV">West Virginia</option>
			<option value="WI">Wisconsin</option>
			<option value="WY">Wyoming</option>
		</select>
	</div>
</div>
<div class="form-group">
	<label for="Zip" class="col-sm-2 control-label">Zip</label>
	<div class="col-sm-10">
		<input type="text" name="Zip" />
	</div>
</div>
<div class="form-group">
    <div class="col-sm-10">
        <div class="iframeholder" id="iframesCardTokenDetails"></div>
    </div>
</div>

<h2>Card Information</h2>
    <div class="form-group">
	<label for="card_number" class="col-sm-2 control-label">Card Number</label>
	<div class="col-sm-10">
		<div class="iframeholder" id="iframesCardNumber"></div>
	</div>
</div>
<div class="form-group">
	<label for="card_cvc" class="col-sm-2 control-label">CVV</label>
	<div class="col-sm-10">
		<div class="iframeholder" id="iframesCardCvv"></div>
	</div>
</div>
<div class="form-group">
	<label for="exp_month" class="col-sm-2 control-label">Expiration Date</label>
	<div class="col-sm-10">
        <div class="iframeholder" id="iframesCardExpiration"></div>
		</div>
</div>
<div class="form-group">
	<div class="col-sm-10">
        <div class="iframeholder" id="iframestoken"></div>
		</div>
</div>
<br>
<div id="iframesSubmit"></div>
</form>
</div>
    <script type="text/javascript">
        (function (document, Heartland) {
            // Create a new `HPS` object with the necessary configuration
            var hps = new Heartland.HPS({
                publicKey: 'pkapi_cert_P6dRqs1LzfWJ6HgGVZ', // This is your public API Key
                type: 'iframe',
                // Configure the iframe fields to tell the library where
                // the iframe should be inserted into the DOM and some
                // basic options
                fields: {
                    cardNumber: {
                        target: 'iframesCardNumber',
                        placeholder: '•••• •••• •••• ••••'
                    },
                    cardExpiration: {
                        target: 'iframesCardExpiration',
                        placeholder: 'MM / YYYY'
                    },
                    cardCvv: {
                        target: 'iframesCardCvv',
                        placeholder: 'CVV'
                    },
                    submit: {
                        target: 'iframesSubmit'
                    }
                },
                // OPTIONAL Collection of CSS to inject into the iframes.
                // These properties can match the site's styles
                // to create a seamless experience.
                style: {
                    'input': {
                        'background': '#fff',
                        'border': '1px solid',
                        'border-color': '#bbb3b9 #c7c1c6 #c7c1c6',
                        'box-sizing': 'border-box',
                        'font-family': 'serif',
                        'font-size': '16px',
                        'line-height': '1',
                        'margin': '0 .5em 0 0',
                        'max-width': '100%',
                        'outline': '0',
                        'padding': '0.5278em',
                        'vertical-align': 'baseline',
                        'width': '100%'
                    }
                },
                // Callback when a token is received from the service
                onTokenSuccess: function (resp) {
                    //alert('Here is a single-use token: ' + resp.token_value);
                    // create field and append to form
                    $("<input>").attr({
                        type: "hidden",
                        id: "token_value",
                        name: "token_value",
                        value: resp.token_value
                    }).appendTo("#iframestoken");
                    // unbind event handler
                    $("#payment_form").unbind('submit');
                    // submit the form
                    $("#payment_form").submit();
                },
                // Callback when an error is received from the service
                onTokenError: function (resp) {
                    alert('There was an error: ' + resp.error.message);
                },
                // Callback when an event is fired within an iFrame
                onEvent: function (ev) {
                    console.log(ev);
                }
            });
            // Attach a handler to interrupt the form submission
            Heartland.Events.addHandler(document.getElementById('iframesSubmit'), 'submit', function (e) {
                // Prevent the form from continuing to the `action` address
                e.preventDefault();
                // Tell the iframes to tokenize the data
                hps.Messages.post(
                  {
                      accumulateData: true,
                      action: 'tokenize',
                      message: 'pkapi_cert_jKc1FtuyAydZhZfbB3'
                  },
                  'cardNumber'
                );
            });
        }(document, Heartland));
    </script>
</body>
</html>
