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
    /// ��̱���
    /// </summary>
    public int milestoneCount = 10;
    public float fallTime = 0;
    public float minFallTime;
    public float multiple;
    /// <summary>
    /// ����ƽ̨����
    /// </summary>
    private int spawnPlatformCount;
    private ManagerVars vars;
    /// <summary>
    /// ƽ̨������λ��
    /// </summary>
    private Vector3 platformSpawnPos;
    /// <summary>
    /// �Ƿ�������
    /// </summary>
    private bool isLeftSpawn = false;
    /// <summary>
    /// ѡ�е�ƽ̨����
    /// </summary>
    private Sprite selectPlatformSprite;
    /// <summary>
    /// ���ƽ̨������
    /// </summary>
    private PlatformGroupType groupType;
    /// <summary>
    /// �������ƽ̨�Ƿ����������
    /// </summary>
    private bool spikeSpawnLeft = false;
    /// <summary>
    /// ����ƽ̨�����λ��
    /// </summary>
    private Vector3 spikeDirPlatformPos;
    /// <summary>
    /// ���ɶ������ƽ̨֮����Ҫ�ڶ��ӷ������ɵ�ƽ̨������
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
        //��������
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
    /// ����ƽ̨����ʱ��
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
    /// ���ƽ̨����
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
    /// ȷ��·��
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
            //��ת���ɷ���
            isLeftSpawn = !isLeftSpawn;
            spawnPlatformCount = Random.Range(1, 4);
            SpawnPlatform();
        }
    }

    /// <summary>
    /// ����ƽ̨
    /// </summary>
    private void SpawnPlatform()
    {
        int obstacleDir = Random.Range(0, 2);   

        //���ɵ���ƽ̨
        if (spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform(obstacleDir);
        }
        //�������ƽ̨
        else if (spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            //����ͨ�����ƽ̨
            if (ran == 0)
            {
                SpawCommonPlatformGroup(obstacleDir);
            }
            //�����������ƽ̨
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
            //���ɶ������ƽ̨
            else
            {
                int value = -1;
                if (isLeftSpawn)
                {
                    value = 0;//�����ұ߷���Ķ���                 
                }
                else
                {
                    value = 1;//������߷���Ķ���
                }
                SpawnSpikePlatform(value);
                isSpawnSpike = true;
                afterSpawnSpikePlatformCount = Random.Range(3, 5);
                if (spikeSpawnLeft)//���������
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

        if (isLeftSpawn)//��������
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
        else//��������
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
    }

    /// <summary>
    /// ������ͨƽ̨��������
    /// </summary>
    private void SpawnNormalPlatform(int obstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, obstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// ����ͨ�����ƽ̨
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
    /// ���ɲݵ����ƽ̨
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
    /// ���ɶ������ƽ̨
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
    /// ���ɶ������ƽ̨
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
    /// ���ɶ���ƽ̨֮����Ҫ���ɵ�ƽ̨
    /// �������ӷ���Ҳ����ԭ���ķ���
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
                if(i == 0)//����ԭ�������ƽ̨
                {
                    temp.transform.position = platformSpawnPos;
                    //�����������ߣ�ԭ��·�������ұ�
                    if (spikeSpawnLeft)
                    {
                        platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
                    }
                }
                else//���ɶ��ӷ����ƽ̨
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
