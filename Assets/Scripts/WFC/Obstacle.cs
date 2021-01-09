using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace WFC
{
    public class Obstacle : MonoBehaviourPun
    {
        [SerializeField] private MeshRenderer[] meshes;

        internal MeshRenderer[] Meshes
        {
            private get => meshes ?? (meshes = GetComponentsInChildren<MeshRenderer>());
            set => meshes = value;
        }

        public void SetMaterial(Material material)
        {
            foreach (var meshRenderer in Meshes)
            {
                meshRenderer.material = material;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Obstacle))]
    public class ObstacleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var me = (Obstacle) target;
            me.Meshes = me.GetComponentsInChildren<MeshRenderer>();
        }
    }
#endif
}
