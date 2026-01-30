using Microsoft.AspNetCore.Identity;

namespace DIgitalFrontDoor;

public class InMemoryUserStore : IUserStore<IdentityUser>, IUserPasskeyStore<IdentityUser>
{
    private static Dictionary<string, IdentityUser> users = new Dictionary<string, IdentityUser>();
    private static Dictionary<string,IdentityUser> normalisedNameToUser = new Dictionary<string, IdentityUser>();
    
    public void Dispose()
    {
        return;
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        if (user.UserName == null) throw new ArgumentException("Identity must have a username");
        
       users.Add(user.Id,user);
       users.Add(user.UserName.ToUpper(), user);
       return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        users[user.Id] = user;
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        users.TryGetValue(userId, out IdentityUser? user);
        
        return Task.FromResult(user);
    }

    public Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        normalisedNameToUser.TryGetValue(normalizedUserName, out IdentityUser? user);
        
        return Task.FromResult(user);
    }

    private static Dictionary<string,List<UserPasskeyInfo>> identityToPassKeysMap = new();
    
    
    public Task AddOrUpdatePasskeyAsync(IdentityUser user, UserPasskeyInfo passkey, CancellationToken cancellationToken)
    {
        if (!identityToPassKeysMap.TryGetValue(user.Id, out List<UserPasskeyInfo>? passkeys))
        {
            passkeys = new List<UserPasskeyInfo>();
            identityToPassKeysMap[user.Id] = passkeys;
        }
        passkeys.Add(passkey);

        return Task.CompletedTask;
    }

    public Task<IList<UserPasskeyInfo>> GetPasskeysAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        if (!identityToPassKeysMap.TryGetValue(user.Id, out List<UserPasskeyInfo>? passkeys))
        {
            passkeys = [];
        }
        
        return Task.FromResult<IList<UserPasskeyInfo>>(passkeys);
    }

    public Task<IdentityUser?> FindByPasskeyIdAsync(byte[] credentialId, CancellationToken cancellationToken)
    {
        IdentityUser? user = identityToPassKeysMap
            .Where(kv => kv.Value.Any(v => v.CredentialId.SequenceCompareTo(credentialId) == 0))
            .Select(kv => kv.Key)
            .Select<string,IdentityUser?>(key => users[key])
            .FirstOrDefault();
        
        return Task.FromResult(user);
    }

    public Task<UserPasskeyInfo?> FindPasskeyAsync(IdentityUser user, byte[] credentialId, CancellationToken cancellationToken)
    {
        UserPasskeyInfo? matchingCredential = identityToPassKeysMap[user.Id].FirstOrDefault(upi => upi.CredentialId.SequenceCompareTo(credentialId) == 0);
        
        return Task.FromResult(matchingCredential);
    }

    public Task RemovePasskeyAsync(IdentityUser user, byte[] credentialId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}