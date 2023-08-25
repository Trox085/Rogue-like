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



    public List<Entity> Entities
    {
        get
        {
            return _Entities;
        }
    }


    private void StartTurn()
    {
        Entity entity = _Entities[_EntityNum];

        if (entity == null)
        {
            return;
        }

        //Debug.Log($"{entity.name} starts its turn!");



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

        if(entity == null)
        {
            return;
        }

        //Debug.Log($"{entity.name} ends its turn!");

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
        //Debug.Log("TurnDelay");

        yield return new WaitForSeconds(_Time);
        StartTurn();
    }



    public void AddEntity(Entity entity)
    {
        _Entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        _Entities.Remove(entity);
    }



    private void Awake()
    {
        // Keep the first instance
        if (Instance == null)
        {
            Instance = this;
            Random.InitState(123);
            return;
        }

        // Self destruct any duplicate instances
        Destroy(gameObject);
    }



    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Do something when the 'Escape' key is pressed
            MapManager mapManager = MapManager.Instance;
            mapManager.Restart();
        }
    }
}
