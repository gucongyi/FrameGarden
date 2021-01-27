
using UnityEngine;

public class GlobalConfigComponent : MonoBehaviour
{
    public static GlobalConfigComponent intance;
    public GlobalProto GlobalProto;

    public void Awake()
    {
        intance = this;
        string configStr = ConfigHelper.GetGlobal();
        this.GlobalProto = JsonHelper.FromJson<GlobalProto>(configStr);
    }
}