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
        //��������
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
    /// ȷ��·��
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
    /// ����ƽ̨
    /// </summary>
    private void SpawnPlatform()
    {
        if (isLeftSpawn)//��������
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
        else//��������
        {
            platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }

        //���ɵ���ƽ̨
        if (spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform();
        }
        //�������ƽ̨
        else if (spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            //����ͨ�����ƽ̨
            if (ran == 0)
            {
                SpawCommonPlatformGroup();
            }
            //�����������ƽ̨
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
            //���ɶ������ƽ̨
            else
            {
                 
            }
        }     
    }

    /// <summary>
    /// ������ͨƽ̨��������
    /// </summary>
    private void SpawnNormalPlatform()
    {
        GameObject go = Instantiate(vars.normalPlatform, transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }

    /// <summary>
    /// ����ͨ�����ƽ̨
    /// </summary>
    private void SpawCommonPlatformGroup()
    {
        int ran = Random.Range(0, vars.commonPlatformGroup.Count);
        GameObject go = Instantiate(vars.commonPlatformGroup[ran], transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }

    /// <summary>
    /// ���ɲݵ����ƽ̨
    /// </summary>
    private void SpawnGrassPlatformGroup()
    {
        int ran = Random.Range(0, vars.grassPlatformGroup.Count);
        GameObject go = Instantiate(vars.grassPlatformGroup[ran], transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }

    /// <summary>
    /// ���ɶ������ƽ̨
    /// </summary>
    private void SpawnWinterPlatformGroup()
    {
        int ran = Random.Range(0, vars.winterPlatformGroup.Count);
        GameObject go = Instantiate(vars.winterPlatformGroup[ran], transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }
}
