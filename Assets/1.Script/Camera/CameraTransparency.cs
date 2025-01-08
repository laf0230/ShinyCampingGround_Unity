using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransparency : MonoBehaviour
{

    public Transform player; // �÷��̾� ĳ������ Transform
    public Terrain terrain; // Terrain ��ü
    public LayerMask layerMask; // ������ ���̾�
    public float transparency = 0.5f; // ���� �� (0: ���� ����, 1: ���� ������)
    private List<TreeInstance> currentTrees = new List<TreeInstance>();
    private Dictionary<TreeInstance, Color> originalColors = new Dictionary<TreeInstance, Color>();

    void Update()
    {
        // ������ ������ �����մϴ�.
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

        // ī�޶�� �÷��̾� ���̿� �ִ� ������Ʈ�� �����մϴ�.
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
                            // ���� ������ �����մϴ�.
                            if (!originalColors.ContainsKey(treeInstance))
                            {
                                originalColors[treeInstance] = treeRenderer.material.color;
                            }

                            // ���� ������ �����մϴ�.
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
