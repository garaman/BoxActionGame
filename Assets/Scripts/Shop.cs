using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] RectTransform uiGroup;
    [SerializeField] Animator animator;

    [SerializeField] GameObject[] ItemObjects;
    [SerializeField] int[] ItemPrice;
    [SerializeField] Transform[] ItemPos;
    [SerializeField] Text talkText;

    [SerializeField] string[] npcTalk;

    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        animator.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = ItemPrice[index];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;

        Vector3 ranVec = Vector3.right*Random.Range(-5, 5) + Vector3.forward * Random.Range(-5, 5);

        Instantiate(ItemObjects[index], ItemPos[index].position + ranVec, ItemPos[index].rotation);

    }

    IEnumerator Talk()
    {
        talkText.text = npcTalk[1];
        yield return new WaitForSeconds(2f);
        talkText.text = npcTalk[0];
    }
}
