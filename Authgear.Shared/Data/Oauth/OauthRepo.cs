using System;
using System.Threading.Tasks;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin.Data.Oauth
{
    internal delegate void ClearSessionCallback(object sender, SessionStateChangeReason reason);

    internal class OauthRepo : IOauthRepo
    {
        private readonly IOauthRepo impl;

        internal event ClearSessionCallback? ClearSessionCallback;

        public OauthRepo(IOauthRepo impl)
        {
            this.impl = impl;
        }

        public string Endpoint => this.impl.Endpoint;

        private void handleException(Exception? ex)
        {
            while (ex != null)
            {
                if (ex is OauthException)
                {
                    var oauthEx = ex as OauthException;
                    if (oauthEx?.Error == "invalid_grant")
                    {
                        this.ClearSessionCallback?.Invoke(this, SessionStateChangeReason.Invalid);
                        return;
                    }
                }
                else if (ex is ServerException)
                {
                    var serverEx = ex as ServerException;
                    if (serverEx?.Reason == "InvalidGrant")
                    {
                        this.ClearSessionCallback?.Invoke(this, SessionStateChangeReason.Invalid);
                        return;
                    }
                }
                ex = ex.InnerException;
            }
        }

        public async Task BiometricSetupRequestAsync(string accessToken, string clientId, string jwt)
        {
            try
            {
                await this.impl.BiometricSetupRequestAsync(accessToken, clientId, jwt).ConfigureAwait(false);
            } catch (Exception ex)
            {
                this.handleException(ex);
                throw;
            }
        }

        public async Task OidcRevocationRequestAsync(string refreshToken)
        {
            try
            {
                await this.impl.OidcRevocationRequestAsync(refreshToken).ConfigureAwait(false);
            } catch (Exception ex)
            {
                this.handleException(ex);
                throw;
            }
        }

        public async Task<UserInfo> OidcUserInfoRequestAsync(string accessToken)
        {
            try
            {
                return await this.impl.OidcUserInfoRequestAsync(accessToken).ConfigureAwait(false);
            } catch (Exception ex)
            {
                this.handleException(ex);
                throw;
            }
        }

        public async Task<OidcConfiguration> GetOidcConfigurationAsync()
        {
            try
            {
                return await this.impl.GetOidcConfigurationAsync().ConfigureAwait(false);
            } catch (Exception ex)
            {
                this.handleException(ex);
                throw;
            }
        }

        public async Task<AppSessionTokenResponse> OauthAppSessionTokenAsync(string refreshToken)
        {
            try
            {
                return await this.impl.OauthAppSessionTokenAsync(refreshToken).ConfigureAwait(false);
            } catch (Exception ex)
            {
                this.handleException(ex);
                throw;
            }
        }

        public async Task<ChallengeResponse> OauthChallengeAsync(string purpose)
        {
            try
            {
                return await this.impl.OauthChallengeAsync(purpose).ConfigureAwait(false);
            } catch (Exception ex)
            {
                this.handleException(ex);
                throw;
            }
        }

        public async Task<OidcTokenResponse> OidcTokenRequestAsync(OidcTokenRequest request)
        {
            try
            {
                return await this.impl.OidcTokenRequestAsync(request).ConfigureAwait(false);
            } catch (Exception ex)
            {
                this.handleException(ex);
                throw;
            }
        }
    }
}
