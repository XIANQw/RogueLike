using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minNum;
        public int maxNum;

        public Count(int min, int max)
        {
            minNum = min;
            maxNum = max;
        }
    }

    public int cols = 10;
    public int rows = 10;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] walls;
    public GameObject[] floors;
    public GameObject[] foods;
    public GameObject[] enemys;
    public GameObject[] outerWalls;

    private Transform boardHolder;
    private List<Vector3> gridPos = new List<Vector3>();


    void InitGridPos()
    {
        for (int x = 2; x < rows - 2; x++)
        {
            for (int y = 2; y < cols - 2; y++)
                gridPos.Add(new Vector3(x, y, 0f));
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("boardHolder").transform;
        for (int x = 0; x < cols; x++)
        {
            for(int y = 0; y < rows; y++)
            {
                GameObject choice = floors[Random.Range(0, floors.Length)];
                if (x == 0 || x == cols - 1 || y == 0 || y == rows - 1)
                    choice = outerWalls[Random.Range(0, outerWalls.Length)];
                GameObject instance = Instantiate(choice, new Vector3(x, y, 0), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 randomPosition()
    {
        int randomIndex = Random.Range(0, gridPos.Count);
        Vector3 res = gridPos[randomIndex];
        gridPos[randomIndex] = gridPos[gridPos.Count - 1];
        gridPos.RemoveAt(gridPos.Count - 1);
        return res;
    }

    void RandomSetup(GameObject[] gameObjs, int minCpt, int maxCpt)
    {
        int cpt = Random.Range(minCpt, maxCpt + 1);
        for(int i = 0; i < cpt; i++)
        {
            GameObject choice = gameObjs[Random.Range(0, gameObjs.Length)];
            Vector3 randomPos = randomPosition();
            Instantiate(choice, randomPos, Quaternion.identity);
        }
    }


    public void SetupScene(int level)
    {
        BoardSetup();
        InitGridPos();
        RandomSetup(walls, wallCount.minNum, wallCount.maxNum);
        RandomSetup(foods, foodCount.minNum, foodCount.maxNum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        RandomSetup(enemys, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(cols - 2, rows - 2, 0f), Quaternion.identity);
    }
}