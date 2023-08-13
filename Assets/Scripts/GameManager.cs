using System.Collections;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;



    [SerializeField] private float _Time = 0.1f;
    [SerializeField] private bool _IsPlayerTurn = true;



    public bool IsPlayerTurn
    {
        get
        {
            return _IsPlayerTurn;
        }
    }



    public void EndTurn()
    {
        Debug.Log("EndTurn");

        _IsPlayerTurn = false;
        StartCoroutine(WaitForTurns());
    }



    private IEnumerator WaitForTurns()
    {
        Debug.Log("WaitForTurns");

        yield return new WaitForSeconds(_Time);
        _IsPlayerTurn = true;
    }



    private void Awake()
    {
        // Keep the first instance
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        // Self destruct any duplicate instances
        Destroy(gameObject);
    }




    private void Start()
    {
        GameObject playerResource = Resources.Load<GameObject>("Player");
        GameObject player = Instantiate(playerResource);
        player.name = "Player";
    }
}
