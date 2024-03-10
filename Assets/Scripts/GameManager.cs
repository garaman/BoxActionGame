using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject menuCamera;
    [SerializeField] GameObject gameCamera;
    [SerializeField] Player player;
    [SerializeField] Boss boss;

    [SerializeField] GameObject overPanel;
    [SerializeField] Text curScoreText;
    [SerializeField] Text rankScoreText;

    [SerializeField] GameObject menuPanel;
    [SerializeField] Text maxScoreText;

    [SerializeField] GameObject gamePanel;
    [SerializeField] Text scoreText;
    [SerializeField] Text sategeText;
    [SerializeField] Text playTimeText;

    [SerializeField] Text playerHealthText;
    [SerializeField] Text playerAmmoText;
    [SerializeField] Text playerCoinText;

    [SerializeField] Image[] weaponIamge;
    [SerializeField] Text[] enemyText;

    [SerializeField] RectTransform bossHealthGroup;
    [SerializeField] RectTransform bossHealthBar;

    [SerializeField] GameObject shopObject;
    [SerializeField] GameObject enemySpawnObject;

    [SerializeField] Transform[] enemySpawnZone;
    [SerializeField] GameObject[] enemyObjects;
    [SerializeField] List<int> enemyList;

    public int stage = 0;
    float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
    

    private void Awake()
    {
        enemyList = new List<int>();
        Init();
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if(PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        
    }

    void Init()
    {
        menuCamera.SetActive(true);
        gameCamera.SetActive(false);

        menuPanel.SetActive(true);
        gamePanel.SetActive(false);

        player.gameObject.SetActive(false);

        shopObject.SetActive(true);
        enemySpawnObject.SetActive(false);

        if(player.gameManager == null) { player.gameManager = this; }
                
    }

    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        menuCamera.SetActive(true);
        gameCamera.SetActive(false);
                
        gamePanel.SetActive(false);
        overPanel.SetActive(true);

        curScoreText.text = scoreText.text;
        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            rankScoreText.text = player.score.ToString();
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }


    private void Update()
    {
        if(isBattle)
        {
            playTime += Time.deltaTime;
        }
    }


    private void LateUpdate()
    {
        scoreText.text = string.Format("{0:n0}", player.score);
        sategeText.text = $"STAGE {stage}";

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour*3600) / 60);
        int second = (int)(playTime % 60);
                

        playTimeText.text = $"{string.Format("{0:x2}",hour)}:{string.Format("{0:x2}",min)}:{string.Format("{0:x2}", second)}";

        playerHealthText.text = $"{player.health} / {player.maxHealth}";
        playerCoinText.text = string.Format("{0:n0}", player.coin);

        if (player.equipWeapon == null || player.equipWeapon.type == Weapon.Type.Melee)
        {
            playerAmmoText.text = $" - / {player.ammo}";
        }
        else
        {
            playerAmmoText.text = $" {player.equipWeapon.currentAmmo} / {player.ammo}";
        }

        for(int i = 0; i < weaponIamge.Length; i++)
        {
            if(i == weaponIamge.Length-1)
            {
                weaponIamge[i].color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);
            }
            else
            {
                weaponIamge[i].color = new Color(1, 1, 1, player.hasWeapon[i] ? 1 : 0);
            }            
        }

        enemyText[0].text = $" x {enemyCntA}";
        enemyText[1].text = $" x {enemyCntB}";
        enemyText[2].text = $" x {enemyCntC}";

        if(boss != null)
        {
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        
    }

    public void StageStart()
    {
        player.transform.position = new Vector3(0,0,0);

        shopObject.SetActive(false);
        enemySpawnObject.SetActive(true);
        bossHealthGroup.gameObject.SetActive(false);
        isBattle = true;
        stage++;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        shopObject.SetActive(true);
        enemySpawnObject.SetActive(false);
        isBattle = false;
        
    }

    IEnumerator InBattle()
    {
        if(stage % 5 ==0)
        {
            enemyCntD++;
            GameObject bossInst = Instantiate(enemyObjects[enemyList[3]], enemySpawnZone[0].position, enemySpawnZone[0].rotation);
            boss = bossInst.GetComponent<Boss>();
            boss.target = player.transform;
            boss.Manager = this;
            bossHealthGroup.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < stage; i++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0: enemyCntA++; break;
                    case 1: enemyCntB++; break;
                    case 2: enemyCntC++; break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);                

                GameObject enemyInst = Instantiate(enemyObjects[enemyList[0]], enemySpawnZone[ranZone].position, enemySpawnZone[ranZone].rotation);
                Enemy enemy = enemyInst.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.Manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(3f);
            }
        }
       

        while(enemyCntA+enemyCntB+enemyCntC+enemyCntD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }
}
