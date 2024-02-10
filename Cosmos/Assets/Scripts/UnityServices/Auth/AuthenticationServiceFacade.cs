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
        [Inject]
        IPublisher<UnityServiceErrorMessage> _unityServiceErrorMessagePublisher;        

        public InitializationOptions GenerateAuthenticationInitOptions(string profileName)
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

        public void SubscribeToSignedInEvent()
        {
            PlayerAccountService.Instance.SignedIn += SignInWithUnity;
        }

        public void UnsubscribeFromSignedInEvent()
        {
            PlayerAccountService.Instance.SignedIn -= SignInWithUnity;
        }

        public async Task SignInAnonymously()
        {   
            if (AuthenticationService.Instance.IsSignedIn)
            {
                return;
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Annonymous Sign in Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }


        /*public async Task InitializeAndSignInAsync(InitializationOptions initializationOptions)
        {
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync(initializationOptions);

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }*/

        public async Task SwitchProfileAndResignInAsync(string profileName)
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }

            AuthenticationService.Instance.SwitchProfile(profileName);

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
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
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task SignInWithUnityAsync()
        {
            // PlayerAccountService.Instance.SignedIn += SignInWithUnity;

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
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public void SignOutFromAuthenticationService(bool clearCredentials)
        {
            // _signedOutFromAuthService = false;

            // AuthenticationService.Instance.SignedOut += OnSignedOutFromAuthService;
            AuthenticationService.Instance.SignOut(clearCredentials);
        }

        public void SignOutFromPlayerAccountService()
        {
            // _signedOutFromPlayerAccountService = false;

            // PlayerAccountService.Instance.SignedOut += OnSignedOutFromPlayerAccountService;
            PlayerAccountService.Instance.SignOut();
        }

        public void ClearSessionTokenAsync()
        {
            // clear session token
            AuthenticationService.Instance.ClearSessionToken();
        }

        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn || PlayerAccountService.Instance.IsSignedIn;

        /*private bool _signedOutFromAuthService = false;

        private void OnSignedOutFromAuthService()
        {
            _signedOutFromAuthService = true;
            AuthenticationService.Instance.SignedOut -= OnSignedOutFromAuthService;
        }

        private bool _signedOutFromPlayerAccountService = false;

        
        private void OnSignedOutFromPlayerAccountService()
        {
            _signedOutFromPlayerAccountService = true;
            PlayerAccountService.Instance.SignedOut -= OnSignedOutFromPlayerAccountService;
        }*/

        async void SignInWithUnity()
        {
            /*if (AuthenticationService.Instance.IsSignedIn)
            {
                return;
            }*/

            try
            {
                await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
            /*finally
            {
                AuthenticationService.Instance.SignedIn -= SignInWithUnity;
            }*/
        }
    }
}
