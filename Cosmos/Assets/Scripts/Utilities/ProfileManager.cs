using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;



#if UNITY_EDITOR
using System.Security.Cryptography;
using System.Text;
#endif


namespace Cosmos.Utilities
{
    public class ProfileManager
    {
        public const string AUTH_PROFILE_COMMAND_LINE_ARG = "-AuthProfile";

        public event Action OnProfileChanged;

        private string _profile;

        public string Profile
        { 
            get
            {
                if (string.IsNullOrEmpty(_profile))
                {
                    _profile = GetProfile();
                }

                return _profile;
            }
            set
            {
                _profile = value;
                OnProfileChanged?.Invoke();
            }
        }

        private List<string> _availableProfiles;
        public ReadOnlyCollection<string> AvailableProfiles
        {
            get
            {
                if (_availableProfiles == null)
                {
                    LoadProfiles();
                }

                return _availableProfiles.AsReadOnly();
            }
        }

        public void CreateProfile(string profile)
        {
            _availableProfiles.Add(profile);
            SaveProfiles();
            Profile = profile;
        }

        public void DeleteProfile(string profile)
        {
            _availableProfiles.Remove(profile);
            SaveProfiles();
        }

        private static string GetProfile()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int  i = 0, length = arguments.Length;  i < length;  i++)
            {
                if (arguments[i] == AUTH_PROFILE_COMMAND_LINE_ARG)
                {
                    return arguments[i + 1];
                }
            }

#if UNITY_EDITOR

            // When running in the Editor, make a unique ID from the Application.dataPath.
            // This will work for cloning projects manually, or with Virtual Projects.
            // Since only a single instance of the Editor can be open for a specific datapath.
            // Uniqueness is guaranteed.
            byte[] hashedBytes = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(Application.dataPath));
            Array.Resize(ref hashedBytes, 16);

            // Authentication service only allows profiles names of maximum 30 characters. We're generating a GUID based
            // on the project's path. Truncating the first 30 characters of said GUID string suffices for uniqueness.
            return new Guid(hashedBytes).ToString("N")[..30];
#else
            return "";
#endif
        }

        private void LoadProfiles()
        {
            _availableProfiles = new List<string>();
            string loadedProfiles = ClientPrefs.GetAvailableProfiles();
            foreach (string profile in loadedProfiles.Split(','))   // this works since we're sanitizing our input strings
            {
                if (!string.IsNullOrEmpty(profile))
                {
                    _availableProfiles.Add(profile);
                }
            }
        }

        private void SaveProfiles()
        {
            string profileToSave = "";
            foreach (string profile in _availableProfiles)
            {
                profileToSave += profile + ",";
            }
            ClientPrefs.SetAvailableProfiles(profileToSave);
        }
    }
}

