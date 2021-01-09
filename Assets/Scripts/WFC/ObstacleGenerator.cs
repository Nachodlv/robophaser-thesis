using System;
using System.Collections;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using WFC;
using Random = UnityEngine.Random;

namespace WFC
{
    public class ObstacleGenerator : MonoBehaviourPun
    {
        [SerializeField] private NetworkSimpleTileWFC wfcTile;
        [SerializeField] private int tries;
        [SerializeField] private int tilesPerFrame;
        [SerializeField, Min(1)] private float width;
        [SerializeField, Min(1)] private float depth;
        [SerializeField] private Material[] customMaterials;
        [SerializeField] private MapLimits mapLimits;

        public event Action OnFinishPlacingObstacles;

        public float Width => width;
        public float Depth => depth;
        private int _materialSelected;
        private NetworkSimpleTileWFC.GameObjectInstantiatedCallback _addMaterialToObstacleCached;
        private NetworkSimpleTileWFC.GameObjectInstantiatedCallback AddMaterialToObstacleCached =>
            _addMaterialToObstacleCached ?? (_addMaterialToObstacleCached = AddMaterialToObstacle);

        private Func<IEnumerator> _generateObstaclesCoroutine;
        private Func<IEnumerator> GenerateObstaclesCoroutine =>
            _generateObstaclesCoroutine ?? (_generateObstaclesCoroutine = GenerateObstacles);

        public void CreateObstacles(Vector3 position, Quaternion rotation, float newWidth, float newDepth)
        {
            photonView.RPC(nameof(RPC_SetPositionAndScale), RpcTarget.All, position, rotation, newWidth, newDepth);
            SelectRandomMaterialForObstacles();
            wfcTile.OnGameObjectInstantiated = AddMaterialToObstacleCached;
            StartCoroutine(GenerateObstaclesCoroutine());
        }

        public void ResetWfcTilePosition()
        {
            wfcTile.transform.localPosition = Vector3.zero;
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

                if (IsCompleted())
                {
                    photonView.RPC(nameof(RPC_FinishPlacingObjects), RpcTarget.All);
                    yield break;
                }

                currentTries++;
            }

            photonView.RPC(nameof(RPC_FinishPlacingObjects), RpcTarget.All);
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
            var widthSize = Mathf.Max(1, Mathf.RoundToInt(width / wfcTile.GridSize));
            var depthSize = Mathf.Max(1, Mathf.RoundToInt(depth / wfcTile.GridSize));
            wfcTile.width = widthSize;
            wfcTile.depth = depthSize;
        }

        private void CenterWfcTile()
        {
            var wfcTransform = wfcTile.transform;
            var wfcTilePosition = wfcTransform.position;
            wfcTilePosition.x -= (wfcTile.width * wfcTile.GridSize) / 2;
            wfcTilePosition.z -= (wfcTile.depth * wfcTile.GridSize) / 2;
            wfcTransform.position = wfcTilePosition;
        }

        private void SelectRandomMaterialForObstacles()
        {
            var randomMaterial = Random.Range(0, customMaterials.Length);
            photonView.RPC(nameof(SetSelectedMaterial), RpcTarget.All, randomMaterial);
        }

        private void AddMaterialToObstacle(GameObject obstacleCreated)
        {
            if (obstacleCreated.TryGetComponent<Obstacle>(out var obstacleComponent))
            {
                obstacleComponent.SetMaterial(customMaterials[_materialSelected]);
            }
        }

        [PunRPC]
        private void RPC_FinishPlacingObjects()
        {
            mapLimits.SetMapLimits(wfcTile.GridSize, wfcTile.width, wfcTile.depth);
            OnFinishPlacingObstacles?.Invoke();
        }

        [PunRPC]
        private void RPC_SetPositionAndScale(Vector3 position, Quaternion rotation, float newWidth, float newDepth)
        {
            var myTransform = transform;
            myTransform.position = position;
            myTransform.rotation = rotation;
            SetUpScale(newWidth, newDepth);
            CenterWfcTile();
        }

        [PunRPC]
        private void SetSelectedMaterial(int material)
        {
            _materialSelected = material;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObstacleGenerator))]
public class ObstacleGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var me = (ObstacleGenerator) target;
        if (GUILayout.Button("generate"))
        {
            me.ResetWfcTilePosition();
            me.CreateObstacles(me.transform.position, me.transform.rotation, me.Width, me.Depth);
        }
    }
}
#endif
