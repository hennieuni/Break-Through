using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string playerPosFile;
    public void PlayGame(){
        

        playerPosFile = Application.dataPath + "/Saves/PlayerPos.txt";
        if (!File.Exists(playerPosFile)){
            File.WriteAllText(playerPosFile, "-9.499884,-1.5");
        }else{
            using (StreamWriter writer = new StreamWriter(playerPosFile,false)){
            writer.WriteLine("-9.499884,-0.5");
            }
        }
        SceneManager.LoadScene(1);
        
    }
   
    public void QuitGame(){
        Debug.Log("Quit");
        Application.Quit(); 
    }
}
