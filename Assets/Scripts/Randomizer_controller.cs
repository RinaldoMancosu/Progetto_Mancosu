using System.IO;
using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class Randomizer_controller : MonoBehaviour
    {
        int contatore = 0;

        private GameObject modello_caricato;
        private Object[] lista_modelli;
        private int contatore_modelli;

        private GameObject my_Light;

        private Object[] lista_skybox;
        private Object[] lista_skybox_images;
        private Object[] lista_skybox_colors;
        private int contatore_skybox;

        private Object[] lista_material;
        private int contatore_material;

        private Camera_Controller controllo_camera;
        private bool camera_attiva;
    
        [Header("Dataset:")]
        [Space(5)]
        [Tooltip("Numero di elementi contenuti nel Dataset")]
        public int grandezza_dataset = 2000;
        [Space(5)]
        [Tooltip("Cartella salvataggio Dataset")]
        public string directory_dataset = "";

        [Space(10)]
        [Header("- Randomizer Texture Oggetto")]
        [Space(5)]
        [Tooltip("Flag per decidere se randomizzare il material del modello")]
        public bool Randomizer_Model_Material = false;

        [Space(5)]
        [Tooltip("Flag per decidere se randomizzare il colore del modello")]
        public bool Randomizer_Model_RGB_Color = false;
        [Space(5)]
        [Tooltip("x: min, y: Max")]
        public Vector2 Randomizer_R_color = new Vector2 (0.4f, 1);
        [Space(5)]
        [Tooltip("x: min, y: Max")]
        public Vector2 Randomizer_G_color = new Vector2 (0.4f, 1);
        [Space(5)]
        [Tooltip("x: min, y: Max")]
        public Vector2 Randomizer_B_color = new Vector2 (0.4f, 1);


        [Space(10)]
        [Header("- Randomizer Posizione ")]
        [Space(5)]
        [Tooltip("Flag per decidere se randomizzare la posizione")]
        public bool Randomizer_Model_Rotation = true;
        [Space(5)]
        [Tooltip("Flag per decidere se randomizzare la rotazione del modello")]
        public bool Randomizer_Model_Position = true;
        [Tooltip("x: min, y: Max")]
        public Vector2 Randomizer_X_Position = new Vector2 (-2f, 2f);
        [Space(5)]
        [Tooltip("x: min, y: Max")]
        public Vector2 Randomizer_Y_Position = new Vector2 (-0.7f, 0.7f);
        [Space(5)]
        [Tooltip("x: min, y: Max")]
        public Vector2 Randomizer_Z_Position = new Vector2 (-1.5f, 1.5f);


        [Space(10)]
        [Header("- Randomizer Luci")]
        [Space(5)]
        [Tooltip("Flag per decidere se randomizzare la direzione della luce")]
        public bool Randomizer_Light_Rotation = true;
        [Space(5)]
        [Tooltip("x: min, y: Max")]
        public Vector2 Randomizer_Light_Intensity = new Vector2 (0.5f, 2f);
        [Space(5)]
        [Tooltip("Flag per decidere se randomizzare il colore della luce")]
        public bool Randomizer_Light_Color = true;
        

        [Space(10)]
        [Header("- Randomizer Skybox")]
        [Space(5)]
        [Tooltip("Flag per decidere se si vogliono immagini come sfondo")]
        public bool Randomizer_Skybox_Images = false;
        [Space(5)]
        [Tooltip("Flag per decidere se si vogliono colori come sfondo")]
        public bool Randomizer_Skybox_Colors = false;
        

        // Start is called before the first frame updateprivate static Random rng = new Random();  
        void Start()
        {
            camera_attiva = true;

            //se non esiste crea una cartella chiamata Dataset negli Assets
            if (directory_dataset == "")
                directory_dataset = Application.dataPath + "/Dataset/";

            contatore_modelli = -1;
            contatore_skybox = -1;
            contatore_material = -1;


            lista_modelli = Resources.LoadAll("Prefabs", typeof(GameObject));

            lista_skybox_images = Resources.LoadAll("Skybox/Images_Skybox", typeof(Material));
            lista_skybox_colors = Resources.LoadAll("Skybox/MonoColor_Skybox", typeof(Material));

            lista_material = Resources.LoadAll("Materials", typeof(Material));

            my_Light = GameObject.Find("Sun");

            GameObject my_Camera = GameObject.Find("Main Camera");
            controllo_camera = my_Camera.GetComponent<Camera_Controller>();
        }

        
        void Update()
        {   
            if(contatore <= grandezza_dataset) {
                contatore++;
                Destroy(modello_caricato);
                modello_caricato = spawnNextModel();

                if(Randomizer_Model_Material) 
                {
                    MeshRenderer modello_mesh_renderer = modello_caricato.GetComponent<MeshRenderer>();
                    modello_mesh_renderer.material = spawnNextModelMaterial();
                }

                if (Randomizer_Skybox_Images | Randomizer_Skybox_Colors)
                {
                    RenderSettings.skybox = spawnNextSkyMaterial();
                    DynamicGI.UpdateEnvironment();
                }
        
                if (Randomizer_Light_Rotation)
                    my_Light.transform.rotation = Random.rotation;
            
                Light temp_light = my_Light.GetComponent<Light>();
                temp_light.intensity = Random.Range(Randomizer_Light_Intensity[0], Randomizer_Light_Intensity[1]);
            
                if (Randomizer_Light_Color)
                    temp_light.color = Random.ColorHSV();
            }
            else {
                return;
            }
               
        }
  
        void LateUpdate()
        {
            if (camera_attiva & contatore <= grandezza_dataset) {
                string nome_cartella = directory_dataset + "/" + lista_modelli[contatore_modelli].name + "/";
                if(Directory.Exists(nome_cartella) == false)
                    Directory.CreateDirectory(nome_cartella);

                int numero_ocorrenza = 0;
                string percorso = nome_cartella + numero_ocorrenza++.ToString() + ".png";
                while(File.Exists(percorso))
                    percorso = nome_cartella + numero_ocorrenza++.ToString() + ".png";

                byte[] Bytes = controllo_camera.CamCapture();
                File.WriteAllBytes(percorso, Bytes);

            }
        }

        GameObject spawnNextModel(bool random = true)
        {
            GameObject my_model;

            if(random)
                contatore_modelli = Random.Range(0, lista_modelli.Length);
            else
                contatore_modelli++;

            if(contatore_modelli >= lista_modelli.Length)
                contatore_modelli = 0;
            
            my_model = Instantiate((GameObject)lista_modelli[contatore_modelli]);  


            if(Randomizer_Model_Rotation)
                my_model.transform.rotation = Random.rotation;

            if (Randomizer_Model_Position)
                my_model.transform.position = new Vector3(Random.Range(Randomizer_X_Position[0], Randomizer_X_Position[1]), Random.Range(Randomizer_X_Position[0], Randomizer_X_Position[1]), Random.Range(Randomizer_X_Position[0], Randomizer_X_Position[1]));
            else
                my_model.transform.position = new Vector3(0, 0, 0);

            if(Randomizer_Model_RGB_Color)
            {
                var temp_renderer = my_model.GetComponent<Renderer>();
                //temp_renderer.material.color = Random.ColorHSV();
                temp_renderer.material.color = new Color(Random.Range(Randomizer_R_color[0], Randomizer_R_color[1]), Random.Range(Randomizer_G_color[0], Randomizer_G_color[1]), Random.Range(Randomizer_B_color[0], Randomizer_B_color[1]), 1);
            }

            return my_model;
        }
        

        Material spawnNextSkyMaterial(bool random = true)
        {
            if(Randomizer_Skybox_Images) 
                lista_skybox = lista_skybox_images;
            else
                lista_skybox = lista_skybox_colors;
                
            Material skybox_material;

            if(random)
                contatore_skybox = Random.Range(0, lista_skybox.Length);
            else
                contatore_skybox++;

            if(contatore_skybox >= lista_skybox.Length)
                contatore_skybox = 0;

            skybox_material = (Material)lista_skybox[contatore_skybox];        

            return skybox_material;
        }
        
       
        Material spawnNextModelMaterial(bool random = true)
        {      
            Material model_material;
            
            if(random)
                contatore_material = Random.Range(0, lista_material.Length);
            else
                contatore_material++;

            if(contatore_material >= lista_material.Length)
            {
                contatore_material = 0;
            }

            model_material = (Material)lista_material[contatore_material];    
            

            return model_material;
        }
        

        
        public GameObject getModel()
        {
            return modello_caricato;
        }

        

        public string getSaveDir()
        {
            if (directory_dataset == "")
                directory_dataset = Application.dataPath + "/Dataset/";

            return directory_dataset;
        }

        public void setSaveDir(string nuova_directory)
        {
            directory_dataset = nuova_directory;
        }

    }

}