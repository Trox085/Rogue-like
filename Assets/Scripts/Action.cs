using UnityEngine;



internal static class Action
{
    public static void EscapeAction()
    {
        Debug.Log("Quit");
        //Application.Quit();
    }



    public static void MovementAction(Entity entity, Vector2 direction)
    {
        Debug.Log($"{entity.name} moves {direction}!");
        entity.Move(direction);

        GameManager gameManager = GameManager.Instance;
        gameManager.EndTurn();
    }



    public static void SkipAction(Entity entity)
    {
        Debug.Log($"{entity.name} skipped their turn!");

        GameManager gameManager = GameManager.Instance;
        gameManager.EndTurn();
    }
}