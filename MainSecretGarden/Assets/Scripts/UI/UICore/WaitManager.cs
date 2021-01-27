public static class WaitManager
{
    public static void BeginRotate()
    {
        UIComponent.CreateUI("UIWait",true);
    }
    public static void EndRotate()
    {
        UIComponent.RemoveUI("UIWait");
    }
}
