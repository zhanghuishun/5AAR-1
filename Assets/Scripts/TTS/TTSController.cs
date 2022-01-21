using GoodEnough.TextToSpeech;
using TextSpeech;

public class TTSController
{
    private static string defaultLanguage = "en";

#if UNITY_IOS
    private static ISpeechSynthesisVoice voice = TTS.GetVoiceForLanguage(defaultLanguage);
    private static SpeechUtteranceParameters options = new SpeechUtteranceParameters();
#endif

    private static TextToSpeech ttsAndroid = new TextToSpeech(defaultLanguage);
#if UNITY_ANDROID
    private static TextToSpeech ttsAndroid = new TextToSpeech();
#endif

    public static void Speak(string text)
    {
#if UNITY_IOS
        options.Volume = SettingsData.volume;
        options.Voice = voice;
        TTS.Speak(text, options);
#endif

#if UNITY_ANDROID
        ttsAndroid.StartSpeak(text);
#endif
    }

}
