using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject menuCamera;
    [SerializeField] GameObject gameCamera;
    [SerializeField] Player player;
    [SerializeField] Boss boss;
    
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

    int stage;
    float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;


    private void Awake()
    {
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        
    }

    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
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


        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
    }

    public void StageStart()
    {
        player.transform.position = new Vector3(0,0,0);

        shopObject.SetActive(false);
        isBattle = true;
        stage++;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        shopObject.SetActive(true);
        isBattle = false;
        
    }

    IEnumerator InBattle()
    {
        yield return new WaitForSeconds(5f);
        StageEnd();
    }
}
