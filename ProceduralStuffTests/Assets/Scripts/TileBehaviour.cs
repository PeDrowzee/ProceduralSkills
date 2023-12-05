using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class TileBehaviour : MonoBehaviour
{
    public virtual string Name { get; set; }
    public virtual int PossibleOrientations { get; set; }
    public virtual bool[] CurrentOrientation { get; set; }

    public abstract string SetDefaultName();
    public abstract int SetPossibleOrientations();

    public abstract bool[] GetPossibleOrientations(int direction);

    public void SetActive(bool status)
    {
        gameObject.SetActive(status);
    }
    public bool[] ChangeOrientation(bool[] currentOrientation)
    {
        int lastIndex = currentOrientation.Length - 1;

        bool temp = currentOrientation[lastIndex];
        for (int i = lastIndex; i >= 0; i--)
        {
            if (i != 0)
            {
                currentOrientation[i] = currentOrientation[i - 1];
            }
            else
            {
                currentOrientation[i] = temp;
            }
        }
        return currentOrientation;
    }


    //used often to debug if my many orientations where correct
    public void DebugOrientation() => Debug.Log($"{Name}: Up {CurrentOrientation[0]}, Right {CurrentOrientation[1]}, Down {CurrentOrientation[2]}, Left {CurrentOrientation[3]}");
}


