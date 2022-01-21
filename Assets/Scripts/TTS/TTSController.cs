using GoodEnough.TextToSpeech;

public class TTSController
{
    private static string defaultLanguage = "en";
    private static ISpeechSynthesisVoice voice = TTS.GetVoiceForLanguage(defaultLanguage);

#if UNITY_IOS
    private static SpeechUtteranceParameters options = new SpeechUtteranceParameters();
#endif

    public static void Speak(string text)
    {
#if UNITY_IOS
        options.Volume = SettingsData.volume;
        options.Voice = voice;
        TTS.Speak(text, options);
#endif
    }

}
