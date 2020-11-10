using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace ProjectShovel
{
    /// <summary>
    /// Processes Textures intended for Sprites.
    /// </summary>
    public class SpriteProcessor : AssetPostprocessor
    {
        #region Const

        private const string SpritePrefixe = "SPR_";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void OnPreprocessTexture()
        {
            var filename = Path.GetFileName(assetPath);
            if (filename.StartsWith(SpritePrefixe, StringComparison.InvariantCultureIgnoreCase))
            {
                var importer = assetImporter as TextureImporter;
                
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.spritePixelsPerUnit = 1;
                importer.filterMode = FilterMode.Point;
            }
        }

        #endregion
    }
}