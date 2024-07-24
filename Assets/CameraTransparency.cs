using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransparency : MonoBehaviour
{

    public Transform player; // 플레이어 캐릭터의 Transform
    public Terrain terrain; // Terrain 객체
    public LayerMask layerMask; // 감지할 레이어
    public float transparency = 0.5f; // 투명도 값 (0: 완전 투명, 1: 완전 불투명)
    private List<TreeInstance> currentTrees = new List<TreeInstance>();
    private Dictionary<TreeInstance, Color> originalColors = new Dictionary<TreeInstance, Color>();

    void Update()
    {
        // 기존의 투명도를 복원합니다.
        foreach (TreeInstance tree in currentTrees)
        {
            TreePrototype treePrototype = terrain.terrainData.treePrototypes[tree.prototypeIndex];
            Renderer treeRenderer = treePrototype.prefab.GetComponent<Renderer>();
            if (treeRenderer != null)
            {
                Material[] materials = treeRenderer.materials;
                foreach (Material material in materials)
                {
                    material.color = originalColors[tree];
                }
            }
        }

        currentTrees.Clear();
        originalColors.Clear();

        // 카메라와 플레이어 사이에 있는 오브젝트를 감지합니다.
        Vector3 direction = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, layerMask);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider is TerrainCollider)
            {
                TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
                foreach (TreeInstance treeInstance in treeInstances)
                {
                    Vector3 treeWorldPosition = Vector3.Scale(treeInstance.position, terrain.terrainData.size) + terrain.transform.position;
                    float distanceToTree = Vector3.Distance(transform.position, treeWorldPosition);

                    if (distanceToTree < direction.magnitude)
                    {
                        TreePrototype treePrototype = terrain.terrainData.treePrototypes[treeInstance.prototypeIndex];
                        GameObject treePrefab = treePrototype.prefab;
                        Renderer treeRenderer = treePrefab.GetComponent<Renderer>();

                        if (treeRenderer != null)
                        {
                            // 원본 색상을 저장합니다.
                            if (!originalColors.ContainsKey(treeInstance))
                            {
                                originalColors[treeInstance] = treeRenderer.material.color;
                            }

                            // 투명 재질을 적용합니다.
                            Material[] materials = treeRenderer.materials;
                            foreach (Material material in materials)
                            {
                                Color color = material.color;
                                color.a = transparency;
                                material.color = color;
                                material.SetFloat("_Mode", 2);
                                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                material.SetInt("_ZWrite", 0);
                                material.DisableKeyword("_ALPHATEST_ON");
                                material.EnableKeyword("_ALPHABLEND_ON");
                                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                material.renderQueue = 3000;
                            }

                            currentTrees.Add(treeInstance);
                        }
                    }
                }
            }
        }
    }
}
