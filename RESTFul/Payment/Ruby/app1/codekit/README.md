# Att::Codekit

Library for easy access to AT&T's cloud services.

## Installation

Compile a gem using:

    $ rake build

Then install it locally by running:

    $ gem install -l pkg/att-codekit-version.gem

You can install per user by issuing:

    $ gem install --user-install -l pkg/att-codekit-version.gem

Substituting version for the compiled version to install

## Usage

###Require the library
    
    require 'att/codekit'

###Include the namespace (optional)
    
    include Att::Codekit::Auth
    include Att::Codekit::Service

###Create oauth object

First step is creating an oauth service. You will need to use AuthCode or ClientCred according to which api you wish to use.
These immutable objects will perform all steps required for using the oauth services.

    #create an authcode oauth service
    authcode = AuthCode.new(fqdn, client_id, client_secret, scope, redirect_uri)

    #create a client credential oauth service
    client   = ClientCred.new(fqdn, client_id, client_secret, scope)

###Create api service 

Now to create an api service we just pass the oauth service to the api we want to use:
    
    immn = IMMNService.new(authcode)
    sms = SMSService.new(client)
