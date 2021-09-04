using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Answer
{
    public string AnswerText;
    public Sprite AnswerSprite;
    public Answer(string AnswerText, Sprite AnswerSprite)
    {
        this.AnswerText = AnswerText;
        this.AnswerSprite = AnswerSprite;
    }
}

[Serializable]
public class Round
{
    public List<Answer> answerList;
}
