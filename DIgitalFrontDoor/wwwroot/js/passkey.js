
async function verifyPasskey(username,mediation)
{
    const response = await fetch(`/api/passkey/PasskeyRequestOptions?username=${username}`,
        {
            method: "POST",
            headers:{
                "Content-Type":"application/json"
            },
        });
    
    const optionsAsJson = await response.json();
    const options = await PublicKeyCredential.parseRequestOptionsFromJSON(optionsAsJson);
    
    const credentials = await navigator.credentials.get({publicKey:options,mediation:mediation})
    
    const verifyCredentialsResponse = await fetch("/api/passkey/VerifyPasskey",
        {
            method: "POST",
            headers:{
                "Content-Type":"application/json"
            },
            body:JSON.stringify(credentials)
        });
    
    if ( verifyCredentialsResponse.ok)
    {
        return true;
    }
    
    return false;
    
}


async function registerPasskey(username , deviceName) 
{
    // Get Passkey options
    const body =  {
        username: username,
        deviceName: deviceName
    };
    
    const response = await fetch("/api/passkey/CreatePassKeyOptions",
        {
            method: "POST",
            headers:{
                "Content-Type":"application/json"
            },
            body:JSON.stringify(body)
        });
    
    const optionsAsJson = await response.json();
    const options = await PublicKeyCredential.parseCreationOptionsFromJSON(optionsAsJson);
    
    const credentials = await navigator.credentials.create({publicKey:options,mediation:"required"});

    // Send credentials to server
    
   const registerResponse = await fetch("/api/passkey/CompletePassKeyRegistration",
       {
              method: "POST",
           headers:{
               "Content-Type":"application/json"
           },
             body:JSON.stringify(credentials)
       })
    
    return registerResponse.ok;
    
    
}