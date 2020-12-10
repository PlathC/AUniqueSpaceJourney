using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AUSJ
{
    public interface IState
    {
        IState Update();
    }
}
