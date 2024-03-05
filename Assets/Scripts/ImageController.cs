using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using TMPro;
using System.IO;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class ImageController : MonoBehaviour
{
    public static Sprite[] images;

    public static GameObject navigationSpheresParent;
    public static GameObject navigationPointPrefab;
    public static GameObject infoPointPrefab;
    public static GameObject quizPointPrefab;
    public static GameObject quizAnswerPrefab;

    public static string jsonFilePath = "Assets/Resources/positions.json";

    private void Awake()
    {
        images = Resources.LoadAll<Sprite>("Photos");
        navigationSpheresParent = GameObject.Find("ActionPoints");
        navigationPointPrefab = Resources.Load<GameObject>("Prefabs/NavigationPointCanvas");
        infoPointPrefab = Resources.Load<GameObject>("Prefabs/InfoPointCanvas");
        quizPointPrefab = Resources.Load<GameObject>("Prefabs/QuizPointCanvas");
        quizAnswerPrefab = Resources.Load<GameObject>("Prefabs/QuizButton");
    }

    public static void NavigationActivate(NavigationPoint navigationPoint)
    {
        NavigateTo(navigationPoint.navigateToImage);
        RotatePlayer(navigationPoint);
        DestroyAllNavigationSpheres();
        LoadPositionsAndInstantiateSpheres();
    }

    private static void RotatePlayer(NavigationPoint navigationPoint)
    {
        GameObject player = GameObject.Find("XR Origin (XR Rig)");
        if (player != null)
        {
            float cameraYRotation = Mathf.Repeat(Camera.main.transform.rotation.eulerAngles.y, 360f);
            float targetYRotationNormalized = Mathf.Repeat(navigationPoint.yRotation, 360f);

            float deltaRotation = targetYRotationNormalized - (0 + cameraYRotation);

            if (deltaRotation > 180f)
                deltaRotation -= 360f;
            else if (deltaRotation < -180f)
                deltaRotation += 360f;

            player.transform.Rotate(0f, deltaRotation, 0f, Space.World);
        }
    }

    public static void LoadPositionsAndInstantiateSpheres()
    {
        string jsonString = File.ReadAllText(jsonFilePath);
        NavigationPoints positionData = JsonConvert.DeserializeObject<NavigationPoints>(jsonString);
        InfoPoints infoPointData = JsonConvert.DeserializeObject<InfoPoints>(jsonString);
        QuizPoints quizPointData = JsonConvert.DeserializeObject<QuizPoints>(jsonString);

        foreach (NavigationPoint position in positionData.navigationPoints)
        {
            if (position.image == GetCurrentImageName())
            {
                InstantiateNavigationPoint(position);
            }
        }

        foreach (InfoPoint infoPoint in infoPointData.infoPoints)
        {
            if (infoPoint.image == GetCurrentImageName())
            {
                InstantiateInfoPoint(infoPoint);
            }
        }

        foreach (QuizPoint quizPoint in quizPointData.quizPoints)
        {
            if (quizPoint.image == GetCurrentImageName())
            {
                InstantiateQuizPoint(quizPoint);
            }
        }
    }

    public static void InstantiateNavigationPoint(NavigationPoint position)
    {
        GameObject sphere = Instantiate(navigationPointPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity);
        sphere.transform.SetParent(navigationSpheresParent.transform);


        sphere.name = $"{position.title}-{position.navigateToImage}";
        NavigationObjectController objectController = sphere.AddComponent<NavigationObjectController>();

        NavigationPoint objectInfo = new()
        {
            title = position.title,
            description = position.description,
            x = position.x,
            y = position.y,
            z = position.z,
            yRotation = position.yRotation,
            image = position.image,
            navigateToImage = position.navigateToImage,
        };

        objectController.objectInfo = objectInfo;
    }

    public static void InstantiateNavigationPoint(Vector3 positionVector)
    {
        GameObject sphere = Instantiate(navigationPointPrefab, new Vector3(positionVector.x, positionVector.y, positionVector.z), Quaternion.identity);
        sphere.transform.SetParent(navigationSpheresParent.transform);
    }

    public static void InstantiateInfoPoint(InfoPoint infoPoint)
    {
        GameObject sphere = Instantiate(infoPointPrefab, new Vector3(infoPoint.x, infoPoint.y, infoPoint.z), Quaternion.identity);
        sphere.transform.SetParent(navigationSpheresParent.transform);
        sphere.name = $"{infoPoint.title}-info";
        InfoObjectController infoObjectController = sphere.AddComponent<InfoObjectController>();

        InfoPoint objectInfo = new()
        {
            title = infoPoint.title,
            description = infoPoint.description,
            x = infoPoint.x,
            y = infoPoint.y,
            z = infoPoint.z,
            yRotation = infoPoint.yRotation,
            image = infoPoint.image,
        };

        infoObjectController.objectInfo = objectInfo;
    }

    public static void InstantiateInfoPoint(Vector3 positionVector)
    {
        GameObject sphere = Instantiate(infoPointPrefab, new Vector3(positionVector.x, positionVector.y, positionVector.z), Quaternion.identity);
        sphere.transform.SetParent(navigationSpheresParent.transform);
    }

    public static void InstantiateQuizPoint(QuizPoint quizPoint)
    {
        GameObject sphere = Instantiate(quizPointPrefab, new Vector3(quizPoint.x, quizPoint.y, quizPoint.z), Quaternion.identity);
        sphere.transform.SetParent(navigationSpheresParent.transform);
        sphere.name = $"{quizPoint.title}-quiz";
        QuizObjectController quizObjectController = sphere.AddComponent<QuizObjectController>();

        QuizPoint objectInfo = new()
        {
            title = quizPoint.title,
            description = quizPoint.description,
            enabled = quizPoint.enabled,
            isCorrect = quizPoint.isCorrect,
            selectedAnswer = quizPoint.selectedAnswer,
            x = quizPoint.x,
            y = quizPoint.y,
            z = quizPoint.z,
            yRotation = quizPoint.yRotation,
            image = quizPoint.image,
            answers = quizPoint.answers,
        };

        quizObjectController.objectInfo = objectInfo;

        if (!quizPoint.enabled)
        {
            ColorBlock colors = sphere.GetComponent<Button>().colors;
            if (quizPoint.isCorrect)
            {
                colors.normalColor = Color.green;
                colors.highlightedColor = Color.green;
                colors.pressedColor = Color.green;
                colors.selectedColor = Color.green;
            }
            else
            {
                colors.normalColor = Color.red;
                colors.highlightedColor = Color.red;
                colors.pressedColor = Color.red;
                colors.selectedColor = Color.red;
            }
            sphere.GetComponent<Button>().colors = colors;
        }
    }
    public static void InstantiateQuizPoint(Vector3 positionVector)
    {
        GameObject sphere = Instantiate(infoPointPrefab, new Vector3(positionVector.x, positionVector.y, positionVector.z), Quaternion.identity);
        sphere.transform.SetParent(navigationSpheresParent.transform);
    }

    public static string GetCurrentImageName()
    {
        GameObject largeSphere = GameObject.Find("ImageSphere");
        if (largeSphere != null)
        {
            Renderer sphereRenderer = largeSphere.GetComponent<Renderer>();
            if (sphereRenderer != null && sphereRenderer.material != null && sphereRenderer.material.mainTexture != null)
            {
                return sphereRenderer.material.mainTexture.name;
            }
        }
        return "DefaultImageName";
    }

    public static void AddNewNavigationPosition(float newX, float newY, float newZ, string newImage)
    {
        string jsonString = File.ReadAllText(jsonFilePath);
        InfoPoints infoPointData = JsonConvert.DeserializeObject<InfoPoints>(jsonString);
        NavigationPoints navigationPointData = JsonConvert.DeserializeObject<NavigationPoints>(jsonString);
        QuizPoints quizPointData = JsonConvert.DeserializeObject<QuizPoints>(jsonString);

        NavigationPoint newNavigationPoint = new()
        {
            x = newX,
            y = newY,
            z = newZ,
            yRotation = 0f,
            title = "Title",
            description = "Description",
            image = newImage,
            navigateToImage = "-",
        };

        navigationPointData ??= new NavigationPoints();

        navigationPointData.navigationPoints ??= new List<NavigationPoint>();

        navigationPointData.navigationPoints.Add(newNavigationPoint);
        SaveAllPositions(navigationPointData, infoPointData, quizPointData);
    }

    public static void AddNewInfoPosition(float newX, float newY, float newZ, string newImage)
    {
        string jsonString = File.ReadAllText(jsonFilePath);
        InfoPoints infoPointData = JsonConvert.DeserializeObject<InfoPoints>(jsonString);
        NavigationPoints navigationPointData = JsonConvert.DeserializeObject<NavigationPoints>(jsonString);
        QuizPoints quizPointData = JsonConvert.DeserializeObject<QuizPoints>(jsonString);
        InfoPoint newInfoPoint = new()
        {
            x = newX,
            y = newY,
            z = newZ,
            yRotation = 0f,
            title = "Title",
            description = "Description",
            image = newImage,
        };

        infoPointData ??= new InfoPoints();
        infoPointData.infoPoints ??= new List<InfoPoint>();

        infoPointData.infoPoints.Add(newInfoPoint);
        SaveAllPositions(navigationPointData, infoPointData, quizPointData);
    }

    public static void AddNewQuizPosition(float newX, float newY, float newZ, string newImage)
    {
        string jsonString = File.ReadAllText(jsonFilePath);
        InfoPoints infoPointData = JsonConvert.DeserializeObject<InfoPoints>(jsonString);
        NavigationPoints navigationPointData = JsonConvert.DeserializeObject<NavigationPoints>(jsonString);
        QuizPoints quizPointData = JsonConvert.DeserializeObject<QuizPoints>(jsonString);
        QuizPoint newQuizPoint = new()
        {
            x = newX,
            y = newY,
            z = newZ,
            yRotation = 0f,
            title = "Title",
            description = "Description",
            image = newImage,
        };

        quizPointData ??= new QuizPoints();
        quizPointData.quizPoints ??= new List<QuizPoint>();

        quizPointData.quizPoints.Add(newQuizPoint);
        SaveAllPositions(navigationPointData, infoPointData, quizPointData);
    }

    public static void ChangeImage()
    {
        GameObject largeSphere = GameObject.Find("ImageSphere");
        Renderer sphereRenderer = largeSphere.GetComponent<Renderer>();
        Sprite image = images[UnityEngine.Random.Range(0, images.Length)];
        sphereRenderer.material.SetTexture("_MainTex", image.texture);
    }

    public static void ChangeImage(Sprite image)
    {
        GameObject largeSphere = GameObject.Find("ImageSphere");
        Renderer sphereRenderer = largeSphere.GetComponent<Renderer>();
        sphereRenderer.material.SetTexture("_MainTex", image.texture);
    }

    public static void DestroyAllNavigationSpheres()
    {
        if (navigationSpheresParent != null)
        {
            foreach (Transform child in navigationSpheresParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public static void NavigateTo(string imageName)
    {
        GameObject largeSphere = GameObject.Find("ImageSphere");
        Renderer sphereRenderer = largeSphere.GetComponent<Renderer>();
        Sprite foundSprite = null;
        foreach (Sprite sprite in images)
        {
            if (sprite.name == imageName)
            {
                foundSprite = sprite;
                break;
            }
        }

        if (foundSprite != null)
        {
            // Do something with the found sprite
            UnityEngine.Debug.Log("Found sprite: " + foundSprite.name);
        }
        else
        {
            UnityEngine.Debug.Log("Sprite not found: " + imageName);
        }
        sphereRenderer.material.SetTexture("_MainTex", foundSprite.texture);
    }

    public static void ChangeInfo(string title, string description)
    {
        ResetQuizAnswers();
        GameObject titleText = GameObject.Find("Title");
        GameObject descriptionText = GameObject.Find("Description");

        titleText.GetComponent<TMPro.TextMeshProUGUI>().text = title;
        descriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = description;
    }

    public static void ShowQuiz(QuizPoint quizPoint)
    {
        ResetQuizAnswers();
        ChangeInfo(quizPoint.title, quizPoint.description);
        GameObject infoPanel = GameObject.Find("InfoPanel");

        foreach (Answer answer in quizPoint.answers)
        {
            GameObject quizAnswer = Instantiate(quizAnswerPrefab, infoPanel.transform);
            quizAnswer.tag = "QuizButton";
            quizAnswer.name = "Answer";
            QuizObjectController quizObjectController = quizAnswer.AddComponent<QuizObjectController>();
            AnswerObjectController answerObjectController = quizAnswer.AddComponent<AnswerObjectController>();
            Answer objectInfo = new()
            {
                id = answer.id,
                text = answer.text,
                isCorrect = answer.isCorrect,
            };
            QuizPoint quizObjectInfo = new()
            {
                title = quizPoint.title,
                description = quizPoint.description,
                enabled = quizPoint.enabled,
                isCorrect = quizPoint.isCorrect,
                selectedAnswer = quizPoint.selectedAnswer,
                x = quizPoint.x,
                y = quizPoint.y,
                z = quizPoint.z,
                yRotation = quizPoint.yRotation,
                image = quizPoint.image,
                answers = quizPoint.answers,
            };

            quizObjectController.objectInfo = quizObjectInfo;
            answerObjectController.objectInfo = objectInfo;
            quizAnswer.GetComponentInChildren<TextMeshProUGUI>().text = answer.text;
            ColorBlock colors = quizAnswer.GetComponent<Button>().colors;

            // If quiz point is not enabled, disable the button
            if (!quizPoint.enabled)
            {
                quizAnswer.GetComponent<Button>().interactable = false;
                // Set the disabled color of the selected answer
                if (quizPoint.selectedAnswer == answer.id)
                {
                    // If the selected answer is correct, set the disabled color to green
                    if (answer.isCorrect)
                    {
                        colors.disabledColor = Color.green;
                    }
                    else
                    {
                        colors.disabledColor = Color.red;
                    }
                }
            }
            quizAnswer.GetComponent<Button>().colors = colors;
        }
    }

    private static void ResetQuizAnswers()
    {
        GameObject infoPanel = GameObject.Find("InfoPanel");
        foreach (Transform child in infoPanel.transform)
        {
            if (child.CompareTag("QuizButton"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public static void SaveAllPositions(NavigationPoints positions, InfoPoints infoPoints, QuizPoints quizPoints)
    {
        DataContainer dataContainer = new();
        foreach (NavigationPoint position in positions.navigationPoints)
        {
            dataContainer.NavigationPoints.Add(position);
        }

        foreach (InfoPoint infoPoint in infoPoints.infoPoints)
        {
            dataContainer.InfoPoints.Add(infoPoint);
        }

        foreach (QuizPoint quizPoint in quizPoints.quizPoints)
        {
            dataContainer.QuizPoints.Add(quizPoint);
        }

        string updatedJson = JsonConvert.SerializeObject(dataContainer, Formatting.Indented);
        File.WriteAllText(jsonFilePath, updatedJson);
    }

    public static bool CheckAnswer(Answer answer)
    {
        if (answer.isCorrect)
        {
            UnityEngine.Debug.Log("Correct answer");
            HapticInteractable.SetHapticImpulse(0.1f, 0.1f);
            return true;
        }
        else
        {
            UnityEngine.Debug.Log("Incorrect answer");
            HapticInteractable.SetHapticImpulse(0.2f, 0.5f);
            return false;
        }
    }

    public static void UpdateQuizPoint(QuizPoint objectInfo)
    {
        string jsonString = File.ReadAllText(jsonFilePath);
        InfoPoints infoPointData = JsonConvert.DeserializeObject<InfoPoints>(jsonString);
        NavigationPoints navigationPointData = JsonConvert.DeserializeObject<NavigationPoints>(jsonString);
        QuizPoints quizPointData = JsonConvert.DeserializeObject<QuizPoints>(jsonString);

        // update quiz point in json
        quizPointData.quizPoints.RemoveAll(x => x.x == objectInfo.x && x.y == objectInfo.y && x.z == objectInfo.z);
        quizPointData.quizPoints.Add(objectInfo);
        SaveAllPositions(navigationPointData, infoPointData, quizPointData);
    }
}
