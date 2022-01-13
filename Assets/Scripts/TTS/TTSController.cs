using GoodEnough.TextToSpeech;

public class TTSController
{
    private static string defaultLanguage = "en";

    public static void Speak(string text)
    {
#if UNITY_IOS
        TTS.Speak(text, defaultLanguage);
#endif
    }

}
