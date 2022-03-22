using HarmonyLib;
using System;
using System.IO;

namespace TormentedEmu_Mods_A19
{
    public class Harmony_TE_MenuMusic
    {
        public static string MyModFolder = "zz_TraderNetwork_MoreJobs__Utils";
        public static string MyMusicFile = "zombiemainmusic.ogg";
        public static AudioType MyMusicType = AudioType.OGGVORBIS;

        public void Start()
        {
            var harmony = new Harmony("TormentedEmu.Mods.A19");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(BackgroundMusicMono), "Start")]
        public class BackgroundMusicMono_Start
        {
            static async void Postfix(BackgroundMusicMono __instance, AudioSource ___audioSource)
            {
                Log.Out("TE_MenuMusic - Attempting to load the mod menu background music...");

                var modMusicClip = await LoadModBackgroundMusic();
                if (modMusicClip == null)
                {
                    Log.Error("TE_MenuMusic - Failed to load the mod music clip!");
                    return;
                }

                Log.Out($"TE_MenuMusic - Loading new menu background music: {modMusicClip}");
                ___audioSource.clip = modMusicClip;
            }

            static async Task<AudioClip> LoadModBackgroundMusic()
            {
                AudioClip audioClip = null;
                string musicPath = "file://" + Path.Combine(Utils.GetGamePath(), "Mods", MyModFolder, "Resources", MyMusicFile);

                using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(musicPath, MyMusicType))
                {
                    webRequest.SendWebRequest();

                    try
                    {
                        while (!webRequest.isDone)
                            await Task.Delay(5);

                        if (webRequest.isNetworkError || webRequest.isHttpError)
                        {
                            Log.Error($"Failed to retrieve new background music clip:  {webRequest.error}");
                        }
                        else
                        {
                            audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
                            audioClip.name = MyMusicFile;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }
                }

                return audioClip;
            }
        }

    }
}