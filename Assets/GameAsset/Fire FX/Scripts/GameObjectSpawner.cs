using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI; // Thêm namespace này

public class GameObjectSpawner : MonoBehaviour
{
    // Used to sort particle system list
    public GameObject[] particles; // GameObjects to spawn (used to only be particle systems aka var naming)
    public Material[] materials;
    public Color[] cameraColors;
    public int maxButtons = 10; // Maximum buttons per page
    public int buttonWidth = 250;
    public int hSpace = 20;
    public bool spawnOnAwake = true; // Instantiate the first model on start
    public bool showInfo; // Show info text on start
    public string removeTextFromButton; // Unwanted text 
    public string removeTextFromMaterialButton; // Unwanted text 
    public float autoChangeDelay;
    public Image image; // Thay vì GUITexture, sử dụng Image

    // Hidden properties
    int page = 0; // Current page
    int pages; // Number of pages
    string currentGOInfo; // Current particle info
    GameObject currentGO; // GameObject currently on stage
    Color currentColor;
    bool isPS; // Toggle to check if this is a PS or a GO

    Material material;
    bool _active = true;

    int counter = -1;
    int matCounter = -1;
    int colorCounter;

    public GUIStyle bigStyle;

    public bool android;

    public void Start()
    {
        // Calculate number of pages
        pages = (int)Mathf.Ceil((float)((particles.Length - 1) / maxButtons));
        if (spawnOnAwake)
        {
            counter = 0;
            ReplaceGO(particles[counter]);
            Info(particles[counter], counter);
        }
        if (autoChangeDelay > 0)
        {
            InvokeRepeating("NextModel", autoChangeDelay, autoChangeDelay);
        }
    }

    public void RandomSpawn()
    {
        int p = UnityEngine.Random.Range(0, particles.Length);
        if (p != 11 && p != 10 && p != 33 && p != 32)
        {
            GameObject pp = (GameObject)Instantiate(particles[p], new Vector3(UnityEngine.Random.Range(175.0f, -175.0f), UnityEngine.Random.Range(175.0f, -175.0f), UnityEngine.Random.Range(0.0f, 550.0f)), transform.rotation);
            pp.GetComponent<ParticleSystem>().loop = false;
            Destroy(pp, pp.GetComponent<ParticleSystem>().duration + 14);
            StartCoroutine(StopEmit(pp));
        }
    }

    public IEnumerator StopEmit(GameObject pp)
    {
        yield return new WaitForSeconds(pp.GetComponent<ParticleSystem>().duration);
        pp.GetComponent<ParticleSystem>().Stop();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _active = !_active;
            if (image != null)
                image.enabled = _active;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextModel();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PrevModel();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && materials.Length > 0)
        {
            matCounter++;
            if (matCounter > materials.Length - 1) matCounter = 0;
            material = materials[matCounter];
            if (currentGO != null)
            {
                currentGO.GetComponent<Renderer>().sharedMaterial = material;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && materials.Length > 0)
        {
            matCounter--;
            if (matCounter < 0) matCounter = materials.Length - 1;
            material = materials[matCounter];
            if (currentGO != null)
            {
                currentGO.GetComponent<Renderer>().sharedMaterial = material;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            colorCounter++;
            if (colorCounter > cameraColors.Length - 1) colorCounter = 0;
        }

        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, cameraColors[colorCounter], Time.deltaTime * 3);

        if (image != null)
        {
            RectTransform rectTransform = image.rectTransform;
            rectTransform.anchoredPosition = new Vector2(Screen.width - image.sprite.rect.width, rectTransform.anchoredPosition.y);
        }
    }

    public void NextModel()
    {
        counter++;
        if (counter > particles.Length - 1) counter = 0;
        ReplaceGO(particles[counter]);
        Info(particles[counter], counter + 1);
    }

    public void OnGUI()
    {
        GUI.skin.button.alignment = TextAnchor.LowerLeft;
        if (showInfo) GUI.Label(new Rect((float)(buttonWidth + hSpace + hSpace), (float)(hSpace * 3), 500.0f, 500.0f), currentGOInfo, bigStyle);

        if (!android)
        {
            if (_active)
            {
                if (particles.Length > maxButtons)
                {
                    if (GUI.Button(new Rect((float)hSpace, (float)((maxButtons + 1) * 18), buttonWidth * .5f, 18.0f), "Prev")) if (page > 0) page--; else page = pages;
                    if (GUI.Button(new Rect(buttonWidth * .5f + hSpace, (float)((maxButtons + 1) * 18), buttonWidth * .5f, 18.0f), "Next")) if (page < pages) page++; else page = 0;
                    GUI.Label(new Rect(60.0f, (float)((maxButtons + 2) * 18), 150.0f, 22.0f), "Page" + (page + 1) + " / " + (pages + 1));
                }

                showInfo = GUI.Toggle(new Rect((float)(buttonWidth + hSpace + hSpace), (float)hSpace, buttonWidth * .5f, 25.0f), showInfo, "Info");

                int pageButtonCount = particles.Length - (page * maxButtons);
                if (pageButtonCount > maxButtons) pageButtonCount = maxButtons;

                for (int i = 0; i < pageButtonCount; i++)
                {
                    string buttonText = particles[i + (page * maxButtons)].transform.name;
                    if (removeTextFromButton != "")
                        buttonText = buttonText.Replace(removeTextFromButton, "");

                    if (GUI.Button(new Rect((float)hSpace, (float)(i * 18 + 18), (float)buttonWidth, 18.0f), buttonText))
                    {
                        if (currentGO != null) Destroy(currentGO);
                        GameObject go = (GameObject)Instantiate(particles[i + page * maxButtons]);
                        currentGO = go;
                        counter = i + (page * maxButtons);
                        if (material != null)
                            go.GetComponent<Renderer>().sharedMaterial = material;
                        Info(go, i + (page * maxButtons) + 1);
                    }
                }

                for (int m = 0; m < materials.Length; m++)
                {
                    string b = materials[m].name;
                    if (removeTextFromMaterialButton != "")
                        b = b.Replace(removeTextFromMaterialButton, "");

                    if (GUI.Button(new Rect((float)hSpace, (float)((maxButtons + m + 4) * 18), 150.0f, 18.0f), b))
                    {
                        material = materials[m];
                        if (currentGO != null)
                        {
                            currentGO.GetComponent<Renderer>().sharedMaterial = material;
                        }
                    }
                }
            }
        }
        else
        {
            if (GUI.Button(new Rect((Screen.width * .5f) - 150, (float)(Screen.height - 120), 150.0f, 100.0f), "Prev"))
            {
                PrevModel();
            }

            if (GUI.Button(new Rect((Screen.width * .5f), (float)(Screen.height - 120), 150.0f, 100.0f), "Next"))
            {
                NextModel();
            }
        }
    }

    public void PrevModel()
    {
        counter--;
        if (counter < 0) counter = particles.Length - 1;
        ReplaceGO(particles[counter]);
        Info(particles[counter], counter + 1);
    }

    public void Info(GameObject go, int i)
    {
        if (go.GetComponent<ParticleSystem>() != null)
        {
            PlayPS(go.GetComponent<ParticleSystem>(), i);
            InfoPS(go.GetComponent<ParticleSystem>(), i);
        }
        else
        {
            InfoGO(go, i);
        }
    }

    public void ReplaceGO(GameObject _go)
    {
        if (currentGO != null) Destroy(currentGO);
        GameObject go = (GameObject)Instantiate(_go);
        currentGO = go;
        if (material != null)
            go.GetComponent<Renderer>().sharedMaterial = material;
    }

    public void PlayPS(ParticleSystem _ps, int _nr)
    {
        Time.timeScale = 1.0f;
        _ps.Play();
    }

    public void InfoGO(GameObject _ps, int _nr)
    {
        currentGOInfo = "" + "" + _nr + "/" + particles.Length + "\n" + _ps.gameObject.name + "\n" + _ps.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3 + " Tris";
        currentGOInfo = currentGOInfo.Replace("_", " ");
    }

    public void Instructions()
    {
        currentGOInfo = currentGOInfo + "\n\nUse mouse wheel to zoom \n" + "Click and hold to rotate\n" + "Press Space to show or hide menu\n" + "Press Up and Down arrows to cycle materials\n" + "Press B to cycle background colors";
        currentGOInfo = currentGOInfo.Replace("(Clone)", "");
    }

    public void InfoPS(ParticleSystem _ps, int _nr)
    {
        currentGOInfo = "System" + ": " + _nr + "/" + particles.Length + "\n" + _ps.gameObject.name + "\n\n";
        currentGOInfo = currentGOInfo.Replace("_", " ");
        currentGOInfo = currentGOInfo.Replace("(Clone)", "");
    }
}
