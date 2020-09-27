using System;
using Photon.Pun;
using UnityEngine;

namespace WFC
{
    public class NetworkSimpleTileWFC: SimpleTiledWFC
    {
        [SerializeField] private float scale = 1f;

        public float Scale => scale;
        public float GridSize => gridsize * scale;

        private void Awake()
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }

        protected override GameObject InstantiateGameObject(string prefabName)
        {
            return base.InstantiateGameObject(prefabName);
        }
    }
}
