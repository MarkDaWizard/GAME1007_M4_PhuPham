
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Testing : MonoBehaviour {

    [SerializeField] private HeatMapGenericVisual heatMapGenericVisual;
    private GridScript<HeatMapGridObject> grid;


    public TextMeshProUGUI scanCounterText, extractCounterText, resourceCounterText, modeText, howToText;
    public GameObject gamePad, exitButton;
    public float currentScan = 15;
    public float currentExtract = 10;
    public float currentGold = 0f;
    public bool isScanningMode = false;
    public bool isPlaying = false;

    private void Start() {

        grid = new GridScript<HeatMapGridObject>(25, 25, 8f, Vector3.zero, (GridScript<HeatMapGridObject> g, int x, int y) => new HeatMapGridObject(g, x, y, false));

        heatMapGenericVisual.SetGrid(grid);
        RandomizeGrid();
    }

    private void Update() {
        Vector3 position = GetMouseWorldPosition();
        UpdateUI();

        if(!isPlaying)
        {
            if(Input.anyKeyDown)
            {
                isPlaying = true;
                gamePad.SetActive(false);
            }
        }


        if (Input.GetMouseButtonDown(0) && !isScanningMode && currentExtract >0) 
        {
            HeatMapGridObject heatMapGridObject = grid.GetGridObject(position);
            if (heatMapGridObject != null) {
                currentGold += heatMapGridObject.ExtractGold();
            }
            Debug.Log(currentGold);
            for (int a = heatMapGridObject.x - 1; a < heatMapGridObject.x + 2; a++)
            {
                for (int b = heatMapGridObject.y - 1; b < heatMapGridObject.y + 2; b++)
                {
                    HeatMapGridObject YHeatMapGridObject = grid.GetGridObject(a, b);
                    YHeatMapGridObject = grid.GetGridObject(a, b);
                    if (YHeatMapGridObject != null)
                    {
                        YHeatMapGridObject.AddValue(-25);
                    }
                }
            }
            currentExtract--;
        }

        if (Input.GetMouseButtonDown(0) && isScanningMode && currentScan >0)
        {
            HeatMapGridObject heatMapGridObject = grid.GetGridObject(position);
            for (int a = heatMapGridObject.x - 1; a < heatMapGridObject.x + 2; a++)
            {
                for (int b = heatMapGridObject.y - 1; b < heatMapGridObject.y + 2; b++)
                {
                    HeatMapGridObject YHeatMapGridObject = grid.GetGridObject(a, b);
                    YHeatMapGridObject = grid.GetGridObject(a, b);
                    if (YHeatMapGridObject != null)
                    {
                        YHeatMapGridObject.Scan();
                    }
                }
            }
            currentScan--;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isScanningMode = !isScanningMode;
        }


    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    private void UpdateUI()
    {
        if (!isPlaying)
            return;
        else
            howToText.text = "";

        if(isScanningMode)
        {
            modeText.text = "Scan Mode";
        }
        else if(!isScanningMode)
        {
            modeText.text = "Extract Mode";
        }
        if(currentExtract == 0)
        {
            modeText.text = "GAME OVER";
            exitButton.SetActive(true);
        }
        scanCounterText.text = "Scans remaining:" + currentScan;
        extractCounterText.text = "Extracts remaining: " + currentExtract;
        resourceCounterText.text = "Current resource: " + currentGold;


    }

    private void RandomizeGrid()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                int randX = Random.Range(0, grid.GetWidth());
                int randY = Random.Range(0, grid.GetHeight());

                HeatMapGridObject heatMapGridObject = grid.GetGridObject(randX,randY);
                if (heatMapGridObject != null)
                {
                    heatMapGridObject.AddValue(100);
                }


                for (int a = heatMapGridObject.x - 2; a < randX + 3; a++)
                {
                    for (int b = heatMapGridObject.y - 2; b < randY + 3; b++)
                    {
                        HeatMapGridObject XHeatMapGridObject = grid.GetGridObject(a, b);
                        if (XHeatMapGridObject != null)
                        {
                            XHeatMapGridObject.AddValue(25);
                        }
                    }
                }

                for (int a = heatMapGridObject.x - 1; a < randX + 2; a++)
                {
                    for (int b = heatMapGridObject.y - 1; b < randY + 2; b++)
                    {
                        HeatMapGridObject YHeatMapGridObject = grid.GetGridObject(a, b);
                        if (YHeatMapGridObject != null)
                        {
                            YHeatMapGridObject.AddValue(25);
                        }
                    }
                }

            }
        }
    }

    // Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}

public class HeatMapGridObject {

    private const int MIN = 0;
    private const int MAX = 100;

    private GridScript<HeatMapGridObject> grid;
    public int x;
    public int y;
    private int value;
    public bool isScanned = false;

    public HeatMapGridObject(GridScript<HeatMapGridObject> grid, int x, int y, bool isScanned) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isScanned = isScanned;
    }

    public void AddValue(int addValue) {
        value += addValue;
        value = Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGridObjectChanged(x, y);
    }

    public float GetValueNormalized() 
    {
        if(!isScanned)
        {
            return 0.1f;
        }
        return (float)value / MAX;
    }

    public float ExtractGold()
    {
        float gold = value;
        value = MIN;
        grid.TriggerGridObjectChanged(x, y);
        return (float)gold;
        
    }

    public void Scan()
    {
        isScanned = true;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString() {
        return value.ToString();
    }

}


