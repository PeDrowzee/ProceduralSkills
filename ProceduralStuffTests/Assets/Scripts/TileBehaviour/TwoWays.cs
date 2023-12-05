using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWays : TileBehaviour
{
    public override string Name { get => SetDefaultName(); set => base.Name = value; }
    public override int PossibleOrientations { get => SetPossibleOrientations(); set => base.PossibleOrientations = value; }


    public override string SetDefaultName() => "2Ways";
    public override int SetPossibleOrientations() => 2;
    public override bool[] GetPossibleOrientations(int direction)
    {

        bool[] possibilities = new bool[4];

        if (direction == 0)
        {
            possibilities[0] = true;
            possibilities[1] = false;
            possibilities[2] = true;
            possibilities[3] = false;
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        if (direction == 1)
        {
            //Right Orientation
            possibilities[0] = false;
            possibilities[1] = true;
            possibilities[2] = false;
            possibilities[3] = true;
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }

        return possibilities;
    }
}
