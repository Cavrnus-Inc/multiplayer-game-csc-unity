using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    [CreateAssetMenu(fileName = "NewColorCollection", menuName = "Cavrnus/Color Collection", order = 1)]
    public class CavrnusTextureCollection : ScriptableObject
    {
        [Serializable]
        public class ColorTextureInfo
        {
            public string DisplayName;
            public Texture Texture;
        }

        public List<ColorTextureInfo> Data;

        public ColorTextureInfo GetDataFromTextureName(string texture)
        {
            return Data.FirstOrDefault(data => data.Texture.name.ToLowerInvariant().Equals(texture.ToLowerInvariant()));
        }
    }
}