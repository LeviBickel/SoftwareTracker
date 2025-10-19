using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using SoftwareTracker.Data;
using System.Net.Http;
using System.Threading.Tasks;

public class Auth0UserService
{
    private readonly string _domain = AkeylessHelper.RetrieveSecret("Domain");
    private readonly string _clientId = AkeylessHelper.RetrieveSecret("ClientId");
    private readonly string _clientSecret = AkeylessHelper.RetrieveSecret("ClientSecret");
    
    private async Task<ManagementApiClient> GetAuth0ClientAsync()
    {
        using var httpClient = new HttpClient();
        var tokenResponse = await httpClient.PostAsync($"https://{_domain}/oauth/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "audience", $"https://{_domain}/api/v2/" },
                { "grant_type", "client_credentials" }
            }));

        var tokenResult = await tokenResponse.Content.ReadFromJsonAsync<Auth0TokenResponse>();

        return new ManagementApiClient(tokenResult.AccessToken, _domain);
    }

    public async Task<User> GetUserByIdAsync(string userId)
    {
        var client = await GetAuth0ClientAsync();
        return await client.Users.GetAsync(userId);
    }

    public async Task<IList<User>> GetAllUsersAsync()
    {
        var client = await GetAuth0ClientAsync();
        // Get all users from Auth0 (pagination might be needed for large user bases)
        var users = await client.Users.GetAllAsync(new Auth0.ManagementApi.Models.GetUsersRequest());
        return users;
    }

    public async Task UpdateUserAsync(string userId, UserUpdateRequest userUpdateRequest)
    {
        var client = await GetAuth0ClientAsync();
        await client.Users.UpdateAsync(userId, userUpdateRequest);
    }

    public async Task DeleteUserAsync(string userId)
    {
        var client = await GetAuth0ClientAsync();
        await client.Users.DeleteAsync(userId);
    }

    public async Task<IList<Role>> GetUserRolesAsync(string userId)
    {
        var client = await GetAuth0ClientAsync();
        return await client.Users.GetRolesAsync(userId);
    }

    public async Task AssignRolesToUserAsync(string userId, AssignRolesRequest rolesRequest)
    {
        var client = await GetAuth0ClientAsync();
        await client.Users.AssignRolesAsync(userId, rolesRequest);
    }

    public async Task RemoveRolesFromUserAsync(string userId, AssignRolesRequest rolesRequest)
    {
        var client = await GetAuth0ClientAsync();
        await client.Users.RemoveRolesAsync(userId, rolesRequest);
    }

    public async Task BlockUserAsync(string userId)
    {
        var client = await GetAuth0ClientAsync();
        await client.Users.UpdateAsync(userId, new UserUpdateRequest { Blocked = true });
    }

    public async Task UnblockUserAsync(string userId)
    {
        var client = await GetAuth0ClientAsync();
        await client.Users.UpdateAsync(userId, new UserUpdateRequest { Blocked = false });
    }
}

public class Auth0TokenResponse
{
    public string AccessToken { get; set; }
}
