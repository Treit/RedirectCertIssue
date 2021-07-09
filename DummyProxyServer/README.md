## Setting up

In a PowerShell window:
    $cert = New-SelfSignedCertificate -Subject "CertForLocalTesting" -TextExtension @("2.5.29.17={text}DNS=localhost&IPAddress=127.0.0.1&IPAddress=::1")

Note the thumbprint of the newly-created certificate and then run:

    netsh http add sslcert ipport=0.0.0.0:1234 certhash="$($cert.Thumbprint)" appid="{00112233-4455-6677-8899-AABBCCDDEEEF}"


