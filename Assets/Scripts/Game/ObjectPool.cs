using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public int initSpawnCount = 5;
    private List<GameObject> normalPlatformList = new List<GameObject>();
    private List<GameObject> commonPlatformGroupList = new List<GameObject>();
    private List<GameObject> grassPlatformGroupList = new List<GameObject>();
    private List<GameObject> winterPlatformGroupList = new List<GameObject>();
    private List<GameObject> spikePlatformLeftList = new List<GameObject>();
    private List<GameObject> spikePlatformRightList = new List<GameObject>();
    private List<GameObject> deathEffectList = new List<GameObject>();
    private ManagerVars vars;

    private void Awake()
    {
        Instance = this;
        vars = ManagerVars.GetManagerVars();
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.normalPlatform, ref normalPlatformList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            for(int j = 0; j < vars.commonPlatformGroup.Count; j++)
            {
                InstantiateObject(vars.commonPlatformGroup[j], ref commonPlatformGroupList);
            }
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.grassPlatformGroup.Count; j++)
            {
                InstantiateObject(vars.grassPlatformGroup[j], ref grassPlatformGroupList);
            }
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            for (int j = 0; j < vars.winterPlatformGroup.Count; j++)
            {
                InstantiateObject(vars.winterPlatformGroup[j], ref winterPlatformGroupList);
            }
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.spikePlatformLeft, ref spikePlatformLeftList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.spikePlatformRight, ref spikePlatformRightList);
        }

        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObject(vars.deathEffect, ref deathEffectList);
        }
    }

    private GameObject InstantiateObject(GameObject prefab, ref List<GameObject> addList)
    {
        GameObject go = Instantiate(prefab, transform);
        go.SetActive(false);
        addList.Add(go);
        return go;
    }

    /// <summary>
    /// 获得单个平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetNormalPlatform()
    {
        for(int i = 0; i < normalPlatformList.Count; i++)
        {
            if (!normalPlatformList[i].activeInHierarchy)
            {
                return normalPlatformList[i];
            }
        }
        return InstantiateObject(vars.normalPlatform, ref normalPlatformList);
    }

    /// <summary>
    /// 获得通用平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetCommonPlatform()
    {
        for (int i = 0; i < commonPlatformGroupList.Count; i++)
        {
            if (!commonPlatformGroupList[i].activeInHierarchy)
            {
                return commonPlatformGroupList[i];
            }
        }
        int ran = Random.Range(0, vars.commonPlatformGroup.Count);
        return InstantiateObject(vars.commonPlatformGroup[ran], ref commonPlatformGroupList);
    }

    /// <summary>
    /// 获得草地组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetGrassPlatform()
    {
        for (int i = 0; i < grassPlatformGroupList.Count; i++)
        {
            if (!grassPlatformGroupList[i].activeInHierarchy)
            {
                return grassPlatformGroupList[i];
            }
        }
        int ran = Random.Range(0, vars.grassPlatformGroup.Count);
        return InstantiateObject(vars.grassPlatformGroup[ran], ref grassPlatformGroupList);
    }

    /// <summary>
    /// 获得冬季组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetWinterPlatform()
    {
        for (int i = 0; i < winterPlatformGroupList.Count; i++)
        {
            if (!winterPlatformGroupList[i].activeInHierarchy)
            {
                return winterPlatformGroupList[i];
            }
        }
        int ran = Random.Range(0, vars.winterPlatformGroup.Count);
        return InstantiateObject(vars.winterPlatformGroup[ran], ref winterPlatformGroupList);
    }

    /// <summary>
    /// 获得左边钉子组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetSpikePlatformLeft()
    {
        for (int i = 0; i < spikePlatformLeftList.Count; i++)
        {
            if (!spikePlatformLeftList[i].activeInHierarchy)
            {
                return spikePlatformLeftList[i];
            }
        }
        return InstantiateObject(vars.spikePlatformLeft, ref spikePlatformLeftList);
    }

    /// <summary>
    /// 获得右边钉子组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetSpikePlatformRight()
    {
        for (int i = 0; i < spikePlatformRightList.Count; i++)
        {
            if (!spikePlatformRightList[i].activeInHierarchy)
            {
                return spikePlatformRightList[i];
            }
        }
        return InstantiateObject(vars.spikePlatformRight, ref spikePlatformRightList);
    }

    /// <summary>
    /// 获得死亡特效
    /// </summary>
    /// <returns></returns>
    public GameObject GetDeathEffect()
    {
        for (int i = 0; i < deathEffectList.Count; i++)
        {
            if (!deathEffectList[i].activeInHierarchy)
            {
                return deathEffectList[i];
            }
        }
        return InstantiateObject(vars.deathEffect, ref deathEffectList);
    }
}
