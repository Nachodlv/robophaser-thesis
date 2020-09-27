using System;
using System.Collections;
using UnityEngine;

namespace WFC
{
    public class ObstacleGenerator: MonoBehaviour
    {
        [SerializeField] private SimpleTiledWFC wfcTile;
        [SerializeField] private int tries;
        [SerializeField] private int tilesPerFrame;
        [SerializeField, Min(1)] private float obstacleQuantityWidth;
        [SerializeField, Min(1)] private float obstacleQuantityDepth;
        // [SerializeField] private float width;
        // [SerializeField] private float depth;

        private Func<IEnumerator> _generateObstaclesCoroutine;

        private void Awake()
        {
            _generateObstaclesCoroutine = GenerateObstacles;
            // CreateObstacles(Vector3.zero, Quaternion.identity, width, depth);
        }

        public void CreateObstacles(Vector3 position, Quaternion rotation, float width, float depth)
        {
            var myTransform = transform;
            myTransform.position = position;
            myTransform.rotation = rotation;
            CenterWfcTile();
            SetUpScale(width, depth);
            StartCoroutine(_generateObstaclesCoroutine());
        }

        private IEnumerator GenerateObstacles()
        {
            var currentTries = 0;
            while (currentTries < tries)
            {
                var currentTiles = 0;
                wfcTile.Generate();
                while (wfcTile.Run())
                {
                    currentTiles++;
                    if (currentTiles <= tilesPerFrame) continue;
                    currentTiles = 0;
                    yield return null;
                }
                if(IsCompleted()) yield break;
                currentTries++;
                Debug.Log($"Tries: {currentTries}");
            }
        }

        private bool IsCompleted()
        {
            for (var i = 0; i < wfcTile.rendering.GetLength(0); i++)
            {
                for (var j = 0; j < wfcTile.rendering.GetLength(1); j++)
                {
                    if (wfcTile.rendering[i, j] == null) return false;
                }
            }

            return true;
        }

        private void SetUpScale(float width, float depth)
        {
            var myTransform = transform;
            var scale = myTransform.localScale;
            var xScale = width / wfcTile.gridsize / obstacleQuantityWidth;
            var zScale = depth / wfcTile.gridsize / obstacleQuantityDepth;
            myTransform.localScale = new Vector3(xScale, scale.y, zScale);
        }

        private void CenterWfcTile()
        {
            var wfcTransform = wfcTile.transform;
            var wfcTilePosition = wfcTransform.position;
            wfcTilePosition.x -= obstacleQuantityWidth / wfcTile.gridsize / 2;
            wfcTilePosition.z -= obstacleQuantityDepth / wfcTile.gridsize / 2;
            wfcTransform.position = wfcTilePosition;
        }
    }
}
