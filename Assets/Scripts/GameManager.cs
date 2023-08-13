using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;



    [SerializeField] private float _Time = 0.1f;
    [SerializeField] private bool _IsPlayerTurn = true;

    [SerializeField] private int _EntityNum = 0;
    [SerializeField] private List<Entity> _Entities = new List<Entity>();




    public bool IsPlayerTurn
    {
        get
        {
            return _IsPlayerTurn;
        }
    }


    private void StartTurn()
    {
        Entity entity = _Entities[_EntityNum];

        Debug.Log($"{entity.name} starts its turn!");



        Player player = entity.GetComponent<Player>();
        if(player != null)
        {
            _IsPlayerTurn = true;
        }
        else if(entity.IsSentient == true)
        {
            // We don't have AI logic yet, so just skip their turn.
            Action.SkipAction(entity);
        }
    }



    public void EndTurn()
    {
        Entity entity = _Entities[_EntityNum];

        Debug.Log($"{entity.name} ends its turn!");

        Player player = entity.GetComponent<Player>();
        if (player != null)
        {
            _IsPlayerTurn = false;
        }

        _EntityNum = (_EntityNum + 1) % _Entities.Count;
        
        StartCoroutine(TurnDelay());
    }



    public IEnumerator TurnDelay()
    {
        Debug.Log("TurnDelay");

        yield return new WaitForSeconds(_Time);
        StartTurn();
    }



    public void AddEntity(Entity entity)
    {
        _Entities.Add(entity);
    }



    public void InsertEntity(Entity entity, int index)
    {
        _Entities.Insert(index, entity);
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
}
