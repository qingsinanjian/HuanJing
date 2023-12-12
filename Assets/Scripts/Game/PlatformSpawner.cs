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
    /// 里程碑数
    /// </summary>
    public int milestoneCount = 10;
    public float fallTime = 0;
    public float minFallTime;
    public float multiple;
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
    /// <summary>
    /// 钉子组合平台是否生成在左边
    /// </summary>
    private bool spikeSpawnLeft = false;
    /// <summary>
    /// 钉子平台方向的位置
    /// </summary>
    private Vector3 spikeDirPlatformPos;
    /// <summary>
    /// 生成钉子组合平台之后需要在钉子方向生成的平台的数量
    /// </summary>
    private int afterSpawnSpikePlatformCount;
    private bool isSpawnSpike;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
        vars = ManagerVars.GetManagerVars();
    }

    private void Start()
    {
        RandomPlatformTheme();
        platformSpawnPos = startSpawnPos;
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }
        //生成人物
        GameObject player = Instantiate(vars.characterPre);
        player.transform.position = new Vector3(0, -1.8f, 0);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }

    private void Update()
    {
        if ((GameManager.Instance.IsGameStarted && GameManager.Instance.IsGameOver == false))
        {
            UpdateFallTime();
        }
    }

    /// <summary>
    /// 更新平台掉落时间
    /// </summary>
    private void UpdateFallTime()
    {
        if(GameManager.Instance.GetGameScore() > milestoneCount)
        {
            milestoneCount *= 2;
            fallTime *= multiple;
            if(fallTime < minFallTime)
            {
                fallTime = minFallTime;
            }
        }
    }

    /// <summary>
    /// 随机平台主题
    /// </summary>
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
        if (isSpawnSpike)
        {
            AfterSpawnSpike();
            return;
        }

        if (spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            //反转生成方向
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
        int obstacleDir = Random.Range(0, 2);   

        //生成单个平台
        if (spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform(obstacleDir);
        }
        //生成组合平台
        else if (spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            //生成通用组合平台
            if (ran == 0)
            {
                SpawCommonPlatformGroup(obstacleDir);
            }
            //生成主题组合平台
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(obstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(obstacleDir);
                        break;
                }
            }
            //生成钉子组合平台
            else
            {
                int value = -1;
                if (isLeftSpawn)
                {
                    value = 0;//生成右边方向的钉子                 
                }
                else
                {
                    value = 1;//生成左边方向的钉子
                }
                SpawnSpikePlatform(value);
                isSpawnSpike = true;
                afterSpawnSpikePlatformCount = Random.Range(3, 5);
                if (spikeSpawnLeft)//钉子在左边
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPos.x - 1.65f, platformSpawnPos.y + vars.nextYPos, 0);
                }
                else
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPos.x + 1.65f, platformSpawnPos.y + vars.nextYPos, 0);
                }
            }
        }

        int ranSpawnDiamond = Random.Range(0, 10);
        if(ranSpawnDiamond == 6 && GameManager.Instance.PlayerIsMove)
        {
            GameObject go = ObjectPool.Instance.GetDiamond();
            go.transform.position = new Vector3(platformSpawnPos.x, platformSpawnPos.y + 0.5f, 0);
            go.SetActive(true);
        }

        if (isLeftSpawn)//向左生成
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
        else//向右生成
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
    }

    /// <summary>
    /// 生成普通平台（单个）
    /// </summary>
    private void SpawnNormalPlatform(int obstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, obstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawCommonPlatformGroup(int obstacleDir)
    {
        int ran = Random.Range(0, vars.commonPlatformGroup.Count);
        GameObject go = ObjectPool.Instance.GetCommonPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, obstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int obstacleDir)
    {
        int ran = Random.Range(0, vars.grassPlatformGroup.Count);
        GameObject go = ObjectPool.Instance.GetGrassPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, obstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成冬季组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup(int obstacleDir)
    {
        int ran = Random.Range(0, vars.winterPlatformGroup.Count);
        GameObject go = ObjectPool.Instance.GetWinterPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, obstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成钉子组合平台
    /// </summary>
    /// <param name="dir"></param>
    private void SpawnSpikePlatform(int dir)
    {
        GameObject go;
        if (dir == 0)
        {
            spikeSpawnLeft = false;
            go = ObjectPool.Instance.GetSpikePlatformRight();
            
        }
        else
        {
            spikeSpawnLeft = true;
            go = ObjectPool.Instance.GetSpikePlatformLeft();
        }
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, dir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成钉子平台之后需要生成的平台
    /// 包括钉子方向也包括原来的方向
    /// </summary>
    private void AfterSpawnSpike()
    {
        if(afterSpawnSpikePlatformCount > 0)
        {
            afterSpawnSpikePlatformCount--;
            for (int i = 0; i < 2; i++)
            {
                GameObject temp = ObjectPool.Instance.GetNormalPlatform();
                temp.SetActive(true);
                if(i == 0)//生成原来方向的平台
                {
                    temp.transform.position = platformSpawnPos;
                    //如果钉子在左边，原先路径就是右边
                    if (spikeSpawnLeft)
                    {
                        platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
                    }
                }
                else//生成钉子方向的平台
                {
                    temp.transform.position = spikeDirPlatformPos;
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nextXPos, spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nextXPos, spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                }
                temp.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, 1);
            }
        }
        else
        {
            isSpawnSpike = false;
            DecidePath();
        }
    }
}
