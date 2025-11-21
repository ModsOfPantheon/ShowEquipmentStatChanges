using MelonLoader;

namespace ModTemplate;

public class ModMain : MelonMod
{
    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("Hello world!");
    }

    public const string ModVersion = "1.0.0";
}