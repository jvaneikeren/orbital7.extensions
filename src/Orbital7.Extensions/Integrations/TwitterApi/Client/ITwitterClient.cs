﻿namespace Orbital7.Extensions.Integrations.TwitterApi;

public interface ITwitterClient :
    IApiClient
{
    string GetAuthorizationUrl(
        string redirectUri,
        string scope,
        string state,
        string codeChallenge);

    Task<TokenInfo> ObtainTokenAsync(
        string authorizationCode,
        string redirectUri,
        string codeVerifier);
}
