using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

public class FileManager : MonoBehaviour {

    private string directory;

    private string currentSaveImagePath;

    void Start () {
        directory = UnityEngine.Application.persistentDataPath;
    }

    public string SaveFilePanel (string title, string defaultName) {
        SaveFileDialog saveFileDialog = new SaveFileDialog ();

        saveFileDialog.Title = title;

        var finalFilename = "";

        if (!string.IsNullOrEmpty (directory)) {
            finalFilename = GetDirectoryPath (directory);
        }

        if (!string.IsNullOrEmpty (defaultName)) {
            finalFilename += defaultName;
        }

        saveFileDialog.FileName = finalFilename;

        saveFileDialog.DefaultExt = ".png";
        saveFileDialog.Filter = "Image (*.png)|*.png";
        var result = saveFileDialog.ShowDialog ();
        var filename = result == DialogResult.OK ? saveFileDialog.FileName : "";
        saveFileDialog.Dispose ();
        return filename;
    }

    public void FolderBrowserPanel (string description) {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog ();
        folderBrowserDialog.Description = description;
        var result = folderBrowserDialog.ShowDialog ();
        var path = result == DialogResult.OK && !string.IsNullOrEmpty (folderBrowserDialog.SelectedPath) ? folderBrowserDialog.SelectedPath : directory;
        folderBrowserDialog.Dispose ();
        directory = path;
    }

    private string GetDirectoryPath (string directory) {
        var directoryPath = Path.GetFullPath (directory);
        if (!directoryPath.EndsWith ("\\")) {
            directoryPath += "\\";
        }
        if (Path.GetPathRoot (directoryPath) == directoryPath) {
            return directory;
        }
        return Path.GetDirectoryName (directoryPath) + Path.DirectorySeparatorChar;
    }

    public void SavePNG (byte[] bytes) {
        string path = SaveFilePanel ("Save", "ScreenShot.png");
        if (!string.IsNullOrEmpty (path)) {
            System.IO.File.WriteAllBytes (path, bytes);
            System.Diagnostics.Process.Start (@path);
            currentSaveImagePath = path;
        }
    }

    public Texture2D LoadPNG (string filePath = "") {
        if (string.IsNullOrEmpty (filePath)) {
            filePath = currentSaveImagePath;
        }

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists (filePath)) {
            fileData = File.ReadAllBytes (filePath);
            tex = new Texture2D (2, 2);
            tex.LoadImage (fileData);
        }
        return tex;
    }
}