
async function verifyPasskey(username,mediation)
{
    const optionsRequestBody = {
        username
    };

    const verifyOptionsRequest = await fetch("/api/Passkey/PasskeyRequestOptions",
        {
            method:"POST",
            headers:{"Content-Type":"application/json"},
            body:JSON.stringify(optionsRequestBody)
        });
    
    const optionsAsJson = await verifyOptionsRequest.json();
    const options = PublicKeyCredential.parseRequestOptionsFromJSON(optionsAsJson);
    
    const credentials = await navigator.credentials.get({publicKey:options,mediation});
    
    const verifyCredentialsRequest = await fetch("/api/Passkey/VerifyPasskey",
        {
            method:"POST",
            headers:{"Content-Type":"application/json"},
            body:JSON.stringify(credentials)
        });
    
    return verifyCredentialsRequest.ok;
}


async function registerPasskey(deviceName) 
{
    const optionsRequestBody = {
        deviceName
    };
    
    const optionsRequest = await fetch("/api/Passkey/CreatePassKeyOptions",
        {
            method:"POST",
            headers:{"Content-Type":"application/json"},
            body:JSON.stringify(optionsRequestBody)
        });
    
    const optionsAsJson = await optionsRequest.json();
    const options = PublicKeyCredential.parseCreationOptionsFromJSON(optionsAsJson);
    
    const credentials = await navigator.credentials.create({publicKey:options});

    const savePasskeyRequest = await fetch("/api/Passkey/CompletePassKeyRegistration",
        {
            method:"POST",
            headers:{"Content-Type":"application/json"},
            body:JSON.stringify(credentials)
        });
    
   return savePasskeyRequest.ok;
}