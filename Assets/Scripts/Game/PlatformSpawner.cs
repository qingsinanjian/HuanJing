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
        player.transform.position = new Vector3(0, -1.8f, 0);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }

    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectPlatformSprite = vars.platformThemeSpriteList[ran];
        if(ran == 2)
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
        if(spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            isLeftSpawn = !isLeftSpawn;
            spawnPlatformCount = Random.Range(1,4);
            SpawnPlatform();
        }
    }

    /// <summary>
    /// ����ƽ̨
    /// </summary>
    private void SpawnPlatform()
    {
        SpawnNormalPlatform();
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
    private void SpawnNormalPlatform()
    {
        GameObject go = Instantiate(vars.normalPlatform, transform);
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite);
    }
}
