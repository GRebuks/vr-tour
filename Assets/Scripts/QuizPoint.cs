using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

[System.Serializable]
public class QuizPoint
{
    public string title;
    public string description;
    public bool enabled;
    public bool isCorrect;
    public int selectedAnswer;
    public float x;
    public float y;
    public float z;
    public float yRotation;
    public string image;
    public List<Answer> answers;
}

[System.Serializable]
public class Answer
{
    public int id;
    public string text;
    public bool isCorrect;
}

[System.Serializable]
public class AnswerObjectController: MonoBehaviour
{
    public Answer objectInfo;
}
