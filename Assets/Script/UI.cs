using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text[] TextField = new Text[8];
    public TileManagement manager;
    public PlayerMove player;

    private int rows;
    private int columns;
    private int startX;
    private int startZ;
    private float initialWeight;
    private int numOfLevel;
    private float weightSpan;
    private float moveTime;

    public void ButtonPressed()
    {
        rows = int.Parse(TextField[0].text);
        columns = int.Parse(TextField[1].text);
        startX = int.Parse(TextField[2].text);
        startZ = int.Parse(TextField[3].text);
        initialWeight = float.Parse(TextField[4].text);
        numOfLevel = int.Parse(TextField[5].text);
        weightSpan = float.Parse(TextField[6].text);
        moveTime = float.Parse(TextField[7].text);

        if (rows < 1 || columns < 1 || startX < 0 || startZ < 0 || initialWeight <= 0 || numOfLevel < 1 || weightSpan <= 0 || moveTime <= 0)
            return;//跨越下界
        if (rows > 300 || columns > 300 || startX >= rows || startZ >= columns)
            return;//跨越上界

        manager.rows = rows;
        manager.columns = columns;
        manager.startX = startX;
        manager.startZ = startZ;
        manager.initialWeight = initialWeight;
        manager.numOfLevel = numOfLevel;
        manager.weightSpan = weightSpan;
        player.moveTime = moveTime;

        manager.reset();
    }

    public void exit()
    {
        Application.Quit();
    }

}
