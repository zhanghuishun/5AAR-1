using GoodEnough.TextToSpeech;

public class TTSController
{

    public static void Speak(string text)
    {
#if UNITY_IOS
        TTS.Speak(text);
#endif
    }

}
