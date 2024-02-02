using Cosmos.Infrastructure;
using System;
using UnityEngine;


namespace Cosmos.Gameplay.Configuration
{
    /// <summary>
    /// This ScriptableObject defines a Player Character for Cosmos. It defines its CharacterClass field for
    /// associated game-specific properties, as well as its graphics representation.
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarData", menuName = "ScriptableObjects/ConfigurationData/AvatarSO")]
    [Serializable]
    public sealed class AvatarSO : GuidSO
    {
        public CharacterClassSO CharacterClass;

        public GameObject Graphics;

        public GameObject GraphicsCharacterSelect;

        public Color raderVisualColor;

        // public Sprite Portrait;
    }
}
