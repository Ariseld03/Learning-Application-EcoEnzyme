using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicableGulaMerah : MonoBehaviour
{
    [SerializeField] GameChapter2 game;
    [SerializeField] private GameObject unslicedObject;
    private GameObject slicedObject;
    public void setSlicedObject(GameObject slicedObject)
    {
        this.slicedObject = slicedObject; 
    }

    public GameObject getSlicedObject()
    {
        return slicedObject; 
    }
    public void Slice()
    {
        Debug.Log("Slice");
        game.PlaySFX(game.cutSFX);
        unslicedObject.SetActive(false);
        unslicedObject=slicedObject;
        slicedObject.SetActive(true);
    }
}
