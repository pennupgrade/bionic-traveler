using UnityEngine;

public class Menu : MonoBehaviour {
    private bool open;
    
    private Canvas canvas;
    
    private void Start() {
        canvas = GetComponent<Canvas>();
        Close();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Toggle();
        }
    }

    public void Open() {
        open = true;
        
        canvas.enabled = true;
        Time.timeScale = 0;
    }

    public void Close() {
        open = false;
        
        canvas.enabled = false;
        Time.timeScale = 1;
    }

    public void Toggle() {
        if (open) {
            Close();
        } else {
            Open();
        }
    }
}
