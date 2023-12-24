using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Cosmos.UnityServices.Lobbies
{
    /// <summary>
    /// Data for a local lobby user interface. This will update data and is observerd to know when to push local user changes to the entire lobby.
    /// </summary>
    [Serializable]
    public class LocalLobbyUser
    {
        public struct UserData
        {
            public bool IsHost { get; set; }
            public string DisplayName { get; set; }
            public string ID { get; set; }
            public UserData(bool isHost, string displayName, string id)
            {
                IsHost = isHost;
                DisplayName = displayName;
                ID = id;
            }
        }

        private UserData _userData;

        /// <summary>
        /// Used for limiting costly OnChanged actions to just the members which actually changed.
        /// </summary>
        [Flags]
        public enum UserMember
        {
            IsHost = 1,
            DisplayName = 2,
            ID = 4,
        }

        private UserMember _lastChangedUserMember;

        public event Action<LocalLobbyUser> OnChanged;

        public LocalLobbyUser()
        {
            _userData = new UserData(isHost: false, displayName: null, id: null);
        }

        public void ResetState()
        {
            _userData = new UserData(false, _userData.DisplayName, _userData.ID);
        }

        public bool IsHost
        {
            get { return _userData.IsHost; }
            set
            {
                if (_userData.IsHost != value)
                {
                    _userData.IsHost = value;
                    _lastChangedUserMember = UserMember.IsHost;
                    OnChanged?.Invoke(this);
                }
            }
        }

        public string DisplayName
        {
            get => _userData.DisplayName;
            set
            {
                if (_userData.DisplayName != value)
                {
                    _userData.DisplayName = value;
                    _lastChangedUserMember = UserMember.DisplayName;
                    OnChanged?.Invoke(this);
                }
            }
        }

        public string ID
        {
            get => _userData.ID;
            set
            {
                if (_userData.ID != value)
                {
                    _userData.ID = value;
                    _lastChangedUserMember = UserMember.ID;
                    OnChanged?.Invoke(this);
                }
            }
        }

        public void CopyDataFrom(LocalLobbyUser lobby)
        {
            UserData data = lobby._userData;
            int lastChanged = // Set flags just for the members that will be changed.
                (_userData.IsHost == data.IsHost ? 0 : (int)UserMember.IsHost) |
                (_userData.DisplayName == data.DisplayName ? 0 : (int)UserMember.DisplayName) |
                (_userData.ID == data.ID ? 0 : (int)UserMember.ID);

            if (lastChanged == 0)
                return;

            _userData = data;
            _lastChangedUserMember = (UserMember)lastChanged;

            OnChanged?.Invoke(this);
        }

        public Dictionary<string, PlayerDataObject> GetDataForUnityServices() =>
            new Dictionary<string, PlayerDataObject>()
            {
                {"DisplayName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DisplayName) }
            };
    }
}
