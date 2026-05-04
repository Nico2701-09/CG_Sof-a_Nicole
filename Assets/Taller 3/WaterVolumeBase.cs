#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Bitgem.VFX.StylisedWater
{
    /// <summary>
    /// Base class for water volume implementations
    /// </summary>
    public abstract class WaterVolumeBase : MonoBehaviour
    {
        #region Protected fields

        protected const int MAX_TILES_X = 256;
        protected const int MAX_TILES_Y = 256;
        protected const int MAX_TILES_Z = 256;

        protected bool[,,] Tiles;

        #endregion

        #region Serialized fields

        [SerializeField]
        protected bool ShowDebug = false;

        [SerializeField]
        protected float TileSize = 1f;

        #endregion

        #region Protected methods

        /// <summary>
        /// Generate the tile configuration
        /// </summary>
        protected abstract void GenerateTiles(ref bool[,,] _tiles);

        /// <summary>
        /// Rebuild the water volume
        /// </summary>
        protected virtual void Rebuild()
        {
            // Initialize tiles array
            Tiles = new bool[MAX_TILES_X, MAX_TILES_Y, MAX_TILES_Z];

            // Generate tiles based on implementation
            GenerateTiles(ref Tiles);
        }

        #endregion
    }
}
