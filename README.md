# HTTP redirect calls fail if GetClientCertificate() is called
This repo illustrates an issue where an existing .NET 4.7.2 OWIN server that is acting as a simple reverse proxy fails for some type of clients. The author started seeing this issue originally after updating to Windows 11, so it *might* be Windows 11 related. The behavior in question is 100% reproducible on my primary development workstation running Windows 11.

## Summary of the issue
The OWIN app acts as a trivial reverse proxy server. In this case, just to simplify the repro, it is hard-coded to respond to any incoming request by redirecting to a simple HTML site (the author's blog) and returning the contents in the response. It uses TLS and the repro requires an SSL cert to be bound to port 1234.

The reverse proxy is taken from a more complex production service that supports both username / password and certificate-based authentication. The error happens when the code checks if a client certificate was provided. In the case where no certificate is provided, this check simply returns null for the certificate. The act of making this check causes the web service call to subsequently fail.

In other words, this code:

    clientCert = request.GetClientCertificate();

...is the root cause of the failure.

The repro includes the ability to skip this check for the client certificate if a specific token (`SkipClientCert`) is present in the query string for the request. This can be used to demonstrate that everything works properly if the call to GetClientCetificate is elided.

Note that the simple test client used here and tools like Postman exhibit failures caused by checking for the client certificate.

*However*, curl.exe is not affected and works properly in both cases where the client certificate check is made and when it is not.

## Setting up
Get an SSL cert bound to port 1234.

In a PowerShell window:

    $cert = New-SelfSignedCertificate -Subject "CertForLocalTesting" -TextExtension @("2.5.29.17={text}DNS=localhost&IPAddress=127.0.0.1&IPAddress=::1")

Now bind the self-signed cert to port 1234:

    netsh http add sslcert ipport=0.0.0.0:1234 certhash="$($cert.Thumbprint)" appid="{00112233-4455-6677-8899-AABBCCDDEEEF}"

## Run the server
Build the DummyProxyServer project and then run the resulting executable.

    cd DummyProxyServer
    dotnet run

## Run the client
From a second PowerShell window, run the client like so:

    cd HttpClient
    dotnet run https://localhost:1234?SkipClientCert

Note that this should succeed.

Now run it again, but remove the `?SkipClientCert` part of the URL:

    dotnet run https://localhost:1234

This will fail, at least on some machines.