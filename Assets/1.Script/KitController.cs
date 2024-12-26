using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitController : MonoBehaviour
{
    public Transform[] kit;
    public int kitIndex;
    public WaitForSeconds _BuildTime = new WaitForSeconds(1.5f);

    private void OnEnabled()
    {
        kit = GetComponentsInChildren<Transform>();
    }

    public IEnumerator IEBuildKit()
    {
        Debug.Log("BuildKit");
        // 배열에 들어있는 오브젝트를 _buildtime 마다 활성화시키고 벗어나는 코드
        foreach (Transform t in kit)
        {
            // Debug.Log("seted Camping tool: " + t.name);
            yield return new WaitForSeconds(3f);
            t.gameObject.SetActive(true);
        }
    }

    public IEnumerator IEPackKit()
    {
        yield return new WaitForSeconds(3f);
        foreach(Transform t in kit)
        {
            t.gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }
}
