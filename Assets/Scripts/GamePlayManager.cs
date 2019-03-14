using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    private int score = 0;
    private int scoreOneLine = 40;
    private int scoreTwoLine = 100;
    private int scoreThreeLine = 300;
    private int scoreFourLine = 1200;
    private int lengthDestroyRows = 0;
    private float restartTime = 1.0f;
    public Text scoreText;
    public Text highScoreText;
    public Text gameOver;
    public Transform tetrominoContainer;
    

    private void Start()
    {
        GenerateTetromino();
        highScoreText.text = "High Score = " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        gameOver.gameObject.SetActive(false);
    }

    void HighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreText.text = "High Score = " + score.ToString();
        }
    }

    public void UpdateScore()
    {
        switch (lengthDestroyRows)
        {
            case 1:
                score += scoreOneLine;
                break;
            case 2:
                score += scoreTwoLine;
                break;
            case 3:
                score += scoreTwoLine;
                break;
            case 4:
                score += scoreThreeLine;
                break;
            case 5:
                score += scoreFourLine;
                break;
        }
        scoreText.text = "Score = " + score;
        HighScore();
        lengthDestroyRows = 0;
    }

    private bool IsRowFullAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        lengthDestroyRows++;
        return true;
    }

    private void DestroyRowAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    private void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += Vector3.down;
            }
        }
    }

    private void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    public void DestroyRow()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsRowFullAt(y))
            {
                DestroyRowAt(y);
                MoveAllRowsDown(y + 1);
                y--;
            }
        }
        UpdateScore();
    }

    public bool IsReachLimitGrid(TetrominoHandler tetromino)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            foreach(Transform mino in tetromino.transform)
            {
                Vector3 pos = Round(mino.position);

                if (pos.y >= gridHeight - 1 &&
                    !tetromino.isActiveAndEnabled)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void GameOver(TetrominoHandler tetromino)
    {
        gameOver.gameObject.SetActive(true);
        Destroy(tetrominoContainer.gameObject);
        tetromino.Restart();
    }

    public bool IsTetrominoInsideAGrid(Vector3 pos)
    {
        return (
            pos.x >= 0 &&
            pos.x < gridWidth &&
            pos.y >= 0
            );
    }

    public Vector3 Round(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            Mathf.Round(pos.y),
            Mathf.Round(pos.z)
        );
    }

    private string GetRandomTetromino()
    {
        int val = Random.Range(0, 7);
        string tetrominoname = "TetrominoT";

        switch (val)
        {
            case 0:
                tetrominoname = "TetrominoI";
                break;
            case 1:
                tetrominoname = "TetrominoJ";
                break;
            case 2:
                tetrominoname = "TetrominoL";
                break;
            case 3:
                tetrominoname = "TetrominoO";
                break;
            case 4:
                tetrominoname = "TetrominoS";
                break;
            case 5:
                tetrominoname = "TetrominoT";
                break;
            case 6:
                tetrominoname = "TetrominoZ";
                break;
        }
        return "Prefabs/" + tetrominoname;
    }

    public Transform GetTransformAtGridPosition(Vector3 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void UpdateGrid(TetrominoHandler tetromino)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tetromino.transform)
        {
            Vector3 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public void GenerateTetromino()
    {
        GameObject tetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)),
                                new Vector3(5.0f, 18.0f, 0.0f), Quaternion.identity);
        tetromino.transform.SetParent(tetrominoContainer);
    }

}
