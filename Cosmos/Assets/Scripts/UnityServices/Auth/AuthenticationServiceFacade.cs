using Cosmos.Infrastructure;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using VContainer;

namespace Cosmos.UnityServices.Auth
{
    /// <summary>
    /// Types of accounts that can be signed in to.
    /// </summary>
    public enum AccountType
    {
        None,
        UnityPlayerAccount,
        GuestAccount,
    }


    /// <summary>
    /// A facade to the Unity Services Authentication service.
    /// It provides a simplified interface to the Unity Services Authentication service.
    /// </summary>
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
        private IPublisher<UnityServiceErrorMessage> _unityServiceErrorMessagePublisher;

        // indicates whether the current operation is a sign-in or account linking operation.
        private bool _linkWithUnityPlayerAccount;

        /// <summary>
        /// indicates the type of the current account.
        /// </summary>
        public AccountType AccountType {get; private set;}

        /// <summary>
        /// Subscribes to various authentication events from the PlayerAccountService and AuthenticationService instances.
        /// </summary>
        public void SubscribeToAuthenticationEvents()
        {
            PlayerAccountService.Instance.SignedIn += SignInWithUnity;
            AuthenticationService.Instance.SignedIn += OnAuthSignedIn;
            AuthenticationService.Instance.SignedOut += OnAuthSignedOutSuccess;
            AuthenticationService.Instance.SignedOut += ClearSessionToken;
        }

        /// <summary>
        /// Unsubscribes from various authentication events from the PlayerAccountService and AuthenticationService instances.
        /// </summary>
        public void UnsubscribeFromAuthenticationEvents()
        {
            PlayerAccountService.Instance.SignedIn -= SignInWithUnity;
            AuthenticationService.Instance.SignedIn -= OnAuthSignedIn;
            AuthenticationService.Instance.SignedOut -= onAuthSignedOutSuccess;
            AuthenticationService.Instance.SignedOut -= ClearSessionToken;
        }

        /// <summary>
        /// Generates initialization options for Unity services, optionally setting a profile name.
        /// </summary>
        /// <param name="profileName">The profile name to set in the initialization options. If no profile name is provided, a random name will be created and used.</param>
        /// <returns> Returns the generated initialization options.</returns>
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

        /// <summary>
        /// Initialize Unity services, with default options.
        /// </summary>
        /// <returns> Returns a task that completes when the initialization is done.</returns>
        public async Task InitializeUnityServicesAsync()
        {
            if (Unity.Services.Core.UnityServices.State == ServicesInitializationState.Initialized)
            {
                return;
            }

            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync();
                AccountType = AccountType.None;
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Intialization to UnityServices Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        /// <summary>
        /// Initialize Unity services, with specified initialization options.
        /// </summary>
        /// <returns> Returns a task that completes when the initialization is done.</returns>
        public async Task InitializeUnityServicesAsync(InitializationOptions initializationOptions)
        {
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync(initializationOptions);
                AccountType = AccountType.None;
            }
            catch (Exception e)
            {
                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Intialization to UnityServices Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        /// <summary>
        /// Starts the sign-in process with Unity.
        /// </summary>
        /// <returns></returns>
        public async Task SignInWithUnityAsync()
        {
            _linkWithUnityPlayerAccount = false;

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

        /// <summary>
        /// signs in anonymously
        /// </summary>
        /// <returns> The task that completes when the sign-in is done.</returns>
        public async Task SignInAnonymously()
        {
            try
            {
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    // throw exception
                    throw new Exception();
                }

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                AccountType = AccountType.GuestAccount;
            }
            catch (Exception e)
            {
                onAuthSignInFailed?.Invoke();

                string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})";
                _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Annonymous Sign in Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        /// <summary>
        /// starts the process of linking the current account with Unity.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// starts the process of unlinking the current account from Unity.
        /// </summary>
        /// <returns> The task that completes when the unlinking is done.</returns>
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

        /// <summary>
        /// Eensures that the player is authorized, signing in anonymously if necessary.
        /// </summary>
        /// <returns> Returns a task that completes when the player is authorized.</returns>
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

        /// <summary>
        /// updates the player's name.
        /// </summary>
        /// <returns>Returns a task that completes when the player's name is updated.</returns>
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

        /// <summary>
        /// clears the session token if it exists.
        /// </summary>
        public void ClearSessionToken()
        {
            if (AuthenticationService.Instance.SessionTokenExists)
                AuthenticationService.Instance.ClearSessionToken();
        }

        /// <summary>
        /// Signs out from the authentication service.
        /// </summary>
        /// <param name="clearCredentials"> If true, clears the credentials.</param>
        public void SignOutFromAuthService(bool clearCredentials = false)
        {
            if (IsSignedIn())
            {
                AuthenticationService.Instance.SignOut(clearCredentials);
                AccountType = AccountType.None;
            }
        }

        /// <summary>
        /// Signs out from the player account service.
        /// </summary>
        public void SignOutFromPlayerAccountService()
        {
            PlayerAccountService.Instance.SignOut();
            AccountType = AccountType.GuestAccount;
        }

        /// <summary>
        /// Gets the player's name.
        /// </summary>
        public string GetPlayerName()
        {
            return AuthenticationService.Instance.PlayerName;
        }

        /// <summary>
        /// Gets the player's id.
        /// </summary>
        public string GetPlayerId()
        {
            return AuthenticationService.Instance.PlayerId;
        }
        
        /// <summary>
        /// Switches the profile. signs out from the authentication service if signed in.
        /// </summary>
        /// <param name="profileName">name of the profile to switch to</param>
        public void SwitchProfile(string profileName)
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                SignOutFromAuthService(true);
            }

            AuthenticationService.Instance.SwitchProfile(profileName);
        }

        /// <summary>
        /// Is the player signed in?
        /// </summary>
        /// <returns></returns>
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
            AccountType = AccountType.None;
            onAuthSignedOutSuccess?.Invoke();
        }

        private async Task GetPlayerNameAsync()
        {
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                await AuthenticationService.Instance.GetPlayerNameAsync();
            }
        }

        private async void SignInWithUnity()
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
                AccountType = AccountType.UnityPlayerAccount;
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
