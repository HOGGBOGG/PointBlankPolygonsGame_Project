using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public ObjectPool Parent;

    public virtual void OnDisable()
    {
        if(Parent == null)
        {
            //Debug.LogWarning("NoParent ObjectPool found..");
            return;
        }
        Parent.ReturnObjectToPool(this);
        //Debug.LogWarning("Successfully Returned to pool.");
    }
}