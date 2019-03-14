using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TetrominoHandler : MonoBehaviour
{
    private float time = 1.0f;
    private GamePlayManager gameplayManager;
    public bool allowRotation = true;
    public bool limitRotation = false;

    private void Start()
    {
        gameplayManager = FindObjectOfType<GamePlayManager>();
    }

    public void Restart()
    {
        SceneManager.LoadScene("main");
    }

    private void Update()
    {
        InputKeyboardHandler();
        UpdateTetromino();
    }

    private void UpdateTetromino()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Handler("Down");
            time = 1.0f;
        }
    }

    private void InputKeyboardHandler()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Handler("Left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Handler("Right");
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Handler("Down");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Handler("Up");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
    }

    private void Handler(string command)
    {
        switch (command)
        {
            case "Left":
                MoveHorizontal(Vector3.left);
                break;
            case "Right":
                MoveHorizontal(Vector3.right);
                break;
            case "Down":
                MoveVertical();
                break;
            case "Up":
                if (allowRotation)
                {
                    ActionLimitRotation(1);
                    if (!IsInValidPosition())
                    {
                        ActionLimitRotation(-1);
                    }
                    else
                    {
                        gameplayManager.UpdateGrid(this);
                    }
                }
                break;
        }
    }

    private void ActionLimitRotation(int modifier)
    {
        if (limitRotation)
        {
            if (transform.rotation.eulerAngles.z >= 90)
            {
                transform.Rotate(Vector3.forward * -90);
            }
            else
            {
                transform.Rotate(Vector3.forward * 90);
            }
        }
        else
        {
            transform.Rotate(Vector3.forward * 90 * modifier);
        }
    }

    private void MoveHorizontal(Vector3 direction)
    {
        transform.position += direction;
        if (!IsInValidPosition())
        {
            transform.position += direction * -1;
        }
        else
        {
            gameplayManager.UpdateGrid(this);
        }
    }

    private void MoveVertical()
    {
        transform.position += Vector3.down;
        if (!IsInValidPosition())
        {
            transform.position += Vector3.up;
            gameplayManager.DestroyRow();
            enabled = false;
            if (gameplayManager.IsReachLimitGrid(this))
            {
                gameplayManager.GameOver(this);
            }
            else
            {
                gameplayManager.GenerateTetromino();
            }
        }
        else
        {
            gameplayManager.UpdateGrid(this);
        } 
    }

    private bool IsInValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector3 pos = gameplayManager.Round(mino.position);
            if (!gameplayManager.IsTetrominoInsideAGrid(pos))
            {
                return false;
            }
            if (gameplayManager.GetTransformAtGridPosition(pos) != null
                && gameplayManager.GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }

}
