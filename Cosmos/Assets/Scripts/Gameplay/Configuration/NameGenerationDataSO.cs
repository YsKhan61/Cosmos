using UnityEngine;

namespace Cosmos.Gameplay.Configuration
{
    /// <summary>
    /// Data storage of all the valid strings used to create a player's name.
    /// Currently names are a two word combination in Adjective-Noun Combo (e.g. Happy Apple)
    /// </summary>
    [CreateAssetMenu(fileName = "NameGenerationData", menuName = "ScriptableObjects/ConfigurationData/NameGenerationDataSO")]
    public class NameGenerationDataSO : ScriptableObject
    {
        [Tooltip("The list of all possible string the game can use as the first word of a player name")]
        public string[] FirstWordList;

        [Tooltip("The list of all possible string the game can use as the second word of a player name")]
        public string[] SecondWordList;

        public string GetRandomName()
        {
            string firstWord = FirstWordList[Random.Range(0, FirstWordList.Length - 1)];
            string secondWord = SecondWordList[Random.Range(0, SecondWordList.Length - 1)];

            return firstWord + "_" + secondWord;
        }
    }
}