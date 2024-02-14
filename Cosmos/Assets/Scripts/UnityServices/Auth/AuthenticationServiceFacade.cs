using Cosmos.Infrastructure;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using VContainer;

namespace Cosmos.UnityServices.Auth
{
    public class AuthenticationServiceFacade
    {
        public event Action onAuthSignInSuccess;
        public event Action onAuthSignInFailed;
        public event Action onAuthSignedOutSuccess;
        public event Action onLinkedInWithUnitySuccess;
        public event Action onLinkedInWithUnityFailed;
        public event Action onUnlinkFromUnitySuccess;
        public event Action onAccountNameUpdateSuccess;
        public event Action onAccountNameUpdateFailed;

        [Inject]
        IPublisher<UnityServiceErrorMessage> _unityServiceErrorMessagePublisher;

        private bool _linkWithUnityPlayerAccount;             // whether it will be sign in or link account

        public void SubscribeToAuthenticationEvents()
        {
            PlayerAccountService.Instance.SignedIn += SignInWithUnity;
            AuthenticationService.Instance.SignedIn += OnAuthSignedIn;
            AuthenticationService.Instance.SignedOut += OnAuthSignedOutSuccess;
            AuthenticationService.Instance.SignedOut += ClearSessionToken;
        }

        public void UnsubscribeFromAuthenticationEvents()
        {
            PlayerAccountService.Instance.SignedIn -= SignInWithUnity;
            AuthenticationService.Instance.SignedIn -= OnAuthSignedIn;
            AuthenticationService.Instance.SignedOut -= onAuthSignedOutSuccess;
            AuthenticationService.Instance.SignedOut -= ClearSessionToken;
        }

        public InitializationOptions GenerateAuthenticationInitOptions(string profileName = null)
        {
            try
            {
                InitializationOptions initializationOptions = new InitializationOptions();
                if (profileName.Length > 0)
                {
                    initializationOptions.SetProfile(profileName);
                }

                return initializationOptions;
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task InitializeToUnityServicesAsync()
        {
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Intialization to UnityServices Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task InitializeToUnityServicesAsync(InitializationOptions initializationOptions)
        {
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync(initializationOptions);
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Intialization to UnityServices Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task SignInWithUnityAsync()
        {
            _linkWithUnityPlayerAccount = false;

            /*if (AuthenticationService.Instance.SessionTokenExists)
            {
                SignOutFromAuthService(true);

                SwitchProfile(string.Empty);
            }*/

            if (PlayerAccountService.Instance.IsSignedIn)
            {
                SignInWithUnity();
                return;
            }

            try
            {
                await PlayerAccountService.Instance.StartSignInAsync();
            }
            catch (Exception e)     // both Authentication and RequestFailedException errors are caught here
            {
                onAuthSignInFailed?.Invoke();

                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                // throw;
            }
        }

        public async Task SignInAnonymously()
        {
            try
            {
                /*if (AuthenticationService.Instance.SessionTokenExists)
                {
                    SignOutFromAuthService(true);

                    SwitchProfile(string.Empty);
                }*/

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    // throw exception
                    throw new Exception();
                }

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                onAuthSignInFailed?.Invoke();

                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Annonymous Sign in Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task LinkAccountWithUnityAsync()
        {
            _linkWithUnityPlayerAccount = true;

            try
            {
                if (!AuthenticationService.Instance.SessionTokenExists)
                {
                    // execute catch block
                    return;
                }

                await PlayerAccountService.Instance.StartSignInAsync();
            }
            catch (Exception e)     // both Authentication and RequestFailedException errors are caught here
            {
                onLinkedInWithUnityFailed?.Invoke();

                _linkWithUnityPlayerAccount = false;
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
            } 
        }

        public async Task UnlinkAccountWithUnityAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(PlayerAccountService.Instance.IdToken))
                {
                    return;
                }
                await AuthenticationService.Instance.UnlinkUnityAsync();

                onUnlinkFromUnitySuccess?.Invoke();
            }
            catch (AuthenticationException ex)
            {
                string reason = ex.InnerException == null ? ex.Message : $"{ex.Message} ({ex.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Unlink Account Error", reason, UnityServiceErrorMessage.Service.Authentication, ex));
            }
            catch (RequestFailedException ex)
            {
                string reason = ex.InnerException == null ? ex.Message : $"{ex.Message} ({ex.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Unlink Account Error", reason, UnityServiceErrorMessage.Service.Authentication, ex));
            }

        }

        public async Task<bool> EnsurePlayerIsAuthorized()
        {
            if (AuthenticationService.Instance.IsAuthorized)
            {
                return true;
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                return true;
            }
            catch (AuthenticationException e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));

                // not rethrouwing for authentication exceptions - any failure to authenticate is considered "handled failure"
                return false;
            }
            catch (Exception e)
            {
                //all other exceptions should still bubble up as unhandled ones
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task UpdatePlayerNameAsync(string playerName)
        {
            if (playerName.Length == 0)
            {
                return;
            }
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                return;
            }
            playerName = playerName[..Math.Min(playerName.Length, 10)];

            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

                onAccountNameUpdateSuccess?.Invoke();
            }
            catch (Exception e)
            {
                onAccountNameUpdateFailed?.Invoke();

                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public void ClearSessionToken()
        {
            if (AuthenticationService.Instance.SessionTokenExists)
                AuthenticationService.Instance.ClearSessionToken();
        }

        public void SignOutFromAuthService(bool clearCredentials = false)
        {
            AuthenticationService.Instance.SignOut(clearCredentials);
        }

        public void SignOutFromPlayerAccountService()
        {
            PlayerAccountService.Instance.SignOut();
        }

        public string GetPlayerName()
        {
            return AuthenticationService.Instance.PlayerName;
        }

        public string GetPlayerId()
        {
            return AuthenticationService.Instance.PlayerId;
        }
        
        public void SwitchProfile(string profileName)
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                SignOutFromAuthService(true);
            }

            AuthenticationService.Instance.SwitchProfile(profileName);
        }

        public bool IsSignedIn()
        {
            return AuthenticationService.Instance.IsSignedIn;
        }

        private async void OnAuthSignedIn()
        {
            await GetPlayerNameAsync();
            onAuthSignInSuccess?.Invoke();
        }

        private void OnAuthSignedOutSuccess()
        {
            onAuthSignedOutSuccess?.Invoke();
        }

        async Task GetPlayerNameAsync()
        {
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                await AuthenticationService.Instance.GetPlayerNameAsync();
            }
        }

        async void SignInWithUnity()
        {
            try
            {
                if (_linkWithUnityPlayerAccount)
                {
                    await AuthenticationService.Instance.LinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);

                    onLinkedInWithUnitySuccess?.Invoke();
                }
                else
                {
                    await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                }
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                onLinkedInWithUnityFailed?.Invoke();

                string reason = ex.InnerException == null ? ex.Message : $"{ex.Message} ({ex.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Link Account Error", reason, UnityServiceErrorMessage.Service.Authentication, ex));
            }
            catch (AuthenticationException ex)
            {
                onLinkedInWithUnityFailed?.Invoke();

                string reason = ex.InnerException == null ? ex.Message : $"{ex.Message} ({ex.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Link Account Error", reason, UnityServiceErrorMessage.Service.Authentication, ex)); ;
            }
            catch (RequestFailedException ex)
            {
                onLinkedInWithUnityFailed?.Invoke();

                string reason = ex.InnerException == null ? ex.Message : $"{ex.Message} ({ex.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Link Account Error", reason, UnityServiceErrorMessage.Service.Authentication, ex));
            }
        }
    }
}
