using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformGroupType
{
    Grass,
    Winter,
}

public class PlatformSpawner : MonoBehaviour
{
    public Vector3 startSpawnPos;
    /// <summary>
    /// 生成平台数量
    /// </summary>
    private int spawnPlatformCount;
    private ManagerVars vars;
    /// <summary>
    /// 平台的生成位置
    /// </summary>
    private Vector3 platformSpawnPos;
    /// <summary>
    /// 是否朝左方生成
    /// </summary>
    private bool isLeftSpawn = false;
    /// <summary>
    /// 选中的平台主题
    /// </summary>
    private Sprite selectPlatformSprite;
    /// <summary>
    /// 组合平台的类型
    /// </summary>
    private PlatformGroupType groupType;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
    }

    private void Start()
    {
        vars = ManagerVars.GetManagerVars();
        platformSpawnPos = startSpawnPos;
        RandomPlatformTheme();
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }
        //生成人物
        GameObject player = Instantiate(vars.characterPre);
        player.transform.position = new Vector3(vars.nextXPos, vars.nextYPos, 0);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }

    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectPlatformSprite = vars.platformThemeSpriteList[ran];
        if (ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else
        {
            groupType = PlatformGroupType.Grass;
        }
    }


    /// <summary>
    /// 确定路径
    /// </summary>
    private void DecidePath()
    {
        if (spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            isLeftSpawn = !isLeftSpawn;
            spawnPlatformCount = Random.Range(1, 4);
            SpawnPlatform();
        }
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        if (isLeftSpawn)//向左生成
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
        else//向右生成
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }

        //生成单个平台
        if (spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform();
        }
        //生成组合平台
        else if (spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            //生成通用组合平台
            if (ran == 0)
            {
                SpawCommonPlatformGroup();
            }
            //生成主题组合平台
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup();
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup();
                        break;
                }
            }
            //生成钉子组合平台
            else
            {
                 
            }
        }     
    }

    /// <summary>
    /// 生成普通平台（单个）
    /// </summary>
    private void SpawnNormalPlatform()
    {
        GameObject go = Instantiate(vars.normalPlatform, transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }

    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawCommonPlatformGroup()
    {
        int ran = Random.Range(0, vars.commonPlatformGroup.Count);
        GameObject go = Instantiate(vars.commonPlatformGroup[ran], transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }

    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup()
    {
        int ran = Random.Range(0, vars.grassPlatformGroup.Count);
        GameObject go = Instantiate(vars.grassPlatformGroup[ran], transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }

    /// <summary>
    /// 生成冬季组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup()
    {
        int ran = Random.Range(0, vars.winterPlatformGroup.Count);
        GameObject go = Instantiate(vars.winterPlatformGroup[ran], transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }
}
