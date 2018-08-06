using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSpeech;

namespace Talent.ClientCommonLib
{
   public class SoundReading
    {
       
       private static SpVoice _Voice  = new SpVoice();
       private SpeechVoiceSpeakFlags _SpFlags;
       private string _voiceStr;
       public SoundReading(string voiceStr)
       {
           _SpFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
           _Voice.SynchronousSpeakTimeout = 50;
           _Voice.Volume = 100;//声音最大(1-100)
           _Voice.Rate = 1;//速度最慢(1-10)
           _voiceStr = voiceStr;
       }
       public  void  Voice()
       {
           try
           {
               _Voice.Speak("", SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
               _Voice.Speak(_voiceStr, _SpFlags);
           }
           catch (Exception ex)
           {
               //throw ex;               
           }
       }
           
    }
}
