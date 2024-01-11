using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Collections;

public class ScreenshotCapture : MonoBehaviour
{
// Specify the key to trigger the screenshot
    public KeyCode screenshotKey = KeyCode.P;

    // Specify the directory path to save screenshots
    public string screenshotDirectory = "Assets/Screenshots";

    // Specify the path for the CSV file
    public string csvFilePath = "Assets/Screenshots/Positions.csv";

    // Reference to the cube GameObject
    public GameObject cube;

    // Specify the amount to move the cube in the y-position
    public float moveAmount = 1.0f;

    // List to store positions
    private List<Vector3> positions = new List<Vector3>();
    private int counter = 0;
    
    private string filenamePrefix = "Screenshot_";

    private string filenameExtension = ".png";

    public float minX = -2f;
    public float maxX = 2f;
    public float minY = -2f;
    public float maxY = 2f;
    public float minZ = -2f;
    public float maxZ = 2f;
    private bool isCapturing = false;
    void Start() {
        // Ensure the directory exists; create it if not
        if (!Directory.Exists(screenshotDirectory))
        {
            Directory.CreateDirectory(screenshotDirectory);
        }
        // Check if the file exists, if not, create it and write the header
        if (!File.Exists(csvFilePath))
        {
            // Create the file and write the header
            using (StreamWriter sw = new StreamWriter(csvFilePath))
            {
                sw.WriteLine("X,Y,Z,FilePath");
            }
        }
        Populate_List();
    }
    void LateUpdate()
    {
        if (counter < positions.Count) {
            if(!isCapturing) {
                MoveCube();
                StartCoroutine(CaptureScreenshotAndWait());
                SavePosition();
                counter++;
            }
        }
    }

    void Populate_List() {
        float increment = 0.2f;
        for (float x = minX; x <= maxX; x+= increment)
        {
            for (float y = minY; y <= maxY; y+= increment)
            {
                for (float z = minZ; z <= maxZ; z+= increment)
                {
                    positions.Add(new Vector3( (float) Math.Round(x, 2), (float) Math.Round(y, 2), (float) Math.Round(z, 2)));
                }
            }
        }
        Debug.Log("positions" + positions.Count.ToString());
    }

    IEnumerator CaptureScreenshotAndWait()
    {
        isCapturing = true;
        
        // Generate a filename for the screenshot (using current date and time)
        string filename = filenamePrefix + counter.ToString() + filenameExtension;

        // Capture the screenshot and save it to the specified directory
        string filePath = Path.Combine(screenshotDirectory, filename);
        ScreenCapture.CaptureScreenshot(filePath);
        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();
        // Wait until the screenshot is captured
        yield return new WaitUntil(() => System.IO.File.Exists(filePath));

        // Your code to be executed after the screenshot is captured
        Debug.Log("Screenshot captured!");

        isCapturing = false;
    }
    void MoveCube()
    {
        // Move the cube in the y-position
        cube.transform.position = positions[counter];
    }

    void SavePosition()
    {
        // Check if the file exists, if not, create it and write the header
        bool fileExists = File.Exists(csvFilePath);
        using (StreamWriter sw = new StreamWriter(csvFilePath, true))
        {
            if (!fileExists)
            {
                // Write the header if the file is newly created
                sw.WriteLine("X,Y,Z,FilePath");
            }

            // Write the coordinates and file path to the CSV file
            sw.WriteLine($"{positions[counter].x},{positions[counter].y},{positions[counter].z},{filenamePrefix + counter.ToString() + filenameExtension}");
        }

        // Print a message to the console
        Debug.Log("Position saved to CSV file: " + csvFilePath);
    }

    private float timer = 0.0f;
    private float delay = 10f;
    void wait() {
        timer += Time.deltaTime;

        if (timer >= delay)
        {
            // Your code after the wait

            // Reset the timer for the next iteration
            timer = 0.0f;
        }
    }
}
