using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace BionicTraveler
{
    public class Menu : MonoBehaviour
    {
        private bool open;
        [SerializeField]
        private Canvas canvas;
        
        // Start is called before the first frame update
        void Start()
        {
            Close();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle();
            }
        }

        public void Open()
        {
            open = true;
            canvas.enabled = true;
            Time.timeScale = 0;
        }

        public void Close()
        {
            open = false;
            canvas.enabled = false;
            Time.timeScale = 1;
        }

        public void Toggle()
        {
            if (open)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}
