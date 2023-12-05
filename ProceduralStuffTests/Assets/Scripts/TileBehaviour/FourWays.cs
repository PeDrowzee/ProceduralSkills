using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWays : TileBehaviour
{
    public override string Name { get => SetDefaultName(); set => base.Name = value; }
    public override int PossibleOrientations { get => SetPossibleOrientations(); set => base.PossibleOrientations = value; }

    public override string SetDefaultName() => "4Ways";
    public override int SetPossibleOrientations() => 1;
    public override bool[] GetPossibleOrientations(int direction)
    {
        bool[] possibilities = new bool[4];
        if (direction == 0)
        {
            possibilities[0] = true;
            possibilities[1] = true;
            possibilities[2] = true;
            possibilities[3] = true;
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        return possibilities;
    }


}
