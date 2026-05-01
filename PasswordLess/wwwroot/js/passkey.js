

async function verifyPasskey(username,mediation)
{

    const optionsRequestBody = {
        username
    };

    const optionsRequest = await fetch("/api/Passkey/PasskeyRequestOptions",
        {
            method:"POST",
            headers:{"content-type":"application/json"},
            body:JSON.stringify(optionsRequestBody)
        });

    const optionsAsJson = await optionsRequest.json();
    const options = PublicKeyCredential.parseRequestOptionsFromJSON(optionsAsJson);
    
    const credential = await navigator.credentials.get({publicKey:options,mediation});
    
    const sendCredentials = await fetch ("/api/Passkey/VerifyPasskey",
        {
            method:"POST",
            headers:{"content-type":"application/json"},
            body:JSON.stringify(credential)
        })

    return sendCredentials.ok;
}


async function registerPasskey(deviceName) 
{
    const optionsRequestBody = {
        deviceName
    };
    
    const optionsRequest = await fetch("/api/Passkey/CreatePassKeyOptions",
        {
            method:"POST",
            headers:{"content-type":"application/json"},
            body:JSON.stringify(optionsRequestBody)
        });
    
    const optionsAsJson = await optionsRequest.json();
    const options = PublicKeyCredential.parseCreationOptionsFromJSON(optionsAsJson);
    
    const credentials = await navigator.credentials.create({publicKey:options});

    
    const sendCredentials = await fetch ("/api/Passkey/CompletePassKeyRegistration",
        {
            method:"POST",
            headers:{"content-type":"application/json"},
            body:JSON.stringify(credentials)
        })
    
   return sendCredentials.ok;
}