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

function toCredentialDTO(credential)
{
    const dto = {
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

    return dto;
}



async function verifyPasskey(username,mediation)
{
    return false;
}


async function registerPasskey(deviceName) 
{
   return false;
}