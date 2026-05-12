function bufferToBase64(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = "";

    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }

    return btoa(binary)
        .replace(/\+/g, "-")
        .replace(/\//g, "_")
        .replace(/=+$/, "");
}

function toNewCredentialDTO(credential)
{
    return {
        id: credential.id,
        rawId: bufferToBase64(credential.rawId),
        type: credential.type,
        response: {
            clientDataJSON: bufferToBase64(credential.response.clientDataJSON),
            attestationObject: bufferToBase64(credential.response.attestationObject)
        },
        clientExtensionResults: credential.getClientExtensionResults?.() ?? {}
    };
}

function toCredentialDTO(credential)
{
    return  {
        id: credential.id,
        rawId: bufferToBase64(credential.rawId),
        type: credential.type,
        authenticatorAttachment: credential.authenticatorAttachment,
        response: {
            clientDataJSON: bufferToBase64(credential.response.clientDataJSON),
            authenticatorData: bufferToBase64(credential.response.authenticatorData),
            signature: bufferToBase64(credential.response.signature),
            userHandle: credential.response.userHandle
                ? bufferToBase64(credential.response.userHandle)
                : null
        },
        clientExtensionResults: credential.getClientExtensionResults?.() ?? {}
    };
}


async function verifyPasskey(username,mediation,signal)
{
    const optionsBody = { username};
    const optionsRequest = await fetch("/api/Passkey/PasskeyRequestOptions",
        {
            "method":"POST",
            "headers":{"content-type":"application/json"},
            body:JSON.stringify(optionsBody)
        });
    
    const optionsAsJson = await optionsRequest.json();
    const options = PublicKeyCredential.parseRequestOptionsFromJSON(optionsAsJson);
    
    const credentials = await navigator.credentials.get({publicKey:options,mediation,signal});

    const sendCredentialsRequest = await fetch("/api/Passkey/VerifyPasskey",
        {
            "method":"POST",
            "headers":{"content-type":"application/json"},
            body:JSON.stringify(toCredentialDTO(credentials))
        });

    return sendCredentialsRequest.ok;
}


async function registerPasskey(deviceName) 
{
    const optionsBody = { deviceName};
    
    const optionsRequest = await fetch("/api/Passkey/CreatePassKeyOptions",
        {
            "method":"POST",
            "headers":{"content-type":"application/json"},
            body:JSON.stringify(optionsBody)
        });
    
    const optionsAsJson = await optionsRequest.json();
    const options = PublicKeyCredential.parseCreationOptionsFromJSON(optionsAsJson);
    
    const credentials = await navigator.credentials.create({publicKey:options});

    const addCredentialsRequest = await fetch("/api/Passkey/CompletePassKeyRegistration",
        {
            "method":"POST",
            "headers":{"content-type":"application/json"},
            body:JSON.stringify(toNewCredentialDTO(credentials))
        });
    
   return addCredentialsRequest.ok;
}