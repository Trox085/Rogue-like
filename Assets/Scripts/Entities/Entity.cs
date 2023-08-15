using UnityEngine;



public class Entity : MonoBehaviour
{
    [SerializeField] private bool _IsSentient = false;



    public bool IsSentient
    {
        get
        {
            return _IsSentient;
        }
    }



    private void Start()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.AddEntity(this);
    }



    public void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction;
    }
}